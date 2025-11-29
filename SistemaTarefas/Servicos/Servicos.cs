using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SistemaTarefas.DTO.Response;
using SistemaTarefas.Enums;
using SistemaTarefas.Models;
using SistemaTarefas.Util;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SistemaTarefas.Servicos
{
    public class Servico
    {
        public Servico()
        {
        }

        #region Constantes
        
        #region Administrador do Sistema

        public const string SENHA_INICIAL_ADMIN = "$2a$11$a00iJK2u54s2C7RA/M4kjevS563lW2yS8zd.KcWWxbuFsHWqfjA1.";

        #endregion

        #region Demais

        public const int TAM_NOMES = 50;
        public const int TAM_NOTASDESCRICAO = 500;
        public const string MSG_EXCEPTION = "Falha indeterminada no processamento.";
        public const string MSG_EXCEDE_TAMANHO = "Há campos que ultrapassam o limite de tamanho permitido.";
        public const string ERRO_USO_API = "Uso incorreto da API";
        public const long TAM_MAX_IMAGEM_PERFIL = 512 * 1024;
        public const string PATH_IMG_PERFIL = "profile";        

#if DEBUG
        public const string PATH_LOG = "logs/API.log";
#else
        public const string PATH_LOG = "logs/API.log";//"/var/log/sistema/API.log";
#endif

        #endregion

#endregion

        #region Log
        public static void GravaLog(string mensagem, Exception? ex = null)
        {
            try
            {
#if DEBUG
                if (ex == null)
                {
                    Console.WriteLine(mensagem);
                }
                else
                {
                    Console.WriteLine($"{mensagem} | {ex.Message}");
                }
#endif

                if (ex != null)
                {
                    string erroStr = ex.ToString().Replace("\n", " ").Replace("\r", "");
                    Log.Error("Tipo=Erro | Mensagem={Mensagem} | Excecao={Erro}", mensagem, erroStr);
                }
                else
                {
                    Log.Information("Tipo=Info | Mensagem={Mensagem}", mensagem);
                }
            }
            catch (Exception exInterno)
            {
#if DEBUG
                Console.WriteLine($"Erro no 'GravaLog': {exInterno.Message}");
#endif
            }
        }

        #endregion

        #region Token
        static public string GerarTokenJWT(TokenConfiguracao configToken, int usuarioID, int nivel)
        {
            SymmetricSecurityKey chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configToken.ChaveSecreta));

            var credencial = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

            #region Claims
            var claim = new[]
            {
                new Claim("i", Encrypt(usuarioID.ToString())),
                new Claim("n", Encrypt( nivel.ToString())),
            };
            #endregion

            var token = new JwtSecurityToken(
                    issuer: configToken.NomeEmpresa,
                    audience: configToken.NomeAplicacao,
                    expires: DateTime.Now.AddMinutes(configToken.TempoVidaMinutos),

                    claims: claim,

                    signingCredentials: credencial
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion

        #region Claims
        private static IHttpContextAccessor? _accessor;

        public static void Configure(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
        public static (int usuarioID, int nivelUsuario) Claims()
        {
            var httpContext = _accessor?.HttpContext;
            var claims = httpContext?.User;

            var idClaim = claims?.FindFirst("i")?.Value;
            var nivelClaim = claims?.FindFirst("n")?.Value;

            var usuarioId = int.TryParse(Decrypt(idClaim!), out int id) ? id : 0;
            var nivel = int.TryParse(Decrypt(nivelClaim!), out int nv) ? nv : 0;

            return (usuarioId, nivel);
        }
        #endregion

        #region Criptografia

        private static byte[] Key = Array.Empty<byte>();
        private static byte[] IV = Array.Empty<byte>();

        public static void Setup(CriptografiaConfiguracao config)
        {
            Key = Encoding.UTF8.GetBytes(config.AES_256_32bytes);
            IV  = Encoding.UTF8.GetBytes(config.AES_16bytes);
        }

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msEncrypt = new();
            using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (StreamWriter swEncrypt = new(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            byte[] buffer = Convert.FromBase64String(cipherText);

            using MemoryStream msDecrypt = new(buffer);
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);

            return srDecrypt.ReadToEnd();
        }

        #endregion
    }
}

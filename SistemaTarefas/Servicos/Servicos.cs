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
        #region Constantes

        #region Para uso no Token
        public const string NOME_EMPRESA_TOKEN = "Sua Empresa";
        public const string NOME_APLICACAO_TOKEN = "Sua Aplicação";
        public const string CHAVE_SECRETA_TOKEN = "89fb361e-45cb-433d-9191-d25a5e115a89";
        public const int TEMPO_VIDA_TOKEN_MINUTOS = 60;
        #endregion

        #region Conexao https

        public const string CHAVE_HTTPS = "6fc57a01-209b-4021-b0cc-8bf727f51a1a";
        public const string SERVIDOR_ORIGEM_1 = "http://localhost:3000";
        public const string SERVIDOR_ORIGEM_2 = "http://192.168.0.50:3000";

#if DEBUG
        public const string PATH_CETIFICADO_PFX = @"C:\Users\souza\Documents\Programa\C_Sharp\API\SistemaTarefas\SistemaTarefas\https-dev.pfx";
        #else
        public const string PATH_CETIFICADO_PFX = "https-dev.pfx";
        #endif
        
#if DEBUG
        public const int PORTA_HTTPS = 7044;
#else
        public const int PORTA_HTTPS = 5001;
#endif

#endregion

        #region Conexao Banco
        public const string CONNECTION_STRING = "Server=./;Database=SistemaTarefas;User Id=sa;Password=masterkey;TrustServerCertificate=True;";
        public const string SENHA_INICIAL_ADMIN = "$2a$11$a00iJK2u54s2C7RA/M4kjevS563lW2yS8zd.KcWWxbuFsHWqfjA1."; // senha123456
        public const int EXPIRACAO_SENHA_DIAS = 60;
        #endregion

        #region Constantes de Teste (Use True para testar)

        public const bool TESTE_SEM_FORCAR_ERROR_CODE = false;

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
        static public string GerarTokenJWT(int usuarioID, int nivel)
        {
            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(CHAVE_SECRETA_TOKEN));
            var credencial = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

            #region Claims
            var claim = new[]
            {
                new Claim("i", Encrypt(usuarioID.ToString())), //usuarioID.ToString().PadLeft(6, '0');
                new Claim("n", Encrypt( nivel.ToString())),
            };
            #endregion

            var token = new JwtSecurityToken(
                    issuer: NOME_EMPRESA_TOKEN,// Cabeçalho 
                    audience: NOME_APLICACAO_TOKEN,// Cabeçalho 
                    expires: DateTime.Now.AddMinutes(TEMPO_VIDA_TOKEN_MINUTOS),// Cabeçalho

                    claims: claim, // Payload = Dados Adicionais

                    signingCredentials: credencial // Assinatura
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
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("LOgZa+3x80No}C<rejD3iC@9RZa16!U#"); // 32 bytes para AES-256
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("heQR;zpe!<?$e#?d"); // 16 bytes para AES

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
            byte[] encrypted = msEncrypt.ToArray();

            return Convert.ToBase64String(encrypted);
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

using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using SistemaTarefas.Controllers;
using SistemaTarefas.Data;
using SistemaTarefas.Enums;
using SistemaTarefas.Models;
using SistemaTarefas.Repositorios;
using SistemaTarefas.Repositorios.Interfaces;
using SistemaTarefas.Servicos;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SistemaTarefas
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region Logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
                .WriteTo.Console()
                .WriteTo.File(Servico.PATH_LOG, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();
            #endregion

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ConfigureHttpsDefaults(httpsOptions =>
                {
                    httpsOptions.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 |
                                                System.Security.Authentication.SslProtocols.Tls13;

                });

                //options.ListenAnyIP(5000); // HTTP

                options.ListenAnyIP(Servico.PORTA_HTTPS, listenOptions =>
                {
                    listenOptions.UseHttps(Servico.PATH_CETIFICADO_PFX, Servico.CHAVE_HTTPS);//listenOptions.UseHttps();// vazio para sem senha e sem certificado.
                });
            });


            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sistema API", Version = "v1" });

                c.MapType<IFormFile>(() => new Microsoft.OpenApi.Models.OpenApiSchema
                {
                    Type = "string",
                    Format = "binary"
                });

                var securitySchema = new OpenApiSecurityScheme
                {
                    Name = "JWT Autentication",
                    Description = "Entre com o JWT Bearer Token",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }

                };

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securitySchema);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securitySchema, new string[] { } }
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DefaultCorsPolicy", policy =>
                {
                    policy.WithOrigins(Servico.SERVIDOR_ORIGEM_1, Servico.SERVIDOR_ORIGEM_2)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();

                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/json" });
            });

            builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.Optimal;
            });

            builder.Services.AddEntityFrameworkSqlServer()
                .AddDbContext<SistemaTarefasDBContex>(
                options => options.UseSqlServer(Servico.CONNECTION_STRING)
            );

            builder.Services.AddScoped<ITarefasRepositorio, TarefasRepositorio>();
            builder.Services.AddScoped<ITramitesRepositorio, TramitesRepositorio>();
            builder.Services.AddScoped<IUsuariosRepositorio, UsuariosRepositorio>();
            builder.Services.AddScoped<IModelosTarefaRepositorio, ModelosTarefaRepositorio>();
            builder.Services.AddScoped<IModelosTramiteRepositorio, ModelosTramiteRepositorio>();
            builder.Services.AddScoped<IFlagsRepositorio, FlagsRepositorio>();

            builder.Services.AddControllers().AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            builder.Services.AddHttpContextAccessor();

            #region Services para Middlewares

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("NivelAcesso1", policy =>
                policy.RequireAssertion(context =>
                {
                    var claimCriptografada = context.User.FindFirst("n")?.Value;

                    if (string.IsNullOrEmpty(claimCriptografada))
                        return false;

                    var claimDescriptografada = Servico.Decrypt(claimCriptografada);
                    return int.TryParse(claimDescriptografada, out int nivel) && nivel == 1;
                }));
                options.AddPolicy("NivelAcesso1a2", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var claimCriptografada = context.User.FindFirst("n")?.Value;

                        if (string.IsNullOrEmpty(claimCriptografada))
                            return false;

                        var claimDescriptografada = Servico.Decrypt(claimCriptografada);
                        return int.TryParse(claimDescriptografada, out int nivel) && nivel >= 1 && nivel <= 2;
                    }));
                options.AddPolicy("NivelAcesso1a3", policy =>
                policy.RequireAssertion(context =>
                {
                    var claimCriptografada = context.User.FindFirst("n")?.Value;

                    if (string.IsNullOrEmpty(claimCriptografada))
                        return false;

                    var claimDescriptografada = Servico.Decrypt(claimCriptografada);
                    return int.TryParse(claimDescriptografada, out int nivel) && nivel >= 1 && nivel <= 3;
                }));
                options.AddPolicy("NivelAcesso1a4", policy =>
                policy.RequireAssertion(context =>
                {
                    var claimCriptografada = context.User.FindFirst("n")?.Value;

                    if (string.IsNullOrEmpty(claimCriptografada))
                        return false;

                    var claimDescriptografada = Servico.Decrypt(claimCriptografada);
                    return int.TryParse(claimDescriptografada, out int nivel) && nivel >= 1 && nivel <= 4;
                }));
            });

            #endregion

            builder.Services.AddAutoMapper(typeof(MappingProfile));


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Servico.NOME_EMPRESA_TOKEN,
                    ValidAudience = Servico.NOME_APLICACAO_TOKEN,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Servico.CHAVE_SECRETA_TOKEN)),
                    ClockSkew = TimeSpan.FromMinutes(5)
                };

                // Middleware
                options.Events = new JwtBearerEvents
                {
                    OnForbidden = async context =>
                    {
                        var usuarioID = context.HttpContext.User.FindFirst("i")?.Value ?? "Desconhecido";
                        var idDescriptografado = Servico.Decrypt(usuarioID);

                        var resposta = new { message = $"Usuário [{idDescriptografado}] não tem permissão para esta operação." };

                        var optionsJson = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        await context.Response.WriteAsync(JsonSerializer.Serialize(resposta, optionsJson));
                    }
                };
            });

            var app = builder.Build();

            Servico.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

            if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("ExibirSwagger"))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();

            app.Use(async (context, next) =>
            {
                Console.WriteLine($"Requisição: {context.Request.Method} {context.Request.Path}");
                Console.WriteLine($"Cabeçalhos: {string.Join(", ", context.Request.Headers)}");
                await next.Invoke();
            });

            // Middlewares
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                    var response = new
                    {
                        RM = "Ocorreu um erro interno.",
                        RC = ResponseCode.Excecao,  // seu enum de resposta
                        OK = false
                    };

                    if (exceptionHandlerPathFeature?.Error != null)
                    {
                        Servico.GravaLog($"Exceção global: {exceptionHandlerPathFeature.Error.Message}");

#if DEBUG
                        response = new
                        {
                            RM = exceptionHandlerPathFeature.Error.Message,
                            RC = ResponseCode.Excecao,
                            OK = false
                        };
#endif
                    }

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                });
            });

            app.UseStaticFiles();
            app.UseResponseCompression();

            app.UseCors("DefaultCorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SistemaTarefas.Data.Map;
using SistemaTarefas.DTO.Query;
using SistemaTarefas.Models;
using SistemaTarefas.Servicos;

namespace SistemaTarefas.Data
{
    public partial class SistemaTarefasDBContex : DbContext
    {
        public SistemaTarefasDBContex(DbContextOptions<SistemaTarefasDBContex> options)
            : base(options)
        {
        }

        public required DbSet<Tarefas> Tarefas { get; set; }
        public required DbSet<Tramites> Tramites { get; set; }
        public required DbSet<ModelosTarefa> ModelosTarefa { get; set; }
        public required DbSet<ModelosTramite> ModelosTramite { get; set; }
        public required DbSet<Usuarios> Usuarios { get; set; }
        public required DbSet<Flags> Flags { get; set; }

        public required DbSet<ModelosTramiteSQL> ModelosTramiteSQL { get; set; }
        public required DbSet<TarefasSQL> TarefasSQL { get; set; }
        public required DbSet<TramitesSQL> TramitesSQL { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TarefasMap());
            modelBuilder.ApplyConfiguration(new TramitesMap());
            modelBuilder.ApplyConfiguration(new ModelosTarefaMap());
            modelBuilder.ApplyConfiguration(new ModelosTramiteMap());
            modelBuilder.ApplyConfiguration(new UsuariosMap());
            modelBuilder.ApplyConfiguration(new FlagsMap());

            modelBuilder.Entity<ModelosTramiteSQL>().HasNoKey().ToView(null);
            modelBuilder.Entity<TarefasSQL>().HasNoKey().ToView(null);
            modelBuilder.Entity<TramitesSQL>().HasNoKey().ToView(null);

            modelBuilder.Entity<Usuarios>().HasData(new Usuarios
            {
                UsuId = 1,
                UsuLogin = "admin",
                UsuSenha = Servico.SENHA_INICIAL_ADMIN,
                UsuNivel = 1,
                UsuNome = "Administrador do Sistema",
                UsuEmail = "mail@mail.com",
                UsuMatricula = "000000"
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

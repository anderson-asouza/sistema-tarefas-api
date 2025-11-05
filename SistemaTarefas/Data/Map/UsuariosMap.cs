using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTarefas.Models;

namespace SistemaTarefas.Data.Map
{
    public class UsuariosMap : IEntityTypeConfiguration<Usuarios>
    {
        public void Configure(EntityTypeBuilder<Usuarios> builder)
        {
            builder.ToTable("Usuarios");

            builder.HasKey(e => e.UsuId);

            builder.HasIndex(e => e.UsuMatricula)
                .IsUnique()
                .HasDatabaseName("IX_Matricula");

            builder.HasIndex(e => e.UsuLogin)
                .IsUnique()
                .HasDatabaseName("IX_Login");

            builder.Property(e => e.UsuId)
                .HasColumnName("USU_ID");

            builder.Property(e => e.UsuEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("USU_Email");

            builder.Property(e => e.UsuMatricula)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("USU_Matricula");

            builder.Property(e => e.UsuNivel)
                .HasColumnName("USU_Nivel");

            builder.Property(e => e.UsuNome)
                .HasMaxLength(100)
                .IsUnicode(true)
                .HasColumnName("USU_Nome");

            builder.Property(e => e.UsuSenha)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("USU_Senha");

            builder.Property(e => e.UsuLogin)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USU_Login");

            builder.Property(e => e.UsuImagemPerfil)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("USU_ImagemPerfil");            

            builder.Property(e => e.UsuDataMudancaSenha)
                .HasColumnName("USU_DataMudancaSenha");
        }
    }
}
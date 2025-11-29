using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTarefas.Enums;
using SistemaTarefas.Models;
using SistemaTarefas.Servicos;

namespace SistemaTarefas.Data.Map
{
    public class TarefasMap : IEntityTypeConfiguration<Tarefas>
    {
        public void Configure(EntityTypeBuilder<Tarefas> builder)
        {
            builder.ToTable("Tarefas");

            builder.HasKey(e => e.TarId).HasName("PK_Tarefa");

            builder.Property(e => e.TarId).HasColumnName("TAR_ID");
            builder.Property(e => e.TarDataComeco).HasColumnName("TAR_DataComeco");
            builder.Property(e => e.TarDataFinalPrevista).HasColumnName("TAR_DataFinalPrevista");
            builder.Property(e => e.TarDataFinal).HasColumnName("TAR_DataFinal");
            builder.Property(e => e.TarFlaId).HasColumnName("TAR_FLA_ID").IsRequired(false);
            builder.Property(t => t.TarStatus).HasConversion(
                v => v.ToCodigo(),                        // Enum → string para salvar
                v => StatusTarefaExtensions.FromCodigo(v) // string → enum ao carregar
                ).HasMaxLength(1);
            builder.Property(e => e.TarDescricao)
                .HasMaxLength(Servico.TAM_NOTASDESCRICAO)
                .IsUnicode(false).HasColumnName("TAR_Descricao");
            builder.Property(e => e.TarNomeTarefa)
                .HasMaxLength(Servico.TAM_NOMES)
                .IsUnicode(false).HasColumnName("TAR_Nome");
            builder.Property(e => e.TarMtarId).HasColumnName("TAR_MTAR_ID");
            builder.Property(e => e.TarUsuIdResponsavelTarefa).HasColumnName("TAR_USU_ID_Responsavel");

            builder.HasOne(d => d.TarMtar).WithMany(p => p.Tarefas)
                .HasForeignKey(d => d.TarMtarId)
                .HasConstraintName("FK_Tarefas_Modelos_Tarefa");

            builder.HasOne(d => d.TarUsuIdResponsavelTarefaNavigation).WithMany(p => p.Tarefas)
                .HasForeignKey(d => d.TarUsuIdResponsavelTarefa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tarefas_Usuarios");

            builder.HasIndex(e => e.TarNomeTarefa)
                   .IsUnique()
                   .HasDatabaseName("IX_Tarefa_TAR_Nome_UNIQUE");

            builder.HasOne(d => d.TarFlaIdNavigation).WithMany(p => p.Tarefas)
                .HasForeignKey(d => d.TarFlaId)
                .HasConstraintName("FK_Tarefas_Flags");
        }
    }
}
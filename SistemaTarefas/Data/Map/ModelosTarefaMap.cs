using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTarefas.Models;
using SistemaTarefas.Servicos;

namespace SistemaTarefas.Data.Map
{
    public class ModelosTarefaMap : IEntityTypeConfiguration<ModelosTarefa>
    {
        public void Configure(EntityTypeBuilder<ModelosTarefa> builder)
        {
            builder.ToTable("ModelosTarefa");

            builder.HasKey(e => e.MtarId).HasName("PK_ModelosTarefa");           
            builder.Property(e => e.MtarId).HasColumnName("MTAR_ID");

            builder.Property(e => e.MtarNome)
                .HasMaxLength(Servico.TAM_NOMES)
                .IsUnicode(false).HasColumnName("MTAR_Nome");

            builder.Property(e => e.MtarDescricao)
                .HasMaxLength(Servico.TAM_NOTASDESCRICAO)
                .IsUnicode(true).HasColumnName("MTAR_Descricao");

            builder.HasIndex(e => e.MtarNome)
                   .IsUnique()
                   .HasDatabaseName("IX_ModelosTarefa_MTAR_Nome_UNIQUE");
        }
    }
}
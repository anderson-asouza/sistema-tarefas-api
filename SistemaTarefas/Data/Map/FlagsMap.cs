using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTarefas.Models;
using SistemaTarefas.Servicos;

namespace SistemaTarefas.Data.Map
{
    public class FlagsMap : IEntityTypeConfiguration<Flags>
    {
        public void Configure(EntityTypeBuilder<Flags> builder)
        {
            builder.ToTable("Flags");

            builder.HasKey(e => e.FlaId);

            builder.HasIndex(e => e.FlaRotulo)
                .IsUnique()
                .HasDatabaseName("IX_Rotulo");

            builder.HasIndex(e => e.FlaCor)
                .IsUnique()
                .HasDatabaseName("IX_Cor");


            builder.Property(e => e.FlaId)
                .HasColumnName("FLA_ID");

            builder.Property(e => e.FlaRotulo)
                .HasMaxLength(Servico.TAM_NOMES)
                .IsUnicode(false)
                .HasColumnName("FLA_ROTULO");

            builder.Property(e => e.FlaCor)
                .HasMaxLength(7)
                .IsUnicode(true)
                .HasColumnName("FLA_COR");
        }
    }
}
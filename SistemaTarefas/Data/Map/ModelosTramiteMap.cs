using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTarefas.Models;
using SistemaTarefas.Servicos;

namespace SistemaTarefas.Data.Map
{
    public class ModelosTramiteMap : IEntityTypeConfiguration<ModelosTramite>
    {
        public void Configure(EntityTypeBuilder<ModelosTramite> builder)
        {
            builder.ToTable("ModelosTramite");

            builder.HasKey(e => e.MtraId).HasName("PK_TramitesTiposTarefa");

            builder.Property(e => e.MtraId).HasColumnName("MTRA_ID");
            builder.Property(e => e.MtraDescricaoTramite)
                .HasMaxLength(Servico.TAM_NOTASDESCRICAO)
                .IsUnicode(true)
                .HasColumnName("MTRA_DescricaoTramite");
            builder.Property(e => e.MtraDuracaoPrevistaDias).HasColumnName("MTRA_DuracaoPrevistaDias");
            builder.Property(e => e.MtraNomeTramite)
                .HasMaxLength(Servico.TAM_NOMES)
                .HasColumnName("MTRA_NomeTramite");
            builder.Property(e => e.MtraOrdem).HasColumnName("MTRA_Ordem");


            builder.Property(e => e.MtraMtarId).HasColumnName("MTRA_MTAR_ID");

            builder.HasOne(d => d.MtraMtarNavigation)
                .WithMany(p => p.ModelosTramite)
                .HasForeignKey(d => d.MtraMtarId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Modelos_Tramite_ModelosTarefa");

            /*builder.HasOne(d => d.MtraMtarNavigation)
                .WithMany(p => p.TramitesTiposTarefa)
                .HasForeignKey(d => d.MtraMtarId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Modelos_Tramite");*/

            builder.Property(e => e.MtraUsuIdIndicacao).HasColumnName("MTRA_USU_ID_Indicacao").IsRequired(false);
            builder.Property(e => e.MtraUsuIdRevisor).HasColumnName("MTRA_USU_ID_Revisor").IsRequired(false);

            builder.HasOne(d => d.MtraUsuIdIndicacaoNavigation)
                .WithMany(p => p.TramitesTipoTarefaIndicacao)
                .HasForeignKey(d => d.MtraUsuIdIndicacao)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Modelos_Tramite_Usuarios_Indicacao");

            builder.HasOne(d => d.MtraUsuIdResponsavelNavigation)
                .WithMany(p => p.TramitesTipoTarefaResponsavel)
                .HasForeignKey(d => d.MtraUsuIdRevisor)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Modeos_Tramite_Usuarios_Responsavel");
        }
    }
}

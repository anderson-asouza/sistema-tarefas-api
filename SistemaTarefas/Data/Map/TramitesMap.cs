using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaTarefas.Servicos;

public class TramitesMap : IEntityTypeConfiguration<Tramites>
{
    public void Configure(EntityTypeBuilder<Tramites> builder)
    {
        builder.ToTable("Tramites");

        builder.HasKey(e => e.TraId).HasName("PK_Tramite");

        builder.Property(e => e.TraId).HasColumnName("TRA_ID");
        builder.Property(e => e.TraStatus)
               .HasColumnName("TRA_Status")
               .HasConversion<int>(); // Converte enum para int e vice-versa
        builder.Property(e => e.TraOrdem)
            .HasColumnName("TRA_Ordem");
        builder.Property(e => e.TraDataInicio)
            .HasColumnName("TRA_DataInicio");
        builder.Property(e => e.TraDataPrevisaoTermino)
            .HasColumnName("TRA_DataPrevisaoTermino");
        builder.Property(e => e.TraDataExecucao)
            .HasColumnName("TRA_DataExecucao");
        builder.Property(e => e.TraDataRevisao)
            .HasColumnName("TRA_DataRevisao");
        builder.Property(e => e.TraUsuIdTramitador).HasColumnName("TRA_USU_ID_Tramitador");
        builder.Property(e => e.TraUsuIdRevisor).HasColumnName("TRA_USU_ID_Revisor");
        builder.Property(e => e.TraNotaTramitador)
            .HasMaxLength(Servico.TAM_NOTASDESCRICAO)
            .IsUnicode(true).HasColumnName("TRA_NotaTramitador");
        builder.Property(e => e.TraNotaRevisor)
            .HasMaxLength(Servico.TAM_NOTASDESCRICAO)
            .IsUnicode(true).HasColumnName("TRA_NotaRevisor");
        builder.Property(e => e.TraRepetido).HasColumnName("TRA_TramiteRepetido");
        builder.Property(e => e.TraTarId).HasColumnName("TRA_TAR_ID");
        builder.Property(e => e.TraMtraId).HasColumnName("TRA_MTRA_ID");

        // Relacionamento com Tarefas
        builder.HasOne(d => d.TraTarIdNavigation).WithMany(p => p.Tramites)
            .HasForeignKey(d => d.TraTarId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Tramites_Tarefas");

        // Relacionamento com Tipo Tarefa
        builder.HasOne(d => d.TraTttIdNavigation).WithMany(p => p.Tramites)
            .HasForeignKey(d => d.TraMtraId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Modelos_Tramites");

        // Relacionamento com o Tramitador
        builder.HasOne(d => d.TraUsuIdTramitadorNavigation)
            .WithMany(p => p.TramitesTramitador)
            .HasForeignKey(d => d.TraUsuIdTramitador)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Tramites_UsuariosTramitador");

        // Relacionamento com o Revisor
        builder.HasOne(d => d.TraUsuIdRevisorNavigation)
            .WithMany(p => p.TramitesRevisor)
            .HasForeignKey(d => d.TraUsuIdRevisor)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Tramites_UsuariosRevisor");
    }
}

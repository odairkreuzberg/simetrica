using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class ProjetoCustoMap : EntityTypeConfiguration<ProjetoCusto>
    {
        public ProjetoCustoMap()
        {
            // Primary Key
            this.HasKey(t => t.idProjetoCusto);

            // Properties
            this.Property(t => t.descricao)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.gerarConta)
                .IsRequired()
                .HasMaxLength(3);
            this.Property(t => t.situacao)
                .IsRequired()
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("projetocusto");
            this.Property(t => t.idProjetoCusto).HasColumnName("idprojetocusto");
            this.Property(t => t.descricao).HasColumnName("descricao");
            this.Property(t => t.valor).HasColumnName("valor");
            this.Property(t => t.gerarConta).HasColumnName("gerarconta");
            this.Property(t => t.idProjeto).HasColumnName("idprojeto");
            this.Property(t => t.idContaPagar).HasColumnName("idcontapagar");
            this.Property(t => t.dtCusto).HasColumnName("dtcusto");

            // Relationships
            this.HasRequired(t => t.projeto)
                .WithMany(t => t.ProjetoCustos)
                .HasForeignKey(d => d.idProjeto);

        }
    }
}

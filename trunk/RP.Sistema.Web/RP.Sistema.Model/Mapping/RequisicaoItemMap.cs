using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RP.Sistema.Model.Entities;

namespace RP.Sistema.Model.Mapping
{
    public class RequisicaoItemMap : EntityTypeConfiguration<RequisicaoItem>
    {
        public RequisicaoItemMap()
        {
            // Primary Key
            this.HasKey(t => t.idItem);

            // Properties
            // Table & Column Mappings
            this.ToTable("requisicaoitem");
            this.Property(t => t.idItem).HasColumnName("iditem");
            this.Property(t => t.idMaterial).HasColumnName("idmaterial");
            this.Property(t => t.nrQuantidade).HasColumnName("nrquantidade");
            this.Property(t => t.vlPreco).HasColumnName("vlpreco");
            this.Property(t => t.idRequisicao).HasColumnName("idrequisicao");

            // Relationships
            this.HasRequired(t => t.Material)
                .WithMany(t => t.RequisicaoItens)
                .HasForeignKey(d => d.idMaterial);
            this.HasRequired(t => t.Requisicao)
                .WithMany(t => t.RequisicaoItens)
                .HasForeignKey(d => d.idRequisicao);

        }
    }
}

using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class ItemCompraMap : EntityTypeConfiguration<ItemCompra>
    {
        public ItemCompraMap()
        {
            // Primary Key
            this.HasKey(t => t.idItemCompra);

            // Table & Column Mappings
            this.ToTable("itemcompra");
            this.Property(t => t.idItemCompra).HasColumnName("iditemcompra");
            this.Property(t => t.idMaterial).HasColumnName("idmaterial");
            this.Property(t => t.idCompra).HasColumnName("idcompra");
            this.Property(t => t.quantidade).HasColumnName("quantidade");
            this.Property(t => t.valor).HasColumnName("valor");

            // Relationships
            this.HasRequired(t => t.Material)
                .WithMany(t => t.ItensCompra)
                .HasForeignKey(d => d.idMaterial);

            this.HasRequired(t => t.Compra)
                .WithMany(t => t.ItensCompra)
                .HasForeignKey(d => d.idCompra);

        }
    }
}

using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class ProdutoMaterialMap : EntityTypeConfiguration<ProdutoMaterial>
    {
        public ProdutoMaterialMap()
        {
            // Primary Key
            this.HasKey(t => t.idProdutoMaterial);

            // Properties
            // Table & Column Mappings
            this.ToTable("produtomaterial");
            this.Property(t => t.idProdutoMaterial).HasColumnName("idprodutomaterial");
            this.Property(t => t.idProduto).HasColumnName("idproduto");
            this.Property(t => t.idMaterial).HasColumnName("idmaterial");
            this.Property(t => t.quantidade).HasColumnName("quantidade");
            this.Property(t => t.margemGanho).HasColumnName("margemganho");
            this.Property(t => t.valor).HasColumnName("valor");
            this.Property(t => t.idCompra).HasColumnName("idcompra");

            // Relationships
            this.HasRequired(t => t.Material)
                .WithMany(t => t.ProdutoMateriais)
                .HasForeignKey(d => d.idMaterial);
            this.HasRequired(t => t.Produto)
                .WithMany(t => t.ProdutoMateriais)
                .HasForeignKey(d => d.idProduto);

        }
    }
}

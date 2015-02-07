using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class MaterialMap : EntityTypeConfiguration<Material>
    {
        public MaterialMap()
        {
            // Primary Key
            this.HasKey(t => t.idMaterial);

            // Properties
            this.Property(t => t.nome)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("material");
            this.Property(t => t.idMaterial).HasColumnName("idmaterial");
            this.Property(t => t.nome).HasColumnName("nome");
            this.Property(t => t.preco).HasColumnName("preco");
            this.Property(t => t.margemGanho).HasColumnName("margemganho");
            this.Property(t => t.idUnidadeMedida).HasColumnName("idunidademedida");
            this.Property(t => t.idFabricante).HasColumnName("idfabricante");
            this.Property(t => t.nrQuantidade).HasColumnName("nrquantidade");

            // Relationships
            this.HasOptional(t => t.Fabricante)
                .WithMany(t => t.Materiais)
                .HasForeignKey(d => d.idFabricante);
            this.HasRequired(t => t.UnidadeMedida)
                .WithMany(t => t.Materiais)
                .HasForeignKey(d => d.idUnidadeMedida);

        }
    }
}

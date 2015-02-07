using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class UnidadeMedidaMap : EntityTypeConfiguration<UnidadeMedida>
    {
        public UnidadeMedidaMap()
        {
            // Primary Key
            this.HasKey(t => t.idUnidadeMedida);

            // Properties
            this.Property(t => t.nome)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.abreviatura)
                .IsRequired()
                .HasMaxLength(2);

            // Table & Column Mappings
            this.ToTable("unidademedida");
            this.Property(t => t.idUnidadeMedida).HasColumnName("idunidademedida");
            this.Property(t => t.nome).HasColumnName("nome");
            this.Property(t => t.abreviatura).HasColumnName("abreviatura");
        }
    }
}

using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class EstadoMap : EntityTypeConfiguration<Estado>
    {
        public EstadoMap()
        {
            // Primary Key
            this.HasKey(t => t.idEstado);

            // Properties
            this.Property(t => t.nome)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.sigla)
                .IsRequired()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("estado");
            this.Property(t => t.idEstado).HasColumnName("idestado");
            this.Property(t => t.nome).HasColumnName("nome");
            this.Property(t => t.sigla).HasColumnName("sigla");
            this.Property(t => t.idPais).HasColumnName("idpais");

            // Relationships
            this.HasRequired(t => t.Pais)
                .WithMany(t => t.Estados)
                .HasForeignKey(d => d.idPais);

        }
    }
}

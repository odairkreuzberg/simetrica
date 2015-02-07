using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class PaisMap : EntityTypeConfiguration<Pais>
    {
        public PaisMap()
        {
            // Primary Key
            this.HasKey(t => t.idPais);

            // Properties
            this.Property(t => t.nome)
                .HasMaxLength(50);

            this.Property(t => t.sigla)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("pais");
            this.Property(t => t.idPais).HasColumnName("idpais");
            this.Property(t => t.nome).HasColumnName("nome");
            this.Property(t => t.sigla).HasColumnName("sigla");
        }
    }
}

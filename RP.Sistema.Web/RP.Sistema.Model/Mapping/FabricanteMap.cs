using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class FabricanteMap : EntityTypeConfiguration<Fabricante>
    {
        public FabricanteMap()
        {
            // Primary Key
            this.HasKey(t => t.idFabricante);

            // Properties
            this.Property(t => t.nome)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("fabricante");
            this.Property(t => t.idFabricante).HasColumnName("idfabricante");
            this.Property(t => t.nome).HasColumnName("nome");
        }
    }
}

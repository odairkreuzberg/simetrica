using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RP.Sistema.Model.Entities;

namespace RP.Sistema.Model.Mapping
{
    public class AreaMap : EntityTypeConfiguration<Area>
    {
        public AreaMap()
        {
            // Primary Key
            this.HasKey(t => t.idArea);

            // Properties
            this.Property(t => t.idArea)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.nmArea)
                .HasMaxLength(100);

            this.Property(t => t.dsArea)
                .HasMaxLength(255);

            this.Property(t => t.flUsaURL)
                .IsFixedLength()
                .HasMaxLength(3);

            // Table & Column Mappings
            this.ToTable("tbarea", Context.Schema);
            this.Property(t => t.idArea).HasColumnName("idarea");
            this.Property(t => t.nmArea).HasColumnName("nmarea");
            this.Property(t => t.dsArea).HasColumnName("dsarea");
            this.Property(t => t.idModulo).HasColumnName("idmodulo");
            this.Property(t => t.flUsaURL).HasColumnName("flusaurl");

            // Relationships
            this.HasRequired(t => t.Modulo)
                .WithMany(t => t.Areas)
                .HasForeignKey(d => d.idModulo);

        }
    }
}

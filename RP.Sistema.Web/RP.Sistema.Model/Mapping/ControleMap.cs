using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class ControleMap : EntityTypeConfiguration<Controle>
    {
        public ControleMap()
        {
            // Primary Key
            this.HasKey(t => t.idControle);

            // Properties
            this.Property(t => t.idControle)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.nmControle)
                .HasMaxLength(100);

            this.Property(t => t.dsControle)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("tbcontrole", Context.Schema);
            this.Property(t => t.idControle).HasColumnName("idcontrole");
            this.Property(t => t.nmControle).HasColumnName("nmcontrole");
            this.Property(t => t.dsControle).HasColumnName("dscontrole");
            this.Property(t => t.idArea).HasColumnName("idarea");

            // Relationships
            this.HasRequired(t => t.Area)
                .WithMany(t => t.Controles)
                .HasForeignKey(d => d.idArea);

        }
    }
}

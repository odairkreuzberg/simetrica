using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RP.Sistema.Model.Entities;

namespace RP.Sistema.Model.Mapping
{
    public class MenuMap : EntityTypeConfiguration<Menu>
    {
        public MenuMap()
        {
            // Primary Key
            this.HasKey(t => t.idMenu);

            // Properties
            this.Property(t => t.idMenu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.nmMenu)
                .HasMaxLength(100);

            this.Property(t => t.dsCor)
                .HasMaxLength(7);

            // Table & Column Mappings
            this.ToTable("tbmenu", Context.Schema);
            this.Property(t => t.idMenu).HasColumnName("idmenu");
            this.Property(t => t.nmMenu).HasColumnName("nmmenu");
            this.Property(t => t.nrOrdem).HasColumnName("nrordem");
            this.Property(t => t.dsCor).HasColumnName("dscor");
        }
    }
}

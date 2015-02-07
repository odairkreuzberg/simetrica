using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class ModuloMap : EntityTypeConfiguration<Modulo>
    {
        public ModuloMap()
        {
            // Primary Key
            this.HasKey(t => t.idModulo);

            // Properties
            this.Property(t => t.idModulo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.nmModulo)
                .HasMaxLength(100);

            this.Property(t => t.dsModulo)
                .HasMaxLength(255);

            this.Property(t => t.btImageMenu)
                .HasMaxLength(2048);

            this.Property(t => t.nmURL)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tbmodulo", Context.Schema);
            this.Property(t => t.idModulo).HasColumnName("idmodulo");
            this.Property(t => t.nmModulo).HasColumnName("nmmodulo");
            this.Property(t => t.dsModulo).HasColumnName("dsmodulo");
            this.Property(t => t.btImageMenu).HasColumnName("btimagemenu");
            this.Property(t => t.nrOrdem).HasColumnName("nrordem");
            this.Property(t => t.nmURL).HasColumnName("nmurl");

            //// Relationships
            //this.HasMany(t => t.Usuarios)
            //    .WithMany(t => t.Modulos)
            //    .Map(m =>
            //    {
            //        m.ToTable("tbusuariomoduloadm", Context.Schema);
            //        m.MapLeftKey("idmodulo");
            //        m.MapRightKey("idusuario");
            //    });

        }
    }
}

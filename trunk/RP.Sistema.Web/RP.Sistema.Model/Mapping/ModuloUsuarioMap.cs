using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class ModuloUsuarioMap : EntityTypeConfiguration<ModuloUsuario>
    {
        public ModuloUsuarioMap()
        {
            // Primary Key
            this.HasKey(t => new { t.idModulo, t.idUsuario });

            // Properties
            this.Property(t => t.idModulo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.idUsuario)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("tbusuariomoduloadm", Context.Schema);
            this.Property(t => t.idModulo).HasColumnName("idmodulo");
            this.Property(t => t.idUsuario).HasColumnName("idusuario");

            // Relationships
            this.HasRequired(t => t.Modulo)
                .WithMany(t => t.Usuarios)
                .HasForeignKey(d => d.idModulo);

            this.HasRequired(t => t.Usuario)
                .WithMany(t => t.Modulos)
                .HasForeignKey(d => d.idUsuario);
        }
    }
}

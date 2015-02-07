using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class PerfilMap : EntityTypeConfiguration<Perfil>
    {
        public PerfilMap()
        {
            // Primary Key
            this.HasKey(t => t.idPerfil);

            // Properties
            this.Property(t => t.idPerfil)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.nmPerfil)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tbperfil", Context.Schema);
            this.Property(t => t.idPerfil).HasColumnName("idperfil");
            this.Property(t => t.nmPerfil).HasColumnName("nmperfil");
        }
    }
}

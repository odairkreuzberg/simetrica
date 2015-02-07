using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class PerfilUsuarioMap : EntityTypeConfiguration<PerfilUsuario>
    {
        public PerfilUsuarioMap() 
        {
            // Primary Key
            this.HasKey(t => new {t.idPerfil , t.idUsuario });

            // Properties
            this.Property(t => t.idPerfil)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.idUsuario)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("tbperfilusuario", Context.Schema);
            this.Property(t => t.idPerfil).HasColumnName("idperfil");
            this.Property(t => t.idUsuario).HasColumnName("idusuario");

            // Relationships
            this.HasRequired(t => t.Usuario)
                .WithMany(t => t.Perfis)
                .HasForeignKey(d => d.idUsuario);

            this.HasRequired(t => t.Perfil)
                .WithMany(t => t.Usuarios)
                .HasForeignKey(d => d.idPerfil);
        }
    }
}

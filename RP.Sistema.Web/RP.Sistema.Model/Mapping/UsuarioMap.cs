using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class UsuarioMap : EntityTypeConfiguration<Usuario>
    {

        public UsuarioMap()
        {
            // Primary Key
            this.HasKey(t => t.idUsuario);

            // Properties
            this.Property(t => t.idUsuario)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.dsLogin)
                .HasMaxLength(50);

            this.Property(t => t.dsSenha)
                .HasMaxLength(100);

            this.Property(t => t.flAtivo)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.nmUsuario)
                .HasMaxLength(255);

            this.Property(t => t.dsEmail)
                .HasMaxLength(255);

            this.Property(t => t.fzUsuario)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tbusuario", Context.Schema);
            this.Property(t => t.idUsuario).HasColumnName("idusuario");
            this.Property(t => t.dsLogin).HasColumnName("dslogin");
            this.Property(t => t.dsSenha).HasColumnName("dssenha");
            this.Property(t => t.flAtivo).HasColumnName("flativo");
            this.Property(t => t.dtValidade).HasColumnName("dtvalidade");
            this.Property(t => t.nmUsuario).HasColumnName("nmusuario");
            this.Property(t => t.dsEmail).HasColumnName("dsemail");
            this.Property(t => t.fzUsuario).HasColumnName("fzusuario");
            this.Property(t => t.dsPreferencia).HasColumnName("dspreferencia");
            this.Property(t => t.nrFalhalogin).HasColumnName("nrfalhalogin");

            // Relationships
            this.HasMany(t => t.Tabelas)
                .WithMany(t => t.Usuarios)
                .Map(m =>
                {
                    m.ToTable("tbusuariotabela", Context.Schema);
                    m.MapLeftKey("idusuario");
                    m.MapRightKey("idtabela");
                });

        }   
    }
}

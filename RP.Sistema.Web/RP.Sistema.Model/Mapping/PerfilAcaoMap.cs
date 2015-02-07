using RP.Sistema.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP.Sistema.Model.Mapping
{
    public class PerfilAcaoMap : EntityTypeConfiguration<PerfilAcao>
    {
        public PerfilAcaoMap()
        {
            // Primary Key
            this.HasKey(t => new { t.idPerfil, t.idAcao });

            // Properties
            this.Property(t => t.idPerfil)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.idAcao)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("tbperfilacao", Context.Schema);
            this.Property(t => t.idPerfil).HasColumnName("idperfil");
            this.Property(t => t.idAcao).HasColumnName("idacao");

            // Relationships
            this.HasRequired(t => t.Acao)
                .WithMany(t => t.Perfis)
                .HasForeignKey(d => d.idAcao);

            this.HasRequired(t => t.Perfil)
                .WithMany(t => t.Acoes)
                .HasForeignKey(d => d.idPerfil);
        }
    }
}

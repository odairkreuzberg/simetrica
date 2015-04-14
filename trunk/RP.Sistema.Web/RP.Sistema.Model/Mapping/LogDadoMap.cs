using RP.Sistema.Model.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP.Sistema.Model.Mapping
{
    public class LogDadoMap : EntityTypeConfiguration<LogDado>
    {
        public LogDadoMap()
        {
            // Primary Key
            this.HasKey(t => t.idLog);

            // Properties
            this.Property(t => t.nmAcao)
                .HasMaxLength(100);

            this.Property(t => t.nmControle)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tblogdado", Context.Schema);
            this.Property(t => t.idLog).HasColumnName("idlog");
            this.Property(t => t.nmAcao).HasColumnName("nmacao");
            this.Property(t => t.nmControle).HasColumnName("nmcontrole");
            this.Property(t => t.dtLog).HasColumnName("dtlog");
            this.Property(t => t.idUsuario).HasColumnName("idusuario");

            // Relationships
            this.HasRequired(t => t.Usuario)
                .WithMany(t => t.Logs)
                .HasForeignKey(d => d.idUsuario);

        }
    }
}

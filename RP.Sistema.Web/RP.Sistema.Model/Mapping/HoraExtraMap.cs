using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class HoraExtraMap : EntityTypeConfiguration<HoraExtra>
    {
        public HoraExtraMap()
        {
            // Primary Key
            this.HasKey(t => t.idHora);

            // Properties
            this.Property(t => t.flTipo)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("horaextra", Context.Schema);
            this.Property(t => t.idHora).HasColumnName("idhora");
            this.Property(t => t.inicioHora).HasColumnName("iniciohora");
            this.Property(t => t.fimHora).HasColumnName("fimhora");
            this.Property(t => t.porcentagem).HasColumnName("porcentagem");
            this.Property(t => t.flTipo).HasColumnName("fltipo");
        }
    }
}

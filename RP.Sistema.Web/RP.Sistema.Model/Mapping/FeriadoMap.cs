using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class FeriadoMap : EntityTypeConfiguration<Feriado>
    {
        public FeriadoMap()
        {
            // Primary Key
            this.HasKey(t => t.idFeriado);

            // Properties
            this.Property(t => t.nmFeriado)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("feriado", Context.Schema);
            this.Property(t => t.idFeriado).HasColumnName("idferiado");
            this.Property(t => t.nmFeriado).HasColumnName("nmferiado");
            this.Property(t => t.nrDia).HasColumnName("nrdia");
            this.Property(t => t.nrMes).HasColumnName("nrmes");
        }
    }
}

using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
   public class ParametroMap : EntityTypeConfiguration<Parametro>
    {
        public ParametroMap()
        {
            // Primary Key
            this.HasKey(t => t.nmParametro);

            // Properties
            this.Property(t => t.nmParametro)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.nmParametro)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.dsParametro)
                .IsRequired()
                .HasMaxLength(1024);

            this.Property(t => t.dsValor)
                .IsRequired()
                .HasMaxLength(1024);

            // Table & Column Mappings
            this.ToTable("tbparametro", Context.Schema);
            this.Property(t => t.nmParametro).HasColumnName("nmparametro");
            this.Property(t => t.dsParametro).HasColumnName("dsparametro");
            this.Property(t => t.dsValor).HasColumnName("dsvalor");
        }
    }
}

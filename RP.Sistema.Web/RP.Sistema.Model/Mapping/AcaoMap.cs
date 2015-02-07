using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class AcaoMap : EntityTypeConfiguration<Acao>
    {
        public AcaoMap()
        {
            // Primary Key
            this.HasKey(t => t.idAcao);

            // Properties
            this.Property(t => t.nmAcao)
                .HasMaxLength(100);

            this.Property(t => t.dsAcao)
                .HasMaxLength(255);

            this.Property(t => t.flMenu)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.nmMenu)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tbacao", Context.Schema);
            this.Property(t => t.idAcao).HasColumnName("idacao");
            this.Property(t => t.nmAcao).HasColumnName("nmacao");
            this.Property(t => t.dsAcao).HasColumnName("dsacao");
            this.Property(t => t.flMenu).HasColumnName("flmenu");
            this.Property(t => t.nmMenu).HasColumnName("nmmenu");
            this.Property(t => t.idControle).HasColumnName("idcontrole");
            this.Property(t => t.idMenu).HasColumnName("idmenu");
            this.Property(t => t.nrOrdem).HasColumnName("nrordem");
            this.Property(t => t.dsIcone).HasColumnName("dsicone");

            // Relationships
            this.HasRequired(t => t.Controle)
                .WithMany(t => t.Acoes)
                .HasForeignKey(d => d.idControle);

            this.HasOptional(t => t.Menu)
                .WithMany(t => t.Acoes)
                .HasForeignKey(d => d.idMenu);

        }
    }
}

using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class CidadeMap : EntityTypeConfiguration<Cidade>
    {
        public CidadeMap()
        {
            // Primary Key
            this.HasKey(t => t.idCidade);

            // Properties
            this.Property(t => t.nome)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("cidade");
            this.Property(t => t.idCidade).HasColumnName("idcidade");
            this.Property(t => t.nome).HasColumnName("nome");
            this.Property(t => t.idEstado).HasColumnName("idestado");

            // Relationships
            this.HasOptional(t => t.Estado)
                .WithMany(t => t.Cidades)
                .HasForeignKey(d => d.idEstado);

        }
    }
}

using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class ClienteFoneMap : EntityTypeConfiguration<ClienteFone>
    {
        public ClienteFoneMap()
        {
            // Primary Key
            this.HasKey(t => t.idClienteFone);

            // Properties
            this.Property(t => t.tipo)
                .HasMaxLength(20);

            this.Property(t => t.numero)
                .HasMaxLength(15);

            // Table & Column Mappings
            this.ToTable("clientefone");
            this.Property(t => t.idClienteFone).HasColumnName("idclientefone");
            this.Property(t => t.idCliente).HasColumnName("idcliente");
            this.Property(t => t.tipo).HasColumnName("tipo");
            this.Property(t => t.numero).HasColumnName("numero");

            // Relationships
            this.HasRequired(t => t.Cliente)
                .WithMany(t => t.Telefones)
                .HasForeignKey(d => d.idCliente);

        }
    }
}

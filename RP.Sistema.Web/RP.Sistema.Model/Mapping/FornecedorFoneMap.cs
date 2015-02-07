using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class FornecedorFoneMap : EntityTypeConfiguration<FornecedorFone>
    {
        public FornecedorFoneMap()
        {
            // Primary Key
            this.HasKey(t => t.idFornecedorFone);

            // Properties
            this.Property(t => t.tipo)
                .HasMaxLength(20);

            this.Property(t => t.numero)
                .HasMaxLength(15);

            // Table & Column Mappings
            this.ToTable("fornecedorfone");
            this.Property(t => t.idFornecedorFone).HasColumnName("idfornecedorfone");
            this.Property(t => t.idFornecedor).HasColumnName("idfornecedor");
            this.Property(t => t.tipo).HasColumnName("tipo");
            this.Property(t => t.numero).HasColumnName("numero");

            // Relationships
            this.HasRequired(t => t.Fornecedor)
                .WithMany(t => t.Telefones)
                .HasForeignKey(d => d.idFornecedor);

        }
    }
}

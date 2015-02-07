using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RP.Sistema.Model.Entities;

namespace RP.Sistema.Model.Mapping
{
    public class TabelaMap : EntityTypeConfiguration<Tabela>
    {
        public TabelaMap()
        {
            // Primary Key
            this.HasKey(t => t.idTabela);

            // Properties
            this.Property(t => t.idTabela)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.nmTabela)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("tbtabela", Context.Schema);
            this.Property(t => t.idTabela).HasColumnName("idtabela");
            this.Property(t => t.nmTabela).HasColumnName("nmtabela");
        }
    }
}

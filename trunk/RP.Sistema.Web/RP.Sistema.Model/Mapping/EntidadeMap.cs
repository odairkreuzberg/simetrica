using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class EntidadeMap : EntityTypeConfiguration<Entidade>
    {
        public EntidadeMap()
        {
            // Primary Key
            this.HasKey(t => t.idEntidade);

            // Properties
            this.Property(t => t.idEntidade)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.nmRazaoSocial)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.nmFantasia)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.nrCNPJ)
                .IsRequired()
                .HasMaxLength(25);

            this.Property(t => t.dsCidade)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.dsBairro)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.dsLogradouro)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.nrEndereco)
                .IsRequired()
                .HasMaxLength(6);

            this.Property(t => t.nrCEP)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(15);

            this.Property(t => t.nrTelefone)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.dsWebSite)
                .HasMaxLength(100);

            this.Property(t => t.dsEmail)
                .HasMaxLength(100);

            this.Property(t => t.hrInicioManha)
                .HasMaxLength(8);

            this.Property(t => t.hrFimManha)
                .HasMaxLength(8);

            this.Property(t => t.hrInicioTarde)
                .HasMaxLength(8);

            this.Property(t => t.hrFimTarde)
                .HasMaxLength(8);

            // Table & Column Mappings
            this.ToTable("tbentidade", Context.Schema);
            this.Property(t => t.idEntidade).HasColumnName("identidade");
            this.Property(t => t.nmRazaoSocial).HasColumnName("nmrazaosocial");
            this.Property(t => t.nmFantasia).HasColumnName("nmfantasia");
            this.Property(t => t.nrCNPJ).HasColumnName("nrcnpj");
            this.Property(t => t.dsCidade).HasColumnName("dscidade");
            this.Property(t => t.dsBairro).HasColumnName("dsbairro");
            this.Property(t => t.dsLogradouro).HasColumnName("dslogradouro");
            this.Property(t => t.nrEndereco).HasColumnName("nrendereco");
            this.Property(t => t.nrCEP).HasColumnName("nrcep");
            this.Property(t => t.nrTelefone).HasColumnName("nrtelefone");
            this.Property(t => t.dsWebSite).HasColumnName("dswebsite");
            this.Property(t => t.imLogo).HasColumnName("imlogo");
            this.Property(t => t.dsEmail).HasColumnName("dsemail");
            this.Property(t => t.hrInicioManha).HasColumnName("hriniciomanha");
            this.Property(t => t.hrFimManha).HasColumnName("hrfimmanha");
            this.Property(t => t.hrInicioTarde).HasColumnName("hriniciotarde");
            this.Property(t => t.hrFimTarde).HasColumnName("hrfimtarde");
        }
    }
}

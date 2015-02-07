using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class CartaoPontoMap : EntityTypeConfiguration<CartaoPonto>
    {
        public CartaoPontoMap()
        {
            // Primary Key
            this.HasKey(t => t.idPonto);

            // Properties
            this.Property(t => t.flSituacao)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.dsObservacao)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("cartaoponto");
            this.Property(t => t.idPonto).HasColumnName("idponto");
            this.Property(t => t.entradaManha).HasColumnName("entradamanha");
            this.Property(t => t.saidaManha).HasColumnName("saidamanha");
            this.Property(t => t.entraTarde).HasColumnName("entratarde");
            this.Property(t => t.saidaTarde).HasColumnName("saidatarde");
            this.Property(t => t.entradaExtra).HasColumnName("entradaextra");
            this.Property(t => t.saidaExtra).HasColumnName("saidaextra");
            this.Property(t => t.idFuncionario).HasColumnName("idfuncionario");
            this.Property(t => t.dtPonto).HasColumnName("dtponto");
            this.Property(t => t.flSituacao).HasColumnName("flsituacao");
            this.Property(t => t.dsObservacao).HasColumnName("dsobservacao");

            // Relationships
            this.HasRequired(t => t.Funcionario)
                .WithMany(t => t.CartaoPontos)
                .HasForeignKey(d => d.idFuncionario);
        }
    }
}

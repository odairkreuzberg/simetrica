using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class ContaReceberMap : EntityTypeConfiguration<ContaReceber>
    {
        public ContaReceberMap()
        {
            // Primary Key
            this.HasKey(t => t.idContaReceber);

            // Properties
            this.Property(t => t.descricao)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.situacao)
                .IsRequired()
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("contareceber");
            this.Property(t => t.idContaReceber).HasColumnName("idcontareceber");
            this.Property(t => t.idCliente).HasColumnName("idcliente");
            this.Property(t => t.parcela).HasColumnName("parcela");
            this.Property(t => t.descricao).HasColumnName("descricao");
            this.Property(t => t.flFormaPagamento).HasColumnName("flformapagamento");
            this.Property(t => t.vencimento).HasColumnName("vencimento");
            this.Property(t => t.pagamento).HasColumnName("pagamento");
            this.Property(t => t.valorConta).HasColumnName("valorconta");
            this.Property(t => t.valorPago).HasColumnName("valorpago");
            this.Property(t => t.situacao).HasColumnName("situacao");
            this.Property(t => t.idCompra).HasColumnName("idcompra");
            this.Property(t => t.idOrigem).HasColumnName("idorigem");
            this.Property(t => t.idUsuario).HasColumnName("idusuario");
            this.Property(t => t.idProjeto).HasColumnName("idprojeto");

            // Relationships
            this.HasRequired(t => t.Cliente)
                .WithMany(t => t.ContasPagar)
                .HasForeignKey(d => d.idCliente);

            // Relationships
            this.HasRequired(t => t.Projeto)
                .WithMany(t => t.ContasReceber)
                .HasForeignKey(d => d.idProjeto);

        }
    }
}

using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class ContaPagarMap : EntityTypeConfiguration<ContaPagar>
    {
        public ContaPagarMap()
        {
            // Primary Key
            this.HasKey(t => t.idContaPagar);

            // Properties
            this.Property(t => t.descricao)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.situacao)
                .IsRequired()
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("contapagar");
            this.Property(t => t.idContaPagar).HasColumnName("idcontapagar");
            this.Property(t => t.idFornecedor).HasColumnName("idfornecedor");
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
            this.Property(t => t.idFolhaPagamento).HasColumnName("idfolhapagamento");

            // Relationships
            this.HasRequired(t => t.Fornecedor)
                .WithMany(t => t.ContasPagar)
                .HasForeignKey(d => d.idFornecedor);

            this.HasRequired(t => t.Projeto)
                .WithMany(t => t.ContasPagar)
                .HasForeignKey(d => d.idProjeto);

        }
    }
}

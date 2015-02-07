using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class CaixaMap : EntityTypeConfiguration<Caixa>
    {
        public CaixaMap()
        {
            // Primary Key
            this.HasKey(t => t.idCaixa);

            // Properties
            this.Property(t => t.situacao)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.descricao)
                .IsRequired()
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("caixa");
            this.Property(t => t.idCaixa).HasColumnName("idcaixa");
            this.Property(t => t.idCaixaExtorno).HasColumnName("idcaixaextorno");
            this.Property(t => t.idContaEeceber).HasColumnName("idcontareceber");
            this.Property(t => t.idContaPagar).HasColumnName("idcontapagar");
            this.Property(t => t.idMovimento).HasColumnName("idmovimento");
            this.Property(t => t.situacao).HasColumnName("situacao");
            this.Property(t => t.valor).HasColumnName("valor");
            this.Property(t => t.saldoAnterior).HasColumnName("saldoanterior");
            this.Property(t => t.saldoAtual).HasColumnName("saldoatual");
            this.Property(t => t.idUsuario).HasColumnName("idusuario");
            this.Property(t => t.descricao).HasColumnName("descricao");
            this.Property(t => t.dtLancamento).HasColumnName("dtlancamento");

            // Relationships
            this.HasOptional(t => t.ContaPagar)
                .WithMany(t => t.Caixas)
                .HasForeignKey(d => d.idContaPagar);
            this.HasOptional(t => t.ContaReceber)
                .WithMany(t => t.Caixas)
                .HasForeignKey(d => d.idContaEeceber);
            this.HasOptional(t => t.MovimentoProfissional)
                .WithMany(t => t.Caixas)
                .HasForeignKey(d => d.idMovimento);

        }
    }
}

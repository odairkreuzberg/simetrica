using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class ProdutoMap : EntityTypeConfiguration<Produto>
    {
        public ProdutoMap()
        {
            // Primary Key
            this.HasKey(t => t.idProduto);

            // Properties
            this.Property(t => t.nome)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.descricao)
                .HasMaxLength(512);

            // Table & Column Mappings
            this.ToTable("produto");
            this.Property(t => t.idProduto).HasColumnName("idproduto");
            this.Property(t => t.idProjeto).HasColumnName("idprojeto");
            this.Property(t => t.nome).HasColumnName("nome");
            this.Property(t => t.descricao).HasColumnName("descricao");
            this.Property(t => t.idProjetista).HasColumnName("idprojetista");
            this.Property(t => t.idMarceneiro).HasColumnName("idmarceneiro");
            this.Property(t => t.vlDesconto).HasColumnName("vldesconto");
            this.Property(t => t.vlProduto).HasColumnName("vlproduto");
            this.Property(t => t.vlVenda).HasColumnName("vlvenda");
            this.Property(t => t.margemGanho).HasColumnName("margemganho");
            this.Property(t => t.porcentagemMarceneiro).HasColumnName("porcentagemmarceneiro");
            this.Property(t => t.porcentagemProjetista).HasColumnName("porcentagemprojetista");

            // Relationships
            this.HasOptional(t => t.Projetista)
                .WithMany(t => t.ProjetistaProdutos)
                .HasForeignKey(d => d.idProjetista);

            this.HasOptional(t => t.Marceneiro)
                .WithMany(t => t.MarceneiroProdutos)
                .HasForeignKey(d => d.idMarceneiro);

            this.HasRequired(t => t.projeto)
                .WithMany(t => t.Produtos)
                .HasForeignKey(d => d.idProjeto);

        }
    }
}

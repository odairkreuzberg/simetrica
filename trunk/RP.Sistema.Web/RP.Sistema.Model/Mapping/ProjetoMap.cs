using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class ProjetoMap : EntityTypeConfiguration<Projeto>
    {
        public ProjetoMap()
        {
            // Primary Key
            this.HasKey(t => t.idProjeto);

            // Properties
            this.Property(t => t.descricao)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.dsObservacao)
                .IsRequired()
                .HasMaxLength(5000);

            this.Property(t => t.dsGarantia)
                .IsRequired()
                .HasMaxLength(5000);

            this.Property(t => t.dsPrevisao)
                .IsRequired()
                .HasMaxLength(5000);

            this.Property(t => t.dsIncluso)
                .IsRequired()
                .HasMaxLength(5000);

            this.Property(t => t.dsValidade)
                .IsRequired()
                .HasMaxLength(5000);

            this.Property(t => t.dsCondicao)
                .IsRequired()
                .HasMaxLength(5000);

            this.Property(t => t.status)
                .IsRequired()
                .HasMaxLength(20);
            this.Property(t => t.flConcluido)
                .IsOptional()
                .HasMaxLength(3);

            // Table & Column Mappings
            this.ToTable("projeto");
            this.Property(t => t.idProjeto).HasColumnName("idprojeto");
            this.Property(t => t.idCliente).HasColumnName("idcliente");
            this.Property(t => t.idVendedor).HasColumnName("idvendedor");
            this.Property(t => t.idUsuario).HasColumnName("idusuario");
            this.Property(t => t.descricao).HasColumnName("descricao");
            this.Property(t => t.vlVenda).HasColumnName("vlvenda");
            this.Property(t => t.vlDesconto).HasColumnName("vldesconto");
            this.Property(t => t.vlProjeto).HasColumnName("vlprojeto");
            this.Property(t => t.status).HasColumnName("status");
            this.Property(t => t.dtInicio).HasColumnName("dtinicio");
            this.Property(t => t.flConcluido).HasColumnName("flconcluido");
            this.Property(t => t.dtFim).HasColumnName("dtfim");
            this.Property(t => t.porcentagemVendedor).HasColumnName("porcentagemvendedor");
            this.Property(t => t.dsObservacao).HasColumnName("dsobservacao");
            this.Property(t => t.dsGarantia).HasColumnName("dsgarantia");
            this.Property(t => t.dsPrevisao).HasColumnName("dsprevisao");
            this.Property(t => t.dsIncluso).HasColumnName("dsincluso");
            this.Property(t => t.dsValidade).HasColumnName("dsvalidade");
            this.Property(t => t.dsCondicao).HasColumnName("dscondicao");

            // Relationships
            this.HasRequired(t => t.Cliente)
                .WithMany(t => t.Projetos)
                .HasForeignKey(d => d.idCliente);

            this.HasRequired(t => t.Vendedor)
                .WithMany(t => t.VendedorProjetos)
                .HasForeignKey(d => d.idVendedor);

            this.HasRequired(t => t.Usuario)
                .WithMany(t => t.projetoes)
                .HasForeignKey(d => d.idUsuario);

        }
    }
}

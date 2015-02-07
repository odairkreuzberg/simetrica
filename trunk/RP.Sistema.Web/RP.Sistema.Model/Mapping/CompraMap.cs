using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class CompraMap : EntityTypeConfiguration<Compra>
    {
        public CompraMap()
        {
            // Primary Key
            this.HasKey(t => t.idCompra);

            // Properties
            this.Property(t => t.flCancelado)
                .IsRequired()
                .HasMaxLength(3);

            // Properties
            this.Property(t => t.descricao)
                .IsRequired()
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("compra");
            this.Property(t => t.idCompra).HasColumnName("idcompra");
            this.Property(t => t.idProjeto).HasColumnName("idprojeto");
            this.Property(t => t.total).HasColumnName("total");
            this.Property(t => t.dtLancamento).HasColumnName("dtlancamento");
            this.Property(t => t.descricao).HasColumnName("descricao");
            this.Property(t => t.flCancelado).HasColumnName("flcancelado");
            this.Property(t => t.idUsuario).HasColumnName("idusuario");

            this.HasOptional(t => t.Projeto)
                .WithMany(t => t.Compras)
                .HasForeignKey(d => d.idProjeto);

            this.HasRequired(t => t.Usuario)
                .WithMany(t => t.compras)
                .HasForeignKey(d => d.idUsuario);

            this.HasOptional(t => t.Fornecedor)
                .WithMany(t => t.Compras)
                .HasForeignKey(d => d.idFornecedor);

        }
    }
}

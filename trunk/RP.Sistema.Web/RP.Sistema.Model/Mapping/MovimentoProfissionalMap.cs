using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class MovimentoProfissionalMap : EntityTypeConfiguration<MovimentoProfissional>
    {
        public MovimentoProfissionalMap()
        {
            // Primary Key
            this.HasKey(t => t.idMovimento);

            // Properties
            this.Property(t => t.tipo)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.situacao)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.descricao)
                .IsRequired()
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("movimentoprofissional");
            this.Property(t => t.idMovimento).HasColumnName("idmovimento");
            this.Property(t => t.tipo).HasColumnName("tipo");
            this.Property(t => t.situacao).HasColumnName("situacao");
            this.Property(t => t.idFuncionario).HasColumnName("idfuncionario");
            this.Property(t => t.idFolhaPagamento).HasColumnName("idfolhapagamento");
            this.Property(t => t.descricao).HasColumnName("descricao");
            this.Property(t => t.valor).HasColumnName("valor");
            this.Property(t => t.dtLancamento).HasColumnName("dtlancamento");
            this.Property(t => t.dtVencimento).HasColumnName("dtvencimento");
            this.Property(t => t.idProjeto).HasColumnName("idprojeto");
            this.Property(t => t.idUsuario).HasColumnName("idusuario");

            // Relationships
            //this.HasOptional(t => t.FolhaPagamento)
            //    .WithMany(t => t.mo)
            //    .HasForeignKey(d => d.idFolhaPagamento);
            //this.HasRequired(t => t.Funcionario)
            //    .WithMany(t => t.MovimentoProfissionais)
            //    .HasForeignKey(d => d.idFuncionario);
            //this.HasOptional(t => t.Projeto)
            //    .WithMany(t => t.mo)
            //    .HasForeignKey(d => d.idProjeto);
            //this.HasRequired(t => t.Usuario)
            //    .WithMany(t => t.movimentoprofissionals)
            //    .HasForeignKey(d => d.idUsuario);

        }
    }
}

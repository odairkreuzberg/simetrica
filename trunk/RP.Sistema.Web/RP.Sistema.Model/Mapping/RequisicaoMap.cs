using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using RP.Sistema.Model.Entities;

namespace RP.Sistema.Model.Mapping
{
    public class RequisicaoMap : EntityTypeConfiguration<Requisicao>
    {
        public RequisicaoMap()
        {
            // Primary Key
            this.HasKey(t => t.idRequisicao);

            // Properties
            this.Property(t => t.dsObservacao)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("requisicao");
            this.Property(t => t.idRequisicao).HasColumnName("idrequisicao");
            this.Property(t => t.idProjeto).HasColumnName("idprojeto");
            this.Property(t => t.idFuncionario).HasColumnName("idfuncionario");
            this.Property(t => t.dsObservacao).HasColumnName("dsobservacao");
            this.Property(t => t.idUsuario).HasColumnName("idusuario");
            this.Property(t => t.dtRequisicao).HasColumnName("dtrequisicao");

            // Relationships
            this.HasRequired(t => t.Funcionario)
                .WithMany(t => t.Requisicoes)
                .HasForeignKey(d => d.idFuncionario);
            this.HasRequired(t => t.Projeto)
                .WithMany(t => t.Requisicoes)
                .HasForeignKey(d => d.idProjeto);

        }
    }
}

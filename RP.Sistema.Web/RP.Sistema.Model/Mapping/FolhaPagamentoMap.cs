using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class FolhaPagamentoMap : EntityTypeConfiguration<FolhaPagamento>
    {
        public FolhaPagamentoMap()
        {
            // Primary Key
            this.HasKey(t => t.idFolhaPagamento);

            // Properties
            this.Property(t => t.situacao)
                .IsRequired()
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("folhapagamento");
            this.Property(t => t.idFolhaPagamento).HasColumnName("idfolhapagamento");
            this.Property(t => t.idFuncionario).HasColumnName("idfuncionario");
            this.Property(t => t.total).HasColumnName("total");
            this.Property(t => t.comissao).HasColumnName("comissao");
            this.Property(t => t.salario).HasColumnName("salario");
            this.Property(t => t.bonificacao).HasColumnName("bonificacao");
            this.Property(t => t.outrosDescontos).HasColumnName("outrosdescontos");
            this.Property(t => t.inss).HasColumnName("inss");
            this.Property(t => t.vale).HasColumnName("vale");
            this.Property(t => t.dtPagamento).HasColumnName("dtpagamento");
            this.Property(t => t.situacao).HasColumnName("situacao");
            this.Property(t => t.idUsuario).HasColumnName("idusuario");
            this.Property(t => t.nrAno).HasColumnName("nrano");
            this.Property(t => t.nrMes).HasColumnName("nrmes");
            this.Property(t => t.horaExtra).HasColumnName("horaextra");

            // Relationships
            this.HasRequired(t => t.Funcionario)
                .WithMany(t => t.FolhaPagamentos)
                .HasForeignKey(d => d.idFuncionario);
            this.HasRequired(t => t.Usuario)
                .WithMany(t => t.folhapagamentoes)
                .HasForeignKey(d => d.idUsuario);

        }
    }
}

using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class FuncionarioMap : EntityTypeConfiguration<Funcionario>
    {
        public FuncionarioMap()
        {
            // Primary Key
            this.HasKey(t => t.idFuncionario);

            // Properties
            this.Property(t => t.nome)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.tipo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.rg)
                .HasMaxLength(20);

            this.Property(t => t.flMensalista)
                .HasMaxLength(3);

            this.Property(t => t.cpf)
                .HasMaxLength(20);

            this.Property(t => t.email)
                .HasMaxLength(150);

            this.Property(t => t.observacao)
                .HasMaxLength(200);

            this.Property(t => t.numero)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.cep)
                .HasMaxLength(10);

            this.Property(t => t.logradouro)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.bairro)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.fone)
                .HasMaxLength(20);

            this.Property(t => t.celular)
                .HasMaxLength(20);

            this.Property(t => t.motivoSaida)
                .HasMaxLength(250);

            this.Property(t => t.status)
                .HasMaxLength(20);

            this.Property(t => t.ctps)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("funcionario");
            this.Property(t => t.idFuncionario).HasColumnName("idfuncionario");
            this.Property(t => t.nome).HasColumnName("nome");
            this.Property(t => t.tipo).HasColumnName("tipo");
            this.Property(t => t.rg).HasColumnName("rg");
            this.Property(t => t.cpf).HasColumnName("cpf");
            this.Property(t => t.email).HasColumnName("email");
            this.Property(t => t.observacao).HasColumnName("observacao");
            this.Property(t => t.numero).HasColumnName("numero");
            this.Property(t => t.cep).HasColumnName("cep"); ;
            this.Property(t => t.flMensalista).HasColumnName("flmensalista");
            this.Property(t => t.logradouro).HasColumnName("logradouro");
            this.Property(t => t.bairro).HasColumnName("bairro");
            this.Property(t => t.idCidade).HasColumnName("idcidade");
            this.Property(t => t.fone).HasColumnName("fone");
            this.Property(t => t.celular).HasColumnName("celular");
            this.Property(t => t.dtNascimento).HasColumnName("dtnascimento");
            this.Property(t => t.dtEntrada).HasColumnName("dtentrada");
            this.Property(t => t.salario).HasColumnName("salario");
            this.Property(t => t.comissao).HasColumnName("comissao");
            this.Property(t => t.idUsuario).HasColumnName("idusuario");
            this.Property(t => t.dtSaida).HasColumnName("dtsaida");
            this.Property(t => t.motivoSaida).HasColumnName("motivosaida");
            this.Property(t => t.status).HasColumnName("status");
            this.Property(t => t.ctps).HasColumnName("ctps");
            this.Property(t => t.nrCargaHoraria).HasColumnName("nrcargahoraria");

            // Relationships
            this.HasRequired(t => t.Cidade)
                .WithMany(t => t.Funcionarios)
                .HasForeignKey(d => d.idCidade);
            this.HasOptional(t => t.Usuario)
                .WithMany(t => t.funcionarios)
                .HasForeignKey(d => d.idUsuario);

        }
    }
}

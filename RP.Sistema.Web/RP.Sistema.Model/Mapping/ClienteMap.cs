using RP.Sistema.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace RP.Sistema.Model.Mapping
{
    public class ClienteMap : EntityTypeConfiguration<Cliente>
    {
        public ClienteMap()
        {
            // Primary Key
            this.HasKey(t => t.idCliente);

            // Properties
            this.Property(t => t.nome)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.tipo)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.documento)
                .HasMaxLength(20);

            this.Property(t => t.email)
                .HasMaxLength(150);

            this.Property(t => t.site)
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

            this.Property(t => t.contato)
                .HasMaxLength(100);

            this.Property(t => t.foneContato)
                .HasMaxLength(20);

            this.Property(t => t.celularContato)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("cliente");
            this.Property(t => t.idCliente).HasColumnName("idcliente");
            this.Property(t => t.nome).HasColumnName("nome");
            this.Property(t => t.tipo).HasColumnName("tipo");
            this.Property(t => t.documento).HasColumnName("cnpjcpf");
            this.Property(t => t.email).HasColumnName("email");
            this.Property(t => t.site).HasColumnName("site");
            this.Property(t => t.observacao).HasColumnName("observacao");
            this.Property(t => t.numero).HasColumnName("numero");
            this.Property(t => t.cep).HasColumnName("cep");
            this.Property(t => t.logradouro).HasColumnName("logradouro");
            this.Property(t => t.bairro).HasColumnName("bairro");
            this.Property(t => t.idCidade).HasColumnName("idcidade");
            this.Property(t => t.contato).HasColumnName("contato");
            this.Property(t => t.foneContato).HasColumnName("fonecontato");
            this.Property(t => t.celularContato).HasColumnName("celularcontato");

            // Relationships
            this.HasRequired(t => t.Cidade)
                .WithMany(t => t.Clientes)
                .HasForeignKey(d => d.idCidade);

        }
    }
}

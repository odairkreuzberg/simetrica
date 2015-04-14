using RP.Sistema.Model.Mapping;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace RP.Sistema.Model
{
    public partial class Context : DbContext, RP.DataAccess.Interfaces.IContext
    {
        public DbSet<Entities.Acao> Acoes { get; set; }
        public DbSet<Entities.Area> Areas { get; set; }
        public DbSet<Entities.Controle> Controles { get; set; }
        public DbSet<Entities.Menu> Menus { get; set; }
        public DbSet<Entities.Modulo> Modulos { get; set; }
        public DbSet<Entities.ModuloUsuario> ModulosUsuario { get; set; }
        public DbSet<Entities.Perfil> Perfis { get; set; }
        public DbSet<Entities.PerfilAcao> PerfisAcao { get; set; }
        public DbSet<Entities.PerfilUsuario> PerfisUsuario { get; set; }
        public DbSet<Entities.Usuario> Usuarios { get; set; }
        public DbSet<Entities.Entidade> Entidades { get; set; }
        public DbSet<Entities.Parametro> Parametros { get; set; }
        public DbSet<Entities.LogDado> LogDados { get; set; }


        public DbSet<Entities.Pais> Paises { get; set; }
        public DbSet<Entities.Estado> Estados { get; set; }
        public DbSet<Entities.Cidade> Cidades { get; set; }
        public DbSet<Entities.Feriado> Feriados { get; set; }
        public DbSet<Entities.HoraExtra> HoraExtras { get; set; }

        public DbSet<Entities.Funcionario> Funcionarios { get; set; }
        public DbSet<Entities.Fornecedor> Fornecedores { get; set; }
        public DbSet<Entities.FornecedorFone> FornecedorFones { get; set; }
        public DbSet<Entities.Cliente> Clientes { get; set; }
        public DbSet<Entities.ClienteFone> ClienteFones { get; set; }

        public DbSet<Entities.Fabricante> Fabricantes { get; set; }
        public DbSet<Entities.Material> Materiais { get; set; }
        public DbSet<Entities.UnidadeMedida> UnidadeMedidas { get; set; }

        public DbSet<Entities.Requisicao> Requisicoes { get; set; }
        public DbSet<Entities.RequisicaoItem> RequisicaoItens { get; set; }
        public DbSet<Entities.Projeto> Projetos { get; set; }
        public DbSet<Entities.ContaPagar> ContasPagar { get; set; }
        public DbSet<Entities.Caixa> Caixas { get; set; }
        public DbSet<Entities.ContaReceber> ContasReceber { get; set; }
        public DbSet<Entities.FolhaPagamento> FolhaPagamentos { get; set; }
        public DbSet<Entities.MovimentoProfissional> MovimentoProfissionais { get; set; }
        public DbSet<Entities.Produto> Produtos { get; set; }
        public DbSet<Entities.ProdutoMaterial> ProdutoMateriais { get; set; }
        public DbSet<Entities.Compra> Compras { get; set; }
        public DbSet<Entities.ProjetoCusto> ProjetoCustos { get; set; }
        public DbSet<Entities.ItemCompra> ItensCompra { get; set; }
        public DbSet<Entities.CartaoPonto> CartaoPontos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            this.Configuration.LazyLoadingEnabled = false;
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Configurations.Add(new ItemCompraMap());
            modelBuilder.Configurations.Add(new AcaoMap());
            modelBuilder.Configurations.Add(new AreaMap());
            modelBuilder.Configurations.Add(new ControleMap());
            modelBuilder.Configurations.Add(new EntidadeMap());
            modelBuilder.Configurations.Add(new MenuMap());
            modelBuilder.Configurations.Add(new ModuloMap());
            modelBuilder.Configurations.Add(new ModuloUsuarioMap());
            modelBuilder.Configurations.Add(new PerfilMap());
            modelBuilder.Configurations.Add(new PerfilAcaoMap());
            modelBuilder.Configurations.Add(new PerfilUsuarioMap());
            modelBuilder.Configurations.Add(new UsuarioMap());
            modelBuilder.Configurations.Add(new TabelaMap());
            modelBuilder.Configurations.Add(new ParametroMap());
            modelBuilder.Configurations.Add(new HoraExtraMap());
            modelBuilder.Configurations.Add(new FeriadoMap());

            modelBuilder.Configurations.Add(new PaisMap());
            modelBuilder.Configurations.Add(new EstadoMap());
            modelBuilder.Configurations.Add(new CidadeMap());

            modelBuilder.Configurations.Add(new FuncionarioMap());
            modelBuilder.Configurations.Add(new FornecedorMap());
            modelBuilder.Configurations.Add(new FornecedorFoneMap());
            modelBuilder.Configurations.Add(new ClienteMap());
            modelBuilder.Configurations.Add(new ClienteFoneMap());

            modelBuilder.Configurations.Add(new FabricanteMap());
            modelBuilder.Configurations.Add(new MaterialMap());
            modelBuilder.Configurations.Add(new UnidadeMedidaMap());

            modelBuilder.Configurations.Add(new ProjetoMap());
            modelBuilder.Configurations.Add(new RequisicaoMap());
            modelBuilder.Configurations.Add(new RequisicaoItemMap());
            modelBuilder.Configurations.Add(new ContaPagarMap());
            modelBuilder.Configurations.Add(new CaixaMap());
            modelBuilder.Configurations.Add(new ContaReceberMap());
            modelBuilder.Configurations.Add(new FolhaPagamentoMap());
            modelBuilder.Configurations.Add(new MovimentoProfissionalMap());
            modelBuilder.Configurations.Add(new ProdutoMap());
            modelBuilder.Configurations.Add(new ProdutoMaterialMap());
            modelBuilder.Configurations.Add(new CompraMap());
            modelBuilder.Configurations.Add(new ProjetoCustoMap());
            modelBuilder.Configurations.Add(new CartaoPontoMap());
            modelBuilder.Configurations.Add(new LogDadoMap());


            base.OnModelCreating(modelBuilder);
        }

    }
}

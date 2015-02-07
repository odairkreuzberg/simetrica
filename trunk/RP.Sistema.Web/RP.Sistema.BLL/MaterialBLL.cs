using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class MaterialBLL : RP.DataAccess.Repository<Material>
    {
        public MaterialBLL(IContext db, int idUsuario) : base(db, idUsuario) { }
        protected override void ValidUpdate(Material bean)
        {
            if (this.Exist(u => u.nome.ToLower() == bean.nome.ToLower() && u.idMaterial != bean.idMaterial))
            {
                throw new Exception("Esta material já esta cadastrado");
            }
        }
        protected override void ValidInsert(Material bean)
        {
            if (this.Exist(u => u.nome.ToLower() == bean.nome.ToLower()))
            {
                throw new Exception("Esta material já esta cadastrado");
            }
        }
        protected override void BeforeInsert(Material bean)
        {
            bean.nrQuantidade = 0;
        }

        public override void Update(Material bean)
        {
            this.ValidUpdate(bean);
            this.BeforeUpdate(bean);
            ((Model.Context)db).Materiais.Attach(bean);
            ((Model.Context)db).Entry(bean).Property(e => e.nrQuantidade).IsModified = true;
            ((Model.Context)db).Entry(bean).Property(e => e.nome).IsModified = true;
            ((Model.Context)db).Entry(bean).Property(e => e.idFabricante).IsModified = true;
            ((Model.Context)db).Entry(bean).Property(e => e.idUnidadeMedida).IsModified = true;
            ((Model.Context)db).Entry(bean).Property(e => e.preco).IsModified = true;
            ((Model.Context)db).Entry(bean).Property(e => e.margemGanho).IsModified = true;
            this.BeforeUpdate(bean);
        }

        public RP.DataAccess.PaginatedList<Material> Search(string filter, string saldo, int? page, int? pagesize)
        {
            IQueryable<Material> query = preSearch(filter);
            if (saldo != "todos")
            {
                query = query.Where(u => u.nrQuantidade > 0);
            }
            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Material>(query.OrderBy(o => o.idMaterial), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<Material> Search(string filter)
        {
            IQueryable<Material> query = preSearch(filter);

            return query.ToList();
        }

        private IQueryable<Material> preSearch(string filter)
        {
            IQueryable<Material> query = this.Find(null, u => u.Fabricante, u => u.UnidadeMedida);

            if (!string.IsNullOrEmpty(filter))
            {
                foreach (string word in filter.NSplit(' '))
                {
                    string temp = word.ToLower();
                    query = query.Where(p => p.nome.ToLower().Contains(temp));
                }
            }
            return query;
        }

        internal void AtualizarSaldo(decimal quantidade, decimal valor, int idMaterial)
        {
            var bean = this.FindSingle(u => u.idMaterial == idMaterial);

            bean.preco = valor;
            bean.nrQuantidade += quantidade;

            ((Model.Context)db).Materiais.Attach(bean);
            ((Model.Context)db).Entry(bean).Property(e => e.nrQuantidade).IsModified = true;
            ((Model.Context)db).Entry(bean).Property(e => e.preco).IsModified = true;
        }

        internal decimal AtualizarSaldo(decimal quantidade, int idMaterial)
        {
            var bean = this.FindSingle(u => u.idMaterial == idMaterial);

            bean.nrQuantidade -= quantidade;

            ((Model.Context)db).Materiais.Attach(bean);
            ((Model.Context)db).Entry(bean).Property(e => e.nrQuantidade).IsModified = true;

            return bean.preco;
        }
    }
}

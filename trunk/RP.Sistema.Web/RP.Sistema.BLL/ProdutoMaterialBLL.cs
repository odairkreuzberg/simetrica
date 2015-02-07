using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class ProdutoMaterialBLL : RP.DataAccess.Repository<ProdutoMaterial>
    {
        public ProdutoMaterialBLL(IContext db, int idUsuario) : base(db, idUsuario) { }

        protected override void BeforeInsert(ProdutoMaterial bean)
        {
            var _bll = new MaterialBLL(db, _idUsuario);
            var _material = _bll.FindSingle(u => u.idMaterial == bean.idMaterial);

            _material.margemGanho = bean.margemGanho;
            _material.preco = bean.valor;

            bean.Material = _material;

            _bll.Update(_material);
        }

        protected override void BeforeUpdate(ProdutoMaterial bean)
        {
            var _bll = new MaterialBLL(db, _idUsuario);
            var _material = _bll.FindSingle(u => u.idMaterial == bean.idMaterial);

            _material.margemGanho = bean.margemGanho;
            _material.preco = bean.valor;

            bean.Material = _material;

            _bll.Update(_material);
        }

        protected override void BeforeDelete(ProdutoMaterial bean)
        {
            if (this.Exist(u => u.Produto.projeto.status == Projeto.CONCLUIDO))
            {
                throw new Exception("Este projeto já esta conluido, atualize a pagina");
            }
        }
    }

}

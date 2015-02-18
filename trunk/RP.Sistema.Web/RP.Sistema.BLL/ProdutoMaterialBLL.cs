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

        public void Orcamento(int? idProduto, int? idprojeto, List<Material> materiais)
        {
            var _materialBLL = new MaterialBLL(db, _idUsuario);
            var produtoMateriasDB = this.Find(u => u.idProduto == idProduto).ToList();
            foreach (var item in materiais.Where(u => produtoMateriasDB.All(k => k.idMaterial != u.idMaterial)))
            {
                var material = _materialBLL.Find(u => u.idMaterial == item.idMaterial)
                    .Select(u => new 
                    {
                        u.margemGanho, 
                        u.preco
                    }).FirstOrDefault();
                var newItem = new ProdutoMaterial 
                { 
                    idProduto = idProduto.Value,
                    idMaterial = item.idMaterial,
                    quantidade = item.nrQuantidade,
                    valor = material.preco,
                    margemGanho = material.margemGanho
                };
                this.Insert(newItem);
            }
            foreach (var item in produtoMateriasDB.Where(u => materiais.All(k => k.idMaterial != u.idMaterial)))
            {
                this.Delete(item);
            }
            foreach (var item in produtoMateriasDB)
            {
                var itemVW = materiais.FirstOrDefault(k => k.idMaterial == item.idMaterial);
                if (itemVW != null)
                {
                    item.quantidade = itemVW.nrQuantidade;
                    this.Update(item);
                }
            }
            db.SaveChanges(_idUsuario);
            var _projetoBLL = new ProjetoBLL(db, _idUsuario);
            _projetoBLL.Atualizar(idprojeto);
        }
    }

}

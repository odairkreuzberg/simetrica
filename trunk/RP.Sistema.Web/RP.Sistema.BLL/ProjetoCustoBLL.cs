using RP.DataAccess.Interfaces;
using RP.Sistema.Model;
using RP.Sistema.Model.Entities;
using RP.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class ProjetoCustoBLL : RP.DataAccess.Repository<ProjetoCusto>
    {
        public ProjetoCustoBLL(IContext db, int idUsuario) : base(db, idUsuario) { }
        protected override void BeforeInsert(ProjetoCusto bean)
        {
            bean.situacao = ProjetoCusto.NORMAL;
            if (bean.gerarConta == "Sim")
            {
                var _contaPagarBLL = new ContaPagarBLL(db, _idUsuario);
                var _caixaBLL = new CaixaBLL(db, _idUsuario);
                var _contaPagar = new ContaPagar
                { 
                    descricao = bean.descricao, 
                    parcela = 1,
                    situacao = ContaPagar.SITUACAO_PAGO,
                    vencimento = (DateTime)bean.dtCusto,
                    pagamento = (DateTime)bean.dtCusto,
                    valorConta = bean.valor,
                    valorPago = bean.valor,
                    idUsuario = this._idUsuario,
                    flFormaPagamento = "Dinheiro"
                };

                _contaPagarBLL.Insert(_contaPagar);

                var _caixa = new Caixa
                {
                    ContaPagar = _contaPagar,
                    descricao = bean.descricao,
                    valor = (bean.valor* (-1)),
                    situacao = Caixa.CORENTE,
                    dtLancamento = DateTime.Now,
                    idUsuario = this._idUsuario                    
                };
                _caixaBLL.Insert(_caixa);
                db.SaveChanges(this._idUsuario);
                bean.idContaPagar = _contaPagar.idContaPagar;
            }
        }

        internal void Cancelar(ProjetoCusto bean)
        {
            bean.situacao = ProjetoCusto.CANCELADO;
            this.Update(bean);

            if (bean.idContaPagar != null)
            {
                var _contaBLL = new ContaPagarBLL(db, _idUsuario);
                var _caixaBLL = new CaixaBLL(db, _idUsuario);

                var _conta = _contaBLL.FindSingle(u => u.idContaPagar == bean.idContaPagar);
                var _caixa = _caixaBLL.FindSingle(u => u.idContaPagar == _conta.idContaPagar);

                _conta.situacao = ContaPagar.SITUACAO_CANCELADO;

                _contaBLL.Update(_conta);

                if (_caixa.situacao == Caixa.CORENTE)
                {
                    _caixaBLL.Update(_caixa);
                    _caixa.situacao = Caixa.CANCELADO;
                    _caixaBLL.Insert(new Caixa
                    {
                        idCaixaExtorno = _caixa.idCaixa,
                        descricao = "Cancelamento de caixa referente ao lançamento de custo para projeto. " + _conta.descricao,
                        dtLancamento = DateTime.Now,
                        situacao = Caixa.EXTORNO,
                        valor = (bean.valor)
                    });
                }
            }
        }
    }
}

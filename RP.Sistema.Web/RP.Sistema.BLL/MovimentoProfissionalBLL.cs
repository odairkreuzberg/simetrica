using RP.DataAccess.Interfaces;
using RP.Sistema.Model.Entities;
using RP.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RP.Sistema.BLL
{
    public class MovimentoProfissionalBLL : RP.DataAccess.Repository<MovimentoProfissional>
    {
        public MovimentoProfissionalBLL(IContext db, int idUsuario) : base(db, idUsuario) { }

        protected override void BeforeInsert(MovimentoProfissional bean)
        {
            bean.dtLancamento = DateTime.Now;
            bean.idUsuario = this._idUsuario;
        }

        public void AtualizaMovimento(int idMovimento, string situacao, FolhaPagamento folha = null)
        {
            var movimento = this.FindSingle(u => u.idMovimento == idMovimento);
            movimento.situacao = situacao;
            movimento.FolhaPagamento = folha;
            this.Update(movimento);
        }
    }
}

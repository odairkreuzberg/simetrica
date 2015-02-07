using System.Collections.Generic;
using System.Linq;
using System.Data;
using RP.Sistema.Model.Entities;
using RP.Util;
using RP.DataAccess.Interfaces;
using System.Web;
using ImageProcessor;
using System.IO;

namespace RP.Sistema.BLL
{
    public class EntidadeBLL : RP.DataAccess.Repository<Entidade>
    {

        public EntidadeBLL(IContext db, int idUsuario) : base(db, idUsuario) { }

        public override void Update(Entidade entity)
        {
            ValidUpdate(entity);
            BeforeUpdate(entity);
            this.db.ChangeState(entity, System.Data.EntityState.Modified);
            ((Model.Context)db).Entry(entity).Property(e => e.imLogo).IsModified = false;
            AfterUpdate(entity);
        }

        public void UpdateLogo(Entidade entity)
        {
            ((Model.Context)db).Entry(entity).Property(e => e.imLogo).IsModified = true;
        }

        public void RemoveLogo(Entidade entidade)
        {
            ((Model.Context)db).Entry(entidade).Property(e => e.imLogo).IsModified = true;
        }

        public RP.DataAccess.PaginatedList<Entidade> Search(string filter, int? page, int? pagesize)
        {
            IQueryable<Entidade> query = preSearch(filter);

            var result = new RP.DataAccess.PaginatedList<RP.Sistema.Model.Entities.Entidade>(query.OrderBy(o => o.idEntidade), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

            return result;
        }

        public ICollection<Entidade> Search(string filter)
        {
            IQueryable<Entidade> query = preSearch(filter);

            return query.ToList();
        }

        private IQueryable<Entidade> preSearch(string filter)
        {

            IQueryable<Entidade> query = this.Find();

	        if (!string.IsNullOrEmpty(filter))
            {
                //filter = RP.Util.Class.Fonetiza.Fonetizar(filter);

                foreach (string word in filter.NSplit(' '))
                {
                    string temp = word;
                    query = query.Where(p => p.nmRazaoSocial.ToLower().Contains(temp.ToLower()));
                }
            }
            return query;
        }

      

        public static DataSet getDtSetEntidade(RP.Sistema.Model.Context db)
        {
            Dictionary<string, System.Data.DataSet> listData = new Dictionary<string, System.Data.DataSet>();
            System.Data.DataSet dsHeader = new System.Data.DataSet("subEntidade");
            System.Data.DataTable table = new System.Data.DataTable("table");
            System.Data.DataRow row;

            try
            {
                var e = from entidade in db.Entidades
                        select entidade;

                table.Columns.Add(new System.Data.DataColumn("identidade", System.Type.GetType("System.Int32")));
                table.Columns.Add(new System.Data.DataColumn("nmrazaosocial", System.Type.GetType("System.String")));
                table.Columns.Add(new System.Data.DataColumn("nmfantasia", System.Type.GetType("System.String")));
                table.Columns.Add(new System.Data.DataColumn("nrcnpj", System.Type.GetType("System.String")));
                table.Columns.Add(new System.Data.DataColumn("dscidade", System.Type.GetType("System.String")));
                table.Columns.Add(new System.Data.DataColumn("dsbairro", System.Type.GetType("System.String")));
                table.Columns.Add(new System.Data.DataColumn("dslogradouro", System.Type.GetType("System.String")));
                table.Columns.Add(new System.Data.DataColumn("nrendereco", System.Type.GetType("System.String")));
                table.Columns.Add(new System.Data.DataColumn("nrcep", System.Type.GetType("System.String")));
                table.Columns.Add(new System.Data.DataColumn("nrtelefone", System.Type.GetType("System.String")));
                table.Columns.Add(new System.Data.DataColumn("dswebsite", System.Type.GetType("System.String")));
                table.Columns.Add(new System.Data.DataColumn("imlogo", System.Type.GetType("System.Byte[]")));
                
                /* Carrega os dados */
                foreach (Entidade entidade in e)
                {
                    row = table.NewRow();
                    row["identidade"] = entidade.idEntidade;
                    row["nmrazaosocial"] = entidade.nmRazaoSocial;
                    row["nmfantasia"] = entidade.nmFantasia;
                    row["nrcnpj"] = entidade.nrCNPJ;
                    row["dscidade"] = entidade.dsCidade;
                    row["dsbairro"] = entidade.dsBairro;
                    row["dslogradouro"] = entidade.dsLogradouro;
                    row["nrendereco"] = entidade.nrEndereco;
                    row["nrcep"] = entidade.nrCEP;
                    row["nrtelefone"] = entidade.nrTelefone;
                    row["dswebsite"] = entidade.dsWebSite;
                    row["imlogo"] = entidade.imLogo;
                    table.Rows.Add(row);
                }
                dsHeader.Tables.Add(table);
                return dsHeader;
            }
            finally
            {
                dsHeader.Dispose();
            }
        }
    }
}

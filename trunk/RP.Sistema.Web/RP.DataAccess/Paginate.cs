using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RP.DataAccess
{
    public class Paginate<T> where T : class
    {
        public class Filter
        {
            public Filter()
            {
                this.predicates = new List<Expression<Func<T, bool>>>();
            }

            public List<Expression<Func<T, bool>>> predicates { get; set; }
        }

        public RP.DataAccess.PaginatedList<T> Paging<TKey>(IQueryable<T> source, int? page, int? pagesize, Expression<Func<T, TKey>> key)
        { 
            return new RP.DataAccess.PaginatedList<T>(source.OrderBy(key), (page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE)), (pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE)));
        }

        public virtual RP.DataAccess.PaginatedList<T> Search<TKey>(IQueryable<T> source, Filter filters, int? page, int? pagesize, Expression<Func<T, TKey>> key)
        {
            return Paging(this.Search(source, filters), page, pagesize, key);
        }

        public virtual IQueryable<T> Search(IQueryable<T> source, Paginate<T>.Filter filters)
        {
            IQueryable<T> query = source;
            if (filters != null)
            {
                foreach (var predicate in filters.predicates)
                {
                    query = query.Where(predicate);
                }
            }
            return query;
        }

    }

}


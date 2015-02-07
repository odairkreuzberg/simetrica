using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RP.DataAccess
{
    public static class Extension
    {
        public static RP.DataAccess.PaginatedList<T> ToPaginatedList<T>(this IOrderedEnumerable<T> list, int pageIndex, int pageSize, int totalCount, int totalPages)
        {
            var _result = new RP.DataAccess.PaginatedList<T>(pageIndex, pageSize, totalCount, totalPages);
            foreach (var item in list)
            {
                _result.Add(item);
            }
            return _result;
        }
    }
}

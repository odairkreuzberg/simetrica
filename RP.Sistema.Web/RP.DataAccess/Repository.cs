using System;
using System.Data.Entity;
using System.Linq;
using RP.DataAccess.Interfaces;

namespace RP.DataAccess
{
    //[Export(typeof(IRepository<>))]
    public class Repository<T> : Paginate<T>, IRepository<T> where T : class
    {
        private IContext _db;

        protected virtual void BeforeInsert(T bean) { }
        protected virtual void AfterInsert(T bean) { }
        protected virtual void BeforeUpdate(T bean) { }
        protected virtual void AfterUpdate(T bean) { }
        protected virtual void BeforeDelete(T bean) { }
        protected virtual void AfterDelete(T bean) { }

        protected virtual void ValidInsert(T bean) { }
        protected virtual void ValidUpdate(T bean) { }
        protected virtual void ValidDelete(T bean) { }

        public Repository(IContext db, int idUsuario)
        {
            _db = db;
            _idUsuario = idUsuario;
        }

        public IContext db { get { return _db; } set { _db = value; } }
        public int _idUsuario { get; set; }

        public virtual void Insert(T entity)
        {
            ValidInsert(entity);
            BeforeInsert(entity);
            this.db.GetEntitySet<T>().Add(entity);
            AfterInsert(entity);
        }

        public virtual void Update(T entity)
        {
            ValidUpdate(entity);
            BeforeUpdate(entity);
            this.db.ChangeState(entity, System.Data.EntityState.Modified);
            AfterUpdate(entity);
        }
        
        public T FindSingle(System.Linq.Expressions.Expression<Func<T, bool>> predicate = null, params System.Linq.Expressions.Expression<Func<T, object>>[] includes)
        {
            var set = FindIncluding(includes);
            return (predicate == null) ?
                   set.FirstOrDefault() :
                   set.FirstOrDefault(predicate);
        }

        public IQueryable<T> Find(System.Linq.Expressions.Expression<Func<T, bool>> predicate = null, params System.Linq.Expressions.Expression<Func<T, object>>[] includes)
        {
            var set = FindIncluding(includes);
            return (predicate == null) ? set : set.Where(predicate);
        }
        //public RP.DataAccess.PaginatedList<T> Search(string filter, int? page, int? pagesize)
        //{
        //    IQueryable<T> query = preSearch(filter);
        //    Type type = typeof(T);
        //    var field = type.GetEnumValues().GetValue(0);
        //    var result = new RP.DataAccess.PaginatedList<T>(query.OrderBy(u => field), page ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGE), pagesize ?? int.Parse(RP.Util.Resource.Message.DEFAULT_PAGESIZE));

        //    return result;
        //}
        //private IQueryable<T> preSearch(string filter)
        //{
        //    IQueryable<T> query = this.Find(null);

        //    if (!string.IsNullOrEmpty(filter))
        //    {
        //        foreach (string word in filter.Split(' '))
        //        {
        //            string temp = word.ToLower();
        //            query = query.Where(p => p.ToString().ToLower().Contains(temp));
        //        }
        //    }
        //    return query;
        //}

        public IQueryable<T> FindIncluding(params System.Linq.Expressions.Expression<Func<T, object>>[] includeProperties)
        {
            var set = this.db.GetEntitySet<T>();
            IQueryable<T> query = set.AsQueryable();

            if (includeProperties != null)
            {
                foreach (var include in includeProperties)
                {
                    query = query.Include(include);
                }
            }
            return query;
        }

        public int Count(System.Linq.Expressions.Expression<Func<T, bool>> predicate = null)
        {
            var set = this.db.GetEntitySet<T>();
            return (predicate == null) ?
                   set.Count() :
                   set.Count(predicate);
        }

        public bool Exist(System.Linq.Expressions.Expression<Func<T, bool>> predicate = null)
        {
            var set = this.db.GetEntitySet<T>();
            return (predicate == null) ? set.Any() : set.Any(predicate);
        }

        public virtual void Delete(T entity)
        {
            ValidDelete(entity);
            BeforeDelete(entity);
            this.db.ChangeState(entity, System.Data.EntityState.Deleted);
            AfterDelete(entity);
        }

        public virtual void Delete(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            
            var entity = this.FindSingle(predicate);
            this.Delete(entity);
        }

        public int SaveChanges()
        {
            return _db.SaveChanges(_idUsuario);
        }

    }
}

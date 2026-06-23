using DAL.Data;
using DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;



namespace DAL.Repository.Implementations
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {


        private readonly ApplicationDbContext _context;
        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Add(T entity)
        {
           _context.Set<T>().Add(entity);
        }
        // Hard Delete 
        // Soft Delete > Service , Booking , User 
        public void Delete(T entity)
        {
            
               _context.Set<T>().Remove(entity);
            
        }


        private IQueryable<T> ApplyIncludes(IQueryable<T> query , Expression<Func<T, object>>[] includes)
        {
            if(includes?.Length > 0)
            {
                foreach (var include in includes)
                    query = query.Include(include);
                
            }

            return query;
        }

        public Task<T> GetAsync(Expression<Func<T, bool>> expression  , params Expression<Func<T , object>>[] includes )
        {
            IQueryable<T> Query = _context.Set<T>();

            Query = ApplyIncludes(Query, includes);

            return Query.FirstOrDefaultAsync(expression);


        }


        public Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().AnyAsync(expression);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }


        public  Task<List<T>> FindAllAsync(Expression<Func<T, bool>> expression ,  params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> Query = _context.Set<T>();
            Query = ApplyIncludes(Query, includes);
            return Query.Where(expression).ToListAsync();
        }

        public Task<List<T>> GetAllAsync(bool ignoreSoftDelete = false, int? skip = null, int? take = null, params Expression<Func<T, object>>[] includes)
        {
          IQueryable<T> Query = _context.Set<T>();

            Query = ApplyIncludes(Query, includes);

            if (ignoreSoftDelete)
            {
                Query = Query.IgnoreQueryFilters();
            }

            if (take.HasValue && skip.HasValue) { 
            
            return Query.AsNoTracking().Skip(skip.Value).Take(take.Value).ToListAsync();

            }

            return Query.AsNoTracking().ToListAsync();



        }
    }
}

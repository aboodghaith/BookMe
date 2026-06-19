using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);

        Task<T> GetAsync(Expression<Func<T , bool>> expression , params Expression<Func<T, object>>[] includes);

        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);


        Task<List<T>> GetAllAsync(bool ignoreSoftDelete = false , int? take = null , int? size = null , params Expression<Func<T, object>>[] includes); 
        void Update(T entity);

        void Delete(T entity);


        Task<List<T>> FindAllAsync(Expression<Func<T, bool>> expression , params Expression<Func<T, object>>[] includes);

    }
}

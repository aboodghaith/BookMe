using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUserAsync(bool IgnoreUserDeleted = true, Expression<Func<User, bool>> expression = null, params Expression<Func<User, object>>[] includes);

        Task<List<User>> GetAllUserWithPaginationAsync(int pageNumber, int pageSize, bool IgnoreUserDeleted = true, Expression<Func<User, bool>> expression = null,
       params Expression<Func<User, object>>[] includes);


        public void Update(User entity);

    }
}

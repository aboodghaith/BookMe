using DAL.Data;
using DAL.Models;
using DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region common methode

        private IQueryable<User> ApplyIncludes(IQueryable<User> query, Expression<Func<User, object>>[] includes)
        {
            if (includes?.Length > 0)
            {
                foreach (var include in includes)
                    query = query.Include(include);

            }

            return query;
        }



        #endregion






        public Task<List<User>> GetAllUserAsync(bool IgnoreUserDeleted = true, Expression<Func<User, bool>> expression = null, params Expression<Func<User, object>>[] includes)
        {
            IQueryable<User> Query = _context.Set<User>();

            Query = ApplyIncludes(Query, includes);

            if (!IgnoreUserDeleted)
            {
                Query = Query.IgnoreQueryFilters();
            }

            if (expression != null)
            {
                Query = Query.Where(expression);
            }

            return Query.AsNoTracking().ToListAsync();
        }



        public Task<List<User>> GetAllUserWithPaginationAsync(int pageNumber, int pageSize, bool IgnoreUserDeleted = true, Expression<Func<User, bool>> expression = null, params Expression<Func<User, object>>[] includes)
        {
            IQueryable<User> Query = _context.Set<User>();

            Query = ApplyIncludes(Query, includes);

            if (!IgnoreUserDeleted)
            {
                Query = Query.IgnoreQueryFilters();
            }

            if (expression != null)
            {
                Query = Query.Where(expression);
            }

            return Query
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }


        public void Update(User entity)
        {
            _context.Users.Update(entity);
        }
    }
}

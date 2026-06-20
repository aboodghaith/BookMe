using DAL.Data;
using DAL.Models;
using DAL.Repository.Implementations;
using DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IRepository<Service> ServiceRepo { get; private set; }

        public IRepository<Booking> BookingRepo { get; private set; }

        public IUserRepository UserRepository { get; private set; }

        public IRepository<Category> CategoryRepository { get; private set; }

        public IRepository<City> CityRepository { get; private set;}

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            ServiceRepo = new BaseRepository<Service>(context);

            BookingRepo = new BaseRepository<Booking>(context);

            UserRepository = new UserRepository(context);


            CategoryRepository = new BaseRepository<Category>(context);

            CityRepository = new BaseRepository<City>(context);

        }


        public Task<int> SaveChanges()
        {
            return _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}

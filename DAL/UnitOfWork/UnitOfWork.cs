using DAL.Data;
using DAL.Models;
using DAL.Repository.Implementations;
using DAL.Repository.Interfaces;
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


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            ServiceRepo = new BaseRepository<Service>(_context);

            BookingRepo = new BaseRepository<Booking>(_context); 
        }


        public Task<int> SaveChanges()
        {
            return _context.SaveChangesAsync();
        }
    }
}

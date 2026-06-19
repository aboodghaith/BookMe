using DAL.Data;
using DAL.Models;
using DAL.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<Service> ServiceRepo { get;  } 

        IRepository<Booking> BookingRepo { get;  }

        public Task<int> SaveChanges();   


    }
}

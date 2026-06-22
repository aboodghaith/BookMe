using DAL.Data;
using DAL.Models;
using DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
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

        IUserRepository UserRepository { get; }

        IRepository<Category> CategoryRepository { get; }

        IRepository<City> CityRepository { get; }


        IRepository<Subscription> SubscriptionRepository { get; }

        IRepository<SubscriptionType> SubscriptionTypeRepository { get; }

        public Task<int> SaveChanges();

        Task<IDbContextTransaction> BeginTransactionAsync();

    }
}

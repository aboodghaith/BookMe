using BLL.Common;
using BLL.DTOs.SubscriptionDTOs;
using BLL.DTOs.UserDTOs;
using BLL.Services.Interfaces;
using DAL.Enums;
using DAL.Models;
using DAL.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Implementations
{
    public class SubscriptionService : ISubscriptionService
    {

        private readonly IUnitOfWork _unitOfWork;
       private readonly IPaymentFactory _paymentFactory;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPaymentService _paymentService;
        public SubscriptionService(IUnitOfWork unitOfWork , IPaymentFactory paymentFactory , 
            UserManager<User> userManager , RoleManager<IdentityRole> roleManager , IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _paymentFactory = paymentFactory;
            _userManager = userManager;
            _roleManager = roleManager;
            _paymentService = paymentService;
        }
        public async Task<ApiResponse<bool>> CreateOrUpgradeSubscription(int subscriptionType, string user , PaymentMethod method = PaymentMethod.DemoPayment)
        {
           var Subscription = await _unitOfWork.SubscriptionRepository.GetAsync(
               s => s.ServiceProviderID == user && s.IsActive == true);
            var User = await _userManager.FindByIdAsync(user);
            if (User == null)
            {
                return ApiResponseHelper.Fail<bool>("User not found", 404);
            }
            var role = await _roleManager.FindByNameAsync("ServiceProvider"); 
            using var Transaction = await _unitOfWork.BeginTransactionAsync();




            try
            {           
                
                
                if (role == null)
            {
                await _roleManager.CreateAsync(new IdentityRole("ServiceProvider"));

               
            }
                var isUserInRole = await _userManager.IsInRoleAsync(User, "ServiceProvider");
                if (!isUserInRole)
                {
                    var addToRoleResult = await _userManager.AddToRoleAsync(User, "ServiceProvider");
                    if (!addToRoleResult.Succeeded)
                    {
                        throw new Exception("Failed to assign ServiceProvider role to user");
                    }
                }

                if (Subscription != null)
                {

                    Subscription.IsActive = false;
                    Subscription.IsDeleted = true; 
                    _unitOfWork.SubscriptionRepository.Update( Subscription );
                }

                var SubscriptionType = await _unitOfWork.SubscriptionTypeRepository.GetAsync(st => st.Id == subscriptionType);
                if(SubscriptionType == null)
                {
                    return ApiResponseHelper.Fail<bool>("The Subscription Type Not Found" , 404);
                }
                var StartDate = DateTime.Now;
                var EndDate = DateTime.Now.AddDays(SubscriptionType.DurationDays);
                var NewSubscription = new Subscription
                {
                     StartDate = StartDate, 
                     EndDate = EndDate, 
                     ServiceProviderID = user , 
                     IsActive = true , 
                     SubscriptionTypeId = SubscriptionType.Id, 
                     SubscriptionTypeName = SubscriptionType.Name, 
                     SubscriptionPrice = SubscriptionType.Price,
                };

                _unitOfWork.SubscriptionRepository.Add(NewSubscription);

                if(await _unitOfWork.SaveChanges() <= 0 )
                {
                    throw new Exception("Failed To Create Subscription");
                }

                var PaymentStratgy = _paymentFactory.GetPaymentStrategy(method);

                var PaymentProcces = await PaymentStratgy.Process(NewSubscription.SubscriptionPrice);
                if (!PaymentProcces)
                    return ApiResponseHelper.Fail<bool>("Problem with the payment process");

          
                var Payment = new Payment
                {
                     Amount = NewSubscription.SubscriptionPrice, 
                     PaymentMethod = method , 
                      PaymentStatus = PaymentStatus.Paid , 
                       SubscriptionId = NewSubscription.Id ,
                       SubscriptionTypeName = SubscriptionType.Name , 
                        UserName = User?.UserName , 
                        
                };


                var isPaymentSaved = await _paymentService.CreatePaymentAsync(Payment);
                if (!isPaymentSaved)
                {
                    throw new Exception("Failed To Create Payment via PaymentService");
                }

                

               await Transaction.CommitAsync();

                return ApiResponseHelper.Success<bool>(true , "Create Subscription Succsessfaly" , 201);

            }
            catch (Exception ex) {


                await Transaction.RollbackAsync();
                return ApiResponseHelper.Fail<bool>($"Subscription failed: {ex.Message}", 500);
            } 
        }

        #region Subscription Read Operations

        public async Task<ApiResponse<SubscriptionReadDTO>> GetSubscriptionById(int subscriptionId)
        {
           
            var sub = await _unitOfWork.SubscriptionRepository.GetAsync(s => s.Id == subscriptionId);
            if (sub == null)
            {
                return ApiResponseHelper.Fail<SubscriptionReadDTO>("Subscription not found", 404);
            }

            var userObj = await _userManager.FindByIdAsync(sub.ServiceProviderID);

            var dto = new SubscriptionReadDTO
            {
                Id = sub.Id,
                StartDate = sub.StartDate,
                EndDate = sub.EndDate,
                IsActive = sub.IsActive,
                ServiceProviderId = sub.ServiceProviderID,
                ProviderName = userObj?.UserName,
                SubscriptionTypeId = sub.SubscriptionTypeId,
                SubscriptionTypeName = sub.SubscriptionTypeName,
                PricePaid = sub.SubscriptionPrice
            };

            return ApiResponseHelper.Success(dto, "Subscription retrieved successfully", 200);
        }

        public async Task<ApiResponse<List<SubscriptionReadDTO>>> GetAllSubscriptions()
        {
            var subscriptions = await _unitOfWork.SubscriptionRepository.GetAllAsync();
            if (subscriptions == null || !subscriptions.Any())
            {
                return ApiResponseHelper.Fail<List<SubscriptionReadDTO>>("No subscriptions found", 404);
            }

            var dtoList = new List<SubscriptionReadDTO>();
            foreach (var sub in subscriptions)
            {
                var userObj = await _userManager.FindByIdAsync(sub.ServiceProviderID);
                dtoList.Add(new SubscriptionReadDTO
                {
                    Id = sub.Id,
                    StartDate = sub.StartDate,
                    EndDate = sub.EndDate,
                    IsActive = sub.IsActive,
                    ServiceProviderId = sub.ServiceProviderID,
                    ProviderName = userObj?.UserName,
                    SubscriptionTypeId = sub.SubscriptionTypeId,
                    SubscriptionTypeName = sub.SubscriptionTypeName,
                    PricePaid = sub.SubscriptionPrice
                });
            }

            return ApiResponseHelper.Success<List<SubscriptionReadDTO>>(dtoList, "All subscriptions retrieved successfully", 200);
        }

        #endregion

        #region Subscription Type CRUD Operations

        public async Task<ApiResponse<List<SubscriptionTypeReadDTO>>> GetAllSubscriptionTypes()
        {
            var types = await _unitOfWork.SubscriptionTypeRepository.GetAllAsync();
            if (types == null || !types.Any())
            {
                return ApiResponseHelper.Fail<List<SubscriptionTypeReadDTO>>("No subscription plans found", 404);
            }

            var dtoList = types.Select(t => new SubscriptionTypeReadDTO
            {
                Id = t.Id,
                Name = t.Name,
                DurationDays = t.DurationDays,
                Price = t.Price
            }).ToList();

            return ApiResponseHelper.Success(dtoList, "Subscription plans retrieved successfully", 200);
        }

        public async Task<ApiResponse<SubscriptionTypeReadDTO>> CreateSubscriptionType(SubscriptionTypeCreateDTO dto)
        {
            var newType = new SubscriptionType
            {
                Name = dto.Name,
                DurationDays = dto.DurationDays,
                Price = dto.Price
            };

            _unitOfWork.SubscriptionTypeRepository.Add(newType);

            if (await _unitOfWork.SaveChanges() <= 0)
            {
                return ApiResponseHelper.Fail<SubscriptionTypeReadDTO>("Failed to create subscription plan", 500);
            }

            var resultDto = new SubscriptionTypeReadDTO
            {
                Id = newType.Id,
                Name = newType.Name,
                DurationDays = newType.DurationDays,
                Price = newType.Price
            };

            return ApiResponseHelper.Success(resultDto, "Subscription plan created successfully", 201);
        }

        public async Task<ApiResponse<SubscriptionTypeReadDTO>> UpdateSubscriptionType(int id, SubscriptionTypeCreateDTO dto)
        {
            var existingType = await _unitOfWork.SubscriptionTypeRepository.GetAsync(t => t.Id == id);
            if (existingType == null)
            {
                return ApiResponseHelper.Fail<SubscriptionTypeReadDTO>("Subscription plan not found", 404);
            }

            existingType.Name = dto.Name;
            existingType.DurationDays = dto.DurationDays;
            existingType.Price = dto.Price;

            _unitOfWork.SubscriptionTypeRepository.Update(existingType);

            if (await _unitOfWork.SaveChanges() <= 0)
            {
                return ApiResponseHelper.Fail<SubscriptionTypeReadDTO>("No changes were made or update failed", 500);
            }

            var resultDto = new SubscriptionTypeReadDTO
            {
                Id = existingType.Id,
                Name = existingType.Name,
                DurationDays = existingType.DurationDays,
                Price = existingType.Price
            };

            return ApiResponseHelper.Success(resultDto, "Subscription plan updated successfully", 200);
        }

        public async Task<ApiResponse<bool>> DeleteSubscriptionType(int id)
        {
            var existingType = await _unitOfWork.SubscriptionTypeRepository.GetAsync(t => t.Id == id);
            if (existingType == null)
            {
                return ApiResponseHelper.Fail<bool>("Subscription plan not found", 404);
            }


            existingType.IsDeleted = true;
            _unitOfWork.SubscriptionTypeRepository.Update(existingType);

            if (await _unitOfWork.SaveChanges() <= 0)
            {
                return ApiResponseHelper.Fail<bool>("Failed to delete subscription plan", 500);
            }

            return ApiResponseHelper.Success(true, "Subscription plan deleted successfully", 200);
        }

     

        #endregion   
        
        public async Task<ApiResponse<bool>> IsTheSubscriptionValid(string user)
        {
            
            var UserSubscription =await _unitOfWork.SubscriptionRepository.GetAsync(s => s.IsActive == true && s.ServiceProviderID == user);

            if (UserSubscription == null) {

                return ApiResponseHelper.Fail<bool>("The Subscription is Not Found" , 404);
            } 

            if(!(DateTime.Now > UserSubscription.EndDate))
            {
                return ApiResponseHelper.Success<bool>(true, "The subscription is still valid");

            }


            UserSubscription.IsActive = false;
             _unitOfWork.SubscriptionRepository.Update(UserSubscription);
            await _unitOfWork.SaveChanges();
            return ApiResponseHelper.Fail<bool>("The subscription has expired and is no longer valid.");
        }
    }
}

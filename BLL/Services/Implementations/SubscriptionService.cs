using Azure.Core;
using BLL.Common;
using BLL.DTOs.SubscriptionDTOs;
using BLL.DTOs.UserDTOs;
using BLL.Services.Interfaces;
using DAL.Enums;
using DAL.Models;
using DAL.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;


namespace BLL.Services.Implementations
{
    public class SubscriptionService : ISubscriptionService
    {

        private readonly IUnitOfWork _unitOfWork;
       private readonly IPaymentFactory _paymentFactory;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPaymentService _paymentService;
        private readonly IImageService _imageService;
        private readonly IUrlService _urlService;
        public SubscriptionService(IUnitOfWork unitOfWork , IPaymentFactory paymentFactory,
            UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IPaymentService paymentService, IImageService imageService, IUrlService urlService)
        {
            _unitOfWork = unitOfWork;
            _paymentFactory = paymentFactory;
            _userManager = userManager;
            _roleManager = roleManager;
            _paymentService = paymentService;
            _imageService = imageService;
            _urlService = urlService;
        }




        public async Task<ApiResponse<bool>> CreateOrUpgradeSubscription(int subscriptionType, string user, PaymentMethod method = PaymentMethod.DemoPayment)
        {
            var User = await _userManager.FindByIdAsync(user);
            if (User == null) return ApiResponseHelper.Fail<bool>("User not found", 404);

            var subscriptionPlan = await _unitOfWork.SubscriptionTypeRepository.GetAsync(st => st.Id == subscriptionType);
            if (subscriptionPlan == null) return ApiResponseHelper.Fail<bool>("The Subscription Type Not Found", 404);

            var existingSubscription = await _unitOfWork.SubscriptionRepository.GetAsync(s => s.ServiceProviderID == user && s.IsActive == true);

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureUserHasServiceProviderRole(User);

                if (existingSubscription != null)
                {
                    ArchiveSubscription(existingSubscription);
                }

                var newSubscription = CreateNewSubscriptionEntity(user, subscriptionPlan);
                _unitOfWork.SubscriptionRepository.Add(newSubscription);
                await _unitOfWork.SaveChanges(); 

         
                var payment = PrepareInitialPayment(newSubscription, subscriptionPlan.Name, User.UserName, method);

               
                var paymentStrategy = _paymentFactory.GetPaymentStrategy(method);
                var isPaymentSuccess = await paymentStrategy.Process(newSubscription.SubscriptionPrice);

                if (!isPaymentSuccess)
                {
                    await HandlePaymentFailure(payment, newSubscription, existingSubscription);
                    return ApiResponseHelper.Fail<bool>("Problem with the payment process", 400);
                }

                payment.PaymentStatus = PaymentStatus.Paid;
                var isPaymentSaved = await _paymentService.CreatePaymentAsync(payment);
                if (!isPaymentSaved)
                {

                    //////////////////////////////////

                    throw new Exception("Payment succeeded via gateway, but database failed to save the record.");
                }

                await transaction.CommitAsync();
                return ApiResponseHelper.Success<bool>(true, "Create Subscription Successfully", 201);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponseHelper.Fail<bool>($"Subscription failed: {ex.Message}", 500);
            }
        }

        #region Private Helper Methods 

        private async Task EnsureUserHasServiceProviderRole(User user)
        {
            var role = await _roleManager.FindByNameAsync("ServiceProvider");
            if (role == null)
            {
                await _roleManager.CreateAsync(new IdentityRole("ServiceProvider"));
            }

            var isUserInRole = await _userManager.IsInRoleAsync(user, "ServiceProvider");
            if (!isUserInRole)
            {
                var addToRoleResult = await _userManager.AddToRoleAsync(user, "ServiceProvider");
                if (!addToRoleResult.Succeeded)
                {
                    throw new Exception("Failed to assign ServiceProvider role to user");
                }
            }
        }

        private void ArchiveSubscription(Subscription subscription)
        {
            subscription.IsActive = false;
            _unitOfWork.SubscriptionRepository.Update(subscription);
        }

        private Subscription CreateNewSubscriptionEntity(string userId, SubscriptionType plan)
        {
            return new Subscription
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(plan.DurationDays),
                ServiceProviderID = userId,
                IsActive = true,
                SubscriptionTypeId = plan.Id,
                SubscriptionTypeName = plan.Name,
                SubscriptionPrice = plan.Price,
                
            };
        }

        private Payment PrepareInitialPayment(Subscription subscription, string planName, string? userName, PaymentMethod method)
        {
            return new Payment
            {
                Amount = subscription.SubscriptionPrice,
                PaymentMethod = method,
                PaymentStatus = PaymentStatus.Pending,
                SubscriptionId = subscription.Id,
                SubscriptionTypeName = planName,
                UserName = userName,
            };
        }

        private async Task HandlePaymentFailure(Payment payment, Subscription newSubscription, Subscription? oldSubscription)
        {
         
            payment.PaymentStatus = PaymentStatus.Failed;
            _unitOfWork.PaymentRepository.Add(payment);

            newSubscription.IsActive = false;
            _unitOfWork.SubscriptionRepository.Update(newSubscription);

            if (oldSubscription != null)
            {
                oldSubscription.IsActive = true;
                _unitOfWork.SubscriptionRepository.Update(oldSubscription);
            }

            await _unitOfWork.SaveChanges();
            await _unitOfWork.CommitAsync(); 
        }

        

        #endregion





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
            var Subscriptions = await _unitOfWork.SubscriptionRepository.GetAllAsync();
            if (Subscriptions == null || !Subscriptions.Any())
            {
                return ApiResponseHelper.Fail<List<SubscriptionReadDTO>>("No subscriptions found", 404);
            }


            var DtoList = Subscriptions.Select(s => new SubscriptionReadDTO
            {
                Id = s.Id,
                IsActive = s.IsActive,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                PricePaid = s.SubscriptionPrice,
                ProviderName = s.ServiceProvider?.UserName,
                ServiceProviderId = s.ServiceProviderID,
                SubscriptionTypeId = s.SubscriptionTypeId,
                SubscriptionTypeName = s.SubscriptionTypeName,
            }).ToList();

            return ApiResponseHelper.Success(DtoList, "All subscriptions retrieved successfully", 200);

        }


        public async Task<ApiResponse<List<SubscriptionReadDTO>>> GetAllSubscriptions(int pageNumber = 1, int PageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1; 
            if(PageSize <= 0) PageSize = 10;

            var Subscriptions = await _unitOfWork.SubscriptionRepository.GetAllAsync(false, (pageNumber - 1) * PageSize, PageSize,
                s=> s.ServiceProvider 
                );

            if(Subscriptions == null || !Subscriptions.Any())
                return ApiResponseHelper.Fail<List<SubscriptionReadDTO>>("No subscriptions found", 404);

            var DtoList =  Subscriptions.Select(s => new SubscriptionReadDTO { 
                Id = s.Id,
                IsActive = s.IsActive,
                StartDate = s.StartDate, 
                EndDate = s.EndDate, 
                PricePaid = s.SubscriptionPrice ,
                ProviderName = s.ServiceProvider?.UserName , 
                ServiceProviderId = s.ServiceProviderID , 
                SubscriptionTypeId = s.SubscriptionTypeId,
                SubscriptionTypeName = s.SubscriptionTypeName,
            }).ToList();

            return ApiResponseHelper.Success(DtoList, "All subscriptions retrieved successfully", 200);
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
                Price = t.Price,
                ImagePath = string.IsNullOrEmpty(t.ImagePath) ? null : $"{_urlService.GetBaseUrl()}{t.ImagePath}",
                
            }).ToList();

            return ApiResponseHelper.Success(dtoList, "Subscription plans retrieved successfully", 200);
        }

        public async Task<ApiResponse<SubscriptionTypeReadDTO>> CreateSubscriptionType(SubscriptionTypeCreateDTO dto)
        {
           
            var ImageFile = await _imageService.UploadImageAsync(dto.ImagePath, "images");
            var newType = new SubscriptionType
            {
                Name = dto.Name,
                DurationDays = dto.DurationDays,
                Price = dto.Price,
                ImagePath = string.IsNullOrEmpty(ImageFile) ? null : ImageFile

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
                Price = newType.Price,
                ImagePath = string.IsNullOrEmpty(ImageFile) ? null : $"{_urlService.GetBaseUrl()}{ImageFile}"
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
            var ImageFile = await _imageService.UploadImageAsync(dto.ImagePath, "images");
            existingType.Name = dto.Name;
            existingType.DurationDays = dto.DurationDays;
            existingType.Price = dto.Price;
            existingType.ImagePath = string.IsNullOrEmpty(ImageFile) ? null : ImageFile;
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
                Price = existingType.Price,

                ImagePath = string.IsNullOrEmpty(ImageFile) ? null : $"{_urlService.GetBaseUrl()}{ImageFile}"
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

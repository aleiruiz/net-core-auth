using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using XLocker.Data;
using XLocker.DTOs.Service;
using XLocker.Entities;
using XLocker.Exceptions.Mailbox;
using XLocker.Exceptions.Service;
using XLocker.Helpers;
using XLocker.Response.Common;
using XLocker.Response.Service;
using XLocker.Types;

namespace XLocker.Services
{

    public interface IGuideService
    {
        Task<ResponseList<ServiceResponse>> GetAll();
        Task<ResponseList<ServiceResponse>> Get(GetServiceDTO request);
        Task<List<ServiceExcel>> GetExcel(GetServiceDTO request);
        Task<ResponseList<ServiceResponse>> GetMyServices(GetServiceDTO request, string userId);
        Task<ServiceResponse?> GetActiveService(string userId);
        Task<ResponseList<ServiceResponse>> GetByStatus(GetServiceDTO request, List<ServiceStatus> status);
        Task<Service> GetServiceById(string serviceId);
        Task<List<ABSService>> GetPendingService();
        Task<List<ABSService>> GetDueServices();
        Task<int> GetActiveServices();
        Task<List<ABSService>> GetNullServices();
        Task<List<ABSService>> GetUrgentReminders();
        Task<List<ABSService>> GetFailedServices();
        Task<int> GetCompletedServices();
        Task<int> GetCreatedServices();
        Task<int> GetOverdueServices();
        Task<ABSService> Create(CreateServiceDTO request, string userId);
        Task<CheckTokenResponse> CheckToken(CheckTokenDTO request, string lockerId);
        Task<bool> DepositPackage(DepositServiceDTO request, string serviceId);
        Task<bool> WithdrawPackage(WithdrawServiceDTO request, string serviceId);
        Task<bool> CreateNovelty(NoveltyServiceDTO request, string serviceId);
        Task<ServiceResponse> CheckSupportToken(CheckTokenDTO request, string lockerId);
        Task<bool> CancelService(string serviceId);
        Task<Service> ServiceBlocked(string serviceId);
        Task<Service> NullService(string serviceId);
        Task<Service> ServiceSentUrgentReminder(string serviceId);
        Task<bool> Delete(string serviceId);
    }
    public class GuideService : IGuideService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILockerService _lockerService;
        private readonly IWalletService _walletService;
        private readonly IStorageService _storageService;
        public GuideService(DataContext context, IMapper mapper, ILockerService lockerService, IStorageService storageService, IWalletService walletService)
        {
            _context = context;
            _mapper = mapper;
            _lockerService = lockerService;
            _storageService = storageService;
            _walletService = walletService;
        }

        public IQueryable<Service> BaseGetRequest(GetServiceDTO request)
        {
            var query = _context.Services.Include(x => x.ServiceTracks).Include(x => x.Mailbox).Include(x => x.User).Include(x => x.Locker).AsQueryable();
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(x => x.User.Name.Contains(request.Search) || x.Id == request.Search || x.Identifier == request.Search);
            }

            if (request.Status != null)
            {
                query = query.Where(x => x.Status == request.Status);
            }

            if (request.ShouldSearchByDate())
            {
                query = query.Where(x => x.UpdatedAt > request.FromDate && x.UpdatedAt < request.ToDate);
            }

            var propertyName = !string.IsNullOrEmpty(request.Sort) ? request.Sort : "createdAt";

            if (QueryHelper.PropertyExists(query, propertyName))
            {
                query = QueryHelper.OrderByProperty(query, propertyName, request.Order);
            }
            return query;
        }

        public async Task<Service> GetServiceById(string serviceId)
        {
            var service = await _context.Services.Include(x => x.Locker).Include(x => x.Mailbox).Include(x => x.User).FirstOrDefaultAsync(l => l.Id == serviceId);
            if (service == null)
            {
                throw new ServiceDoesNotExistException();
            }
            return service;
        }

        public void ValidateNextStep(ServiceStatus currentStep, ServiceStatus nextStep)
        {
            if (!ServiceTypeOrder.ValidateNextStatus(currentStep, nextStep))
            {
                throw new ServiceWrongStepException();
            }
        }

        public void ValidateCheckToken(ServiceStatus currentStep)
        {
            var validStatus = new List<ServiceStatus> { ServiceStatus.SC, ServiceStatus.SD, ServiceStatus.SB };
            if (!validStatus.Contains(currentStep))
            {
                throw new ServiceWrongStepException();
            }
        }

        public int GetServiceCost(Service service)
        {
            double cost;
            if (service.Status == ServiceStatus.SB)
            {
                cost = 8;
            }
            else
            {
                var time = DateTime.UtcNow;
                cost = Math.Ceiling((time - service.DepositDate).GetValueOrDefault().TotalHours);
                if (cost > 8)
                {
                    cost = 8;
                }
            }
            // We substract one as the initial request already covers the price of one hour
            return Convert.ToInt32(cost - 1);
        }

        public async Task<ResponseList<ServiceResponse>> GetAll()
        {
            var services = await _context.Services.Include(x => x.ServiceTracks).Include(x => x.Mailbox).Include(x => x.User).Include(x => x.Locker).ToListAsync();
            var mappedService = _mapper.Map<List<ServiceResponse>>(services);
            return new ResponseList<ServiceResponse> { TotalCount = services.Count, Data = mappedService };
        }

        public async Task<ResponseList<ServiceResponse>> Get(GetServiceDTO request)
        {
            var query = BaseGetRequest(request);
            var totalCount = await query.CountAsync();

            var mappedService = _mapper.Map<List<ServiceResponse>>(await query.Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync());
            return new ResponseList<ServiceResponse> { TotalCount = totalCount, Data = mappedService };
        }

        public async Task<List<ServiceExcel>> GetExcel(GetServiceDTO request)
        {
            var query = BaseGetRequest(request);
            return _mapper.Map<List<ServiceExcel>>((await query.ToListAsync()).Select(x => { x.LockerName = x.Locker.Name; x.MailboxNumber = x.Mailbox?.Number; x.UserName = x.User?.Name; x.UserPhoneNumber = x.User.PhoneNumber; x.UserEmail = x.User.Email; return x; }));
        }

        public async Task<ResponseList<ServiceResponse>> GetMyServices(GetServiceDTO request, string userId)
        {
            var query = BaseGetRequest(request).Where(x => x.UserId == userId);
            var totalCount = await query.CountAsync();

            var mappedService = _mapper.Map<List<ServiceResponse>>(await query.Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync());
            return new ResponseList<ServiceResponse> { TotalCount = totalCount, Data = mappedService };
        }

        public async Task<ServiceResponse?> GetActiveService(string userId)
        {
            List<ServiceStatus> activeStatus = new List<ServiceStatus> {
                ServiceStatus.SC,
                ServiceStatus.SD,
                ServiceStatus.SB,
                ServiceStatus.SV,
                ServiceStatus.SVA,
            };
            var service = await _context.Services.Include(x => x.Mailbox).Include(x => x.User).Include(x => x.Locker).Where(x => x.UserId == userId && activeStatus.Contains(x.Status)).FirstOrDefaultAsync();

            return service != null ? _mapper.Map<ServiceResponse>(service) : null;
        }

        public async Task<ResponseList<ServiceResponse>> GetByStatus(GetServiceDTO request, List<ServiceStatus> status)
        {
            var query = BaseGetRequest(request).Where(x => status.Contains(x.Status));

            var totalCount = await query.CountAsync();

            var mappedService = _mapper.Map<List<ServiceResponse>>(await query.Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync());
            return new ResponseList<ServiceResponse> { TotalCount = totalCount, Data = mappedService };
        }

        public async Task<List<ABSService>> GetPendingService()
        {
            var dueDate = DateTime.UtcNow.AddHours(-1);
            var services = await _context.Services.Where(x => x.UpdatedAt < dueDate && x.Status == ServiceStatus.SC).ToListAsync();
            return _mapper.Map<List<ABSService>>(services);
        }

        public async Task<List<ABSService>> GetDueServices()
        {
            var dueDate = DateTime.UtcNow.AddHours(-8);
            var services = await _context.Services.Where(x => x.DepositDate < dueDate && x.Status == ServiceStatus.SD).ToListAsync();
            return _mapper.Map<List<ABSService>>(services);
        }

        public async Task<List<ABSService>> GetNullServices()
        {
            var dueDate = DateTime.UtcNow.AddDays(-1);
            var services = await _context.Services.Where(x => x.DepositDate < dueDate && x.Status == ServiceStatus.SB).ToListAsync();
            return _mapper.Map<List<ABSService>>(services);
        }

        public async Task<List<ABSService>> GetUrgentReminders()
        {
            var dueDate = DateTime.UtcNow.AddDays(-1.5);
            var services = await _context.Services.Where(x => x.DepositDate < dueDate && x.Status == ServiceStatus.SD && !x.UrgentReminderSent).ToListAsync();
            return _mapper.Map<List<ABSService>>(services);
        }

        public async Task<int> GetActiveServices()
        {
            return await _context.Services.Where(x => x.Status == ServiceStatus.SC || x.Status == ServiceStatus.SD).CountAsync();
        }

        public async Task<List<ABSService>> GetFailedServices()
        {
            var dueDate = DateTime.UtcNow.AddDays(-3);
            var services = await _context.Services.Where(x => x.WithdrawalDate < dueDate && x.Status == ServiceStatus.SVR).ToListAsync();
            return _mapper.Map<List<ABSService>>(services);
        }

        public async Task<int> GetCompletedServices()
        {
            return await _context.Services.Where(x => x.Status == ServiceStatus.SR || x.Status == ServiceStatus.SF || x.Status == ServiceStatus.SDB).CountAsync();
        }

        public async Task<int> GetOverdueServices()
        {
            return await _context.Services.Where(x => x.Status == ServiceStatus.SV).CountAsync();
        }

        public async Task<int> GetCreatedServices()
        {
            return await _context.Services.Where(x => x.Status == ServiceStatus.SC).CountAsync();
        }

        public async Task<ABSService> Create(CreateServiceDTO request, string userId)
        {
            var customerToken = GenerateRandomNumber.GetRandomNumber(6);
            var qrCodeByte = GenerateQRCodeByServiceToken.Exec(customerToken);

            Random r = new Random();
            int rInt = r.Next(100000, 999999);

            var mailbox = await _context.Mailboxes.Where(x => x.Size == MailboxSize.ST && x.Status == MailboxStatus.Available && x.LockerId == request.LockerId).FirstOrDefaultAsync();
            if (mailbox == null)
            {
                throw new MailboxNotAvailableException();
            }

            var activeService = await this.GetActiveService(userId);
            if (activeService != null)
            {
                throw new ActiveServiceException();
            }

            var service = new Service
            {
                LockerId = request.LockerId,
                CustomerToken = customerToken,
                SupportToken = GenerateRandomNumber.GetRandomNumber(6),
                Status = ServiceStatus.SC,
                UserId = userId,
                MailboxId = mailbox.Id,
                Identifier = $"SV{rInt}",
                Cost = 1,
            };
            service.QRCode = await _storageService.UploadFileFromByteArray(qrCodeByte, $"{service.Id}.png");
            mailbox.Status = MailboxStatus.Unavailable;
            await _context.Services.AddAsync(service);
            await _walletService.Pay(userId, 1, "Creacion de servicio");
            await _context.SaveChangesAsync();
            return _mapper.Map<ABSService>(service);
        }

        public async Task<CheckTokenResponse> CheckToken(CheckTokenDTO request, string lockerId)
        {
            var locker = await _lockerService.GetLockerById(lockerId);

            var service = await _context.Services.Include(x => x.Mailbox).Include(x => x.User).Where(x => x.LockerId == locker.Id && x.CustomerToken == request.token).FirstOrDefaultAsync();
            if (service == null)
            {
                throw new ServiceDoesNotExistException();
            }
            ValidateCheckToken(service.Status);

            var cost = GetServiceCost(service);

            var balance = await _walletService.GetBalance(service.UserId);
            var responseObj = _mapper.Map<VerboseServiceResponse>(service);
            if (balance < cost && service.Status != ServiceStatus.SB)
            {
                return new CheckTokenResponse { CanOpen = false, Data = responseObj, Message = "No tiene saldo suficiente para retirar el paquete", Total = cost };
            }
            if (balance < 1 && service.Status == ServiceStatus.SB)
            {
                return new CheckTokenResponse { CanOpen = false, Data = responseObj, Message = "Su saldo a dia de hoy es negativo, necesita recargar los creditos faltantes.", Total = cost };
            }

            return new CheckTokenResponse { CanOpen = true, Data = responseObj, Message = "Paquete disponible para retiro", Total = cost }; ;
        }

        public async Task<bool> DepositPackage(DepositServiceDTO request, string serviceId)
        {
            var service = await GetServiceById(serviceId);

            if (service.MailboxId == null)
            {
                throw new MailboxNotAssignedException();
            }

            ValidateNextStep(service.Status, ServiceStatus.SD);


            service.Status = ServiceStatus.SD;
            service.DepositDate = DateTime.UtcNow;

            if (request.Image != string.Empty)
            {
                var serviceTrack = new ServiceTrack
                {
                    Description = request.Description,
                    Image = request.Image,
                    ServiceId = serviceId,
                };

                await _context.ServiceTracks.AddAsync(serviceTrack);
            }

            service.Mailbox.Position = MailboxPosition.Close;


            var response = await _context.SaveChangesAsync();

            return response > 0;
        }

        public async Task<bool> WithdrawPackage(WithdrawServiceDTO request, string serviceId)
        {
            var service = await GetServiceById(serviceId);

            ValidateNextStep(service.Status, ServiceStatus.SR);



            service.Status = service.Status == ServiceStatus.SB ? ServiceStatus.SDB : ServiceStatus.SR;
            service.WithdrawalDate = DateTime.UtcNow;


            if (request.Image != string.Empty)
            {
                var serviceTrack = new ServiceTrack
                {
                    Description = request.Description,
                    Image = request.Image,
                    ServiceId = serviceId,
                };

                await _context.ServiceTracks.AddAsync(serviceTrack);
            }

            var mailbox = await _context.Mailboxes.Where(x => x.Id == service.MailboxId).FirstOrDefaultAsync();
            if (mailbox != null)
            {
                mailbox.Status = MailboxStatus.Available;
            }

            if (service.Status != ServiceStatus.SDB)
            {
                var cost = GetServiceCost(service);

                service.Cost = cost + 1;

                await _walletService.Pay(service.UserId, Convert.ToInt32(cost), "Pago de Servicio");
            }

            var response = await _context.SaveChangesAsync();

            return response > 0;
        }

        public async Task<bool> CreateNovelty(NoveltyServiceDTO request, string serviceId)
        {
            var service = await GetServiceById(serviceId);

            ValidateNextStep(service.Status, ServiceStatus.SN);

            service.Status = ServiceStatus.SN;
            service.NoveltyDate = DateTime.UtcNow;
            service.NoveltyType = request.NoveltyType;

            var response = await _context.SaveChangesAsync();

            return response > 0;
        }

        public async Task<ServiceResponse> CheckSupportToken(CheckTokenDTO request, string lockerId)
        {

            var locker = await _lockerService.GetLockerById(lockerId);

            var service = await _context.Services.Include(x => x.Mailbox).Where(x => x.LockerId == locker.Id && x.SupportToken == request.token).FirstOrDefaultAsync();
            if (service == null)
            {
                throw new ServiceDoesNotExistException();
            }

            var withdrawalService = await _context.WithdrawalOrders.FirstOrDefaultAsync(x => x.ServiceId == service.Id);
            var response = _mapper.Map<ServiceResponse>(service);
            response.WithdrawalOrderId = withdrawalService?.Id;
            return response;
        }

        public async Task<bool> CancelService(string serviceId)
        {
            var service = await GetServiceById(serviceId);

            ValidateNextStep(service.Status, ServiceStatus.SF);

            service.Status = ServiceStatus.SF;


            var mailbox = await _context.Mailboxes.Where(x => x.Id == service.MailboxId).FirstOrDefaultAsync();
            if (mailbox != null)
            {
                mailbox.Status = MailboxStatus.Available;
            }

            var response = await _context.SaveChangesAsync();

            return response > 0;
        }

        public async Task<Service> ServiceBlocked(string serviceId)
        {
            var service = await GetServiceById(serviceId);

            ValidateNextStep(service.Status, ServiceStatus.SB);

            service.Cost = 8;

            service.Status = ServiceStatus.SB;

            await _context.SaveChangesAsync();

            await _walletService.Pay(service.UserId, 7, "Servicio vencido");

            return service;
        }

        public async Task<Service> NullService(string serviceId)
        {
            var service = await GetServiceById(serviceId);

            service.Status = ServiceStatus.SV;

            await _context.SaveChangesAsync();

            return service;
        }

        public async Task<Service> ServiceSentUrgentReminder(string serviceId)
        {
            var service = await GetServiceById(serviceId);

            service.UrgentReminderSent = true;

            await _context.SaveChangesAsync();

            return service;
        }

        public async Task<bool> Delete(string serviceId)
        {
            var service = await GetServiceById(serviceId);

            _context.Services.Remove(service);
            var response = await _context.SaveChangesAsync();
            return response > 0;
        }
    }
}

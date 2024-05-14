using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using XLocker.Data;
using XLocker.DTOs.WithdrawalOrder;
using XLocker.Entities;
using XLocker.Exceptions.Service;
using XLocker.Exceptions.WithdrawalOrder;
using XLocker.Helpers;
using XLocker.Response.Common;
using XLocker.Response.WithdrawalOrder;
using XLocker.Types;

namespace XLocker.Services
{

    public interface IWithdrawalOrderService
    {
        Task<ResponseList<WithdrawalOrderResponse>> GetAll();
        Task<ResponseList<WithdrawalOrderResponse>> Get(GetWithdrawalOrderDTO request);
        Task<List<WithdrawalOrderExcel>> GetExcel(GetWithdrawalOrderDTO request);
        Task<ResponseList<WithdrawalOrderResponse>> GetCompletedWithdrawals(GetWithdrawalOrderDTO request, string userId);
        Task<ResponseList<WithdrawalOrderResponse>> GetByStatus(GetWithdrawalOrderDTO request, List<ServiceStatus> status);
        Task<ResponseList<WithdrawalOrderResponse>> GetAssignedToUser(GetWithdrawalOrderDTO request, string UserId);
        Task<ABSWithdrawalOrder> Create(CreateWithdrawalOrderDTO request);
        Task<bool> Assign(AssignWithdrawalOrderDTO request, string orderId);
        Task<bool> Complete(CompleteWithdrawalOrderDTO request, string orderId);
    }
    public class WithdrawalOrderService : IWithdrawalOrderService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public WithdrawalOrderService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public IQueryable<WithdrawalOrder> BaseGetRequest(GetWithdrawalOrderDTO request)
        {
            var query = _context.WithdrawalOrders.Include(x => x.Service).Include(x => x.Service.Locker).Include(x => x.Service.Mailbox).Include(x => x.User).AsQueryable();
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(x => x.Service.User.Name.Contains(request.Search));
            }

            if (request.Status != null)
            {
                query = query.Where(x => x.ServiceStatus == request.Status);
            }

            var propertyName = !string.IsNullOrEmpty(request.Sort) ? request.Sort : "createdAt";

            if (QueryHelper.PropertyExists(query, propertyName))
            {
                query = QueryHelper.OrderByProperty(query, propertyName, request.Order);
            }

            if (request.ShouldSearchByDate())
            {
                query = query.Where(x => x.UpdatedAt > request.FromDate && x.UpdatedAt < request.ToDate);
            }

            return query;
        }

        public async Task<WithdrawalOrder> GetWithdrawalById(string orderId)
        {
            var order = await _context.WithdrawalOrders.Include(x => x.Service).Include(x => x.User).FirstOrDefaultAsync(l => l.Id == orderId || l.ServiceId == orderId);
            if (order == null)
            {
                throw new WithdrawalDoesNotExistException();
            }
            return order;
        }

        public async Task<Service> GetServiceById(string serviceId)
        {
            var service = await _context.Services.FirstOrDefaultAsync(l => l.Id == serviceId);
            if (service == null)
            {
                throw new ServiceDoesNotExistException();
            }
            return service;
        }

        public async Task<ResponseList<WithdrawalOrderResponse>> GetAll()
        {
            var orders = await _context.WithdrawalOrders.Include(x => x.Service).Include(x => x.User).ToListAsync();
            var mappedOrders = _mapper.Map<List<WithdrawalOrderResponse>>(orders);
            return new ResponseList<WithdrawalOrderResponse> { TotalCount = orders.Count, Data = mappedOrders };
        }

        public async Task<ResponseList<WithdrawalOrderResponse>> Get(GetWithdrawalOrderDTO request)
        {
            var query = BaseGetRequest(request);

            var totalCount = await query.CountAsync();

            var mappedService = _mapper.Map<List<WithdrawalOrderResponse>>(await query.Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync());
            return new ResponseList<WithdrawalOrderResponse> { TotalCount = totalCount, Data = mappedService };
        }
        public async Task<List<WithdrawalOrderExcel>> GetExcel(GetWithdrawalOrderDTO request)
        {
            var response = (await BaseGetRequest(request).ToListAsync()).Select(x =>
                {
                    x.ServiceLockerName = x.Service.Locker.Name;
                    x.ServiceMailboxNumber = x.Service.Mailbox?.Number;
                    x.UserName = x.User?.Name;
                    x.UserEmail = x.User?.Email;
                    return x;
                });
            return _mapper.Map<List<WithdrawalOrderExcel>>(response);
        }

        public async Task<ResponseList<WithdrawalOrderResponse>> GetCompletedWithdrawals(GetWithdrawalOrderDTO request, string userId)
        {
            var query = BaseGetRequest(request).Where(x => x.UserId == userId);
            var totalCount = await query.CountAsync();

            var mappedService = _mapper.Map<List<WithdrawalOrderResponse>>(await query.Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync());
            return new ResponseList<WithdrawalOrderResponse> { TotalCount = totalCount, Data = mappedService };
        }

        public async Task<ResponseList<WithdrawalOrderResponse>> GetByStatus(GetWithdrawalOrderDTO request, List<ServiceStatus> status)
        {
            var query = BaseGetRequest(request).Where(x => status.Contains(x.ServiceStatus));

            var totalCount = await query.CountAsync();

            var mappedService = _mapper.Map<List<WithdrawalOrderResponse>>(await query.Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync());
            return new ResponseList<WithdrawalOrderResponse> { TotalCount = totalCount, Data = mappedService };
        }

        public async Task<ResponseList<WithdrawalOrderResponse>> GetAssignedToUser(GetWithdrawalOrderDTO request, string UserId)
        {
            var query = BaseGetRequest(request).Where(x => x.UserId == UserId && x.ServiceStatus == ServiceStatus.SVA);

            var totalCount = await query.CountAsync();

            var mappedService = _mapper.Map<List<WithdrawalOrderResponse>>(await query.Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync());
            return new ResponseList<WithdrawalOrderResponse> { TotalCount = totalCount, Data = mappedService };
        }

        public async Task<ABSWithdrawalOrder> Create(CreateWithdrawalOrderDTO request)
        {
            var service = await GetServiceById(request.ServiceId);

            if (service.Status != ServiceStatus.SV)
            {
                throw new ServiceWrongStepException();
            }

            var order = new WithdrawalOrder
            {
                ServiceId = request.ServiceId,
                ServiceStatus = ServiceStatus.SVC,
            };

            service.Status = ServiceStatus.SVC;

            service.WithdrawlRequested = true;

            await _context.WithdrawalOrders.AddAsync(order);
            await _context.SaveChangesAsync();
            return _mapper.Map<ABSWithdrawalOrder>(order);
        }

        public async Task<bool> Assign(AssignWithdrawalOrderDTO request, string orderId)
        {
            var order = await GetWithdrawalById(orderId);
            var service = await GetServiceById(order.ServiceId);

            if (order.ServiceStatus != ServiceStatus.SVC)
            {
                throw new ServiceWrongStepException();
            }

            order.UserId = request.UserId;
            order.ServiceStatus = ServiceStatus.SVA;
            service.Status = ServiceStatus.SVA;

            var response = await _context.SaveChangesAsync();

            return response > 0;
        }

        public async Task<bool> Complete(CompleteWithdrawalOrderDTO request, string orderId)
        {
            var order = await GetWithdrawalById(orderId);
            var service = await GetServiceById(order.ServiceId);
            var mailbox = await _context.Mailboxes.Where(x => x.Id == service.MailboxId).FirstOrDefaultAsync();


            if (order.ServiceStatus != ServiceStatus.SVA)
            {
                throw new ServiceWrongStepException();
            }

            order.ServiceStatus = ServiceStatus.SVR;
            service.Status = ServiceStatus.SVR;

            if (mailbox != null)
            {
                mailbox.Status = MailboxStatus.Available;
            }

            if (!string.IsNullOrEmpty(request.Image))
            {
                var serviceTrack = new ServiceTrack
                {
                    Description = request.Description,
                    Image = request.Image,
                    ServiceId = order.ServiceId,
                };

                await _context.ServiceTracks.AddAsync(serviceTrack);
            }

            var response = await _context.SaveChangesAsync();

            return response > 0;
        }
    }
}

using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using XLocker.Data;
using XLocker.DTOs.MaintanceOrder;
using XLocker.Entities;
using XLocker.Exceptions.Maintance;
using XLocker.Exceptions.Service;
using XLocker.Helpers;
using XLocker.Response.Common;
using XLocker.Response.Maintance;
using XLocker.Types;

namespace XLocker.Services
{

    public interface IMaintanceService
    {
        Task<ResponseList<MaintanceResponse>> GetAll();
        Task<ResponseList<MaintanceResponse>> Get(GetMaintanceDTO request);
        Task<ResponseList<MaintanceResponse>> GetByStatus(GetMaintanceDTO request, MaintanceStatus status);
        Task<ResponseList<MaintanceResponse>> GetAssinedToUser(GetMaintanceDTO request, string userId);
        Task<ABSMaintanceOrder> Create(CreateMaintanceDTO request, string reporterId);
        Task<bool> Assign(AssignMaintanceDTO request, string maintanceId);
        Task<bool> Complete(string maintanceId);
        Task<bool> Update(UpdateMaintanceDTO request, string maintanceId);
        Task<bool> Delete(string maintanceId);
    }
    public class MaintanceService : IMaintanceService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MaintanceService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IQueryable<MaintanceOrder> BaseGetRequest(GetMaintanceDTO request)
        {
            var query = _context.MaintanceOders.Include(x => x.Locker).Include(x => x.User).Include(x => x.Reporter).AsQueryable();
            if (request.MaintanceType != null)
            {
                query = query.Where(x => x.MaintanceType == request.MaintanceType);
            }
            if (request.LockerId != null)
            {
                query = query.Where(x => x.LockerId == request.LockerId);
            }

            var propertyName = !string.IsNullOrEmpty(request.Sort) ? request.Sort : "createdAt";

            if (QueryHelper.PropertyExists(query, propertyName))
            {
                query = QueryHelper.OrderByProperty(query, propertyName, request.Order);
            }
            return query;
        }

        public async Task<ResponseList<MaintanceResponse>> GetAll()
        {
            var maintanceOrders = await _context.MaintanceOders.Include(x => x.Locker).Include(x => x.User).ToListAsync();
            var mappedMaintanceOrders = _mapper.Map<List<MaintanceResponse>>(maintanceOrders);
            return new ResponseList<MaintanceResponse> { TotalCount = mappedMaintanceOrders.Count, Data = mappedMaintanceOrders };
        }

        public async Task<ResponseList<MaintanceResponse>> Get(GetMaintanceDTO request)
        {

            var query = BaseGetRequest(request);
            var totalCount = await query.CountAsync();
            var mappedMaintanceOrders = _mapper.Map<List<MaintanceResponse>>(await query.Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync());
            return new ResponseList<MaintanceResponse> { TotalCount = totalCount, Data = mappedMaintanceOrders };
        }

        public async Task<ResponseList<MaintanceResponse>> GetByStatus(GetMaintanceDTO request, MaintanceStatus status)
        {
            var query = BaseGetRequest(request).Where(x => x.Status == status);
            var totalCount = await query.CountAsync();
            var mappedMaintanceOrders = _mapper.Map<List<MaintanceResponse>>(await query.Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync());
            return new ResponseList<MaintanceResponse> { TotalCount = totalCount, Data = mappedMaintanceOrders };
        }

        public async Task<ResponseList<MaintanceResponse>> GetAssinedToUser(GetMaintanceDTO request, string userId)
        {
            var query = BaseGetRequest(request).Where(x => x.UserId == userId && x.Status == MaintanceStatus.MA);
            var totalCount = await query.CountAsync();
            var mappedMaintanceOrders = _mapper.Map<List<MaintanceResponse>>(await query.Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync());
            return new ResponseList<MaintanceResponse> { TotalCount = totalCount, Data = mappedMaintanceOrders };
        }

        public async Task<ABSMaintanceOrder> Create(CreateMaintanceDTO request, string reporterId)
        {
            var maintanceOrder = new MaintanceOrder
            {
                Description = request.Description,
                MaintanceDate = request.MaintanceDate,
                MaintanceType = request.MaintanceType,
                LockerId = request.LockerId,
                Status = MaintanceStatus.MC,
                ReporterId = reporterId
            };
            await _context.MaintanceOders.AddAsync(maintanceOrder);
            await _context.SaveChangesAsync();
            return _mapper.Map<ABSMaintanceOrder>(maintanceOrder);
        }

        public async Task<bool> Assign(AssignMaintanceDTO request, string maintanceId)
        {
            var maintanceOrder = await _context.MaintanceOders.FirstOrDefaultAsync(l => l.Id == maintanceId);
            if (maintanceOrder == null)
            {
                throw new MaintanceOrderDoesNotExistException();
            }
            if (MaintanceStatus.MC != maintanceOrder.Status)
            {
                throw new ServiceWrongStepException();
            }
            maintanceOrder.Status = MaintanceStatus.MA;
            maintanceOrder.UserId = request.UserId;

            var response = await _context.SaveChangesAsync();

            return response > 0;
        }

        public async Task<bool> Complete(string maintanceId)
        {
            var maintanceOrder = await _context.MaintanceOders.FirstOrDefaultAsync(l => l.Id == maintanceId);
            if (maintanceOrder == null)
            {
                throw new MaintanceOrderDoesNotExistException();
            }
            if (MaintanceStatus.MA != maintanceOrder.Status)
            {
                throw new ServiceWrongStepException();
            }
            maintanceOrder.Status = MaintanceStatus.MR;

            var response = await _context.SaveChangesAsync();

            return response > 0;
        }

        public async Task<bool> Update(UpdateMaintanceDTO request, string maintanceId)
        {
            var maintanceOrder = await _context.MaintanceOders.FirstOrDefaultAsync(l => l.Id == maintanceId);
            if (maintanceOrder == null)
            {
                throw new MaintanceOrderDoesNotExistException();
            }
            maintanceOrder.Description = request.Description;
            maintanceOrder.MaintanceDate = request.MaintanceDate;
            maintanceOrder.MaintanceType = request.MaintanceType;
            maintanceOrder.LockerId = request.LockerId;

            var response = await _context.SaveChangesAsync();

            return response > 0;
        }

        public async Task<bool> Delete(string maintanceId)
        {
            var maintanceOrder = await _context.MaintanceOders.FirstOrDefaultAsync(l => l.Id == maintanceId);
            if (maintanceOrder == null)
            {
                throw new MaintanceOrderDoesNotExistException();
            }

            _context.MaintanceOders.Remove(maintanceOrder);
            var response = await _context.SaveChangesAsync();
            return response > 0;
        }
    }
}

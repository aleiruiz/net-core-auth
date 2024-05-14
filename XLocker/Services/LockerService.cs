using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using XLocker.Data;
using XLocker.DTOs.Locker;
using XLocker.Entities;
using XLocker.Exceptions.Locker;
using XLocker.Helpers;
using XLocker.Response.Common;

namespace XLocker.Services
{

    public interface ILockerService
    {
        Task<Locker> GetLockerById(string lockerId);
        Task<ResponseList<ABSLocker>> GetAllActive();
        Task<ResponseList<ABSLocker>> GetAllAssignableLockers();
        Task<ResponseList<ABSLocker>> Get(GetLockerDTO request);
        Task<int> GetActiveLockers();
        Task<ABSLocker> Create(CreateLockerDTO request);
        Task<bool> Update(UpdateLockerDTO request, string lockerId);
        Task<bool> UpdateStatus(UpdateLockerStatusDTO request, string lockerId);
        Task<bool> Delete(string lockerId);
        Task<List<ABSLocker>> GetInactiveLockers();
        Task<bool> OfflineLocker(string lockerId);
    }
    public class LockerService : ILockerService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public LockerService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<Locker> GetLockerById(string lockerId)
        {
            var locker = await _context.Lockers.FirstOrDefaultAsync(l => l.Id == lockerId);
            if (locker == null)
            {
                throw new LockerDoesNotExistException();
            }
            return locker;
        }

        public async Task<ResponseList<ABSLocker>> GetAllActive()
        {
            var lockers = (await _context.Lockers.Include(x => x.Mailboxes).Where(x => x.Status == Types.LockerStatus.Enabled).ToListAsync()).Select(x => { x.MailboxQuantity = x.Mailboxes.Count(); x.MailboxAvailableQuantity = x.Mailboxes.Where(x => x.Status == Types.MailboxStatus.Available).Count(); return x; });
            var mappedLockers = _mapper.Map<List<ABSLocker>>(lockers);
            return new ResponseList<ABSLocker> { TotalCount = lockers.Count(), Data = mappedLockers };
        }

        public async Task<ResponseList<ABSLocker>> GetAllAssignableLockers()
        {
            var lockers = await _context.Lockers.Include(x => x.Mailboxes).Where(x => x.Mailboxes.Count() < x.MaxMailboxes).ToListAsync();
            var mappedLockers = _mapper.Map<List<ABSLocker>>(lockers);
            return new ResponseList<ABSLocker> { TotalCount = lockers.Count, Data = mappedLockers };
        }

        public async Task<ResponseList<ABSLocker>> Get(GetLockerDTO request)
        {
            var query = _context.Lockers.Include(x => x.Mailboxes).AsQueryable();
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(x => x.Name.Contains(request.Search));
            }

            if (request.Status != null)
            {
                query = query.Where(x => x.Status == request.Status);
            }

            var propertyName = !string.IsNullOrEmpty(request.Sort) ? request.Sort : "createdAt";

            if (QueryHelper.PropertyExists(query, propertyName))
            {
                query = QueryHelper.OrderByProperty(query, propertyName, request.Order);
            }

            var totalCount = await query.CountAsync();
            var response = (await query.Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync()).Select(x => { x.MailboxQuantity = x.Mailboxes.Count(); x.MailboxAvailableQuantity = x.Mailboxes.Where(x => x.Status == Types.MailboxStatus.Available).Count(); return x; });

            var mappedLockers = _mapper.Map<List<ABSLocker>>(response);
            return new ResponseList<ABSLocker> { TotalCount = totalCount, Data = mappedLockers };
        }

        public async Task<List<ABSLocker>> GetInactiveLockers()
        {
            var dueDate = DateTime.UtcNow.AddMinutes(-2);
            var lockers = await _context.Lockers.Where(x => x.LastConnection < dueDate).ToListAsync();
            return _mapper.Map<List<ABSLocker>>(lockers);
        }

        public async Task<int> GetActiveLockers()
        {
            return await _context.Lockers.Where(x => x.Status == Types.LockerStatus.Enabled).CountAsync();
        }

        public async Task<ABSLocker> Create(CreateLockerDTO request)
        {
            var alreadyExist = await _context.Lockers.FirstOrDefaultAsync(u => u.Name == request.Name);
            if (alreadyExist != null)
            {
                throw new LockerAlreadyExistException();
            }
            var locker = new Locker
            {
                Name = request.Name,
                Address = request.Address,
                Reference = request.Reference,
                Status = request.Status,
                LastConnection = DateTime.UtcNow,
                MaxMailboxes = request.MaxMailboxes,
            };

            await _context.Lockers.AddAsync(locker);
            await _context.SaveChangesAsync();
            return _mapper.Map<ABSLocker>(locker);
        }

        public async Task<bool> Update(UpdateLockerDTO request, string lockerId)
        {
            var locker = await GetLockerById(lockerId);
            var alreadyExist = await _context.Lockers.FirstOrDefaultAsync(l => l.Name == request.Name && l.Id != lockerId);
            if (alreadyExist != null)
            {
                throw new LockerAlreadyExistException();
            }
            locker.Status = request.Status;
            locker.Reference = request.Reference;
            locker.Address = request.Address;
            locker.Name = request.Name;
            locker.MaxMailboxes = request.MaxMailboxes;

            var response = await _context.SaveChangesAsync();

            return response > 0;
        }


        public async Task<bool> UpdateStatus(UpdateLockerStatusDTO request, string lockerId)
        {
            var locker = await GetLockerById(lockerId);
            locker.Status = request.Status;

            var response = await _context.SaveChangesAsync();

            return response > 0;
        }

        public async Task<bool> OfflineLocker(string lockerId)
        {
            var locker = await GetLockerById(lockerId);

            locker.ConnectionState = Types.ConnectionState.Offline;

            var response = await _context.SaveChangesAsync();

            return response > 0;
        }

        public async Task<bool> Delete(string lockerId)
        {
            var locker = await GetLockerById(lockerId);

            _context.Lockers.Remove(locker);
            var response = await _context.SaveChangesAsync();
            return response > 0;
        }
    }
}

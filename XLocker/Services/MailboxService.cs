using XLocker.Data;
using XLocker.DTOs.Mailbox;
using XLocker.Entities;
using XLocker.Exceptions.Locker;
using XLocker.Exceptions.Mailbox;
using XLocker.Helpers;
using XLocker.Response.Common;
using XLocker.Response.Mailbox;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace XLocker.Services
{

    public interface IMailboxService
    {

        Task<ResponseList<MailboxResponse>> GetAll();
        Task<ResponseList<MailboxResponse>> Get(GetMailboxDTO request);
        Task<int> GetAvailableLockers();
        Task<ABSMailbox> Create(CreateMailboxDTO request);
        Task<bool> Update(UpdateMailboxDTO request, string mailboxId);
        Task<bool> UpdateStatus(UpdateMailboxStatusDTO request, string mailboxId);
        Task<bool> Delete(string mailboxId);
    }
    public class MailboxService : IMailboxService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MailboxService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseList<MailboxResponse>> GetAll()
        {
            var mailboxes = await _context.Mailboxes.Include(x => x.Locker).ToListAsync();
            var mappedMailBoxes = _mapper.Map<List<MailboxResponse>>(mailboxes);
            return new ResponseList<MailboxResponse> { TotalCount = mailboxes.Count, Data = mappedMailBoxes };
        }

        public async Task<ResponseList<MailboxResponse>> Get(GetMailboxDTO request)
        {
            var query = _context.Mailboxes.AsQueryable();
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(x => x.Number.Contains(request.Search));
            }

            if (request.LockerId != null)
            {
                query = query.Where(x => x.LockerId == request.LockerId);
            }

            if (request.Status != null)
            {
                query = query.Where(x => x.Status == request.Status);
            }

            if (request.Size != null)
            {
                query = query.Where(x => x.Size == request.Size);
            }

            if (request.Position != null)
            {
                query = query.Where(x => x.Position == request.Position);
            }

            var propertyName = !string.IsNullOrEmpty(request.Sort) ? request.Sort : "createdAt";

            if (QueryHelper.PropertyExists(query, propertyName))
            {
                query = QueryHelper.OrderByProperty(query, propertyName, request.Order);
            }

            var totalCount = await query.CountAsync();

            var mailboxes = await query.Include(x => x.Locker).Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync();
            var mappedMailBoxes = _mapper.Map<List<MailboxResponse>>(mailboxes);
            return new ResponseList<MailboxResponse> { TotalCount = totalCount, Data = mappedMailBoxes };
        }

        public async Task<int> GetAvailableLockers()
        {
            return await _context.Mailboxes.Where(x => x.Status == Types.MailboxStatus.Available).CountAsync();
        }

        public async Task<ABSMailbox> Create(CreateMailboxDTO request)
        {
            var alreadyExist = await _context.Mailboxes.FirstOrDefaultAsync(u => u.Number == request.Number && u.LockerId == request.LockerId);
            if (alreadyExist != null)
            {
                throw new MailboxAlreadyExistException();
            }
            var mailbox = new Mailbox
            {
                Number = request.Number,
                Position = request.Position,
                Status = request.Status,
                Size = request.Size,
                LockerId = request.LockerId,
            };
            await _context.Mailboxes.AddAsync(mailbox);
            await _context.SaveChangesAsync();
            return _mapper.Map<ABSMailbox>(mailbox);
        }

        public async Task<bool> Update(UpdateMailboxDTO request, string mailboxId)
        {
            var mailbox = await _context.Mailboxes.FirstOrDefaultAsync(l => l.Id == mailboxId);
            if (mailbox == null)
            {
                throw new LockerDoesNotExistException();
            }
            var alreadyExist = await _context.Mailboxes.FirstOrDefaultAsync(u => u.Number == request.Number &&
                                                                            u.LockerId == request.LockerId &&
                                                                            u.Id != mailboxId);
            if (alreadyExist != null)
            {
                throw new MailboxAlreadyExistException();
            }
            mailbox.Status = request.Status;
            mailbox.Position = request.Position;
            mailbox.Number = request.Number;
            mailbox.Size = request.Size;
            mailbox.LockerId = request.LockerId;

            var response = await _context.SaveChangesAsync();

            return response > 0;
        }

        public async Task<bool> UpdateStatus(UpdateMailboxStatusDTO request, string mailboxId)
        {
            var mailbox = await _context.Mailboxes.FirstOrDefaultAsync(l => l.Id == mailboxId);
            if (mailbox == null)
            {
                throw new LockerDoesNotExistException();
            }
            mailbox.Status = request.Status;

            var response = await _context.SaveChangesAsync();

            return response > 0;
        }

        public async Task<bool> Delete(string mailboxId)
        {
            var mailbox = await _context.Mailboxes.FirstOrDefaultAsync(l => l.Id == mailboxId);
            if (mailbox == null)
            {
                throw new LockerDoesNotExistException();
            }

            _context.Mailboxes.Remove(mailbox);
            var response = await _context.SaveChangesAsync();
            return response > 0;
        }
    }
}

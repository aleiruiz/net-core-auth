using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using XLocker.Data;
using XLocker.DTOs.Diagnostic;
using XLocker.Entities;
using XLocker.Exceptions.Diagnostic;
using XLocker.Response.Common;
using XLocker.Response.Maintance;

namespace XLocker.Services
{

    public interface IDiagnosticService
    {
        Task<ResponseList<DiagnosticResponse>> GetAll();
        Task<ResponseList<DiagnosticResponse>> Get(GetDiagnosticDTO request);
        Task<ABSDiagnostic> Create(CreateDiagnosticDTO request);
        Task<bool> Delete(string diagnosticId);
    }
    public class DiagnosticService : IDiagnosticService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public DiagnosticService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseList<DiagnosticResponse>> GetAll()
        {
            var diagnostics = await _context.MaintanceOders.Include(x => x.Locker).ToListAsync();
            var mappedDiagnostics = _mapper.Map<List<DiagnosticResponse>>(diagnostics);
            return new ResponseList<DiagnosticResponse> { TotalCount = mappedDiagnostics.Count, Data = mappedDiagnostics };
        }

        public async Task<ResponseList<DiagnosticResponse>> Get(GetDiagnosticDTO request)
        {
            var query = _context.Diagnostics.Include(x => x.Locker).AsQueryable();
            if (request.LockerId != null)
            {
                query = query.Where(x => x.LockerId == request.LockerId);
            }
            var totalCount = await query.CountAsync();
            var mappedDiagnostic = _mapper.Map<List<DiagnosticResponse>>(await query.Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync());
            return new ResponseList<DiagnosticResponse> { TotalCount = totalCount, Data = mappedDiagnostic };
        }

        public async Task<ABSDiagnostic> Create(CreateDiagnosticDTO request)
        {
            var diagnostic = new Diagnostic
            {
                ClosedLocks = request.ClosedLocks != null ? string.Join(",", request.ClosedLocks) : "",
                OpenLocks = request.OpenLocks != null ? string.Join(",", request.OpenLocks) : "",
                MailboxQuantity = request.MailboxQuantity,
                MailboxOpenQuantity = request.MailboxOpenQuantity,
                LockerId = request.LockerId,
            };
            var locker = await _context.Lockers.FirstOrDefaultAsync(x => x.Id == request.LockerId);
            if (locker != null)
            {
                locker.LastConnection = DateTime.UtcNow;
                locker.ConnectionState = Types.ConnectionState.Online;
            }
            await _context.Diagnostics.AddAsync(diagnostic);
            await _context.SaveChangesAsync();
            return _mapper.Map<ABSDiagnostic>(diagnostic);
        }

        public async Task<bool> Delete(string diagnosticId)
        {
            var diagnostic = await _context.Diagnostics.FirstOrDefaultAsync(l => l.Id == diagnosticId);
            if (diagnostic == null)
            {
                throw new DiagnosticDoesNotExistsException();
            }

            _context.Diagnostics.Remove(diagnostic);
            var response = await _context.SaveChangesAsync();
            return response > 0;
        }
    }
}

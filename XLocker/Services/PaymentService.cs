using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using XLocker.API;
using XLocker.Data;
using XLocker.DTOs.Package;
using XLocker.DTOs.Payment;
using XLocker.Entities;
using XLocker.Exceptions.CreditPackage;
using XLocker.Helpers;
using XLocker.Response.Common;
using XLocker.Response.Payment;
using XLocker.Types;

namespace XLocker.Services
{
    public interface IPaymentService
    {
        Task<ResponseList<PaymentResponse>> GetAll();
        Task<ResponseList<PaymentResponse>> Get(GetPaymentDTO request);
        Task<List<PaymentExcel>> GetExcel(GetPaymentDTO request);
        Task<ResponseList<PaymentResponse>> GetMyPayments(GetPaymentDTO request, string userId);
        Task<ABSPayment> CreatePurchaseOrder(PurchasePackageDTO request, string userId);
        Task<bool> CompletePurchase(string purchaseId);
        Task<List<Payment>> GetPendingPayments();
    }

    public class PaymentService : IPaymentService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IBoldAPI _boldAPI;
        private string boldSecretKey;
        private string boldIdentityKey;
        public PaymentService(DataContext context, IMapper mapper, IConfiguration configuration, IBoldAPI boldApi)
        {
            _context = context;
            _mapper = mapper;
            boldSecretKey = configuration["BOLD:SECRET_KEY"] ?? "";
            boldIdentityKey = configuration["BOLD:ACCESS_KEY"] ?? "";
            _boldAPI = boldApi;
        }

        public async Task<ResponseList<PaymentResponse>> GetAll()
        {
            var diagnostics = await _context.Payments.Where(x => x.Status == PaymentStatus.Completed).Include(x => x.User).Include(x => x.CreditPackage).ToListAsync();
            var mappedDiagnostics = _mapper.Map<List<PaymentResponse>>(diagnostics);
            return new ResponseList<PaymentResponse> { TotalCount = mappedDiagnostics.Count, Data = mappedDiagnostics };
        }

        public async Task<ResponseList<PaymentResponse>> Get(GetPaymentDTO request)
        {
            var query = _context.Payments.Where(x => x.Status == PaymentStatus.Completed).Include(x => x.User).Include(x => x.CreditPackage).AsQueryable();
            if (request.Search != null)
            {
                query = query.Where(x => x.User.Name.Contains(request.Search));
            }

            if (request.ShouldSearchByDate())
            {
                query = query.Where(x => x.UpdatedAt > request.FromDate && x.UpdatedAt < request.ToDate);
            }
            var totalCount = await query.CountAsync();
            var mappedDiagnostic = _mapper.Map<List<PaymentResponse>>(await query.Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync());
            return new ResponseList<PaymentResponse> { TotalCount = totalCount, Data = mappedDiagnostic };
        }

        public async Task<List<PaymentExcel>> GetExcel(GetPaymentDTO request)
        {
            var query = _context.Payments.Where(x => x.Status == PaymentStatus.Completed).Include(x => x.User).Include(x => x.CreditPackage).AsQueryable();
            if (request.Search != null)
            {
                query = query.Where(x => x.User.Name.Contains(request.Search));
            }

            if (request.ShouldSearchByDate())
            {
                query = query.Where(x => x.UpdatedAt > request.FromDate && x.UpdatedAt < request.ToDate);
            }
            var diagnostics = (await query.ToListAsync()).Select(x => { x.UserName = x.User.Name; x.CreditPackageName = x.CreditPackage.Name; return x; });
            return _mapper.Map<List<PaymentExcel>>(diagnostics);
        }

        public async Task<ResponseList<PaymentResponse>> GetMyPayments(GetPaymentDTO request, string userId)
        {
            var query = _context.Payments.Include(x => x.User).Where(x => x.UserId == userId);
            if (request.Search != null)
            {
                query = query.Where(x => x.User.Name.Contains(request.Search));
            }
            var totalCount = await query.CountAsync();
            var mappedDiagnostic = _mapper.Map<List<PaymentResponse>>(await query.Skip(request.PageSize * (request.Page - 1)).Take(request.PageSize).ToListAsync());
            return new ResponseList<PaymentResponse> { TotalCount = totalCount, Data = mappedDiagnostic };
        }

        public async Task<ABSPayment> CreatePurchaseOrder(PurchasePackageDTO request, string userId)
        {
            var package = await _context.CreditPackages.FirstOrDefaultAsync(x => x.Id == request.PackageId);
            if (package == null)
            {
                throw new CreditPackageDoesNotExistsException();
            }

            Random r = new Random();
            var identifier = $"VEN-{r.Next(100000, 999999)}";
            var checkPayments = await _context.Payments.FirstOrDefaultAsync(x => x.GuideNumber == identifier);
            while (checkPayments != null)
            {
                identifier = $"VEN-{r.Next(100000, 999999)}";
                checkPayments = await _context.Payments.FirstOrDefaultAsync(x => x.GuideNumber == identifier);
            }

            var boldString = $"{identifier}{package.Price}COP{boldSecretKey}";

            var payment = new Payment { Amount = package.Price, Credits = package.CreditQuantity, Concept = "Compra de creditos", UserId = userId, GuideNumber = identifier, GuideHash = Hash.HashWithSHA256(boldString), CreditPackageId = package.Id };

            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            var response = _mapper.Map<ABSPayment>(payment);
            response.IdentityKey = boldIdentityKey;
            return response;
        }

        public async Task<List<Payment>> GetPendingPayments()
        {
            return await _context.Payments.Where(x => x.Status == PaymentStatus.Pending).ToListAsync();
        }

        public async Task<bool> CompletePurchase(string purchaseId)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(x => x.Id == purchaseId);
            if (payment == null)
            {
                throw new PaymentDoesNotExistException();
            }

            var failedStatus = new List<string> { "NO_TRANSACTION_FOUND", "REJECTED", "FAILED" };
            var paymentStatus = await _boldAPI.GetPaymentStatus(payment.GuideNumber);
            if (paymentStatus.errors?.Count > 0)
            {
                var dueDate = DateTime.UtcNow.AddMinutes(-10);
                if (dueDate > payment.CreatedAt)
                {
                    payment.Status = PaymentStatus.Failed;
                }
            }
            if (paymentStatus?.payment_status == "APPROVED")
            {
                payment.Status = PaymentStatus.Completed;
                var walletTransact = new Wallet { Concept = "Compra de creditos", Credits = payment.Credits, TransactionType = TransactionType.Deposit, UserId = payment.UserId };
                await _context.Wallets.AddAsync(walletTransact);
            }
            else if (failedStatus.Contains(paymentStatus?.payment_status ?? ""))
            {
                payment.Status = PaymentStatus.Failed;
            }

            var response = await _context.SaveChangesAsync();
            return response > 0;
        }
    }
}

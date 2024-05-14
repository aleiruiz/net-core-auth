using Hangfire;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json.Serialization;
using XLocker.API;
using XLocker.Auth;
using XLocker.Data;
using XLocker.Entities;
using XLocker.Jobs;
using XLocker.Services;
using XLocker.Types;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ILockerService, LockerService>();
builder.Services.AddScoped<IMailboxService, MailboxService>();
builder.Services.AddScoped<IMaintanceService, MaintanceService>();
builder.Services.AddScoped<IDiagnosticService, DiagnosticService>();
builder.Services.AddScoped<IBusinessService, BusinessService>();
builder.Services.AddScoped<IGuideService, GuideService>();
builder.Services.AddScoped<IWithdrawalOrderService, WithdrawalOrderService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<IMapper, Mapper>();
builder.Services.AddScoped<ICreditPackageService, CreditPackageService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<IBoldAPI, BoldAPI>();
builder.Services.AddScoped<ISMSProvider, AltiriaAPI>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "authToken";
        options.Cookie.Expiration = TimeSpan.FromMinutes(120);
        options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
    });

builder.Services.AddAuthorization(options =>
{
    var permissions = Enum.GetNames(typeof(Permissions)).ToList();
    foreach (var permission in permissions)
    {
        options.AddPolicy(permission, policy => policy.RequireClaim("Permission", permission));
    }
});

builder.Services.AddIdentityApiEndpoints<User>().AddRoles<Role>().AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();


builder.Services.AddSingleton<IEmailSender<User>, EmailSender>();

builder.Services.AddHangfire(configuration => configuration
            .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();

builder.Services.AddCors();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Dependencies

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGroup("api/Auth").MapIdentityApi<User>();
app.MapControllers();


app.UseHangfireDashboard();

app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials());

RecurringJob.AddOrUpdate<CheckDueServices>("CheckDueServices", job => job.Execute(), Cron.Minutely);
RecurringJob.AddOrUpdate<CheckNullServices>("CheckNullServices", job => job.Execute(), Cron.Minutely);
RecurringJob.AddOrUpdate<CheckPendingServices>("CheckPendingServices", job => job.Execute(), Cron.Minutely);
RecurringJob.AddOrUpdate<CheckOfflineLockers>("CheckOfflineLockers", job => job.Execute(), Cron.Minutely);
RecurringJob.AddOrUpdate<CheckPaymentStatus>("CheckPaymentStatus", job => job.Execute(), Cron.Minutely);

app.MapHangfireDashboard();

app.Run();

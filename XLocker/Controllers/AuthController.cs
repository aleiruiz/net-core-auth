using MapsterMapper;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using XLocker.DTOs.Auth;
using XLocker.Entities;
using XLocker.Exceptions.Auth;
using XLocker.Exceptions.User;
using XLocker.Response.Common;
using XLocker.Services;

namespace XLocker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUserService _userService;
        private readonly IWalletService _walletService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private static readonly EmailAddressAttribute _emailAddressAttribute = new();

        public AuthController(ILogger<AuthController> logger, IPasswordHasher<User> passwordHasher, IServiceProvider serviceProvider, UserManager<User> userManager, RoleManager<Role> roleManager, IMapper mapper, IUserService userService, IWalletService walletService, IEmailService emailSender)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _userService = userService;
            _walletService = walletService;
            _emailService = emailSender;
        }
        [HttpPost("VerifyToken")]
        [Authorize]
        public async Task<ActionResult<ABSUser?>> VerifyToken()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Ok(null);
            }
            var roles = await _userManager.GetRolesAsync(user);
            var permissions = new List<string>();
            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var claims = await _roleManager.GetClaimsAsync(role);
                    permissions.AddRange(claims.Where(x => x.Type == "Permission").Select(x => x.Value).ToList());
                }
            }
            var mappedUser = _mapper.Map<ABSUser>(user);
            mappedUser.Permissions = permissions;
            mappedUser.Role = roles.FirstOrDefault();
            mappedUser.Balance = await _walletService.GetBalance(mappedUser.Id);
            return Ok(mappedUser);
        }

        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<ActionResult<bool>> ChangePassword(ChangePasswordDTO request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest();
            }

            if (request.Password != request.ConfirmPassword)
            {
                throw new PasswordMismatchException();
            }

            var validPassword = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);

            if (!validPassword)
            {
                throw new InvalidPasswordException();
            }

            return Ok(await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.Password));
        }

        [HttpPost("AdminLogin")]
        public async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> Login(LoginDTO loginRequest)
        {
            var signInManager = _serviceProvider.GetRequiredService<SignInManager<User>>();

            signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;


            var result = await signInManager.PasswordSignInAsync(loginRequest.Email, loginRequest.Password, false, lockoutOnFailure: true);

            var userIsAdmin = await _userService.CheckIfAdminByEmail(loginRequest.Email);
            if (!result.Succeeded)
            {
                return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
            }

            if (!userIsAdmin)
            {
                return TypedResults.Problem("Acceso restringido, solo administradores", statusCode: StatusCodes.Status401Unauthorized);
            }

            return TypedResults.Empty;
        }

        [HttpPost("Refresh")]
        public async Task<Results<Ok<AccessTokenResponse>, UnauthorizedHttpResult, SignInHttpResult, ChallengeHttpResult>> Refresh(RefreshDTO refreshRequest)
        {
            var signInManager = _serviceProvider.GetRequiredService<SignInManager<User>>();
            var refreshTokenProtector = _serviceProvider.GetRequiredService<IOptionsMonitor<BearerTokenOptions>>().Get(IdentityConstants.BearerScheme).RefreshTokenProtector;
            var refreshTicket = refreshTokenProtector.Unprotect(refreshRequest.RefreshToken);

            // Reject the /refresh attempt with a 401 if the token expired or the security stamp validation fails
            if (refreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc ||
                _serviceProvider.GetRequiredService<TimeProvider>().GetUtcNow() >= expiresUtc ||
                await signInManager.ValidateSecurityStampAsync(refreshTicket.Principal) is not User user)

            {
                return TypedResults.Challenge();
            }

            var newPrincipal = await signInManager.CreateUserPrincipalAsync(user);
            return TypedResults.SignIn(newPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
        }

        private static ValidationProblem CreateValidationProblem(IdentityResult result)
        {
            // We expect a single error code and description in the normal case.
            // This could be golfed with GroupBy and ToDictionary, but perf! :P
            Debug.Assert(!result.Succeeded);
            var errorDictionary = new Dictionary<string, string[]>(1);

            foreach (var error in result.Errors)
            {
                string[] newDescriptions;

                if (errorDictionary.TryGetValue(error.Code, out var descriptions))
                {
                    newDescriptions = new string[descriptions.Length + 1];
                    Array.Copy(descriptions, newDescriptions, descriptions.Length);
                    newDescriptions[descriptions.Length] = error.Description;
                }
                else
                {
                    newDescriptions = [error.Description];
                }

                errorDictionary[error.Code] = newDescriptions;
            }

            return TypedResults.ValidationProblem(errorDictionary);
        }

        [HttpPost("SignUp")]
        public async Task<Results<Ok, ValidationProblem>> Register(RegisterDTO registration)
        {
            if (registration.Password != registration.ConfirmPassword)
            {
                throw new PasswordMismatchException();
            }

            var userManager = _serviceProvider.GetRequiredService<UserManager<User>>();

            var userStore = _serviceProvider.GetRequiredService<IUserStore<User>>();
            var emailStore = (IUserEmailStore<User>)userStore;
            var email = registration.Email;

            if (string.IsNullOrEmpty(email) || !_emailAddressAttribute.IsValid(email))
            {
                return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(email)));
            }


            var user = new User();
            await userStore.SetUserNameAsync(user, email, CancellationToken.None);
            await emailStore.SetEmailAsync(user, email, CancellationToken.None);
            user.Name = registration.Name;
            user.PhoneNumber = registration.PhoneNumber;
            user.EmailConfirmed = true;
            user.Status = Types.UserStatus.Active;
            var result = await userManager.CreateAsync(user, registration.Password);

            if (!result.Succeeded)
            {
                return CreateValidationProblem(result);
            }

            _ = await _emailService.AccountCreated(user);

            return TypedResults.Ok();
        }


        [HttpPost("SendPhoneVerificationCode")]
        [Authorize]
        public async Task<ActionResult<bool>> SendPhoneVerificationCode(SendPhoneVerificationDTO request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest();
            }

            return Ok(await _userService.SendPhoneVerificationCode(request, user.Id));
        }


        [HttpPost("VerifyPhoneNumber")]
        [Authorize]
        public async Task<ActionResult<bool>> VerifyPhoneNumber(VerifyPhoneDTO request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest();
            }

            return Ok(await _userService.VerifyPhone(request, user.Id));
        }

    }
}

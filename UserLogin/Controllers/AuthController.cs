using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using API.DAL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly DBManager _dBContext;
        public record SignInRequest(string? UserName, string? Password);
        public record Response(bool IsSuccess, string Message);
        public record UserClaim(string Type, string Value);
        public record UserDto(string UserName, string Password, string IpAddress, DateTime TimeStamp);

        internal static List<UserDto> badLoginAttempts = new();

        public AuthController(IConfiguration configuration)
        {
            _dBContext = new DBManager(configuration);
        }



        //Endpoint for registering new users. only to be usen in emergency if neww user needs to be created through Swagger. Then Authorize
        //Needs to be out commneted temporary. Should only be used by developer on localhost, to avoid security breach
        //[Authorize]
        [HttpPost("Register")]
        public ActionResult<User> Register(SignInRequest request)
        {
            try
            {
                User user = new();
                CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

                user.UserName = request.UserName;
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                _dBContext.AddUser(user);
                return Ok(user);
            }
            catch (Exception)
            {
                return BadRequest(new Response(false, "Something went wrong! User has not been created!"));
                throw;
            }
        }



        //Endpoint for logging in
        [HttpPost("Login")]
        public async Task<IActionResult> SignInAsync([FromBody] SignInRequest signInRequest)
        {

            //Checking for bad loginattemts (Feedback need to be provided to client)

            int allowedTries = 5;
            int counter = 0;
            var result = badLoginAttempts.Where(x => x.TimeStamp.AddMinutes(5) > DateTime.Now && x.UserName == signInRequest.UserName);
            foreach (UserDto item in result)
            {
                counter++;
            }
            int triesLeft = allowedTries - counter;
            //--------------------------------------------------
            //Cleaning old attempts is missing (Not finnished) So for now wrong attempts will be locked out forever :)
            //--------------------------------------------------
            var attemtsToRemove = badLoginAttempts.SingleOrDefault(x => x.TimeStamp.AddMinutes(5) < DateTime.Now);



            if (triesLeft > 0)
            {
                try
                {
                    User user = _dBContext.GetUser(signInRequest.UserName);
                    if (user.UserName != signInRequest.UserName)
                    {
                        UserDto badLogin = new(signInRequest.UserName, "", "", DateTime.Now);
                        badLoginAttempts.Add(badLogin);
                        return BadRequest(new Response(false, "Wrong UserName or password."));
                    }

                    if (!VerifyPasswordHash(signInRequest.Password, user.PasswordHash, user.PasswordSalt))
                    {
                        UserDto badLogin = new(signInRequest.UserName, "", "", DateTime.Now);
                        badLoginAttempts.Add(badLogin);
                        return BadRequest(new Response(false, "Wrong UserName or password."));
                    }
                     string oldPassword = Convert.ToBase64String(user.PasswordHash);
                    //byte[] oldSalt = user.PasswordSalt;
                    //User oldUser = user;

                    CreatePasswordHash(signInRequest.Password, out byte[] passwordHash, out byte[] passwordSalt);

                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    _dBContext.UpdateUserSalt(user);



                    //Adding claims to the cookie
                    List<Claim> claims = new() { new Claim(type: ClaimTypes.Name, value: signInRequest.UserName) };

                    ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity),
                        new AuthenticationProperties
                        {
                            IsPersistent = true,
                            AllowRefresh = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(6)
                        });

                    return Ok(new Response(true, "Signed in successfully"));
                }
                catch (Exception)
                {
                    UserDto badLogin = new(signInRequest.UserName, "", "", DateTime.Now);
                    badLoginAttempts.Add(badLogin);
                    return BadRequest(new Response(false, "Wrong UserName or password."));
                    throw;
                }

            }
            return BadRequest(new Response(false, "Du har tastet din kode forkert for mange gange. Nu må du lige køle lidt ned og tænke dig om. Du kan prøve igen om 589 år. (Eller måske lige om lidt)"));
        }



        // Endpoint for getting userclaims
        [Authorize]
        [HttpGet("User")]
        public IActionResult GetUser()
        {
            try
            {
                List<UserClaim>? userClaims = User.Claims.Select(x => new UserClaim(x.Type, x.Value)).ToList();

                return Ok(userClaims);
            }
            catch (Exception)
            {
                throw;
            }
        }



        // Endpoint for logging out
        [Authorize]
        [HttpGet("Logout")]
        public async Task SignOutAsync()
        {
            try
            {
               
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch (Exception)
            {
                throw;
            }
        }



        //Method to create password hash and salt. (Used in the Register endpoint)
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            try
            {
                using HMACSHA512 hmac = new();
                //string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
                //passwordSalt = Encoding.ASCII.GetBytes(salt);
                passwordSalt = hmac.Key;
                //passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
            catch (Exception)
            {
                throw;
            }
        }



        //Method to verify if the password is correct. (Used in the Login endpoint)
        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            try
            {
                using HMACSHA512 hmac = new(passwordSalt);
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
            catch (Exception)
            {
                throw;
            }
        }

     
    }
}

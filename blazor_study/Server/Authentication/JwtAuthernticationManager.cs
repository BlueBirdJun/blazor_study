using blazor_study.Shared;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace blazor_study.Server.Authentication
{
    public class JwtAuthernticationManager
    {
        public const string JWT_SECURITY_KEY = "9lHN74fx1Uo9vIbprTYyDN1PHzunU7TGLzJQWKIEF6qX0X3f";
        public int JWT_TOKEN_VALIDITY_MINS = 20;
        private UserAccountService _userAccountService;
        public JwtAuthernticationManager(UserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }
        public UserSession? GenerateJwtToken(string username , string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;
            var userAccount = _userAccountService.GetUserAccountByUserName(username);
            if (userAccount == null || userAccount.Password != password)
                return null;

            var toeknExpiryTimeStamp = DateTime.Now.AddMinutes(JWT_TOKEN_VALIDITY_MINS);
            var tokenKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);
            var clamimsIdentity = new ClaimsIdentity(new List<Claim>
            { 
                new Claim(ClaimTypes.Name,userAccount.UserName),
                new Claim(ClaimTypes.Role,userAccount.Role),
            });
            var signingCredentitals = new SigningCredentials
            ( new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature);
            
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = clamimsIdentity,
                Expires = toeknExpiryTimeStamp,
                SigningCredentials = signingCredentitals
            };
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToekn = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            var token = jwtSecurityTokenHandler.WriteToken(securityToekn);
            var userSession = new UserSession
            {
                UserName = userAccount.UserName,
                Role = userAccount.Role,
                Token=token,
                ExpiresIn =(int)toeknExpiryTimeStamp.Subtract(DateTime.Now).TotalSeconds
            };
            return userSession;
        }
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using ColorExtractorApi.Models;

namespace ColorExtractorApi.Services.Helpers
{
    // Helper service for creating and validating JWT tokens for auth.
    public class JwtUtils
    {
        private readonly string _jwtKey;

        // Constructor: Retrieves JWT secret key from either env var or appsettings.json:
        public JwtUtils(string jwtKey)
        {
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new ArgumentNullException(nameof(jwtKey), "JWT key must not be null or empty.");
            }
            _jwtKey = jwtKey;
        }

        // Generates a JWT token for the given user.
        // The token includes user ID, email, and full name as claims.
        // The token is signed using HMAC SHA-256 and expires in 2 hours.
        public (string Token, DateTime ExpiresAt) GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = IsBase64String(_jwtKey)
                ? Convert.FromBase64String(_jwtKey)
                : Encoding.ASCII.GetBytes(_jwtKey);

            var expiresAt = DateTime.UtcNow.AddHours(2); // Expiry 

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, $"{user.Name} {user.Surname}")
                }),
                Expires = expiresAt, // Sets JWT expiry claim
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (tokenHandler.WriteToken(token), expiresAt);
        }

        // Validates given JWT token.
        // Returns the user ID if the token is valid; otherwise returns null (token not valid).
        public int? ValidateToken(string token)
        {
            if (token == null) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = IsBase64String(_jwtKey)
                ? Convert.FromBase64String(_jwtKey)
                : Encoding.ASCII.GetBytes(_jwtKey);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.FromMinutes(2) // safety buffer for time mismatches
                    //ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                
                var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "nameid");
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return null;
                }

                return userId; // Extracts and returns uesdId from JWT if valid
            }
            catch (Exception ex)
            {
                Console.WriteLine("JWT validation failed: " + ex.Message);
                return null;
            }
        }

        // Helper: checks if the given str is valid Base64.        
        private static bool IsBase64String(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            value = value.Trim();

            // Base64 strs must have len divisible by 4
            if (value.Length % 4 != 0) return false;

            try
            {
                Convert.FromBase64String(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
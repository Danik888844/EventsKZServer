using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Auth.Common
{
    public class AuthOptions
    {
        public string Issuer { get; set; } // who generate
        public string Audience { get; set; } // for who generated
        public string Secret { get; set; } // secret string for generate
        public int TokenLifetime { get; set; } // secs
        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));
        }
    }
}
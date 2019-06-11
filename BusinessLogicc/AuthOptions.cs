using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BusinessLogic
{
    public class AuthOptions
    {
        public const string ISSUER = "TraderAuthServer";
        public const string AUDIENCE = "http://localhost:5001/";
        const string KEY = "examplesecret_secrektey!123";
        public const int LIFETIME = 600;

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }

    }
}

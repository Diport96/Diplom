using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MqttWebApp.Models.JwtSecurity
{
    public class TokenAuthOptions
    {
        public const string ISSUER = "MqttWebApp"; // Издатель токена
        public const string AUDIENCE = "DiplomApp"; // Потребитель токена
        const string KEY = "285b403a493a6c6557513b2671676c7d5e4239375f7171583c322f406f";   // Ключ для шифрации
        public const int LIFETIME = 1; // Время жизни токена - 1 минута

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}

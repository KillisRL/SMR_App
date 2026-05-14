using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SMRDominio.ClassePessoa;

namespace SMRApi.Services
{
    public static class TokenService
    {
        public static string GenerateToken(Pessoa pessoa)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("SmrAppUelerBernardoLuizFelipeTCC");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Role, pessoa.id_pessoa_tipo.ToString()), 
                new Claim("id_pessoa", pessoa.id_pessoa.ToString())
            }),
                Expires = DateTime.UtcNow.AddHours(8), 
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}

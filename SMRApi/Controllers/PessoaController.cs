using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMRApi.Services;
using SMRDominio.ClassePessoa;
using SMRInfraestrutura;

namespace SMRApi.Controllers
{
    [ApiController]
    [Route("pessoa")]
    public class PessoaController : ControllerBase
    {
        private SMRDBContext _dbContext;
        public PessoaController(SMRDBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [HttpPost("cadastrar")]
        [AllowAnonymous]
        public async Task<IActionResult> CriarPessoa(Pessoa pessoa)
        {
            var criarpessoa = new Pessoa
            {
                nome = pessoa.nome,
                id_pessoa_tipo = pessoa.id_pessoa_tipo,
                celular = pessoa.celular,
                email = pessoa.email,
                senha_hash = BCrypt.Net.BCrypt.HashPassword(pessoa.senha_hash),
                login = pessoa.login,
                ativo = pessoa.ativo,
                data_cadastro = pessoa.data_cadastro
            };

            _dbContext.Pessoa.Add(criarpessoa);
            await _dbContext.SaveChangesAsync();

            return Ok(criarpessoa);
        }
    

        [HttpPut ("alterar")]
        [Authorize]
        public async Task<IActionResult> AlterarPessoa([FromBody]Pessoa pessoa)
        {
           _dbContext.Update(pessoa);

           await _dbContext.SaveChangesAsync();

           return Ok(pessoa);        
        }

        [HttpPost ("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] PessoaLogin pessoalogin)
        {
            var pessoa = await _dbContext.Pessoa.FirstOrDefaultAsync(u => u.login == pessoalogin.login);

            if (pessoa == null || !BCrypt.Net.BCrypt.Verify(pessoalogin.senha_hash, pessoa.senha_hash))
            {
                return Unauthorized(new { Message = "Login ou senha inválidos!" });
            }

            var token = TokenService.GenerateToken(pessoa);

            return Ok(new
            {
                usuario = pessoa,
                Token = token
            });
        }
    }
}

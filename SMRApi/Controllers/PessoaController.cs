using Microsoft.AspNetCore.Mvc;
using SMRInfraestrutura;
using SMRDominio.ClassePessoa;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost("cadastro")]
        public async Task<IActionResult> CriarPessoa(Pessoa pessoa)
        {
            var criarpessoa = new Pessoa
            {
                nome = pessoa.nome,
                id_pessoatipo = pessoa.id_pessoatipo,
                documento = pessoa.documento,
                telefone = pessoa.telefone,
                email = pessoa.email,
                senha_hash = pessoa.senha_hash,
                login = pessoa.login,
                data_cadastro = pessoa.data_cadastro
            };

            _dbContext.Pessoa.Add(criarpessoa);
            await _dbContext.SaveChangesAsync();

            return Ok(criarpessoa);

        }
    }
}

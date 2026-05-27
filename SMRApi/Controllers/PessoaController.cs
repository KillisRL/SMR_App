using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMRApi.Services;
using SMRDominio.ClassePessoa;
using SMRDominio.DTOs;
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
        public async Task<IActionResult> CriarPessoa([FromBody] CadastroPessoaDTO dto)
        {
            // Iniciar Transação
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var criarPessoa = new Pessoa
                {
                    id_pessoa_tipo = dto.id_pessoa_tipo,
                    email = dto.email,
                    senha_hash = BCrypt.Net.BCrypt.HashPassword(dto.senha_hash),
                    ativo = dto.ativo,
                    data_cadastro = dto.data_cadastro
                };

                _dbContext.Pessoa.Add(criarPessoa);
                await _dbContext.SaveChangesAsync();

                if (dto.id_pessoa_tipo == PessoaTipo.Empresa)
                {
                    var novaEmpresa = new Empresa
                    {
                        id_pessoa = criarPessoa.id_pessoa,
                        razao_social = dto.razao_social,
                        nome_fantasia = dto.nome,
                        cnpj = dto.documento,
                        telefone1 = dto.telefone1,
                        telefone2 = dto.telefone2,
                        cor_padrao = "#00A0FF"
                    };
                    _dbContext.Empresa.Add(novaEmpresa);
                }
                else if (dto.id_pessoa_tipo == PessoaTipo.Promotor)
                {
                    var novoPromotor = new Promotor
                    {
                        id_pessoa = criarPessoa.id_pessoa,
                        nome = dto.nome,
                        cpf = dto.documento,
                        celular = dto.celular,
                        pontos_acumulados = 0
                    };
                    _dbContext.Promotor.Add(novoPromotor);
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(criarPessoa);
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();

                string erroBanco = dbEx.InnerException?.Message ?? dbEx.Message;

                if (erroBanco.Contains("Duplicate entry"))
                {
                    // Se o erro do banco contiver a palavra 'email'
                    if (erroBanco.Contains("email"))
                        return BadRequest(new { Message = "Este e-mail já está cadastrado em nosso sistema." });

                    // Se o erro do banco contiver 'cpf' OU 'cnpj'
                    if (erroBanco.Contains("cpf") || erroBanco.Contains("cnpj"))
                        return BadRequest(new { Message = "Este CPF/CNPJ já está cadastrado em nosso sistema." });
                }

                // Mantém a genérica para outros bloqueios não mapeados
                return BadRequest(new { Message = "Já existe um cadastro com estes dados.", Detalhe = erroBanco });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { Message = "Erro inesperado ao realizar o cadastro", Detalhe = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("perfil/{id}")]
        [AllowAnonymous] // Considerar [Authorize]
        public async Task<IActionResult> ObterPerfil(int id)
        {
            var pessoa = await _dbContext.Pessoa.FirstOrDefaultAsync(p => p.id_pessoa == id);
            if (pessoa == null) return NotFound(new { Message = "Pessoa não encontrada" });

            var dto = new CadastroPessoaDTO
            {
                nome = pessoa.nome,
                email = pessoa.email,
                id_pessoa_tipo = pessoa.id_pessoa_tipo,
                ativo = pessoa.ativo,
                data_cadastro = pessoa.data_cadastro
            };

            // Se Promotor
            if (pessoa.id_pessoa_tipo == PessoaTipo.Promotor)
            {
                var promotor = await _dbContext.Promotor.FirstOrDefaultAsync(p => p.id_pessoa == id);
                if (promotor != null)
                {
                    dto.documento = promotor.cpf;
                    dto.celular = promotor.celular;
                }
            }
            // Se Empresa
            else if (pessoa.id_pessoa_tipo == PessoaTipo.Empresa)
            {
                var empresa = await _dbContext.Empresa.FirstOrDefaultAsync(e => e.id_pessoa == id);
                if (empresa != null)
                {
                    dto.nome_fantasia = empresa.nome_fantasia;
                    dto.razao_social = empresa.razao_social;
                    dto.documento = empresa.cnpj;
                    dto.telefone1 = empresa.telefone1;
                    dto.telefone2 = empresa.telefone2;
                }
            }

            return Ok(dto);
        }


        [HttpPut ("alterar")]
        [AllowAnonymous]
        public async Task<IActionResult> AlterarPessoa([FromBody] CadastroPessoaDTO dto)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // Busca a Pessoa base no banco de dados
                var pessoa = await _dbContext.Pessoa.FirstOrDefaultAsync(p => p.id_pessoa == dto.id_pessoa);
                if (pessoa == null) return NotFound(new { Message = "Usuário não encontrado." });

                // Atualiza os dados da tabela base
                pessoa.nome = dto.nome;
                pessoa.email = dto.email;
                pessoa.ativo = dto.ativo;

                // Só faz o hash e altera a senha se o usuário digitou algo no campo
                if (!string.IsNullOrWhiteSpace(dto.senha_hash))
                {
                    pessoa.senha_hash = BCrypt.Net.BCrypt.HashPassword(dto.senha_hash);
                }

                _dbContext.Pessoa.Update(pessoa);

                //  Atualiza a tabela filha específica
                if (dto.id_pessoa_tipo == PessoaTipo.Empresa)
                {
                    var empresa = await _dbContext.Empresa.FirstOrDefaultAsync(e => e.id_pessoa == dto.id_pessoa);
                    if (empresa != null)
                    {
                        empresa.razao_social = dto.razao_social;
                        empresa.nome_fantasia = dto.nome_fantasia;
                        empresa.cnpj = dto.documento;
                        empresa.telefone1 = dto.telefone1;
                        empresa.telefone2 = dto.telefone2;
                        _dbContext.Empresa.Update(empresa);
                    }
                }
                else if (dto.id_pessoa_tipo == PessoaTipo.Promotor)
                {
                    var promotor = await _dbContext.Promotor.FirstOrDefaultAsync(p => p.id_pessoa == dto.id_pessoa);
                    if (promotor != null)
                    {
                        promotor.cpf = dto.documento;
                        promotor.celular = dto.celular;
                        _dbContext.Promotor.Update(promotor);
                    }
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(pessoa);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { Message = "Erro ao atualizar o perfil", Detalhe = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpDelete("deletar/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeletarPessoa(int id)
        {
            try
            {
                var pessoa = await _dbContext.Pessoa.FirstOrDefaultAsync(p => p.id_pessoa == id);

                if (pessoa == null)
                    return NotFound(new { Message = "Usuário não encontrado." });

                // EXCLUSÃO LÓGICA (Soft Delete): Apenas mudamos o status para inativo
                pessoa.ativo = false;

                _dbContext.Pessoa.Update(pessoa);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Message = "Conta desativada com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro ao desativar a conta.", Detalhe = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost ("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] PessoaLogin pessoalogin)
        {
            int? idPessoaEncontrada = null;

            // 1. Tenta achar o documento na tabela de Promotor (CPF)
            var promotor = await _dbContext.Promotor.FirstOrDefaultAsync(p => p.cpf == pessoalogin.documento);
            if (promotor != null)
            {
                idPessoaEncontrada = promotor.id_pessoa;
            }
            else
            {
                // 2. Se não achou no Promotor, tenta achar na tabela de Empresa (CNPJ)
                var empresa = await _dbContext.Empresa.FirstOrDefaultAsync(e => e.cnpj == pessoalogin.documento);
                if (empresa != null)
                {
                    idPessoaEncontrada = empresa.id_pessoa;
                }
            }

            // Se o ID continuou nulo, é porque esse CPF/CNPJ não existe em lugar nenhum
            if (idPessoaEncontrada == null)
            {
                return Unauthorized(new { Message = "Usuário não encontrado!" });
            }

            // 3. Agora que sabemos o ID, buscamos a Pessoa para conferir a senha
            var pessoa = await _dbContext.Pessoa.FirstOrDefaultAsync(u => u.id_pessoa == idPessoaEncontrada);

            if (pessoa == null || !BCrypt.Net.BCrypt.Verify(pessoalogin.senha_hash, pessoa.senha_hash))
            {
                return Unauthorized(new { Message = "Senha inválida!" });
            }

            var token = TokenService.GenerateToken(pessoa);

            return Ok(new
            {
                usuario = pessoa,
                Token = token
            });

            /*
            var pessoa = await _dbContext.Pessoa.FirstOrDefaultAsync(u => u.login == pessoalogin.documento);

            if (pessoa == null || !BCrypt.Net.BCrypt.Verify(pessoalogin.senha_hash, pessoa.senha_hash))
            {
                return Unauthorized(new { Message = "Login ou senha inválidos!" });
            }

            var token = TokenService.GenerateToken(pessoa);

            return Ok(new
            {
                usuario = pessoa,
                Token = token
            });*/
        }
    }
}

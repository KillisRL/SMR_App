using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMRApi.Services;
using SMRDominio.ClassePessoa;
using SMRDominio.DTOs;
using SMRInfraestrutura;
<<<<<<< HEAD
=======
using System.Net;
>>>>>>> dfa26fb (criação da service e api de recompensas)

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

<<<<<<< HEAD
=======
        #region cadastrar
>>>>>>> dfa26fb (criação da service e api de recompensas)
        [HttpPost("cadastrar")]
        [AllowAnonymous]
        public async Task<IActionResult> CriarPessoa([FromBody] CadastroPessoaDTO dto)
        {
            // Iniciar Transação
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
<<<<<<< HEAD
                var criarPessoa = new Pessoa
                {
                    id_pessoa_tipo = dto.id_pessoa_tipo,
                    email = dto.email,
                    senha_hash = BCrypt.Net.BCrypt.HashPassword(dto.senha_hash),
                    ativo = true,
=======
                // ========================================================
                // 1. VERIFICAÇÃO DE EXISTÊNCIA E REATIVAÇÃO DO CADASTRO
                // ========================================================
                int? idPessoaExistente = null;

                // Procura o documento na tabela filha correspondente
                if (dto.id_pessoa_tipo == PessoaTipo.Promotor)
                {
                    var promotorExistente = await _dbContext.Promotor.FirstOrDefaultAsync(p => p.cpf == dto.documento);
                    if (promotorExistente != null) idPessoaExistente = promotorExistente.id_pessoa;
                }
                else if (dto.id_pessoa_tipo == PessoaTipo.Empresa)
                {
                    var empresaExistente = await _dbContext.Empresa.FirstOrDefaultAsync(e => e.cnpj == dto.documento);
                    if (empresaExistente != null) idPessoaExistente = empresaExistente.id_pessoa;
                }

                // Se encontrou o documento no banco...
                if (idPessoaExistente != null)
                {
                    var pessoaExistente = await _dbContext.Pessoa.FirstOrDefaultAsync(p => p.id_pessoa == idPessoaExistente);

                    if (pessoaExistente != null)
                    {
                        if (pessoaExistente.ativo == true)
                        {
                            // Já existe e está ativo. Barra o cadastro na hora, antes de dar erro no banco.
                            return BadRequest(new { Message = "Este CPF/CNPJ já está cadastrado e ativo em nosso sistema." });
                        }
                        else
                        {
                            // CONTA EXCLUÍDA: Reativar e atualizar com os dados novos da tela
                            pessoaExistente.ativo = true;
                            pessoaExistente.nome = dto.nome;
                            pessoaExistente.email = dto.email;

                            if (!string.IsNullOrWhiteSpace(dto.senha_hash))
                            {
                                pessoaExistente.senha_hash = BCrypt.Net.BCrypt.HashPassword(dto.senha_hash);
                            }
                            _dbContext.Pessoa.Update(pessoaExistente);

                            // Atualiza também os dados específicos na tabela filha
                            if (dto.id_pessoa_tipo == PessoaTipo.Promotor)
                            {
                                var promotor = await _dbContext.Promotor.FirstOrDefaultAsync(p => p.id_pessoa == idPessoaExistente);
                                if (promotor != null)
                                {
                                    promotor.celular = dto.celular;
                                    _dbContext.Promotor.Update(promotor);
                                }
                            }
                            else if (dto.id_pessoa_tipo == PessoaTipo.Empresa)
                            {
                                var empresa = await _dbContext.Empresa.FirstOrDefaultAsync(e => e.id_pessoa == idPessoaExistente);
                                if (empresa != null)
                                {
                                    empresa.razao_social = dto.razao_social;
                                    empresa.telefone1 = dto.telefone1;
                                    empresa.telefone2 = dto.telefone2;
                                    _dbContext.Empresa.Update(empresa);
                                }
                            }

                            await _dbContext.SaveChangesAsync();
                            await transaction.CommitAsync();

                            // Sai do método aqui, retorna sucesso fingindo que foi um cadastro comum.
                            return Ok(new { Message = "Conta reativada com sucesso!" });
                        }
                    }
                }

                // ========================================================
                // 2. FLUXO NORMAL DE CADASTRO (Se o documento não existe)
                // ========================================================
                var criarPessoa = new Pessoa
                {
                    nome = dto.nome,
                    id_pessoa_tipo = dto.id_pessoa_tipo,
                    email = dto.email,
                    senha_hash = BCrypt.Net.BCrypt.HashPassword(dto.senha_hash),
                    ativo = dto.ativo,
>>>>>>> dfa26fb (criação da service e api de recompensas)
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
<<<<<<< HEAD
                        nome_fantasia = dto.nome,
=======
>>>>>>> dfa26fb (criação da service e api de recompensas)
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
<<<<<<< HEAD
                        nome = dto.nome,
=======
>>>>>>> dfa26fb (criação da service e api de recompensas)
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
<<<<<<< HEAD

        [HttpGet("perfil/{id}")]
        [AllowAnonymous] // Considerar [Authorize]
        public async Task<IActionResult> ObterPerfil(int id)
        {
=======
        #endregion

        #region perfil
        [HttpGet("perfil/{id}")]
        [Authorize]
        public async Task<IActionResult> ObterPerfil(int id)
        {
            var usuarioClaim = User.FindFirst("id_pessoa")?.Value;

            if (string.IsNullOrEmpty(usuarioClaim) || !int.TryParse(usuarioClaim, out int idPessoaLogada))
            {
                return Unauthorized(new { Message = "Usuário não autenticado ou identificador inválido no token." });
            }

>>>>>>> dfa26fb (criação da service e api de recompensas)
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
<<<<<<< HEAD
                    dto.nome_fantasia = empresa.nome_fantasia;
=======
>>>>>>> dfa26fb (criação da service e api de recompensas)
                    dto.razao_social = empresa.razao_social;
                    dto.documento = empresa.cnpj;
                    dto.telefone1 = empresa.telefone1;
                    dto.telefone2 = empresa.telefone2;
                }
            }

            return Ok(dto);
        }
<<<<<<< HEAD


        [HttpPut ("alterar")]
        [AllowAnonymous]
=======
        #endregion

        #region alterar
        [HttpPut("alterar")]
        [Authorize]
>>>>>>> dfa26fb (criação da service e api de recompensas)
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
<<<<<<< HEAD
                        empresa.nome_fantasia = dto.nome_fantasia;
=======
>>>>>>> dfa26fb (criação da service e api de recompensas)
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
<<<<<<< HEAD

        [HttpDelete("deletar/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeletarPessoa(int id)
        {
=======
        #endregion

        #region deletar
        [HttpDelete("deletar/{id}")]
        [Authorize]
        public async Task<IActionResult> DeletarPessoa(int id)
        {


>>>>>>> dfa26fb (criação da service e api de recompensas)
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
<<<<<<< HEAD

        [HttpPost ("login")]
=======
        #endregion

        #region login
        [HttpPost("login")]
>>>>>>> dfa26fb (criação da service e api de recompensas)
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

            if (pessoa.ativo == false)
            {
                return Unauthorized(new { Message = "Esta conta foi desativada ou excluída." });
            }

            var token = TokenService.GenerateToken(pessoa);

            return Ok(new
            {
                usuario = pessoa,
                Token = token
            });

        }
<<<<<<< HEAD
=======
        #endregion

        #region solicitar_codigo
        // ========================================================
        //        ENDPOINT PARA RECUPERAÇÃO DE SENHA
        // ========================================================
        [HttpPost("solicitar-codigo")]
        public async Task<IActionResult> SolicitarCodigoRecuperacao([FromBody] RecuperarSenhaRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new { erro = "O e-mail é obrigatório." });
            }

            try
            {
                // 1. Verificar se o e-mail existe na sua tabela de Pessoas
                var usuarioExiste = await _dbContext.Pessoa.AnyAsync(p => p.email == request.Email);

                if (!usuarioExiste)
                {
                    return NotFound(new { erro = "E-mail não encontrado no sistema." });
                }

                // 2. Gerar o código de 6 dígitos (Garante os zeros à esquerda com o "D6")
                string codigo = new Random().Next(1, 999999).ToString("D6");
                DateTime dataExpiracao = DateTime.Now.AddMinutes(15);

                // 3. Inserir direto na tabela que criamos no HeidiSQL
                var sql = "INSERT INTO recuperacao_senha (email, codigo, data_expiracao) VALUES ({0}, {1}, {2})";
                await _dbContext.Database.ExecuteSqlRawAsync(sql, request.Email, codigo, dataExpiracao);

                // 4. Configuração do envio de e-mail nativo do C# (SmtpClient)
                try
                {
                    var mailMessage = new System.Net.Mail.MailMessage();
                    mailMessage.From = new System.Net.Mail.MailAddress("felipe120505@gmail.com");
                    mailMessage.To.Add(request.Email);
                    mailMessage.Subject = "SMR APP - Código de Recuperação";
                    mailMessage.Body = $"Olá!\n\nSeu código de verificação é: {codigo}\n\nEste código é válido por 15 minutos.";

                    using (var smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com"))
                    {
                        smtpClient.Port = 587; // Porta padrão de segurança do Gmail
                        smtpClient.EnableSsl = true; // Criptografia ativada
                        smtpClient.UseDefaultCredentials = false;

                        // Suas credenciais
                        smtpClient.Credentials = new NetworkCredential(
                            "felipe120505@gmail.com", // O mesmo e-mail do 'From'
                            "ugnw uygz hvem pnqi"
                        );

                        // Enviando o e-mail!
                        smtpClient.Send(mailMessage);
                    }
                }
                catch
                {
                    // Ignora o erro de envio físico do e-mail no ambiente de desenvolvimento local
                    // para permitir que você teste a lógica gravando direto no banco
                }

                return Ok(new { mensagem = "Código de recuperação gerado com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = $"Erro interno no servidor: {ex.Message}" });
            }
        }
        #endregion

        #region validacao_codigo_recuperacao
        // ========================================================
        // VALIDAÇÃO DO CÓDIGO DE RECUPERAÇÃO
        // ========================================================
        [HttpPost("validar-codigo")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidarCodigo([FromBody] ValidarCodigoRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Codigo))
                return BadRequest(new { Message = "E-mail e código são obrigatórios." });

            bool codigoValido = false;

            // Como não criamos um 'DbSet' para a tabela recuperacao_senha para poupar tempo,
            // abrimos uma conexão rápida direto com o MariaDB para fazer o SELECT:
            var connection = _dbContext.Database.GetDbConnection();
            try
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    // A query verifica se o código bate, se a data de expiração ainda é maior que o AGORA, e se não foi utilizado (0)
                    command.CommandText = "SELECT COUNT(*) FROM recuperacao_senha WHERE email = @email AND codigo = @codigo AND data_expiracao >= NOW() AND utilizado = 0";

                    var paramEmail = command.CreateParameter();
                    paramEmail.ParameterName = "@email";
                    paramEmail.Value = request.Email;
                    command.Parameters.Add(paramEmail);

                    var paramCodigo = command.CreateParameter();
                    paramCodigo.ParameterName = "@codigo";
                    paramCodigo.Value = request.Codigo;
                    command.Parameters.Add(paramCodigo);

                    var result = await command.ExecuteScalarAsync();
                    codigoValido = Convert.ToInt32(result) > 0;
                }
            }
            finally
            {
                await connection.CloseAsync();
            }

            if (!codigoValido)
                return BadRequest(new { Message = "Código inválido ou expirado." });

            return Ok(new { Message = "Código validado com sucesso!" });
        }

        // ========================================================
        // REDEFINIÇÃO DA SENHA FINAL
        // ========================================================
        [HttpPost("redefinir-senha")]
        [AllowAnonymous]
        public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.NovaSenha) || string.IsNullOrEmpty(request.Codigo))
                return BadRequest(new { Message = "Dados incompletos." });

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // 1. Busca a Pessoa pelo email que tá na sua model
                var pessoa = await _dbContext.Pessoa.FirstOrDefaultAsync(p => p.email == request.Email);
                if (pessoa == null)
                    return NotFound(new { Message = "Usuário não encontrado." });

                // 2. Faz o hash da senha nova com o BCrypt (Igual você já faz no seu Cadastro)
                pessoa.senha_hash = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);
                _dbContext.Pessoa.Update(pessoa);

                // 3. Queima o código marcando utilizado como 1 (Para evitar hackers tentando reutilizar)
                var sqlInvalida = "UPDATE recuperacao_senha SET utilizado = 1 WHERE email = {0} AND codigo = {1}";
                await _dbContext.Database.ExecuteSqlRawAsync(sqlInvalida, request.Email, request.Codigo);

                // 4. Salva no banco!
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { Message = "Senha redefinida com sucesso!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { Message = "Erro ao redefinir senha.", Detalhe = ex.InnerException?.Message ?? ex.Message });
            }
        }
        #endregion

        // ========================================================
        // DTOs PARA AS REQUISIÇÕES
        // ========================================================
        public class RecuperarSenhaRequest
        {
            public string Email { get; set; }
        }

        public class ValidarCodigoRequest
        {
            public string Email { get; set; }
            public string Codigo { get; set; }
        }

        public class RedefinirSenhaRequest
        {
            public string Email { get; set; }
            public string Codigo { get; set; }
            public string NovaSenha { get; set; }
        }

>>>>>>> dfa26fb (criação da service e api de recompensas)
    }
}

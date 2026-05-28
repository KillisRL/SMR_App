using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMRApi.Services;
using SMRDominio.ClassePessoa;
using SMRDominio.ClasseRecompensa;
using SMRDominio.DTOs;
using SMRInfraestrutura;

namespace SMRApi.Controllers
{
    [ApiController]
    [Route ("recompensas")]
    public class RecompensaController : Controller
    {
        SMRDBContext _dbContext;

        public RecompensaController(SMRDBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }


        [HttpPost("cadastrar")]
        [Authorize]
        public async Task<IActionResult> CadastrarRecompensa([FromBody] Recompensa recompensa)
        {
            if (recompensa == null || !ModelState.IsValid)
            {
                return BadRequest(new { Message = "Dados da recompensa inválidos." });
            }

            try
            {

                //var usuarioClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                //                   ?? User.FindFirst("id")?.Value;

                //if (string.IsNullOrEmpty(usuarioClaim) || !int.TryParse(usuarioClaim, out int codigoUsuarioLogado))
                //{
                //    return Unauthorized(new { Message = "Usuário não autenticado ou identificador inválido no token." });
                //}
                var novaRecompensa = new Recompensa
                {
                    id_empresa =  recompensa.id_empresa,//codigoUsuarioLogado,
                    titulo = recompensa.titulo,
                    descricao = recompensa.descricao,
                    pontos_necessarios = recompensa.pontos_necessarios,
                    Ativo = recompensa.Ativo
                };

                _dbContext.Recompensas.Add(novaRecompensa);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Message = "Recompensa cadastrada com sucesso!", Id = novaRecompensa.id });

            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro ao cadastrar a recompensa", Detalhe = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}

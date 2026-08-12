using SMRDominio.ClasseBase;
using SMRDominio.ClasseBonificacao;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
// using SMRDominio.DTOs; // Descomente para importar os DTOs do seu TCC

namespace SMR_App.Services
{
    public class ApiServicesBonificacao
    {
        private readonly HttpClient _httpClient;

        // Mantendo o HTTPS que você corrigiu e apontando para a porta do seu Swagger
        private const string BaseUrl = "https://localhost:7190/";

        public ApiServicesBonificacao()
        {
            // O Handler que driba a rede da faculdade, igual você fez na ConfigEmpresaViewModel
            var handler = new HttpClientHandler { UseProxy = false };
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        // ==========================================================
        // CADASTRAR BONIFICAÇÃO (POST)
        // ==========================================================
        // Troque 'object' pelo seu DTO real, ex: CadastroBonificacaoDTO
        public async Task<(bool Sucesso, string Mensagem)> CadastrarBonificacao(Bonificacao bonificacao, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsJsonAsync("bonificacao/cadastrar", bonificacao);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var retorno = await response.Content.ReadFromJsonAsync<ApiRetornoMensagem>(options);

                if (response.IsSuccessStatusCode)
                {
                    string mensagemSucesso = retorno.Mensagem;

                    return (true, mensagemSucesso);
                }
                string erroApi = retorno.Mensagem;
                return (false, erroApi);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceção ao cadastrar: {ex.Message}");
                return (false, "Falha de comunicação com o servidor.");
            }
        }

        // ==========================================================
        // ALTERAR BONIFICAÇÃO (PUT)
        // ==========================================================
        public async Task<(bool Sucesso, string Mensagem)> AlterarBonificacao(Bonificacao bonificacao, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PutAsJsonAsync("bonificacao/alterar", bonificacao);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var retorno = await response.Content.ReadFromJsonAsync<ApiRetornoMensagem>(options);

                if (response.IsSuccessStatusCode)
                {
                    string mensagemSuceso = retorno.Mensagem;
                    return (true, mensagemSuceso);
                }
                else
                {
                    string mensagemErro = retorno.Mensagem;
                    return (false, mensagemErro);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceção ao cadastrar: {ex.Message}");
                return (false, "Falha de comunicação com o servidor.");
            }
        }

        // ==========================================================
        // DELETAR BONIFICAÇÃO (DELETE)
        // ==========================================================
        public async Task<(bool Sucesso, string Mensagem)> DeletarBonificacaoService(int codigoBonificacao, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Passa o ID na rota para deletar do banco
                var response = await _httpClient.DeleteAsync($"bonificacao/excluir/{codigoBonificacao}");
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true};

                var retorno = await response.Content.ReadFromJsonAsync<ApiRetornoMensagem>(options);
                
                if (response.IsSuccessStatusCode)
                {
                    string mensagemSucesso = retorno.Mensagem;
                    return(true, mensagemSucesso);
                }
                else
                {
                    string mensagemErro = retorno.Mensagem;
                    return (false, mensagemErro);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceção ao cadastrar: {ex.Message}");
                return (false, "Falha de comunicação com o servidor.");
            }
        }

        // ==========================================================
        // CONSULTAR BONIFICAÇÃO (GET)
        // ==========================================================
        public async Task<(bool Sucesso, string Mensagem, List<Bonificacao> Dados)> ConsultarBonificacao(string token, string? nome, bool? ativo)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var query = new List<string>();

                // 1. CORREÇÃO: Adicionado o IF para validar se 'nome' não é nulo antes de sanitizar a string
                if (!string.IsNullOrWhiteSpace(nome))
                {
                    query.Add($"nome={Uri.EscapeDataString(nome)}");
                }

                if (ativo.HasValue)
                {
                    query.Add($"ativo={ativo.Value.ToString().ToLower()}");
                }

                // Monta a URL dinâmica
                var urlCompleta = query.Any()
                    ? $"bonificacao/consultar?{string.Join("&", query)}"
                    : "bonificacao/consultar";

                var resultado = await _httpClient.GetAsync(urlCompleta);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // 2. CORREÇÃO: Lê o JSON apenas quando necessário, de acordo com o status HTTP
                if (resultado.IsSuccessStatusCode)
                {
                    var dados = await resultado.Content.ReadFromJsonAsync<List<Bonificacao>>(options);
                    return (true, string.Empty, dados ?? new List<Bonificacao>());
                }
                else
                {
                    var retorno = await resultado.Content.ReadFromJsonAsync<ApiRetornoMensagem>(options);
                    string mensagemErro = retorno?.Mensagem ?? "Falha ao consultar bonificações.";

                    return (false, mensagemErro, new List<Bonificacao>());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceção ao consultar bonificação: {ex.Message}");
                return (false, "Falha de comunicação com o servidor.", null);
            }
        }

        public async Task<(bool Sucesso, List<Bonificacao> Dados, string Mensagem)> ConsultarBonificacaoIndicacao(string token, int codigoEmpresa)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var resultado = await _httpClient.GetAsync($"bonificacao/casultar-indicacao{codigoEmpresa}");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if (resultado.IsSuccessStatusCode)
                {
                    var dados = await resultado.Content.ReadFromJsonAsync<List<Bonificacao>>(options);

                    return (true, dados ?? new List<Bonificacao>(), string.Empty);
                }
                else
                {
                    var retorno = await resultado.Content.ReadFromJsonAsync<ApiRetornoMensagem>(options);
                    string mensagem = retorno.Mensagem;
                    return (false, null, mensagem);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceção ao consultar bonificação: {ex.Message}");
                return (false, null, "Falha de comunicação com o servidor");
            }
        }

    }
}
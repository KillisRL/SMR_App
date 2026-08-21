using SMRDominio.ClasseBase;
using SMRDominio.ClassePessoa;
using SMRDominio.DTOs;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace SMR_App.Services
{
    public class ApiServicesPessoa
    {
        private readonly HttpClient _httpClient;
        public ApiServicesPessoa()
        {
            var handler = new HttpClientHandler();
            handler.UseProxy = false;

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(ConfiguracoesApp.UrlApi)
            };
        }

        public async Task<(bool Sucesso, List<Empresa> Dados, string Mensagem)> ConsultarEmpresa(string? token, string? razaoSocial)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var parametro = new List<string> { $"razaoSocial={razaoSocial}" };

                var url = $"pessoa/consultar-empresa?{string.Join("&", parametro)}";

                var resultado = await _httpClient.GetAsync(url);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if (resultado.IsSuccessStatusCode)
                {
                    var dados = await resultado.Content.ReadFromJsonAsync<List<Empresa>>(options);

                    return (true, dados, string.Empty);
                }
                else
                {

                    var retorno = await resultado.Content.ReadFromJsonAsync<ApiRetornoMensagem>(options);
                    string mensagemErro = retorno.Mensagem;
                    return (false, null, mensagemErro);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceção ao cadastrar pessoa: {ex.Message}");
                return (false, null, "Servidor indisponível no momento.");
            }

        }
        public async Task<(bool Sucesso, string Mensagem)> CadastrarPessoaService(CadastroPessoaDTO dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("pessoa/cadastrar", dto);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Cadastro realizado com sucesso!");
                }
                else
                {
                    // Obter Erro da API
                    var errorJson = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var jsonDoc = JsonDocument.Parse(errorJson);
                        if (jsonDoc.RootElement.TryGetProperty("message", out var msg))
                        {
                            return (false, msg.GetString()); // Retorna a mensagem amigável
                        }
                    }
                    catch
                    {
                        // Se não for JSON, ignora
                    }

                    return (false, "Falha ao cadastrar. Verifique os dados.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceção ao cadastrar pessoa: {ex.Message}");
                return (false, "Servidor indisponível no momento.");
            }
        }

        // 'Read' do CRUD: Consulta os dados completos da API usando GET
        public async Task<CadastroPessoaDTO?> ObterPerfilCompleto(int id, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"pessoa/perfil/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CadastroPessoaDTO>();
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao buscar perfil: {ex.Message}");
                return null;
            }
        }

        // 'Update' do CRUD: Envia a alteração para a API usando PUT
        public async Task<(bool Sucesso, string Mensagem)> AlterarPessoaService(CadastroPessoaDTO dto, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                // Atenção aqui: PutAsJsonAsync em vez de Post
                var response = await _httpClient.PutAsJsonAsync("pessoa/alterar", dto);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Perfil atualizado com sucesso!");
                }
                else
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var jsonDoc = JsonDocument.Parse(errorJson);
                        if (jsonDoc.RootElement.TryGetProperty("message", out var msg))
                        {
                            return (false, msg.GetString());
                        }
                    }
                    catch
                    {
                        // Ignora se falhar o parse
                    }

                    return (false, "Falha ao atualizar o perfil. Verifique os dados.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceção ao alterar pessoa: {ex.Message}");
                return (false, "Servidor indisponível no momento.");
            }
        }

        #region EXCLUIR
        public async Task<(bool Sucesso, string Mensagem)> DeletarPessoaService(int id, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.DeleteAsync($"pessoa/deletar/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Conta excluída com sucesso!");
                }
                return (false, "Falha ao excluir a conta.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao deletar: {ex.Message}");
                return (false, "Erro de comunicação com o servidor.");
            }
        }
        #endregion

        public async Task<Pessoa?> Login(PessoaLogin login)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("pessoa/login", login);

                if (response.IsSuccessStatusCode) // Sucesso durante processo de Login
                {
                    // resultado = Usuario+Token
                    var resultado = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (resultado != null && !string.IsNullOrEmpty(resultado.token))
                    {
                        await SecureStorage.Default.SetAsync("jwt_token", resultado.token);
                    }
                    return resultado?.usuario;

                }
                else // Falha durante processo de login
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Falha ao realizar o login. Status: {response.StatusCode}, Erro: {errorMessage}");
                    return null;
                }
            }
            catch (Exception ex) // Falhas inesperadas
            {
                Debug.WriteLine($"Exceção ao cadastrar pessoa: {ex.Message}");
                return null;
            }
        }
    }
}

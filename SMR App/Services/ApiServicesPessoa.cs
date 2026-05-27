using SMRDominio.ClassePessoa;
using SMRDominio.DTOs;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;

namespace SMR_App.Services
{
    public class ApiServicesPessoa
    {
        private readonly HttpClient _httpClient;
        public ApiServicesPessoa()
        {
            string baseURL = "http://localhost:5015";

            _httpClient = new HttpClient
            {
                BaseAddress =  new Uri(baseURL)
            };

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
        public async Task<CadastroPessoaDTO?> ObterPerfilCompleto(int id)
        {
            try
            {
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
        public async Task<(bool Sucesso, string Mensagem)> AlterarPessoaService(CadastroPessoaDTO dto)
        {
            try
            {
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

        public async Task<Pessoa?> Login(PessoaLogin login)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("pessoa/login", login);

                if (response.IsSuccessStatusCode) // Sucesso durante processo de Login
                {
                    // resultado = Usuario+Token
                    var resultado = await response.Content.ReadFromJsonAsync<LoginResponse>();
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

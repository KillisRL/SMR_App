using SMRDominio.ClassePessoa;
using SMRDominio.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
                    // Pega o erro que veio da API
                    var errorJson = await response.Content.ReadAsStringAsync();
                    try
                    {
                        // Tenta extrair aquela propriedade "Message" que criamos no BadRequest da Controller
                        var jsonDoc = JsonDocument.Parse(errorJson);
                        if (jsonDoc.RootElement.TryGetProperty("message", out var msg))
                        {
                            return (false, msg.GetString()); // Retorna a mensagem amigável (ex: "Este e-mail já está...")
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

using SMRDominio.ClassePessoa;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
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
        public async Task<bool> CadastrarPessoaService(Pessoa pessoa)
        {
            
            try
            {
                var response = await _httpClient.PostAsJsonAsync("pessoa/cadastrar", pessoa);

                if (response.IsSuccessStatusCode)
                {
                    var pessoaNova = await response.Content.ReadFromJsonAsync<Pessoa>();
                    return true;
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Falha ao cadastrar pessoa. Status: {response.StatusCode}, Erro: {errorMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Exceção ao cadastrar pessoa: {ex.Message}");
                return false;
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

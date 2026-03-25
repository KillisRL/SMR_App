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

        public async Task<bool> Login(PessoaLogin login)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("pessoa/login", login);

                if (response.IsSuccessStatusCode)//Aqui é quando temos sucesso no login
                {
                    var entrar = await response.Content.ReadFromJsonAsync<PessoaLogin>();
                    return true;
                }
                else //Aqui é quando temos falha durante o processo de login
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Falha ao realizar o login. Status: {response.StatusCode}, Erro: {errorMessage}");
                    return false;
                }
            }
            catch (Exception ex) //falhas inesperadas
            {
                Debug.WriteLine($"Exceção ao cadastrar pessoa: {ex.Message}");
                return false;
            }
        }
    }
}

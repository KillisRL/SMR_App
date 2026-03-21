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
                var response = await _httpClient.PostAsJsonAsync("pessoa/cadastro", pessoa);

                if (response.IsSuccessStatusCode)
                {
                    // Sucesso!
                    var pessoaNova = await response.Content.ReadFromJsonAsync<Pessoa>();
                    return true;
                }
                else
                {
                    //Falhas tratadas
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Falha ao cadastrar pessoa. Status: {response.StatusCode}, Erro: {errorMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Erros de rede ou exceções inesperadas
                Debug.WriteLine($"Exceção ao cadastrar pessoa: {ex.Message}");
                return false;
            }
        }
    }
}

<<<<<<< HEAD
<<<<<<< HEAD
﻿using SMR_App.Services;
using SMRDominio.ClassePessoa;
using System.Windows.Input;
=======
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
>>>>>>> dfa26fb (criação da service e api de recompensas)
=======
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using SMR_App.Services;
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34

namespace SMR_App.ViewModels
{
    public class ConfigEmpresaViewModel : BaseViewModel
    {
<<<<<<< HEAD
<<<<<<< HEAD
=======
        private readonly HttpClient _httpClient;
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34

        // Mantemos APENAS o comando de importação.
        // O "AbrirTelaCommand" que você usou nos botões do XAML já está sendo herdado do BaseViewModel!
        public ICommand ImportarClientesCommand { get; }

        public ConfigEmpresaViewModel()
        {
            // O Handler com UseProxy = false driba a rede da faculdade!
            var handler = new HttpClientHandler { UseProxy = false };

            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            _httpClient = new HttpClient(handler)
            {
                // Ajuste para o IP/Porta do seu Swagger (use 10.0.2.2 no emulador Android)
                BaseAddress = new Uri("https://localhost:7190/")
            };

            ImportarClientesCommand = new Command(async () => await ImportarClientesCsvAsync());
        }

        // ==========================================================
        // LÓGICA DE IMPORTAÇÃO DE ARQUIVO
        // ==========================================================
        private async Task ImportarClientesCsvAsync()
        {
            try
            {
                // A. Força o celular a mostrar apenas arquivos .csv
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.comma-separated-values-text" } },
                    { DevicePlatform.Android, new[] { "text/csv", "text/comma-separated-values" } },
                    { DevicePlatform.WinUI, new[] { ".csv" } }
                });

                var options = new PickOptions
                {
                    PickerTitle = "Selecione a planilha CSV de clientes",
                    FileTypes = customFileType,
                };

                // B. Abre a gaveta de arquivos do celular
                var resultadoArquivo = await FilePicker.Default.PickAsync(options);

                if (resultadoArquivo != null)
                {
                    // C. Lê o conteúdo do arquivo
                    using var stream = await resultadoArquivo.OpenReadAsync();
                    using var reader = new StreamReader(stream);
                    var conteudoCompleto = await reader.ReadToEndAsync();

                    // D. Quebra o texto em linhas
                    var linhas = conteudoCompleto.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    var listaClientes = new List<ClienteImportacaoDTO>();

                    // E. Loop para ler cada linha (Começamos do 'i = 1' para pular o cabeçalho)
                    for (int i = 1; i < linhas.Length; i++)
                    {
                        // O delimitador do CSV geralmente é vírgula ou ponto-e-vírgula
                        var colunas = linhas[i].Split(';', ',');

                        if (colunas.Length >= 2)
                        {
                            listaClientes.Add(new ClienteImportacaoDTO
                            {
                                Nome = colunas[0].Trim(),
                                Documento = colunas[1].Trim()
                            });
                        }
                    }

                    // F. Envia a lista para a API
                    if (listaClientes.Count > 0)
                    {
                        // 1. Resgata o ID da empresa salvo no momento do Login
                        // (O número '0' no final é o valor padrão caso ele não encontre nada)
                        int idEmpresaLogada = ApiServicesSessaoPessoa.PessoaLogada.id_pessoa;

                        // 2. Trava de segurança: se for 0, o usuário não está logado corretamente
                        if (idEmpresaLogada == 0)
                        {
                            await Shell.Current.DisplayAlert("Sessão Expirada", "Não foi possível identificar a empresa. Por favor, faça login novamente.", "OK");
                            return;
                        }

                        // 3. Usa a variável dinâmica na URL da API!
                        var response = await _httpClient.PostAsJsonAsync($"pessoa/{idEmpresaLogada}/importar", listaClientes);

                        if (response.IsSuccessStatusCode)
                        {
                            await Shell.Current.DisplayAlert("Show!", $"{listaClientes.Count} clientes importados com sucesso na base!", "OK");
                        }
                        else
                        {
                            await Shell.Current.DisplayAlert("Ops", "A API recusou a importação. Verifique se a empresa existe.", "OK");
                        }
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Aviso", "O arquivo parece estar vazio ou no formato incorreto.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Falha ao ler arquivo: {ex.Message}", "OK");
            }
        }
    }
<<<<<<< HEAD
}
=======
        private readonly HttpClient _httpClient;

        // Mantemos APENAS o comando de importação.
        // O "AbrirTelaCommand" que você usou nos botões do XAML já está sendo herdado do BaseViewModel!
        public ICommand ImportarClientesCommand { get; }

        public ConfigEmpresaViewModel()
        {
            // O Handler com UseProxy = false driba a rede da faculdade!
            var handler = new HttpClientHandler { UseProxy = false };
            _httpClient = new HttpClient(handler)
            {
                // Ajuste para o IP/Porta do seu Swagger (use 10.0.2.2 no emulador Android)
                BaseAddress = new Uri("http://localhost:7190/api/empresa/")
            };

            ImportarClientesCommand = new Command(async () => await ImportarClientesCsvAsync());
        }

        // ==========================================================
        // LÓGICA DE IMPORTAÇÃO DE ARQUIVO
        // ==========================================================
        private async Task ImportarClientesCsvAsync()
        {
            try
            {
                // A. Força o celular a mostrar apenas arquivos .csv
                var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.comma-separated-values-text" } },
                    { DevicePlatform.Android, new[] { "text/csv", "text/comma-separated-values" } },
                    { DevicePlatform.WinUI, new[] { ".csv" } }
                });

                var options = new PickOptions
                {
                    PickerTitle = "Selecione a planilha CSV de clientes",
                    FileTypes = customFileType,
                };

                // B. Abre a gaveta de arquivos do celular
                var resultadoArquivo = await FilePicker.Default.PickAsync(options);

                if (resultadoArquivo != null)
                {
                    // C. Lê o conteúdo do arquivo
                    using var stream = await resultadoArquivo.OpenReadAsync();
                    using var reader = new StreamReader(stream);
                    var conteudoCompleto = await reader.ReadToEndAsync();

                    // D. Quebra o texto em linhas
                    var linhas = conteudoCompleto.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    var listaClientes = new List<ClienteImportacaoDTO>();

                    // E. Loop para ler cada linha (Começamos do 'i = 1' para pular o cabeçalho)
                    for (int i = 1; i < linhas.Length; i++)
                    {
                        // O delimitador do CSV geralmente é vírgula ou ponto-e-vírgula
                        var colunas = linhas[i].Split(';', ',');

                        if (colunas.Length >= 2)
                        {
                            listaClientes.Add(new ClienteImportacaoDTO
                            {
                                Nome = colunas[0].Trim(),
                                Documento = colunas[1].Trim()
                            });
                        }
                    }

                    // F. Envia a lista para a API
                    if (listaClientes.Count > 0)
                    {
                        var response = await _httpClient.PostAsJsonAsync("importar", listaClientes);

                        if (response.IsSuccessStatusCode)
                        {
                            await Application.Current.MainPage.DisplayAlert("Show!", $"{listaClientes.Count} clientes importados com sucesso na base!", "OK");
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Ops", "A API recusou a importação. Verifique os logs.", "OK");
                        }
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Aviso", "O arquivo parece estar vazio ou no formato incorreto.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Falha ao ler arquivo: {ex.Message}", "OK");
            }
        }
    }
=======
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34

    // ==========================================================
    // DTO DE TRANSFERÊNCIA (O formato do JSON)
    // ==========================================================
    public class ClienteImportacaoDTO
    {
        public string Nome { get; set; }
        public string Documento { get; set; }
    }
<<<<<<< HEAD
}
>>>>>>> dfa26fb (criação da service e api de recompensas)
=======
}
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34

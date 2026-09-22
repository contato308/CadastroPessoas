using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CadastroPessoas.WebForms.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CadastroPessoas.WebForms.Services
{
    public class PessoasApiClient
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private readonly string _baseUrl;

        public PessoasApiClient()
        {
            var configuredUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            Uri baseUri;

            if (!Uri.TryCreate(configuredUrl, UriKind.Absolute, out baseUri))
            {
                throw new ConfigurationErrorsException("A configuração ApiBaseUrl está ausente ou inválida.");
            }

            _baseUrl = baseUri.ToString().TrimEnd('/') + "/";
        }

        public Task<PessoaApiModel[]> ListarAsync()
        {
            return GetAsync<PessoaApiModel[]>("api/pessoas");
        }

        public Task<PessoaApiModel> BuscarPorIdAsync(int id)
        {
            return GetAsync<PessoaApiModel>("api/pessoas/" + id);
        }

        public Task<PessoaApiModel> AdicionarAsync(PessoaRequest pessoa)
        {
            return SendPessoaAsync(HttpMethod.Post, "api/pessoas", pessoa);
        }

        public Task<PessoaApiModel> AtualizarAsync(int id, PessoaRequest pessoa)
        {
            return SendPessoaAsync(HttpMethod.Put, "api/pessoas/" + id, pessoa);
        }

        public async Task ExcluirAsync(int id)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, _baseUrl + "api/pessoas/" + id))
            using (var response = await EnviarAsync(request).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    return;
                }

                throw await CriarErroAsync(response).ConfigureAwait(false);
            }
        }

        private Task<T> GetAsync<T>(string path)
        {
            return SendAsync<T>(new HttpRequestMessage(HttpMethod.Get, _baseUrl + path));
        }

        private Task<PessoaApiModel> SendPessoaAsync(HttpMethod method, string path, PessoaRequest pessoa)
        {
            var json = JsonConvert.SerializeObject(pessoa);
            var request = new HttpRequestMessage(method, _baseUrl + path)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            return SendAsync<PessoaApiModel>(request);
        }

        private async Task<T> SendAsync<T>(HttpRequestMessage request)
        {
            using (request)
            using (var response = await EnviarAsync(request).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await CriarErroAsync(response).ConfigureAwait(false);
                }

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                try
                {
                    return JsonConvert.DeserializeObject<T>(content);
                }
                catch (JsonException)
                {
                    throw new PessoasApiException("A API retornou uma resposta inválida.");
                }
            }
        }

        private static async Task<HttpResponseMessage> EnviarAsync(HttpRequestMessage request)
        {
            try
            {
                return await HttpClient.SendAsync(request).ConfigureAwait(false);
            }
            catch (HttpRequestException)
            {
                throw new PessoasApiException("Não foi possível acessar a API.");
            }
            catch (TaskCanceledException)
            {
                throw new PessoasApiException("A API demorou para responder.");
            }
        }

        private static async Task<PessoasApiException> CriarErroAsync(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new PessoasApiException("Pessoa não encontrada.");
            }

            if ((int)response.StatusCode >= 500)
            {
                return new PessoasApiException("A API não conseguiu concluir a operação.");
            }

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var message = ExtrairMensagem(content);

            if (!string.IsNullOrWhiteSpace(message))
            {
                return new PessoasApiException(message);
            }

            return new PessoasApiException("A API não conseguiu concluir a operação.");
        }

        private static string ExtrairMensagem(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }

            try
            {
                var json = JToken.Parse(content);
                if (json.Type == JTokenType.String)
                {
                    return json.Value<string>();
                }

                var message = json["Message"];
                return message == null ? null : message.Value<string>();
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }

    public class PessoasApiException : Exception
    {
        public PessoasApiException(string message)
            : base(message)
        {
        }
    }
}

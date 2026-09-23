using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using CadastroPessoas.Mvc.Models;

namespace CadastroPessoas.Mvc.Services
{
    public class PessoasApiClient
    {
        private static readonly HttpClient HttpClient = CriarHttpClient();
        private readonly string _baseUrl;

        public PessoasApiClient()
        {
            var configuredUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            Uri baseUri;

            if (!Uri.TryCreate(configuredUrl, UriKind.Absolute, out baseUri) ||
                (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ConfigurationErrorsException("A configuração ApiBaseUrl está ausente ou inválida.");
            }

            _baseUrl = baseUri.ToString().TrimEnd('/') + "/";
        }

        public Task<List<PessoaApiModel>> ListarAsync()
        {
            return GetAsync<List<PessoaApiModel>>("api/pessoas");
        }

        public Task<PessoaApiModel> BuscarPorIdAsync(int id)
        {
            return GetAsync<PessoaApiModel>("api/pessoas/" + id);
        }

        public Task<PessoaApiModel> AdicionarAsync(PessoaRequest pessoa)
        {
            return EnviarPessoaAsync(HttpMethod.Post, "api/pessoas", pessoa);
        }

        public Task<PessoaApiModel> AtualizarAsync(int id, PessoaRequest pessoa)
        {
            return EnviarPessoaAsync(HttpMethod.Put, "api/pessoas/" + id, pessoa);
        }

        public async Task ExcluirAsync(int id)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, _baseUrl + "api/pessoas/" + id))
            using (var response = await EnviarAsync(request).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await CriarErroAsync(response).ConfigureAwait(false);
                }
            }
        }

        private Task<T> GetAsync<T>(string path)
        {
            return EnviarAsync<T>(new HttpRequestMessage(HttpMethod.Get, _baseUrl + path));
        }

        private Task<PessoaApiModel> EnviarPessoaAsync(HttpMethod method, string path, PessoaRequest pessoa)
        {
            var json = new JavaScriptSerializer().Serialize(pessoa);
            var request = new HttpRequestMessage(method, _baseUrl + path)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            return EnviarAsync<PessoaApiModel>(request);
        }

        private async Task<T> EnviarAsync<T>(HttpRequestMessage request)
        {
            using (request)
            using (var response = await EnviarAsync(request).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw await CriarErroAsync(response).ConfigureAwait(false);
                }

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(content) ||
                    string.Equals(content.Trim(), "null", StringComparison.OrdinalIgnoreCase))
                {
                    throw new PessoasApiException("A API retornou uma resposta inválida.");
                }

                try
                {
                    return new JavaScriptSerializer().Deserialize<T>(content);
                }
                catch (InvalidOperationException)
                {
                    throw new PessoasApiException("A API retornou uma resposta inválida.");
                }
                catch (ArgumentException)
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
            return string.IsNullOrWhiteSpace(message)
                ? new PessoasApiException("A API não conseguiu concluir a operação.")
                : new PessoasApiException(message);
        }

        private static string ExtrairMensagem(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }

            try
            {
                var serializer = new JavaScriptSerializer();
                var value = serializer.DeserializeObject(content) as Dictionary<string, object>;
                if (value != null && value.ContainsKey("Message"))
                {
                    return Convert.ToString(value["Message"]);
                }

                return serializer.Deserialize<string>(content);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        private static HttpClient CriarHttpClient()
        {
            return new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
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

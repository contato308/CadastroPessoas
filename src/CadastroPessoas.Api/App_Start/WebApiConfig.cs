using System.Web.Http;

namespace CadastroPessoas.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration configuration)
        {
            configuration.MapHttpAttributeRoutes();
        }
    }
}

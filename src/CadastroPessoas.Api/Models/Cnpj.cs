using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace CadastroPessoas.Api.Models
{
    public class Cnpj
    {
        public int Id { get; set; }

        public string Numero { get; set; }

        public int PessoaId { get; set; }

        // Evita referência circular durante a serialização.
        [JsonIgnore]
        [IgnoreDataMember]
        [XmlIgnore]
        public Pessoa Pessoa { get; set; }
    }
}

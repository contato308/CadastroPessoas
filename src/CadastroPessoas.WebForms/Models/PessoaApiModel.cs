using System.Collections.Generic;

namespace CadastroPessoas.WebForms.Models
{
    public class PessoaApiModel
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public int Tipo { get; set; }

        public string Cpf { get; set; }

        public List<CnpjApiModel> Cnpjs { get; set; }
    }
}

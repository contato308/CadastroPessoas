using System.ComponentModel.DataAnnotations;

namespace CadastroPessoas.Mvc.Models
{
    public class PessoaFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Selecione o tipo de pessoa.")]
        public int? Tipo { get; set; }

        public string Cpf { get; set; }

        public string Cnpjs { get; set; }
    }
}

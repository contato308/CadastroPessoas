namespace CadastroPessoas.Api.Models
{
    public class ResultadoCriacaoPessoa
    {
        public bool Sucesso { get; set; }

        public string Mensagem { get; set; }

        public Pessoa Pessoa { get; set; }
    }
}

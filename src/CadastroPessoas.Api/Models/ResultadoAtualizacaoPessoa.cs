namespace CadastroPessoas.Api.Models
{
    public class ResultadoAtualizacaoPessoa
    {
        public bool Sucesso { get; set; }

        public bool PessoaNaoEncontrada { get; set; }

        public string Mensagem { get; set; }

        public Pessoa Pessoa { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using CadastroPessoas.WebForms.Models;
using CadastroPessoas.WebForms.Services;

namespace CadastroPessoas.WebForms
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RegisterAsyncTask(new PageAsyncTask(CarregarPessoasAsync));
            }
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(SalvarPessoaAsync));
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            MessageLabel.Text = string.Empty;
        }

        protected string FormatarTipo(object value)
        {
            var tipo = Convert.ToInt32(value);
            return tipo == 1 ? "Pessoa Física" : tipo == 2 ? "Pessoa Jurídica" : "";
        }

        protected string FormatarCnpjs(object value)
        {
            var cnpjs = value as IEnumerable<CnpjApiModel>;
            return cnpjs == null ? string.Empty : string.Join(", ", cnpjs.Select(cnpj => cnpj.Numero));
        }

        private async Task CarregarPessoasAsync()
        {
            try
            {
                var client = new PessoasApiClient();
                PeopleGridView.DataSource = await client.ListarAsync();

                PeopleGridView.DataBind();
            }
            catch (PessoasApiException ex)
            {
                PeopleGridView.DataSource = null;
                PeopleGridView.DataBind();
                MostrarErro(ex.Message);
            }
        }

        private async Task SalvarPessoaAsync()
        {
            int tipo;
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MostrarErro("Nome é obrigatório.");
                return;
            }

            if (!int.TryParse(TypeDropDownList.SelectedValue, out tipo))
            {
                MostrarErro("Selecione o tipo de pessoa.");
                return;
            }

            var pessoa = new PessoaRequest
            {
                Nome = NameTextBox.Text.Trim(),
                Tipo = tipo,
                Cpf = CpfTextBox.Text.Trim(),
                Cnpjs = CnpjsTextBox.Text
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                    .Select(numero => numero.Trim())
                    .Where(numero => numero.Length > 0)
                    .ToList()
            };

            try
            {
                var client = new PessoasApiClient();
                await client.AdicionarAsync(pessoa);

                LimparFormulario();
                MessageLabel.CssClass = "message success";
                MessageLabel.Text = "Pessoa cadastrada com sucesso.";
                await CarregarPessoasAsync();
            }
            catch (PessoasApiException ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private void LimparFormulario()
        {
            NameTextBox.Text = string.Empty;
            TypeDropDownList.SelectedIndex = 0;
            CpfTextBox.Text = string.Empty;
            CnpjsTextBox.Text = string.Empty;
        }

        private void MostrarErro(string mensagem)
        {
            MessageLabel.CssClass = "message error";
            MessageLabel.Text = mensagem;
        }
    }
}

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

        protected void PeopleGridView_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id))
            {
                return;
            }

            if (e.CommandName == "Editar")
            {
                RegisterAsyncTask(new PageAsyncTask(() => PrepararEdicaoAsync(id)));
            }
            else if (e.CommandName == "Excluir")
            {
                RegisterAsyncTask(new PageAsyncTask(() => ExcluirPessoaAsync(id)));
            }
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
                int id;
                var editando = int.TryParse(PersonIdHiddenField.Value, out id);

                if (editando)
                {
                    await client.AtualizarAsync(id, pessoa);
                }
                else
                {
                    await client.AdicionarAsync(pessoa);
                }

                LimparFormulario();
                MessageLabel.CssClass = "message success";
                MessageLabel.Text = editando ? "Pessoa atualizada com sucesso." : "Pessoa cadastrada com sucesso.";
                await CarregarPessoasAsync();
            }
            catch (PessoasApiException ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private async Task PrepararEdicaoAsync(int id)
        {
            try
            {
                var client = new PessoasApiClient();
                var pessoa = await client.BuscarPorIdAsync(id);

                PersonIdHiddenField.Value = pessoa.Id.ToString();
                NameTextBox.Text = pessoa.Nome;
                TypeDropDownList.SelectedValue = pessoa.Tipo.ToString();
                CpfTextBox.Text = pessoa.Cpf;
                CnpjsTextBox.Text = string.Join(Environment.NewLine, (pessoa.Cnpjs ?? new List<CnpjApiModel>()).Select(cnpj => cnpj.Numero));
                FormTitleLiteral.Text = "Editar pessoa";
                SaveButton.Text = "Salvar alterações";
                CancelButton.Visible = true;
                MessageLabel.Text = string.Empty;
            }
            catch (PessoasApiException ex)
            {
                MostrarErro(ex.Message);
                await CarregarPessoasAsync();
            }
        }

        private async Task ExcluirPessoaAsync(int id)
        {
            try
            {
                var client = new PessoasApiClient();
                await client.ExcluirAsync(id);

                if (PersonIdHiddenField.Value == id.ToString())
                {
                    LimparFormulario();
                }

                MessageLabel.CssClass = "message success";
                MessageLabel.Text = "Pessoa excluída com sucesso.";
                await CarregarPessoasAsync();
            }
            catch (PessoasApiException ex)
            {
                MostrarErro(ex.Message);
                await CarregarPessoasAsync();
            }
        }

        private void LimparFormulario()
        {
            PersonIdHiddenField.Value = string.Empty;
            NameTextBox.Text = string.Empty;
            TypeDropDownList.SelectedIndex = 0;
            CpfTextBox.Text = string.Empty;
            CnpjsTextBox.Text = string.Empty;
            FormTitleLiteral.Text = "Nova pessoa";
            SaveButton.Text = "Salvar";
            CancelButton.Visible = false;
        }

        private void MostrarErro(string mensagem)
        {
            MessageLabel.CssClass = "message error";
            MessageLabel.Text = Server.HtmlEncode(mensagem);
        }
    }
}

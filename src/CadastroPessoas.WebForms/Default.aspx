<%@ Page Language="C#" AutoEventWireup="true" Async="true" CodeBehind="Default.aspx.cs" Inherits="CadastroPessoas.WebForms.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Cadastro de Pessoas</title>
    <link href="Content/Site.css" rel="stylesheet" />
</head>
<body>
    <form id="mainForm" runat="server">
        <main class="page">
            <h1>Cadastro de Pessoas</h1>

            <section class="panel" aria-labelledby="formTitle">
                <h2 id="formTitle"><asp:Literal ID="FormTitleLiteral" runat="server" Text="Nova pessoa" /></h2>
                <asp:Label ID="MessageLabel" runat="server" CssClass="message" EnableViewState="false" />
                <asp:HiddenField ID="PersonIdHiddenField" runat="server" />

                <div class="form-grid">
                    <div class="field field-wide">
                        <asp:Label ID="NameLabel" runat="server" AssociatedControlID="NameTextBox" Text="Nome" />
                        <asp:TextBox ID="NameTextBox" runat="server" MaxLength="200" />
                    </div>
                    <div class="field">
                        <asp:Label ID="TypeLabel" runat="server" AssociatedControlID="TypeDropDownList" Text="Tipo de pessoa" />
                        <asp:DropDownList ID="TypeDropDownList" runat="server">
                            <asp:ListItem Text="Selecione..." Value="" Selected="True" />
                            <asp:ListItem Text="Pessoa Física" Value="1" />
                            <asp:ListItem Text="Pessoa Jurídica" Value="2" />
                        </asp:DropDownList>
                    </div>
                    <div class="field">
                        <asp:Label ID="CpfLabel" runat="server" AssociatedControlID="CpfTextBox" Text="CPF" />
                        <asp:TextBox ID="CpfTextBox" runat="server" MaxLength="14" />
                    </div>
                    <div class="field field-wide">
                        <asp:Label ID="CnpjsLabel" runat="server" AssociatedControlID="CnpjsTextBox" Text="CNPJs" />
                        <asp:TextBox ID="CnpjsTextBox" runat="server" TextMode="MultiLine" Rows="4" />
                        <span class="hint">Informe um CNPJ por linha. O campo pode ficar vazio.</span>
                    </div>
                </div>

                <div class="actions">
                    <asp:Button ID="SaveButton" runat="server" Text="Salvar" CssClass="button button-primary" OnClick="SaveButton_Click" />
                    <asp:Button ID="CancelButton" runat="server" Text="Cancelar" CssClass="button" CausesValidation="false" Visible="false" OnClick="CancelButton_Click" />
                </div>
            </section>

            <section class="panel" aria-labelledby="peopleTitle">
                <h2 id="peopleTitle">Pessoas cadastradas</h2>
                <div class="table-scroll">
                    <asp:GridView ID="PeopleGridView" runat="server" AutoGenerateColumns="false" CssClass="people-table" GridLines="None" EmptyDataText="Nenhuma pessoa cadastrada." OnRowCommand="PeopleGridView_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="Nome" HeaderText="Nome" />
                            <asp:TemplateField HeaderText="Tipo">
                                <ItemTemplate><%# FormatarTipo(Eval("Tipo")) %></ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Cpf" HeaderText="CPF" />
                            <asp:TemplateField HeaderText="CNPJs">
                                <ItemTemplate><%#: FormatarCnpjs(Eval("Cnpjs")) %></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Ações">
                                <ItemTemplate>
                                    <asp:LinkButton ID="EditLinkButton" runat="server" Text="Editar" CssClass="row-action" CommandName="Editar" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" />
                                    <asp:LinkButton ID="DeleteLinkButton" runat="server" Text="Excluir" CssClass="row-action row-action-delete" CommandName="Excluir" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" OnClientClick="return confirm('Excluir esta pessoa?');" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </section>
        </main>
    </form>
</body>
</html>

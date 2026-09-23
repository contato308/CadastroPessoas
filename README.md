# Cadastro de Pessoas

Aplicação em .NET Framework 4.8 para cadastro de pessoas físicas e jurídicas. A solução reúne uma Web API, consumida pelas interfaces Web Forms e ASP.NET MVC. Uma pessoa física pode ter nenhum ou vários CNPJs.

## Tecnologias

- Visual Studio 2022
- .NET Framework 4.8 e C#
- ASP.NET Web API 2
- ASP.NET Web Forms
- ASP.NET MVC 5
- Entity Framework 6
- SQL Server LocalDB
- HTTP e JSON

## Estrutura da solução

A solução `CadastroPessoas.sln` contém três projetos:

- `CadastroPessoas.Api`: disponibiliza os endpoints HTTP, concentra as regras da aplicação e realiza a persistência dos dados.
- `CadastroPessoas.WebForms`: interface Web Forms que consome a Web API via HTTP; não acessa o banco diretamente.
- `CadastroPessoas.Mvc`: interface ASP.NET MVC que consome a mesma Web API via HTTP; não acessa o banco diretamente. Seu layout se adapta a desktop, tablets e dispositivos móveis.

## Arquitetura

As interfaces enviam e recebem dados em JSON pela Web API. A API usa Entity Framework 6 para acessar o SQL Server LocalDB.

```text
Web Forms ----\
               +-- HTTP/JSON --> Web API --> Entity Framework --> SQL Server LocalDB
ASP.NET MVC --/
```

## Requisitos

- Windows
- Visual Studio 2022
- .NET Framework 4.8
- SQL Server LocalDB disponível no ambiente

## Como executar

1. Clone o repositório.
2. Abra `CadastroPessoas.sln` no Visual Studio 2022.
3. Restaure os pacotes NuGet, se necessário, e compile a solução.
4. Inicie a Web API antes de executar uma das interfaces Web Forms ou MVC.

Para executar uma interface, selecione o projeto correspondente no Visual Studio e inicie-o com a Web API em execução.

## Configuração da Web API

As duas interfaces usam atualmente `http://localhost:52341/` como endereço da API. A porta pode variar conforme a configuração local do IIS Express ou do Visual Studio. Se o endereço mudar, atualize a chave `ApiBaseUrl` nestes arquivos:

- `src/CadastroPessoas.WebForms/Web.config`
- `src/CadastroPessoas.Mvc/Web.config`

Exemplo:

```xml
<add key="ApiBaseUrl" value="http://localhost:52341/" />
```

## Banco de dados

A API usa o banco `CadastroPessoasDb` no SQL Server LocalDB, com persistência por Entity Framework 6. A string de conexão está em `src/CadastroPessoas.Api/Web.config`, e as migrações estão no projeto da API.

Para criar ou atualizar o banco local, defina `CadastroPessoas.Api` como projeto de inicialização e como projeto padrão no Package Manager Console do Visual Studio e execute:

```powershell
Update-Database
```

O modelo contém `Pessoa` (Id, Nome, Tipo e CPF) e `Cnpj` (Id, Número e PessoaId), relacionados como uma pessoa para zero ou vários CNPJs.

## Operações disponíveis

| Método | Endpoint | Operação |
| --- | --- | --- |
| GET | `/api/pessoas` | Lista pessoas |
| GET | `/api/pessoas/{id}` | Consulta uma pessoa |
| POST | `/api/pessoas` | Cadastra uma pessoa |
| PUT | `/api/pessoas/{id}` | Atualiza uma pessoa |
| DELETE | `/api/pessoas/{id}` | Exclui uma pessoa |

## Observações

As interfaces Web Forms e MVC oferecem cadastro, listagem, edição e exclusão. No MVC, em telas menores, o formulário passa para uma coluna e a listagem é adaptada para facilitar a leitura.

using System.Collections.Generic;
using CadastroPessoas.Api.Dtos;
using CadastroPessoas.Api.Models;

namespace CadastroPessoas.Api.Services
{
    public class PessoaService
    {
        private readonly List<Pessoa> _pessoas = new List<Pessoa>();
        private readonly object _sincronizacao = new object();
        private int _proximoPessoaId = 1;
        private int _proximoCnpjId = 1;

        public IReadOnlyCollection<Pessoa> Listar()
        {
            lock (_sincronizacao)
            {
                return _pessoas.ToArray();
            }
        }

        public Pessoa BuscarPorId(int id)
        {
            lock (_sincronizacao)
            {
                return BuscarPorIdSemBloqueio(id);
            }
        }

        public ResultadoCriacaoPessoa Adicionar(CriarPessoaDto dto)
        {
            if (dto == null)
            {
                return CriarErro("Dados inválidos.");
            }

            var mensagemErro = ValidarDadosPessoa(dto.Nome, dto.Tipo, dto.Cpf, dto.Cnpjs);
            if (mensagemErro != null)
            {
                return CriarErro(mensagemErro);
            }

            lock (_sincronizacao)
            {
                var pessoa = new Pessoa
                {
                    Id = _proximoPessoaId++,
                    Nome = dto.Nome,
                    Tipo = dto.Tipo,
                    Cpf = dto.Cpf
                };

                AdicionarCnpjs(pessoa, dto.Cnpjs);
                _pessoas.Add(pessoa);

                return new ResultadoCriacaoPessoa
                {
                    Sucesso = true,
                    Pessoa = pessoa
                };
            }
        }

        public ResultadoAtualizacaoPessoa Atualizar(int id, AtualizarPessoaDto dto)
        {
            if (dto == null)
            {
                return CriarErroAtualizacao("Dados inválidos.");
            }

            var mensagemErro = ValidarDadosPessoa(dto.Nome, dto.Tipo, dto.Cpf, dto.Cnpjs);
            if (mensagemErro != null)
            {
                return CriarErroAtualizacao(mensagemErro);
            }

            lock (_sincronizacao)
            {
                var pessoa = BuscarPorIdSemBloqueio(id);
                if (pessoa == null)
                {
                    return new ResultadoAtualizacaoPessoa
                    {
                        Sucesso = false,
                        PessoaNaoEncontrada = true,
                        Mensagem = "Pessoa não encontrada.",
                        Pessoa = null
                    };
                }

                pessoa.Nome = dto.Nome;
                pessoa.Tipo = dto.Tipo;
                pessoa.Cpf = dto.Cpf;
                pessoa.Cnpjs = new List<Cnpj>();
                AdicionarCnpjs(pessoa, dto.Cnpjs);

                return new ResultadoAtualizacaoPessoa
                {
                    Sucesso = true,
                    PessoaNaoEncontrada = false,
                    Pessoa = pessoa
                };
            }
        }

        public bool Excluir(int id)
        {
            lock (_sincronizacao)
            {
                var pessoa = BuscarPorIdSemBloqueio(id);
                if (pessoa == null)
                {
                    return false;
                }

                return _pessoas.Remove(pessoa);
            }
        }

        private static string ValidarDadosPessoa(
            string nome,
            TipoPessoa tipo,
            string cpf,
            ICollection<string> cnpjs)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return "Nome é obrigatório.";
            }

            if (tipo != TipoPessoa.Fisica && tipo != TipoPessoa.Juridica)
            {
                return "Tipo de pessoa inválido.";
            }

            if (tipo == TipoPessoa.Fisica && string.IsNullOrWhiteSpace(cpf))
            {
                return "CPF é obrigatório para pessoa física.";
            }

            if (cnpjs != null)
            {
                foreach (var numeroCnpj in cnpjs)
                {
                    if (string.IsNullOrWhiteSpace(numeroCnpj))
                    {
                        return "Número do CNPJ é obrigatório.";
                    }
                }
            }

            return null;
        }

        private void AdicionarCnpjs(Pessoa pessoa, ICollection<string> numerosCnpj)
        {
            if (numerosCnpj == null)
            {
                return;
            }

            foreach (var numeroCnpj in numerosCnpj)
            {
                var cnpj = new Cnpj
                {
                    Id = _proximoCnpjId++,
                    Numero = numeroCnpj,
                    PessoaId = pessoa.Id,
                    Pessoa = pessoa
                };

                pessoa.Cnpjs.Add(cnpj);
            }
        }

        private Pessoa BuscarPorIdSemBloqueio(int id)
        {
            return _pessoas.Find(pessoa => pessoa.Id == id);
        }

        private static ResultadoCriacaoPessoa CriarErro(string mensagem)
        {
            return new ResultadoCriacaoPessoa
            {
                Sucesso = false,
                Mensagem = mensagem,
                Pessoa = null
            };
        }

        private static ResultadoAtualizacaoPessoa CriarErroAtualizacao(string mensagem)
        {
            return new ResultadoAtualizacaoPessoa
            {
                Sucesso = false,
                PessoaNaoEncontrada = false,
                Mensagem = mensagem,
                Pessoa = null
            };
        }
    }
}

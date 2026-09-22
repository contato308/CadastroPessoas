using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CadastroPessoas.Api.Data;
using CadastroPessoas.Api.Dtos;
using CadastroPessoas.Api.Models;

namespace CadastroPessoas.Api.Services
{
    public class PessoaService : IDisposable
    {
        private readonly CadastroPessoasContext _context;

        public PessoaService()
            : this(new CadastroPessoasContext())
        {
        }

        public PessoaService(CadastroPessoasContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            _context = context;
        }

        public IReadOnlyCollection<Pessoa> Listar()
        {
            return _context.Pessoas
                .Include(pessoa => pessoa.Cnpjs)
                .ToList();
        }

        public Pessoa BuscarPorId(int id)
        {
            return BuscarComCnpjs(id);
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

            var pessoa = new Pessoa
            {
                Nome = dto.Nome,
                Tipo = dto.Tipo,
                Cpf = dto.Cpf
            };

            AdicionarCnpjs(pessoa, dto.Cnpjs);
            _context.Pessoas.Add(pessoa);
            _context.SaveChanges();

            return new ResultadoCriacaoPessoa
            {
                Sucesso = true,
                Pessoa = pessoa
            };
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

            var pessoa = BuscarComCnpjs(id);
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
            _context.Cnpjs.RemoveRange(pessoa.Cnpjs.ToList());
            pessoa.Cnpjs.Clear();
            AdicionarCnpjs(pessoa, dto.Cnpjs);
            _context.SaveChanges();

            return new ResultadoAtualizacaoPessoa
            {
                Sucesso = true,
                PessoaNaoEncontrada = false,
                Pessoa = pessoa
            };
        }

        public bool Excluir(int id)
        {
            var pessoa = BuscarPorId(id);
            if (pessoa == null)
            {
                return false;
            }

            _context.Pessoas.Remove(pessoa);
            _context.SaveChanges();
            return true;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private Pessoa BuscarComCnpjs(int id)
        {
            return _context.Pessoas
                .Include(pessoa => pessoa.Cnpjs)
                .SingleOrDefault(pessoa => pessoa.Id == id);
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

        private static void AdicionarCnpjs(Pessoa pessoa, ICollection<string> numerosCnpj)
        {
            if (numerosCnpj == null)
            {
                return;
            }

            foreach (var numeroCnpj in numerosCnpj)
            {
                pessoa.Cnpjs.Add(new Cnpj
                {
                    Numero = numeroCnpj
                });
            }
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

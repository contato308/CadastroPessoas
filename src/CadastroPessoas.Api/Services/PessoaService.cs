using System.Collections.Generic;
using CadastroPessoas.Api.Dtos;
using CadastroPessoas.Api.Models;

namespace CadastroPessoas.Api.Services
{
    public class PessoaService
    {
        private readonly List<Pessoa> _pessoas = new List<Pessoa>();
        private int _proximoPessoaId = 1;
        private int _proximoCnpjId = 1;

        public ResultadoCriacaoPessoa Adicionar(CriarPessoaDto dto)
        {
            if (dto == null)
            {
                return CriarErro("Dados inválidos.");
            }

            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                return CriarErro("Nome é obrigatório.");
            }

            if (dto.Tipo != TipoPessoa.Fisica && dto.Tipo != TipoPessoa.Juridica)
            {
                return CriarErro("Tipo de pessoa inválido.");
            }

            if (dto.Tipo == TipoPessoa.Fisica && string.IsNullOrWhiteSpace(dto.Cpf))
            {
                return CriarErro("CPF é obrigatório para pessoa física.");
            }

            if (dto.Cnpjs != null)
            {
                foreach (var numeroCnpj in dto.Cnpjs)
                {
                    if (string.IsNullOrWhiteSpace(numeroCnpj))
                    {
                        return CriarErro("Número do CNPJ é obrigatório.");
                    }
                }
            }

            var pessoa = new Pessoa
            {
                Id = _proximoPessoaId++,
                Nome = dto.Nome,
                Tipo = dto.Tipo,
                Cpf = dto.Cpf
            };

            if (dto.Cnpjs != null)
            {
                foreach (var numeroCnpj in dto.Cnpjs)
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

            _pessoas.Add(pessoa);

            return new ResultadoCriacaoPessoa
            {
                Sucesso = true,
                Pessoa = pessoa
            };
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
    }
}

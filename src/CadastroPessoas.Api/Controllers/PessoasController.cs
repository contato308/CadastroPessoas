using System;
using System.Net;
using System.Web.Http;
using CadastroPessoas.Api.Dtos;
using CadastroPessoas.Api.Services;

namespace CadastroPessoas.Api.Controllers
{
    public class PessoasController : ApiController
    {
        private readonly PessoaService _service;

        public PessoasController()
            : this(new PessoaService())
        {
        }

        public PessoasController(PessoaService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            _service = service;
        }

        public IHttpActionResult Get()
        {
            return Ok(_service.Listar());
        }

        public IHttpActionResult Get(int id)
        {
            var pessoa = _service.BuscarPorId(id);
            if (pessoa == null)
            {
                return NotFound();
            }

            return Ok(pessoa);
        }

        public IHttpActionResult Post(CriarPessoaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = _service.Adicionar(dto);
            if (!resultado.Sucesso)
            {
                return BadRequest(resultado.Mensagem);
            }

            return CreatedAtRoute(
                "DefaultApi",
                new { controller = "pessoas", id = resultado.Pessoa.Id },
                resultado.Pessoa);
        }

        public IHttpActionResult Put(int id, AtualizarPessoaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = _service.Atualizar(id, dto);
            if (resultado.PessoaNaoEncontrada)
            {
                return NotFound();
            }

            if (!resultado.Sucesso)
            {
                return BadRequest(resultado.Mensagem);
            }

            return Ok(resultado.Pessoa);
        }

        public IHttpActionResult Delete(int id)
        {
            if (!_service.Excluir(id))
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _service.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}

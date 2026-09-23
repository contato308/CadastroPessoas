using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CadastroPessoas.Mvc.Models;
using CadastroPessoas.Mvc.Services;

namespace CadastroPessoas.Mvc.Controllers
{
    public class PessoasController : Controller
    {
        private readonly PessoasApiClient _apiClient;

        public PessoasController()
            : this(new PessoasApiClient())
        {
        }

        public PessoasController(PessoasApiClient apiClient)
        {
            if (apiClient == null)
            {
                throw new ArgumentNullException("apiClient");
            }

            _apiClient = apiClient;
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                var pessoas = await _apiClient.ListarAsync();
                return View(pessoas ?? new List<PessoaApiModel>());
            }
            catch (PessoasApiException ex)
            {
                ViewData["ApiError"] = ex.Message;
                return View(new List<PessoaApiModel>());
            }
        }

        public ActionResult Criar()
        {
            return View(new PessoaFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Criar(PessoaFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _apiClient.AdicionarAsync(CriarRequisicao(model));
                TempData["Mensagem"] = "Pessoa cadastrada com sucesso.";
                return RedirectToAction("Index");
            }
            catch (PessoasApiException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public async Task<ActionResult> Editar(int id)
        {
            try
            {
                var pessoa = await _apiClient.BuscarPorIdAsync(id);
                return View(CriarViewModel(pessoa));
            }
            catch (PessoasApiException ex)
            {
                TempData["Erro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(int id, PessoaFormViewModel model)
        {
            if (id != model.Id)
            {
                return new HttpStatusCodeResult(400);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _apiClient.AtualizarAsync(id, CriarRequisicao(model));
                TempData["Mensagem"] = "Pessoa atualizada com sucesso.";
                return RedirectToAction("Index");
            }
            catch (PessoasApiException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Excluir(int id)
        {
            try
            {
                await _apiClient.ExcluirAsync(id);
                TempData["Mensagem"] = "Pessoa excluída com sucesso.";
            }
            catch (PessoasApiException ex)
            {
                TempData["Erro"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        private static PessoaRequest CriarRequisicao(PessoaFormViewModel model)
        {
            return new PessoaRequest
            {
                Nome = model.Nome == null ? null : model.Nome.Trim(),
                Tipo = model.Tipo.GetValueOrDefault(),
                Cpf = model.Cpf == null ? null : model.Cpf.Trim(),
                Cnpjs = (model.Cnpjs ?? string.Empty)
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                    .Select(numero => numero.Trim())
                    .Where(numero => numero.Length > 0)
                    .ToList()
            };
        }

        private static PessoaFormViewModel CriarViewModel(PessoaApiModel pessoa)
        {
            return new PessoaFormViewModel
            {
                Id = pessoa.Id,
                Nome = pessoa.Nome,
                Tipo = pessoa.Tipo,
                Cpf = pessoa.Cpf,
                Cnpjs = string.Join(Environment.NewLine, (pessoa.Cnpjs ?? new List<CnpjApiModel>())
                    .Select(cnpj => cnpj.Numero))
            };
        }
    }
}

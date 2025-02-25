using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Data;
using MinhasFinancas.Enums;
using MinhasFinancas.Models;

namespace MinhasFinancas.Controllers
{
    public class TransacaoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransacaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transacao
        public IActionResult Index(string mes, string descricao)
        {
            var transacoes = _context.TransacaoModel.Include(t => t.ContaBancaria).AsQueryable();

            // Filtro por mês
            if (!string.IsNullOrEmpty(mes))
            {
                DateTime primeiroDiaMes;
                if (DateTime.TryParse(mes + "-01", out primeiroDiaMes))
                {
                    DateTime ultimoDiaMes = primeiroDiaMes.AddMonths(1).AddDays(-1);
                    transacoes = transacoes.Where(t => t.DataEfetivacao >= primeiroDiaMes && t.DataEfetivacao <= ultimoDiaMes);
                }
            }

            // Filtro por descrição
            if (!string.IsNullOrEmpty(descricao))
            {
                transacoes = transacoes.Where(t => t.Descricao.Contains(descricao));
            }

            // Passa os filtros selecionados para a View
            ViewData["MesSelecionado"] = mes;
            ViewData["DescricaoSelecionada"] = descricao;

            return View(transacoes.ToList());
        }


        // GET: Transacao/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var transacaoModel = await _context.TransacaoModel
                .FirstOrDefaultAsync(m => m.Id == id);

            if (transacaoModel == null)
                return NotFound();

            return View(transacaoModel);
        }

        // GET: Transacao/Create
        public IActionResult Create(TransacaoTipo? tipo)
        {
            TransacaoModel? model = PrepararViewCreate(tipo: tipo);
            return View(model);
        }

        // GET: Transacao/Duplicar/{id}
        public IActionResult Duplicar(int id)
        {
            var transacao = _context.TransacaoModel.Find(id);
            if (transacao == null)
            {
                return NotFound();
            }

            var novaTransacao = new TransacaoModel
            {
                Descricao = transacao.Descricao,
                Tipo = transacao.Tipo,
                Status = transacao.Status,
                Valor = transacao.Valor,
                DataEfetivacao = transacao.DataEfetivacao,
                ContaBancariaId = transacao.ContaBancariaId,
                DataCriacao = DateTime.Now,
                DataAlteracao = DateTime.Now,
                DataProximaTransacao = transacao.DataProximaTransacao,
                SaldoAtual = transacao.SaldoAtual
            };

            var model = PrepararViewCreate(transacao: novaTransacao);
            return View("Create", model);
        }


        private TransacaoModel? PrepararViewCreate(TransacaoModel? transacao = null, TransacaoTipo? tipo = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var ultimaTransacao = _context.TransacaoModel
                                          .Join(_context.ContaBancaria,
                                                t => t.ContaBancariaId,
                                                c => c.Id,
                                                (t, c) => new { Transacao = t, ContaBancaria = c })
                                          .Where(x => x.ContaBancaria.UsuarioId == userId)
                                          .OrderByDescending(x => x.Transacao.DataEfetivacao)
                                          .Select(x => x.Transacao)
                                          .FirstOrDefault();

            var contaBancariaId = ultimaTransacao?.ContaBancariaId;

            var contas = _context.ContaBancaria
                                 .Where(c => c.UsuarioId == userId)
                                 .Select(c => new { c.Id, c.Descricao })
                                 .ToList();

            ViewBag.ContaBancaria = new SelectList(contas, "Id", "Descricao", transacao?.ContaBancariaId ?? contaBancariaId);
            ViewBag.Tipo = new SelectList(Enum.GetValues<TransacaoTipo>(), transacao?.Tipo ?? tipo ?? TransacaoTipo.Despesa);
            ViewBag.Status = new SelectList(Enum.GetValues<TransacaoStatus>(), transacao?.Status ?? TransacaoStatus.Pendente);

            return transacao;
        }

        // POST: Transacao/Create
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Descricao,NumeroConta,Tipo,Status,Valor,DataEfetivacao,ContaBancariaId,Id")] TransacaoModel transacaoModel)
        {
            if (ModelState.IsValid)
            {
                //transacaoModel.UpdateBalance(_context);
                _context.Add(transacaoModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.Erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(transacaoModel);
            }
        }

        // GET: Transacao/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transacaoModel = await _context.TransacaoModel.FindAsync(id);
            if (transacaoModel == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var contas = _context.ContaBancaria
                                 .Where(c => c.UsuarioId == userId)
                                 .Select(c => new { c.Id, c.Descricao })
                                 .ToList();

            ViewBag.ContaBancaria = new SelectList(contas, "Id", "Descricao");
            ViewBag.Tipo          = new SelectList(Enum.GetValues<TransacaoTipo>(), TransacaoTipo.Despesa);
            ViewBag.Status        = new SelectList(Enum.GetValues<TransacaoStatus>(), TransacaoStatus.Pendente);

            return View(transacaoModel);
        }

        // POST: Transacao/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Descricao,NumeroConta,Tipo,Status,Valor,DataEfetivacao,ContaBancariaId,Id,DataCriacao,DataAlteracao")] TransacaoModel transacaoModel)
        {
            if (id != transacaoModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transacaoModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransacaoModelExists(transacaoModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(transacaoModel);
        }

        // GET: Transacao/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transacaoModel = await _context.TransacaoModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transacaoModel == null)
            {
                return NotFound();
            }

            return View(transacaoModel);
        }

        // POST: Transacao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transacaoModel = await _context.TransacaoModel.FindAsync(id);
            if (transacaoModel != null)
            {
                _context.TransacaoModel.Remove(transacaoModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransacaoModelExists(int id)
        {
            return _context.TransacaoModel.Any(e => e.Id == id);
        }
    }
}

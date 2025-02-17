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
        public async Task<IActionResult> Index()
        {
            var transacoes = await _context.TransacaoModel
                                    .Include(t => t.ContaBancaria) 
                                    .ToListAsync();
            return View(transacoes);
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
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var contas = _context.ContaBancaria
                                 .Where(c => c.UsuarioId == userId)
                                 .Select(c => new { c.Id, c.Descricao })
                                 .ToList();

            ViewBag.ContaBancaria = new SelectList(contas, "Id", "Descricao");
            ViewBag.Tipo = new SelectList(Enum.GetValues<TransacaoTipo>(), TransacaoTipo.Despesa);
            ViewBag.Status = new SelectList(Enum.GetValues<TransacaoStatus>(), TransacaoStatus.Pendente);
            return View();
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

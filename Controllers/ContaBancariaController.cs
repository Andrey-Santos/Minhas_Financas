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
using MinhasFinancas.Models;

namespace MinhasFinancas.Controllers
{
    public class ContaBancariaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContaBancariaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ContaBancaria
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ContaBancaria.Include(c => c.Usuario);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ContaBancaria/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contaBancariaModel = await _context.ContaBancaria
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contaBancariaModel == null)
            {
                return NotFound();
            }

            return View(contaBancariaModel);
        }

        // GET: ContaBancaria/Create
        public IActionResult Create()
        {
            ViewBag.UsuarioId  = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return View();
        }

        // POST: ContaBancaria/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Descricao,NumeroConta,Saldo,SaldoInicial,UsuarioId,Id")] ContaBancariaModel contaBancariaModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contaBancariaModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.UsuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return View(contaBancariaModel);
        }

        // GET: ContaBancaria/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contaBancariaModel = await _context.ContaBancaria.FindAsync(id);
            if (contaBancariaModel == null)
            {
                return NotFound();
            }

            ViewBag.UsuarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return View(contaBancariaModel);
        }

        // POST: ContaBancaria/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Descricao,NumeroConta,SaldoInicial,UsuarioId,Id")] ContaBancariaModel contaBancariaModel)
        {
            if (id != contaBancariaModel.Id)
            {   
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contaBancariaModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContaBancariaModelExists(contaBancariaModel.Id))
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
            ViewBag.UsuarioId  = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return View(contaBancariaModel);
        }

        // GET: ContaBancaria/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contaBancariaModel = await _context.ContaBancaria
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contaBancariaModel == null)
            {
                return NotFound();
            }

            return View(contaBancariaModel);
        }

        // POST: ContaBancaria/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contaBancariaModel = await _context.ContaBancaria.FindAsync(id);
            if (contaBancariaModel != null)
            {
                _context.ContaBancaria.Remove(contaBancariaModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContaBancariaModelExists(int id)
        {
            return _context.ContaBancaria.Any(e => e.Id == id);
        }
    }
}

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MinhasFinancas.Data;
using MinhasFinancas.ViewModels;
using MinhasFinancas.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MinhasFinancas.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? mes)
        {
            var cultura = new CultureInfo("pt-BR");

            // Se o parâmetro não for passado, usa o mês atual
            int mesSelecionado = mes ?? DateTime.Now.Month;

            // Converte o número do mês para nome (ex: "Março")
            string mesAtual = cultura.TextInfo.ToTitleCase(new DateTime(DateTime.Now.Year, mesSelecionado, 1).ToString("MMMM", cultura));

            // Passa os meses anterior e próximo para a navegação
            int mesAnterior = mesSelecionado == 1 ? 12 : mesSelecionado - 1;
            int mesProximo = mesSelecionado == 12 ? 1 : mesSelecionado + 1;
     
            ViewBag.MesAtual = cultura.TextInfo.ToTitleCase(DateTime.Now.ToString("MMMM", cultura));
            string? userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var model = await TransacaoModel.ObterSaldosAsync(_context, userId, mesSelecionado);
            model.UltimasTransacoes = await _context.TransacaoModel
                                                    .Where(t => t.ContaBancaria.UsuarioId == userId)
                                                    .OrderByDescending(t => t.DataEfetivacao)
                                                    .ThenByDescending(t => t.DataCriacao)
                                                    .Take(20)
                                                    .ToListAsync();

            model.MesAtual = mesAtual;
            model.MesSelecionado = mesSelecionado;
            model.MesAnterior = mesAnterior;
            model.MesProximo = mesProximo;

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

using Microsoft.EntityFrameworkCore;
using MinhasFinancas.ViewModels;
using MinhasFinancas.Data;
using MinhasFinancas.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace MinhasFinancas.Models
{
    [Table("MF_TRANSACAO")]
    public class TransacaoModel : BaseModel
    {
        [Display(Name = "Conta Bancária")]
        public required int ContaBancariaId { get; set; }

        public ContaBancariaModel? ContaBancaria { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data de Efetivação")]
        public required DateTime DataEfetivacao { get; set; }

        public required DateTime DataProximaTransacao { get; set; }

        [Display(Name = "Descrição")]
        public required string Descricao { get; set; }

        [Precision(18, 2)]
        [Display(Name = "Saldo Atual")]
        public required decimal SaldoAtual { get; set; } = 0;

        [Display(Name = "Status")]
        public required TransacaoStatus Status { get; set; }

        [Display(Name = "Tipo")]
        public required TransacaoTipo Tipo { get; set; }

        [Precision(18, 2)]
        [Display(Name = "Valor")]

        public required decimal Valor { get; set; }

        public TransacaoModel? GetOldTransaction(ApplicationDbContext context)
        {
            return context.TransacaoModel
                          .Where(t => t.ContaBancariaId == ContaBancariaId && t.DataEfetivacao <= DataEfetivacao &&
                                      context.TransacaoModel.Any(z => z.Id == t.Id &&
                                                                     z.ContaBancariaId == t.ContaBancariaId &&
                                                                     z.DataEfetivacao <= t.DataEfetivacao &&
                                                                     z.DataProximaTransacao > DataEfetivacao))
                          .OrderByDescending(t => t.DataEfetivacao)
                          .FirstOrDefault();
        }

        public Boolean HasFutureTransaction(ApplicationDbContext context)
        {
            return context.TransacaoModel.Any(t => t.ContaBancariaId == ContaBancariaId && t.DataEfetivacao > DataEfetivacao);
        }

        public static async Task<HomeViewModel> ObterSaldosAsync(ApplicationDbContext context, string? userId, int mes)
        {
            DateTime dataBase = new DateTime(DateTime.Now.Year, mes, 1);

            var saldo = await (from t in context.TransacaoModel
                               join c in context.ContaBancaria on t.ContaBancariaId equals c.Id
                               where c.UsuarioId == userId &&
                                     t.DataEfetivacao.Year == dataBase.Year &&
                                     (t.DataEfetivacao.Year == dataBase.AddMonths(-1).Year && t.DataEfetivacao.Month == dataBase.AddMonths(-1).Month ||
                                      t.DataEfetivacao.Year == dataBase.Year && t.DataEfetivacao.Month == dataBase.Month ||
                                      t.DataEfetivacao.Year == dataBase.AddMonths(1).Year && t.DataEfetivacao.Month == dataBase.AddMonths(1).Month)
                               group t by 1 into g
                               select new
                               {
                                   SaldoInicialReceita = g.Where(t => t.DataEfetivacao.Year == dataBase.AddMonths(-1).Year &&
                                                                      t.DataEfetivacao.Month == dataBase.AddMonths(-1).Month &&
                                                                      t.Tipo == TransacaoTipo.Receita)
                                                          .Sum(t => (decimal?)t.Valor) ?? 0,

                                   SaldoInicialDespesa = g.Where(t => t.DataEfetivacao.Year == dataBase.AddMonths(-1).Year &&
                                                                      t.DataEfetivacao.Month == dataBase.AddMonths(-1).Month &&
                                                                      t.Tipo == TransacaoTipo.Despesa)
                                                          .Sum(t => (decimal?)t.Valor) ?? 0,

                                   SaldoAtualReceita = g.Where(t => t.DataEfetivacao.Year == dataBase.Year &&
                                                                    t.DataEfetivacao.Month == dataBase.Month &&
                                                                    t.Tipo == TransacaoTipo.Receita &&
                                                                    t.Status == TransacaoStatus.Efetivado)
                                                         .Sum(t => (decimal?)t.Valor) ?? 0,

                                   SaldoAtualDespesa = g.Where(t => t.DataEfetivacao.Year == dataBase.Year &&
                                                                    t.DataEfetivacao.Month == dataBase.Month &&
                                                                    t.Tipo == TransacaoTipo.Despesa &&
                                                                    t.Status == TransacaoStatus.Efetivado)
                                                         .Sum(t => (decimal?)t.Valor) ?? 0,

                                   SaldoPrevistoReceita = g.Where(t => t.DataEfetivacao.Year == dataBase.AddMonths(1).Year &&
                                                                       t.DataEfetivacao.Month == dataBase.AddMonths(1).Month &&
                                                                       t.Tipo == TransacaoTipo.Receita)
                                                           .Sum(t => (decimal?)t.Valor) ?? 0,

                                   SaldoPrevistoDespesa = g.Where(t => t.DataEfetivacao.Year == dataBase.AddMonths(1).Year &&
                                                                       t.DataEfetivacao.Month == dataBase.AddMonths(1).Month &&
                                                                       t.Tipo == TransacaoTipo.Despesa)
                                                           .Sum(t => (decimal?)t.Valor) ?? 0
                               })
                               .FirstOrDefaultAsync();


            if (saldo == null)
            {
                return new HomeViewModel
                {
                    SaldoInicial = 0,
                    SaldoAtual = 0,
                    SaldoPrevisto = 0
                };
            }

            // Criar a ViewModel com os valores obtidos
            return new HomeViewModel
            {
                SaldoInicial = saldo.SaldoInicialReceita - saldo.SaldoInicialDespesa,
                SaldoAtual = saldo.SaldoAtualReceita - saldo.SaldoAtualDespesa,
                SaldoPrevisto = (saldo.SaldoAtualReceita - saldo.SaldoAtualDespesa) +
                                (saldo.SaldoPrevistoReceita - saldo.SaldoPrevistoDespesa)
            };

        }
    }
}

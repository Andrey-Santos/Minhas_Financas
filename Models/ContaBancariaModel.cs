using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhasFinancas.Models
{
    [Table("MF_CONTABANCARIA")]
    public class ContaBancariaModel : BaseModel
    {
        [Display(Name = "Descrição")]
        public required string Descricao { get; set; }

        [Display(Name = "Número da Conta")]
        public required string NumeroConta { get; set; }

        [Display(Name = "Usuário")]
        public required string UsuarioId { get; set; }

        public required IdentityUser Usuario { get; set; }
    }
}
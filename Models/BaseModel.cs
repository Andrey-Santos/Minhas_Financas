using MinhasFinancas.Data;
using System.ComponentModel.DataAnnotations;

namespace MinhasFinancas.Models
{
    public class BaseModel
    {
        public int Id { get; set; }

        [Display(Name = "Data de criação")]
        public required DateTime DataCriacao { get; set; } = DateTime.Now;

        [Display(Name = "Data de alteração")]
        public required DateTime DataAlteracao { get; set; } = DateTime.Now;

        public void OnBeforeSaving()
        {
            DataAlteracao = DateTime.Now;
        }

    }
}

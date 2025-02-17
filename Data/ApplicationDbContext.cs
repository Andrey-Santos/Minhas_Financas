using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinhasFinancas.Models;

namespace MinhasFinancas.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<MinhasFinancas.Models.ContaBancariaModel> ContaBancaria { get; set; } = default!;
        public DbSet<MinhasFinancas.Models.TransacaoModel> TransacaoModel { get; set; } = default!;

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries())
                if ((entry.Entity is BaseModel entity) && (entry.State == EntityState.Modified))
                    entity.DataAlteracao = DateTime.Now;

            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}

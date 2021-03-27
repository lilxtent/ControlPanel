using ControlPanel.Model;
using Microsoft.EntityFrameworkCore;

namespace ControlPanel.Services
{
    public class ApplicationContext : DbContext
    {
        public DbSet<ClientModel> ClientsModels { get; private set; }
        public DbSet<PaymentModel> Payments { get; private set; }
        public DbSet<VisitModel> Visits { get; private set; }
        public DbSet<TrainerModel> Trainers { get; private set; }

        public ApplicationContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@"Data Source=Database\clients.db");
    }
}
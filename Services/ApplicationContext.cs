using ControlPanel.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;

namespace ControlPanel.Services
{
    public class ApplicationContext : DbContext
    {
        public string PathToDataBase
        {
            get => ConfigurationManager.AppSettings["DBPath"].ToString();

            set
            {
                if (value is null)
                    throw new ArgumentNullException("Value was null");

                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configuration.AppSettings.Settings["DBPath"].Value = value;
                configuration.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
        }

        public DbSet<ClientModel> ClientsModels { get; private set; }
        public DbSet<PaymentModel> Payments { get; private set; }
        public DbSet<VisitModel> Visits { get; private set; }
        public DbSet<TrainerModel> Trainers { get; private set; }

        public ApplicationContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={PathToDataBase}");
    }
}
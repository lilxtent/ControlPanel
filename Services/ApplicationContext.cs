using System;
using System.Collections.Generic;
using System.Text;
using ControlPanel.Model;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace ControlPanel.Services
{
    class ApplicationContext : DbContext
    {

        public DbSet<ClientModel> ClientsModels { get; private set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@"Data Source=..\..\..\Database\clients.db");
    }
}

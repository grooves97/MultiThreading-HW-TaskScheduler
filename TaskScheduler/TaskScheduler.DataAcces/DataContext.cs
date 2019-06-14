using System;
using System.Data.Entity;
using System.Linq;
using TaskScheduler.Models;

namespace TaskScheduler.DataAcces
{
    public class DataContext : DbContext
    {
        public DataContext()
            : base("name=DataContext")
        {
        }
        public DbSet<Execution> Executions { get; set; }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace cuteAudioNet.Postgresql
{
    public class PgContextFactory : IDesignTimeDbContextFactory<PgContext>
    {
        public PgContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PgContext>();
            optionsBuilder.UseNpgsql("Password=1303;Persist Security Info=True;Username=DrCharlatan;Database=cuteAudioNetDb;Host=localhost");
            return new PgContext(optionsBuilder.Options);
        }
    }
}

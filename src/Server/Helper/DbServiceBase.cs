using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace OpenDokoBlazor.Server.Helper
{
    public abstract class DbServiceBase<TDbContext>
        where TDbContext : DbContext
    {
        protected IServiceProvider Services { get; }

        protected DbServiceBase(IServiceProvider services)
        {
            Services = services;
        }

        protected TDbContext CreateDbContext()
        {
            var serviceScope = Services.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<TDbContext>();
            return context;
        }
    }
}

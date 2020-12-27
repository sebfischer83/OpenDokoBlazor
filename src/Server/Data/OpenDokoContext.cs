using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OpenDokoBlazor.Server.Data.Models;

namespace OpenDokoBlazor.Server.Data
{
    public class OpenDokoContext : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<OpenDokoUser>
    {
        private IHttpContextAccessor _httpContextAccessor;

        public OpenDokoContext(DbContextOptions<OpenDokoContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    }
}

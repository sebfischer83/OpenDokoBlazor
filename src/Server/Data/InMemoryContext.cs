using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenDokoBlazor.Shared.ViewModels.Chat;

namespace OpenDokoBlazor.Server.Data
{
    public class InMemoryContext : DbContext
    {
        public DbSet<ChatUser> ChatUsers { get; protected set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; protected set; } = null!;

        public InMemoryContext(DbContextOptions<InMemoryContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var user = modelBuilder.Entity<ChatUser>();
            user.HasIndex(u => u.Name);

            var message = modelBuilder.Entity<ChatMessage>();
            message.HasIndex(m => m.UserId);
            message.HasIndex(m => m.CreatedAt);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("inmemory");
            base.OnConfiguring(optionsBuilder);
        }
    }
}

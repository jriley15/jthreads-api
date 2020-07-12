using System;
using System.Collections.Generic;
using System.Text;
using JThreads.Data.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JThreads.Data
{
    public class JThreadsDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Namespace> Namespaces { get; set; }
        public DbSet<Thread> Threads { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentRating> CommentRatings { get; set; }
        public DbSet<ThreadRating> ThreadRatings { get; set; }
        public DbSet<Guest> Guests { get; set; }

        public JThreadsDbContext(DbContextOptions<JThreadsDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
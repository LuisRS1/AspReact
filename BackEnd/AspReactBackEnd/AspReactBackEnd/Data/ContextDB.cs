﻿namespace AspReactBackEnd.Data
{
    public class ContextDB : DbContext
    {
        public ContextDB(DbContextOptions<ContextDB> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; } = null!;
    }
}

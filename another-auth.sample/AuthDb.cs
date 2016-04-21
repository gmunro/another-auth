using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using another_auth.Interfaces;

namespace another_auth.sample
{
    internal class AuthDb : DbContext,IAuthDb
    {
        public DbSet<SampleUser> Users { get; set; }
        public DbSet<SampleLogin> StandardLogins { get; set; }
        public AuthDb()
        {

        }
        

        public void Add<T>(T entity) where T : class
        {
            Set<T>().Add(entity);
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return Set<T>();
        }

        public void Save()
        {
                SaveChanges();
        }

        public async Task SaveAsync()
        {
            await SaveChangesAsync();
        }


    }
}
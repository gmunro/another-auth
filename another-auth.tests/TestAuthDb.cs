using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace another_auth.tests
{
    internal class TestAuthDb : IAuthDb
    {
        IList<UserAccount> Added = new List<UserAccount>();
        public TestAuthDb()
        {
        }

        public bool SaveCalled { get; internal set; }

        public void Add<T>(UserAccount userAccount)
        {
            Added.Add(userAccount);
        }

        public IQueryable<UserAccount> Query<T>()
        {
            return Added.AsQueryable();
        }

        public void Save()
        {
            SaveCalled = true;
        }

        public async Task SaveAsync()
        {
            SaveCalled = true;
        }


    }
}
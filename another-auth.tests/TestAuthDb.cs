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
        public readonly Dictionary<Type, List<object>> Backing = new Dictionary<Type, List<object>>();
        public TestAuthDb()
        {
        }

        public bool SaveCalled { get; internal set; }

        public void Add<T>(T entity)
        {
            Backing.Add(typeof (T), new List<object> {entity});
            // Reset SaveCalled so that tests are always checking that save was called following the 
            // latest change.
            SaveCalled = false;
        }

        public IQueryable<T> Query<T>()
        {
            return Backing[typeof (T)].Cast<T>().AsQueryable();
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
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
        private readonly Dictionary<Type, List<object>> _backing = new Dictionary<Type, List<object>>();
        public TestAuthDb()
        {
        }

        public bool SaveCalled { get; internal set; }

        public void Add<T>(T entity)
        {
            _backing.Add(typeof (T), new List<object> {entity});
            // Reset SaveCalled so that tests are always checking that save was called following the 
            // latest change.
            SaveCalled = false;
        }

        public IQueryable<T> Query<T>()
        {
            return _backing[typeof (T)].Cast<T>().AsQueryable();
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
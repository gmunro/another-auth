using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using another_auth.Interfaces;

namespace another_auth.tests
{
    internal class TestAuthDb : IAuthDb
    {
        public readonly Dictionary<Type, List<object>> Backing = new Dictionary<Type, List<object>>();
        public TestAuthDb()
        {
        }

        public bool SaveCalled { get; internal set; }

        public void Add<T>(T entity) where T : class
        {
            if (Backing.ContainsKey(typeof (T)))
            {
                Backing[typeof(T)].Add(entity);
            }
            else
            {
                Backing.Add(typeof(T), new List<object> { entity });
            }
            // Reset SaveCalled so that tests are always checking that save was called following the 
            // latest change.
            SaveCalled = false;
        }

        public bool ModelPresent<T>() where T : class
        {
            return Backing.ContainsKey(typeof (T));
        }

        public IQueryable<T> Query<T>() where T : class
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
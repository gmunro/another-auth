using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using another_auth.Interfaces;

namespace another_auth.sample
{
    internal class SampleNonPersistantAuthDb : IAuthDb
    {

        public readonly Dictionary<Type, List<object>> Backing = new Dictionary<Type, List<object>>();
        /// <summary>
        /// Note that this implementation is not really compliant with the interface
        /// because changes are immediately committed to the in-memory backing
        /// </summary>
        public SampleNonPersistantAuthDb()
        {

        }
        

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
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return Backing[typeof (T)].Cast<T>().AsQueryable();
        }

        public void Save()
        {
        }

        public async Task SaveAsync()
        {
        }


    }
}
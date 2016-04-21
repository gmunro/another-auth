using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace another_auth.Interfaces
{
    public interface IAuthDb
    {
        void Save();
        Task SaveAsync();
        void Add<T>(T entity) where T : class; 
        IQueryable<T> Query<T>() where T : class;

        bool ModelPresent<T>() where T : class;
    }
}
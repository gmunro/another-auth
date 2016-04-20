using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace another_auth.Interfaces
{
    public interface IAuthDb
    {
        void Save();
        Task SaveAsync();
        void Add<T>(T entity);
        IQueryable<T> Query<T>();
    }
}
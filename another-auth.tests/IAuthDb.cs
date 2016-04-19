using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace another_auth.tests
{
    internal interface IAuthDb
    {
        void Save();
        Task SaveAsync();
        void Add<T>(UserAccount userAccount);
        IQueryable<UserAccount> Query<T>();
    }
}
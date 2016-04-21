using System.ComponentModel.DataAnnotations;

namespace another_auth
{
    public abstract class Login<TUser> where TUser : User
    {
        public string LoginUsername { get; set; }
        public string Salt { get; set; }
        public string Hash { get; set; }
        // User is logged in by login
        public TUser User { get; set; }
    }
}
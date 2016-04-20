using System.ComponentModel.DataAnnotations;

namespace another_auth.tests
{
    internal class StandardLogin
    {
        [Key]
        public string LoginUsername { get; set; }
        public string Salt { get; set; }
        public string Hash { get; set; }
        // User is logged in by login
        public User User { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace another_auth.tests
{
    internal class StandardLogin
    {
        [Key]
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        // User is logged in by login
        public User User { get; set; }
    }
}
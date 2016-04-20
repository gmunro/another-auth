using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace another_auth
{
    public class User
    {
        [Key]
        public string Id { get; set; }

        public string PrimaryEmailAddress { get; set; }
    }
}

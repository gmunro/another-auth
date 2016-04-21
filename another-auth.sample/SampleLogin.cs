using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace another_auth.sample
{
    class SampleLogin : StandardLogin<SampleUser>
    {
        [Key]
        public int Id { get; set; }

        public SampleUser User { get; set; }

    }
}

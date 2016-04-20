using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace another_auth
{
    public class EmailAddressValidator : IUserNameValidator
    {
        public bool IsValid(string primaryEmail)
        {
            try
            {
                MailAddress m = new MailAddress(primaryEmail);

                if (!string.Equals(m.Address, primaryEmail))
                {
                    // If the library automagically fixed it, consider it invalid
                    return false;
                }
            }
            catch (Exception)
            {
                // else if the library was unable to validate it, consider it invalid
                return false;
            }
            return true;
        }
    }
}

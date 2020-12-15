using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace SimplySocial.Server.Data.Identity
{
    public class User : IdentityUser
    {
        public Boolean HasProfilePic { get; set; }
        public Byte[] ProfilePicture { get; set; }

        #region Personal Data
        [PersonalData]
        public String FirstName { get; set; }

        [PersonalData]
        public String LastName  { get; set; }

        public String FullName  { get { return $"{FirstName} {LastName}"; } }
        #endregion
    }
}

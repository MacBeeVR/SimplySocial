using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplySocial.Server.Data.UserData.Models
{
    public class Post
    {
        public String       CreatorID   { get; set; }
        public String       Content     { get; set; }
        public DateTime     Timestamp   { get; set; }
        public DateTime?    LastEditOn  { get; set; }
    }
}

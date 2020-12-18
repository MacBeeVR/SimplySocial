using System;

namespace SimplySocial.Server.Core.Settings
{
    public class SmtpSettings
    {
        public Int32    Port        { get; set; }
        public String   Server      { get; set; }
        public String   UserName    { get; set; }
        public String   Password    { get; set; }
        public String   SenderName  { get; set; }
        public String   SenderEmail { get; set; }
    }
}

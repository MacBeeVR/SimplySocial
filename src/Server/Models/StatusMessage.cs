using System;

namespace SimplySocial.Server.Models
{
    public class StatusMessage
    {
        public enum StatusType
        {
            Info,
            Error
        }

        public String Title     { get; set; }
        public String Content   { get; set; }

        public StatusType Type  { get; set; }

        public String CssClass
        {
            get
            {
                String cssClass = "";
                switch (Type)
                {
                    case StatusType.Info:
                        cssClass = "e-toast e-toast-info";
                        break;
                    case StatusType.Error:
                        cssClass = "e-toast e-toast-danger";
                        break;
                    default:
                        break;
                }

                return cssClass;
            }
        }
    }
}

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

using SimplySocial.Server.Models;
using SimplySocial.Server.Core.Extensions;

namespace SimplySocial.Server.Pages
{
    public abstract class SSRazorPageBase<TModel> : RazorPage<TModel>
    {
        [RazorInject]
        private ILogger<SSRazorPageBase<TModel>> _logger { get; set; }

        public StatusMessage GetStatusMessage()
        {
            try
            {
                return TempData.Get<StatusMessage>("StatusMessage");
            }
            catch(Exception ex)
            {
                _logger.LogError($"Unable to Read StatusMessage from TempData", ex);
                return null;
            }
        }
    }
}

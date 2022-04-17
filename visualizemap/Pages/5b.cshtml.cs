using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace visualizemap.Pages
{
    public class _5bModel : PageModel
    {
        private readonly ILogger<_5bModel> _logger;

        public _5bModel(ILogger<_5bModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }


}

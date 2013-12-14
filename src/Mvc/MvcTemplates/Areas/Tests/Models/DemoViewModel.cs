#if DEMO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace N2.Templates.Mvc.Areas.Tests.Models
{
    public class DemoViewModel
    {
        [DisplayName("Email"), Required, RegularExpression("[^@]+@[^.]+(\\.[^.]*)+", ErrorMessage = "Come on, that's not even close.")]
        public string Email { get; set; }

        public const string FeedbackInstruction = "How so, and what would you have liked to see?";
    }
}
#endif

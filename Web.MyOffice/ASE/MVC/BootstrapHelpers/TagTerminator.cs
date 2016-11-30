using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASE.MVC.BootstrapHelpers
{
    public class TagTerminator : IDisposable
    {
        readonly HtmlHelper helper;
        readonly string terminator;

        public TagTerminator(HtmlHelper helper, string terminator)
        {
            this.helper = helper;
            this.terminator = terminator;
        }

        public void Dispose()
        {
            helper.ViewContext.Writer.Write(terminator);
        }
    }
}
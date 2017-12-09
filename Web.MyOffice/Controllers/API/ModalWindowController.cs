using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ASE;
using ASE.EF;
using ASE.MVC;
using ASE.Json;
using MyBank.Models;
using MVC = Web.MyOffice.Controllers.MyBank;
using Method = System.Web.Http;


using Web.MyOffice.Data;

namespace Web.MyOffice.Controllers.API
{
    public class ModalWindowController : ControllerApiAdv<DB>
    {
        [Method.HttpGet]
        public HttpResponseMessage GetForm(string controllerName, string viewName)
        {
            var htmlText = string.Empty;
            switch (controllerName)
            {
                case "AccountsController":
                    //htmlText = (new MVC.AccountsController()).RenderPartialView(viewName);
                    break;
                case "UserBudgetsController":
                    htmlText = (new MVC.UserBudgetsController()).RenderPartialView(viewName);
                    break;
                case "CurrenciesController":
                    htmlText = (new MVC.CurrenciesController()).RenderPartialView(viewName);
                    break;
            }
            return ResponseObject2Json(htmlText);
        }
    }
}

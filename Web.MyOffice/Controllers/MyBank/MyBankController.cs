using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.SqlClient;
using System.IO;
using System.Data.Entity;

using ASE.MVC;
using MyBank.Models;
using System.Configuration;
using System.Reflection;
using System.Xml;
using System.Drawing;
using Web.MyOffice.Data;

namespace MyBank.Controllers
{
    [RequireHttps]
    [Authorize]
    public class MyBankController : ControllerAdv<DB>
    {
        public static readonly string LoadingImage = String.Format("<img src='{0}' />", VirtualPathUtility.ToAbsolute("~/Images/loading.gif"));

        private GlobalDAL DAL = GlobalDAL.CreateInstance;

        protected override void Dispose(bool disposing)
        {
            DAL.Dispose();
            base.Dispose(disposing);
        }

        [AllowAnonymous]
        public ActionResult Index(Guid categoryId)
        {
            if (!Request.IsAuthenticated)
                return View("IndexNA");

            if (DAL.Accounts.Count == 0)
                return RedirectToAction("Index", "References");

            var list = db.Accounts
                .Include(x => x.Category)
                .Include(x => x.Motions)
                .Include(x => x.Motions.Select(z => z.Item))
                .Include(x => x.Currency)
                .Where(x => x.CategoryId == categoryId & x.BudgetId == DAL.CurrentOwner.Id)
                .ToList();

            return View(list);
        }


        [Authorize]
        public JsonResult AutocompleteItem(string term)
        {
            List<object> list = new List<object>();

            if ((!String.IsNullOrWhiteSpace(term)) && (!term.StartsWith("+")))
            {
                    foreach (Item item in DAL.FindItemsByName(term))
                        list.Add(new { id = 0, name = item.Name, isAccount = false });
            }
            else if (term.StartsWith("+"))
            {
                foreach (SelectListItem item in DAL.AccountsSelectList)
                    if (term.Length > 1)
                    {
                        if (item.Text.IndexOf(term.Substring(1)) != -1)
                            list.Add(new { id = item.Value.ToString(), name = "+" + item.Text });
                    }
                    else
                        list.Add(new { id = item.Value.ToString(), name = "+" + item.Text });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult AutocompleteItemCategory(string term)
        {
            List<object> list = new List<object>();

            foreach (CategoryItem item in DAL.FindItemCategoryByName(term))
                list.Add(new { value = item.Name, label = item.Name });

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #region Motions 

        [Authorize]
        public JsonResult AddSaveMotion(Guid accountId, Guid? accountId2, Guid motionId, DateTime? dt, string item, decimal sumP, decimal sumM, string note)
        {
            if ((sumP != 0) || (sumM != 0))
                accountId = DAL.AddSaveMotion(accountId, motionId, (dt == null) ? DateTime.MinValue : (DateTime)dt, item, sumP, sumM, note).AccountId;

            if ((accountId2.HasValue) && (accountId2.Value != Guid.Empty))
            {
                item = "+" + R.R.MyAccount + ": " + DAL.AccountGet(accountId).Name;
                accountId2 = DAL.AddSaveMotion(accountId2.Value, motionId, (dt == null) ? DateTime.MinValue : (DateTime)dt, item, sumM, sumP, note).AccountId;
            }

            return new JsonResult
            {
                Data = new
                {
                    accountId = accountId,
                    accountId2 = accountId2,
                    result = true
                }
            };
        }

        [Authorize]
        public JsonResult DeleteMotion(Guid motionId, bool delete)
        {
            Motion motion  = DAL.RemoveDeleteMotion(motionId, delete, false);

            return new JsonResult
            {
                Data = new
                {
                    accountId = motion.AccountId,
                    result = true
                }
            };
        }

        [Authorize]
        public JsonResult MotionInfo(Guid id)
        {
            return new JsonResult
            {
                Data = new
                {
                    html = this.RenderPartialView("MotionInfo", DAL.MotionGet(id))
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        #endregion Motions

        #region Account

        [Authorize]
        public JsonResult GetAccountShortDetail(Guid id, DateTime from, DateTime to)
        {
            Account account = DAL.AccountGet(id);
            ViewData["From"] = from;
            ViewData["To"] = to;

            return new JsonResult
            {
                Data = new
                {
                    header = this.RenderPartialView("AccountShortDetail", account),
                    body = this.RenderPartialView("AccountShortDetailMotions", account)
                }
            };
        }

        #endregion Account

        public void Conver3gpToMp3(string fileName)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
            psi.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/bin/ffmpeg.exe");
            psi.Arguments = String.Format("-i {0}.3gp -vn -acodec libmp3lame -ab 64k {0}.mp3", fileName.Replace(".3gp", ""));
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
            System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
            p.WaitForExit(5000);
            try
            {
                if (!p.HasExited)
                {
                    p.Kill();
                }
            }
            catch
            {
            }
        }

        private void MakeResizedImage(string file)
        {
            try
            {
                using(Image img = Image.FromFile(file))
                {

                    Bitmap result = new Bitmap(640, 480);

                    result.SetResolution(img.HorizontalResolution, img.VerticalResolution);

                    using (Graphics graphics = Graphics.FromImage(result))
                    {
                        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        graphics.DrawImage(img, 0, 0, result.Width, result.Height);
                    }

                    using(Image img2 = Image.FromHbitmap(result.GetHbitmap()))
                        img2.Save(file + "x", System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
            catch/*(Exception exc)*/
            {
            }
        }

        /*[Authorize]
        public JsonResult MotionInfoAdd(string id, string key, string name, string comment, string gps, string lastGps, string lastNet, string audio, string photo, string video, string datetime, string sum)
        {
            Guid gId = Guid.Parse(id);

            string retId = DAL.MotionInfoAdd(id, key, name, comment, gps, lastGps, lastNet, audio, photo, video, datetime, sum);

            string userdir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/App_Data/InData/", GlobalDAL.CreateInstance.CurrentOwner.Id + "/");
            if (!Directory.Exists(userdir))
                Directory.CreateDirectory(userdir);

            foreach (string attach in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[attach];
                if (file.ContentLength > 1024 * 1024 * 5)
                    continue;

                FileStream fs = null;
                if (attach == "voice")
                    fs = new FileStream(userdir + id.ToString() + ".3gp", FileMode.Create);
                else if (attach == "photo")
                    fs = new FileStream(userdir + id.ToString() + ".jpg", FileMode.Create);
                else if (attach == "video")
                    fs = new FileStream(userdir + id.ToString() + ".mp4", FileMode.Create);

                file.InputStream.CopyTo(fs);
                fs.Close();
                if (attach == "voice")
                {
                    if (System.IO.File.Exists(userdir + id.ToString() + ".mp3"))
                        System.IO.File.Delete(userdir + id.ToString() + ".mp3");
                    Conver3gpToMp3(userdir + id.ToString() + ".3gp");
                    System.IO.File.Delete(userdir + id.ToString() + ".3gp");
                }
                if (attach == "photo")
                {
                    MakeResizedImage(userdir + id.ToString() + ".jpg");
                    System.IO.File.Delete(userdir + id.ToString() + ".jpg");
                    System.IO.File.Move(userdir + id.ToString() + ".jpgx", userdir + id.ToString() + ".jpg");
                }
            }

            return new JsonResult
            {
                Data = new
                {
                    result = true,
                    id = retId
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }*/

        [Authorize]
        public JsonResult ImportList(string accountId)
        {
            ViewBag.AccountId = accountId;
            return new JsonResult
            {
                Data = new
                {
                    result = true,
                    html = this.RenderPartialView("ImportList", DAL.MotionInfos),
                    accountId = accountId
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [Authorize]
        [HttpPost]
        public JsonResult DeleteImport(Guid id, string accountId)
        {
            var item = db.BankRecords.Find(id);
            db.BankRecords.Remove(item);
            db.SaveChanges();

            return new JsonResult
            {
                Data = new
                {
                    result = true,
                    html = this.RenderPartialView("ImportList", DAL.MotionInfos),
                    accountId = accountId
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        /*public JsonResult Test()
        {
            return new JsonResult
            {
                Data = new
                {
                    html = this.RenderPartialView("test", null)
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }*/
    }
}

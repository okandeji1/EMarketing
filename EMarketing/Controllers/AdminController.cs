using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EMarketing.Models;
namespace EMarketing.Controllers
{
    public class AdminController : Controller
    {
        emarketingEntities db = new emarketingEntities();
        // GET: Admin
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(admin avm)
        {
            admin ad = db.admins.Where(x => x.ad_username == avm.ad_username && x.ad_password == avm.ad_password).SingleOrDefault();

            if(ad != null)
            {
                Session["ad_id"] = ad.ad_id.ToString();
                return RedirectToAction("Create");
            }
            else
            {
                ViewBag.error = "Invalid username or password";
            }
            return View();
        }

        public ActionResult Create()
        {
            if(Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        //Post method
        [HttpPost]
        public ActionResult Create(tbl_category cvm, HttpPostedFileBase imgfile)
        {
            string path = uploadImgFile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded....";
            }
            else
            {
                tbl_category cat = new tbl_category();
                cat.cat_name = cvm.cat_name;
                cat.cat_image = path;
                cat.cat_status = 1;
                cat.cat_fk_ad = Convert.ToInt32(Session["ad_id"].ToString());
                db.tbl_category.Add(cat);
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            return View();
        } //end,,,,,,,,,,

        public ActionResult ViewCategory()
        {
            return View();
        }

        public string uploadImgFile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if(file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if(extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {
                        path = Path.Combine(Server.MapPath("~/content/upload/"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/content/upload/" + random + Path.GetFileName(file.FileName);
                    }
                    catch (Exception)
                    {

                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpe, jpeg or png formats are acceptable...');</script>");
                }
            }
            else
            {
                Response.Write("<script>alert('Please select a file');</script>");
                path = "-1";
            }
            return path;
        }
    }
}
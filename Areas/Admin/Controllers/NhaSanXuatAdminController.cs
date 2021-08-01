using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Electro.Models;
using PagedList;
namespace Electro.Areas.Admin.Controllers
{
    public class NhaSanXuatAdminController : Controller
    {
        // GET: Admin/NhaSanXuatAdmin
        ElectroDbContext db = new ElectroDbContext();
        public ActionResult Index(int? page)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Admin");

            }
            int iPageNum = (page ?? 1);
            var lst = db.NhaSanXuats.ToList().OrderBy(n => n.MaNSX).ToPagedList(iPageNum, 10);
            return View(lst);
        }
        public ActionResult SanPham(int MaNSX, int ? page, string url)
        {
            ViewBag.Url = url;
            ViewBag.MaNSX = MaNSX;
            int iPageNum = (page ?? 1);
            int iPageSize = 7;
            var lst = db.SanPhams.Where(n => n.MaNSX == MaNSX).ToList().OrderBy(n => n.MaSP).ToPagedList(iPageNum, iPageSize);
            return View(lst);
        }
    }
}
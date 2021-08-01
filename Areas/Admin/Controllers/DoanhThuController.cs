using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Electro.Models;
namespace Electro.Areas.Admin.Controllers
{
    public class DoanhThuController : Controller
    {
        // GET: Admin/DoanhThu
        ElectroDbContext db = new ElectroDbContext();
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Admin");

            }
            DateTime ToDay = DateTime.Now.Date;
            var lstDDH = db.DonDatHangs.Where(n => ( n.NgayLap >= ToDay));
            ViewBag.SoLuong = lstDDH.Count();
            double DoanhThu = 0;
            foreach (var item in lstDDH)
            {
                DoanhThu += (double)item.TongTien;
            }
            ViewBag.TongTien = DoanhThu;
            ViewBag.TuNgay = ToDay;
            ViewBag.DenNgay = ToDay;
            return View(lstDDH); ;
        }
        [HttpPost]
        public ActionResult Index(FormCollection f)
        {
            DateTime TuNgay = Convert.ToDateTime(f["TuNgay"]);
            DateTime DenNgay = Convert.ToDateTime(f["DenNgay"]);
            var lstDDH = db.DonDatHangs.Where(n => (n.NgayLap >= TuNgay && n.NgayLap <= DenNgay)).Select(n => n).OrderBy(n => n.NgayLap);
            ViewBag.SoLuong = lstDDH.Count();
            double DoanhThu = 0;
            foreach (var item in lstDDH)
            {
                DoanhThu += (double)item.TongTien;
            }
            ViewBag.TongTien = DoanhThu;
            ViewBag.TuNgay = TuNgay;
            ViewBag.DenNgay = DenNgay;
            return View(lstDDH); ;
        }
    }
}
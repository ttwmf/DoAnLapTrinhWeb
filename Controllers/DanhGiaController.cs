using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Electro.Models;
using PagedList.Mvc;
using PagedList;
namespace Electro.Controllers
{
    
    public class DanhGiaController : Controller
    {
        ElectroDbContext db = new ElectroDbContext();
        private void UpdateTrungBinhDanhGia(int MaSP)
        {
            SanPham sp = db.SanPhams.Single(n => n.MaSP == MaSP);
            int SoLuongDanhGia = db.BinhLuans.Count(n => n.MaSP == MaSP);
            int TongDanhGia = (int)(from bl in db.BinhLuans where bl.MaSP == MaSP select bl).Sum(n => n.DanhGia);
            if (SoLuongDanhGia != 0)
                sp.TrungBinhDanhGia = (TongDanhGia * 10 / SoLuongDanhGia) / 10;
            else
                sp.TrungBinhDanhGia = 0;
            db.SaveChanges();
        }
        // GET: DanhGia
        public ActionResult Index(int MaSP, double ? GiaSale, int? Page)
        {
            ViewBag.GiaSale = GiaSale;
            ViewBag.MaSP = MaSP;
            ViewBag.Page = Page;
            return View();
        }
        public ActionResult RatingPartial(int MaSP)
        {
            ViewBag.SoLuongDanhGia = db.BinhLuans.Count(n => n.MaSP == MaSP);
            ViewBag.TongDanhGia = (from bl in db.BinhLuans where bl.MaSP == MaSP select bl).Sum(n => n.DanhGia);
            var DanhGia = from bl in db.BinhLuans where bl.MaSP == MaSP
                          group bl by bl.DanhGia into dg
                          select new TakeEverything
                          {
                              SoSao = (int)dg.Key,
                              SoLuongDanhGia = dg.Count()
                          };
            return PartialView(DanhGia);
        }
        public ActionResult ReviewPartial(int MaSP, double? GiaSale, int ? Page)
        {
            ViewBag.GiaSale = GiaSale;
            ViewBag.MaSP = MaSP;
            int iPageNum = (Page ?? 1);
            var ListBinhLuan = (from bl in db.BinhLuans where bl.MaSP == MaSP select bl).OrderByDescending(n => n.MaBinhLuan).ToList();
            return PartialView(ListBinhLuan.ToPagedList(iPageNum, 3));
        }
        [HttpGet]
        public ActionResult ReviewFormPartial(int MaSP, double ? GiaSale)
        {
            ViewBag.MaSP = MaSP;
            ViewBag.GiaSale = GiaSale;
            return PartialView();
        }
        [HttpPost]
        public ActionResult ReviewFormPartial(FormCollection f)
        {
            string Email = f["Email"];
            string HoTen = f["HoTen"];
            string NoiDung = f["NoiDung"];
            int MaSP = Int32.Parse(f["MaSP"]);
            double GiaSale = Double.Parse(f["GiaSale"]);
            int DanhGia = Int32.Parse(f["Rating"]);
            BinhLuan bl = new BinhLuan();
            bl.Email = Email;
            bl.MaSP = MaSP;
            bl.NguoiBinhLuan = HoTen;
            bl.NoiDung = NoiDung;
            bl.DanhGia = DanhGia;
            bl.NgayBinhLuan = DateTime.Now;
            db.BinhLuans.Add(bl);
            db.SaveChanges();
            UpdateTrungBinhDanhGia(MaSP);
            return RedirectToAction("ChiTietSanPham", "Home", new { MaSP = MaSP, GiaSale = GiaSale });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Electro.Models;
using PagedList;
using System.IO;
namespace Electro.Areas.Admin.Controllers
{
    public class SanPhamController : Controller
    {
        // GET: Admin/QuanLy
        ElectroDbContext db = new ElectroDbContext();

        public ActionResult Index(int? page)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Admin");

            }

            int iPageNum = (page ?? 1);
            int iPageSize = 7;
            var lst = db.SanPhams.ToList().OrderBy(n => n.MaSP).Where(n => n.DaXoa == false).ToPagedList(iPageNum, iPageSize);
            return View(lst);

        }
        public ActionResult ChiTiet(int MaSP, string url)
        {
            ViewBag.Url = url;
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);

        }

        [HttpGet]
        public ActionResult Sua(int MaSP, string url)
        {
            ViewBag.Url = url;
            var sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaNCC = new SelectList(db.NhaCungCaps.ToList().OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", sp.MaNCC);
            ViewBag.MaNSX = new SelectList(db.NhaSanXuats.ToList().OrderBy(n => n.TenNSX), "MaNSX", "TenNSX", sp.MaNSX);
            return View(sp);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Sua(int MaSP, FormCollection f, HttpPostedFileBase fFileUpload)
        {
            var sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            // if (ModelState.IsValid)
            {
                if (fFileUpload != null)
                {
                    var sFileName = Path.GetFileName(fFileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/img"), sFileName);
                    if (!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                    }
                    sp.HinhAnh = sFileName;
                }
                sp.TenSP = f["TenSP"];
                sp.MoTa = f["MoTa"].Replace("<p>", "").Replace("</p>", "/n");
                sp.MoTa = f["CauHinh"].Replace("<p>", "").Replace("</p>", "/n");
                sp.NgayCapNhat = DateTime.Now;
                sp.SoLuongTon = int.Parse(f["SoLuongTon"]);
                sp.DonGia = decimal.Parse(f["DonGia"]);
                sp.MaNCC = int.Parse(f["MaNCC"]);
                sp.MaNSX = int.Parse(f["MaNSX"]);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

        }
        public ActionResult Xoa(int MaSP)
        {
            var sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            sp.DaXoa = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
        
}
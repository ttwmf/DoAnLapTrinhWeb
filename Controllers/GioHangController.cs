using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Electro.Models;
using System.Net;
using System.Net.Mail;
namespace Electro.Controllers
{
    public class GioHangController : Controller
    {
        ElectroDbContext db = new ElectroDbContext();
        private int LayMaKM()
        {
            var lstKM = db.KhuyenMais.Where(n => n.NgayBatDau <= DateTime.Now && n.NgayKetThuc >= DateTime.Now).OrderByDescending(n => n.MaKM);
            int MaKM = 0;
            if (lstKM.Count() > 0)
            {
                KhuyenMai km = lstKM.Take(1).Single();
                MaKM = km.MaKM;
            }
            else
            {
                MaKM = 0;
            }
            return MaKM;
        }
        public List<GioHang> LayGiohang()
        {
            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;
            if (listGioHang == null)
            {
                listGioHang = new List<GioHang>();
                Session["GioHang"] = listGioHang;
            }

            return listGioHang;
        }
        // GET: GioHang
        public ActionResult XemGioHang()
        {
            KhachHang kh =  Session["TaiKhoan"] as KhachHang;
            List <GioHang> lstGioHang = new List<GioHang>();
            double TongTien = 0;
            if(kh!= null)
            {
                lstGioHang = (from xx in db.GioHangs where xx.MaKH == kh.MaKH select xx).ToList();
            }
            else
            {
                lstGioHang = Session["GioHang"] as List<GioHang>;
            }
            int MaKM = LayMaKM();

            var listSPKM = from ctkm in db.ChiTietKhuyenMais
                           join km in db.KhuyenMais on ctkm.MaKM equals km.MaKM
                           join sp in db.SanPhams on ctkm.MaSP equals sp.MaSP
                           where km.MaKM == MaKM
                           select new TakeEverything
                           {
                               ID = sp.MaSP,
                               TenSP = sp.TenSP,
                               DonGia = (decimal)sp.DonGia,
                               GiamGia = (int)ctkm.GiamGia,
                               HinhAnh = sp.HinhAnh,
                               TrungBinhDanhGia = (double)sp.TrungBinhDanhGia

                           };
            List<TakeEverything> lstSanPham = new List<TakeEverything>();
            if(lstGioHang.Count() > 0)
            {
                foreach (GioHang gh in lstGioHang)
                {
                    SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == gh.MaSP);
                    TakeEverything tk = new TakeEverything();
                    tk.MaSP = sp.MaSP;
                    int GiamGia = 0;
                    foreach (TakeEverything item in listSPKM)
                    {
                        if (item.ID == sp.MaSP)
                        {
                            GiamGia = item.GiamGia;
                        }
                    }
                    tk.text = "wtf" + sp.MaSP.ToString();
                    tk.DonGia = (decimal)sp.DonGia * (100 - GiamGia) / 100;
                    tk.TenSP = sp.TenSP;
                    tk.SoLuongTrongGioHang = (int)gh.SoLuong;
                    tk.CauHinh = sp.CauHinh;
                    tk.SoLuongTon = (int)sp.SoLuongTon;
                    lstSanPham.Add(tk);
                    TongTien += (double)tk.DonGia * tk.SoLuongTrongGioHang;
                }

            }

            ViewBag.TongTien = TongTien;
            if (lstSanPham.Count() > 0)
                return View(lstSanPham);
            else
                return RedirectToAction("Index", "Home");
        }
        public ActionResult XoaGioHang(int MaSP)
        {
            KhachHang kh = Session["TaiKhoan"] as KhachHang;
            List<GioHang> lstGioHang = new List<GioHang>();
            if (kh != null)
            {
                GioHang gh = db.GioHangs.SingleOrDefault(n => n.MaKH == kh.MaKH && n.MaSP == MaSP);
                db.GioHangs.Remove(gh);
                db.SaveChanges();
            }
            else
            {
                lstGioHang = Session["GioHang"] as List<GioHang>;
                List<GioHang> res = new List<GioHang>();
                foreach(GioHang gh in lstGioHang)
                {
                    if(gh.MaSP != MaSP)
                    {
                        res.Add(gh);
                    }
                }
                Session["GioHang"] = res;


            }
            return RedirectToAction("XemGioHang");

        }
        public ActionResult GioHangPartial()
        {
            KhachHang kh = Session["TaiKhoan"] as KhachHang;
            if(kh != null)
            {
                ViewBag.xxx = (from gh in db.GioHangs where gh.MaKH == 1 select gh).Count();
            }
            else
            {
                List<GioHang> lstGioHang = LayGiohang();
                ViewBag.xxx = lstGioHang.Count();
            }
                
            return PartialView();
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            int MaKM = LayMaKM();
            KhachHang kh = Session["TaiKhoan"] as KhachHang;
            List<GioHang> lstGioHang = new List<GioHang>();
            double TongTien = 0;
            if (kh != null)
            {
                lstGioHang = (from xx in db.GioHangs where xx.MaKH == kh.MaKH select xx).ToList();
                ViewBag.HoTen = kh.HoTen;
                ViewBag.SDT = kh.DienThoai;
                ViewBag.DiaChi = kh.DiaChi;
                ViewBag.Email = kh.Email;
            }
            else
            {
                lstGioHang = Session["GioHang"] as List<GioHang>;
            }
            var listSPKM = from ctkm in db.ChiTietKhuyenMais
                           join km in db.KhuyenMais on ctkm.MaKM equals km.MaKM
                           join sp in db.SanPhams on ctkm.MaSP equals sp.MaSP
                           where km.MaKM == MaKM
                           select new TakeEverything
                           {
                               ID = sp.MaSP,
                               TenSP = sp.TenSP,
                               DonGia = (decimal)sp.DonGia,
                               GiamGia = (int)ctkm.GiamGia,
                               HinhAnh = sp.HinhAnh,
                               TrungBinhDanhGia = (double)sp.TrungBinhDanhGia
                           };
            List<TakeEverything> lstSanPham = new List<TakeEverything>();
            foreach (GioHang gh in lstGioHang)
            {
                SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == gh.MaSP);
                TakeEverything tk = new TakeEverything();
                tk.MaSP = sp.MaSP;
                int GiamGia = 0;
                foreach (TakeEverything item in listSPKM)
                {
                    if (item.ID == sp.MaSP)
                    {
                        GiamGia = item.GiamGia;
                    }
                }
                tk.DonGia = (decimal)sp.DonGia * (100 - GiamGia) / 100;
                tk.TenSP = sp.TenSP;
                tk.SoLuongTrongGioHang = (int)gh.SoLuong;
                tk.CauHinh = sp.CauHinh;
                lstSanPham.Add(tk);
                TongTien += (double)tk.DonGia * tk.SoLuongTrongGioHang;
            }
            ViewBag.TongTien = TongTien;
            return View(lstSanPham);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection f, decimal TongSoTien)
        {
            KhachHang kh = Session["TaiKhoan"] as KhachHang;
            DonDatHang ddh = new DonDatHang();
            ddh.Hoten = f["HoTen"];
            ddh.DiaChiGiaoHang = f["DiaChiNhanHang"];
            if (kh != null)
            {
                ddh.MaKH = kh.MaKH;
                if(ddh.DiaChiGiaoHang == "")
                    ddh.DiaChiGiaoHang = f["DiaChi"];
                
            }
            else
            {
                ddh.DiaChiGiaoHang = f["DiaChi"];
            }
            ddh.DienThoai = f["DienThoai"];
            ddh.Email = f["Email"];
            ddh.NgayLap = DateTime.Now;
            var pttt = int.Parse(f["PhuongThucThanhToan"]);
            ddh.PhuongThucThanhToan = "Thanh toán khi nhận hàng";
            if(pttt == 1)
            {
                ddh.PhuongThucThanhToan = "Đã chuyển khoản";
            }
            ddh.TongTien = TongSoTien;
            db.DonDatHangs.Add(ddh);
            db.SaveChanges();
            // Xử lý chi tiết phiếu đặt hàng
            DonDatHang dhh = (from dh in db.DonDatHangs select dh).OrderByDescending(n => n.MaDDH).First();
            int MaDDH = ddh.MaDDH;
            List<GioHang> lstGioHang = new List<GioHang>();
            double TongTien = 0;
            if (kh != null)
            {
                lstGioHang = (from xx in db.GioHangs where xx.MaKH == kh.MaKH select xx).ToList();
                ViewBag.HoTen = kh.HoTen;
                ViewBag.SDT = kh.DienThoai;
                ViewBag.DiaChi = kh.DiaChi;
                ViewBag.Email = kh.Email;
            }
            else
            {
                lstGioHang = Session["GioHang"] as List<GioHang>;
            }
            var listSPKM = from ctkm in db.ChiTietKhuyenMais
                           join km in db.KhuyenMais on ctkm.MaKM equals km.MaKM
                           join sp in db.SanPhams on ctkm.MaSP equals sp.MaSP
                           where km.MaKM == 1
                           select new TakeEverything
                           {
                               ID = sp.MaSP,
                               TenSP = sp.TenSP,
                               DonGia = (decimal)sp.DonGia,
                               GiamGia = (int)ctkm.GiamGia,
                               HinhAnh = sp.HinhAnh,
                               TrungBinhDanhGia = (double)sp.TrungBinhDanhGia
                           };
            List<TakeEverything> lstSanPham = new List<TakeEverything>();
            foreach (GioHang gh in lstGioHang)
            {
                SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == gh.MaSP);
                TakeEverything tk = new TakeEverything();
                tk.MaSP = sp.MaSP;
                int GiamGia = 0;
                foreach (TakeEverything item in listSPKM)
                {
                    if (item.ID == sp.MaSP)
                    {
                        GiamGia = item.GiamGia;
                    }
                }
                tk.DonGia = (decimal)sp.DonGia * (100 - GiamGia) / 100;
                tk.TenSP = sp.TenSP;
                tk.SoLuongTrongGioHang = (int)gh.SoLuong;
                tk.CauHinh = sp.CauHinh;
                lstSanPham.Add(tk);
                TongTien += (double)tk.DonGia * tk.SoLuongTrongGioHang;
            }
            ViewBag.TongTien = TongTien;
            foreach(var item in lstSanPham)
            {
                ChiTietDonDatHang ct = new ChiTietDonDatHang();
                ct.MaDDH = MaDDH;
                ct.MaSP = item.MaSP;
                ct.SoLuong = item.SoLuongTrongGioHang;
                ct.TenSP = item.TenSP;
                ct.DonGiaBan = item.DonGia;
                db.ChiTietDonDatHangs.Add(ct);
                CapNhatSoLuongTon(item.MaSP, item.SoLuongTrongGioHang);
            }
            db.SaveChanges();
            // Send mail
            var mail = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("email@...", "password"),
                EnableSsl = true,
            };
            var message = new MailMessage();
            var mailFrom = "hoangquangthai57@gmail.com";
            message.From = new MailAddress("hoangquangthai57@gmail.com");
            message.ReplyToList.Add(mailFrom);
            message.To.Add(new MailAddress(ddh.Email));
            message.Subject = "Don Hang So 1424 Da dat thanh cong" ;
            mail.Send(message);
            XoaToanBoGioHang();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ThemGioHang(int MaSP, String url)
        {
            SanPham x = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            if(x.SoLuongTon <= 0)
            {
                return Redirect(url);
            }
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            if (kh == null)
            {
                List<GioHang> listGioHang = LayGiohang();
                GioHang sp = listGioHang.Find(n => n.MaSP == MaSP);
                if (sp == null)
                {
                    sp = new GioHang(MaSP, 1);
                    listGioHang.Add(sp);
                }
                else
                {
                    sp.SoLuong++;
                }

            }
            else
            {
                List<GioHang> listGH = (from xx in db.GioHangs where xx.MaKH == kh.MaKH select xx).ToList();
                GioHang gh = listGH.Find(n => n.MaSP == MaSP);
                if (gh == null)
                {
                    GioHang tmp = new GioHang(MaSP, kh.MaKH, 1);
                    db.GioHangs.Add(tmp);
                    db.SaveChanges();
                }
                else
                {
                    GioHang sp = db.GioHangs.SingleOrDefault(n => n.MaSP == MaSP && n.MaKH == kh.MaKH);
                    sp.SoLuong++;
                    db.SaveChanges();
                    
                }

            }
            
            return Redirect(url);

        }
        public ActionResult CapNhatGioHang(FormCollection f, int? MaSP, int SoLuongTon)
        {
            KhachHang kh = Session["TaiKhoan"] as KhachHang;
            List<GioHang> lstGioHang = new List<GioHang>();
            if (kh != null)
            {
                GioHang gh = db.GioHangs.SingleOrDefault(n => n.MaKH == kh.MaKH && n.MaSP == MaSP);
                gh.SoLuong = int.Parse(f["SoLuong"]);
                if(gh.SoLuong > SoLuongTon)
                {
                    gh.SoLuong = SoLuongTon;
                }
                db.SaveChanges();
            }
            else
            {
                lstGioHang = Session["GioHang"] as List<GioHang>;
                foreach (GioHang gh in lstGioHang)
                {
                    if (gh.MaSP == MaSP)
                    {
                        gh.SoLuong = int.Parse(f["SoLuong"]);
                        if (gh.SoLuong > SoLuongTon)
                        {
                            gh.SoLuong = SoLuongTon;
                        }
                    }
                }



            }
            return RedirectToAction("XemGioHang");
        }
        private void XoaToanBoGioHang()
        {
            KhachHang kh = Session["TaiKhoan"] as KhachHang;
            if (kh != null)
            {
                var lstGioHang = from gh in db.GioHangs where gh.MaKH == kh.MaKH select gh;
                foreach (var item in lstGioHang)
                {
                    db.GioHangs.Remove(item);
                }
                db.SaveChanges();
            }
            else
            {
                List<GioHang> lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;

            }
        }
        private void CapNhatSoLuongTon(int MaSP, int SoLuong)
        {
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == MaSP);
            sp.SoLuongTon -= SoLuong;
            db.SaveChanges();
        }
    }
}
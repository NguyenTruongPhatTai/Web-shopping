using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nhom9.Models;
using System.Data.Entity;

namespace Nhom9.Controllers
{
    public class BillController : Controller
    {
        Nhom9DB db = new Nhom9DB();
        // GET: Bill
        //Hoán đơn 
        [HttpGet]
        public ActionResult ListBills()
        {
            List<HoaDon> list = new List<HoaDon>();
            TaiKhoanNguoiDung tk = (TaiKhoanNguoiDung)Session[Nhom9.Session.ConstaintUser.USER_SESSION];
            list = db.HoaDons.Where(p => p.MaTK == tk.MaTK).OrderByDescending(x => x.NgayDat).ToList();
            return View(list);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            TaiKhoanNguoiDung tk = (TaiKhoanNguoiDung)Session[Nhom9.Session.ConstaintUser.USER_SESSION];
            if (tk == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            else
            {
                if(db.HoaDons.FirstOrDefault(x => x.MaTK == tk.MaTK) == null)
                {
                    return RedirectToAction("PageNotFound", "Error");
                }
            }
            HoaDon hd = db.HoaDons.Include("TaiKhoanNguoiDung").Include(x => x.ChiTietHoaDons.Select(y => y.SanPhamChiTiet.SanPham))
                .Include(x => x.ChiTietHoaDons.Select(y => y.SanPhamChiTiet.KichCo))
                .Where(x => x.MaHD == id).FirstOrDefault();
            return View(hd);
        }

        [HttpPost]
        public JsonResult CreateBill(HoaDon hd)
        {
            try
            {
                hd.NgayDat = DateTime.Now;
                hd.NgaySua = DateTime.Now;
                hd.TrangThai = 1;
                db.HoaDons.Add(hd);
                db.SaveChanges();
                List<ChiTietHoaDon> list = (List<ChiTietHoaDon>)Session[Nhom9.Session.ConstainCart.CART];
                foreach (ChiTietHoaDon item in list)
                {
                    item.MaHD = hd.MaHD;
                    db.ChiTietHoaDons.Add(item);
                    db.SaveChanges();
                }
                Session.Remove(Nhom9.Session.ConstainCart.CART);
                return Json(new { status = true, billid = hd.MaHD });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Có lỗi gì đó! Thử lại sau" + ex.Message });
            }

        }

        [HttpPost]
        public JsonResult ChangeStatus(int mahd, int stt)
        {
            try
            {
                TaiKhoanNguoiDung tk = (TaiKhoanNguoiDung)Session[Nhom9.Session.ConstaintUser.USER_SESSION];
                HoaDon hd = db.HoaDons.Where(x => x.MaHD == mahd).FirstOrDefault();
                if(hd.TrangThai != 1)
                {
                    return Json(new { status = false }, JsonRequestBehavior.AllowGet);
                }
                hd.TrangThai = stt;
                hd.NguoiSua = tk.HoTen;
                hd.NgaySua = DateTime.Now;
                db.SaveChanges();
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nhom9.Models;
using PagedList;
using System.Data.Entity;

namespace Nhom9.Areas.Admin.Controllers
{
    public class BillController : BaseController
    {
        Nhom9DB db = new Nhom9DB();
        // GET: Admin/Bill
        //Hóa Đơn
        [HttpGet]
        public ActionResult Index(DateTime? searchString,int? status ,int page = 1, int pageSize = 10)
        {
            List<HoaDon> hoaDons = db.HoaDons.Include("TaiKhoanNguoiDung").Select(p => p).ToList();
            if(status != null)
            {
                hoaDons = hoaDons.Where(x => x.TrangThai == status).ToList();
                ViewBag.Status = status;
            }
            if (searchString != null)
            {
                ViewBag.searchString = searchString.Value.ToString("yyyy-MM-dd");
                string search = searchString.Value.ToString("dd/MM/yyyy");
                hoaDons = hoaDons.Where(hd => hd.NgayDat.ToString().Contains(search)).ToList();
            }
            return View(hoaDons.OrderBy(hd => hd.NgayDat).ToPagedList(page, pageSize));
        }

        [HttpPost]
        public JsonResult Index(int id)
        {
            HoaDon hd = db.HoaDons.Include("TaiKhoanNguoiDung")
                .Where(x => x.MaHD == id).FirstOrDefault();
            IEnumerable<ChiTietHoaDon> chiTietHoaDons = db.ChiTietHoaDons.Include("SanPhamChiTiet")
                .Include("SanPhamChiTiet.KichCo")
                .Where(x => x.MaHD == id);
            List<SanPham> list = new List<SanPham>();
            foreach(ChiTietHoaDon item in chiTietHoaDons)
            {
                list.Add(db.SanPhams.Where(x => x.MaSP == item.SanPhamChiTiet.MaSP).FirstOrDefault());
            }
            return Json(new { hoadon = hd, cthd = chiTietHoaDons, sp = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChangeStatus(int mahd, int stt)
        {
            try
            {
                TaiKhoanQuanTri tk = (TaiKhoanQuanTri)Session[Nhom9.Session.ConstaintUser.ADMIN_SESSION];
                HoaDon hd = db.HoaDons.Where(x => x.MaHD == mahd).FirstOrDefault();
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
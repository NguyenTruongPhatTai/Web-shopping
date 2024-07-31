using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nhom9.Models;
using PagedList;

namespace Nhom9.Controllers
{
    public class ProductController : Controller
    {
        Nhom9DB db = new Nhom9DB();
        // GET: Product
        public ActionResult Shop(string searchString, int? madm, int page = 1, int pageSize = 9)
        {
            ViewBag.searchString = searchString;
            ViewBag.madm = madm;
            var sanphams = db.SanPhams.Select(p => p);
            if (!String.IsNullOrEmpty(searchString))
            {
                sanphams = sanphams.Where(sp => sp.TenSP.Contains(searchString));
            }
            if (madm != null && madm != 0)
            {
                sanphams = sanphams.Where(s => s.MaDM == madm);
                ViewBag.DanhMuc = db.DanhMucs.Where(d => d.MaDM == madm).FirstOrDefault();
            }
            return View(sanphams.OrderBy(sp => sp.MaSP).ToPagedList(page, pageSize));
        }

        public ActionResult ProductDetail(int id)
        {
            SanPham sp = db.SanPhams.Include("DanhMuc").Where(s => s.MaSP.Equals(id)).FirstOrDefault();
            List<SanPhamChiTiet> list = db.SanPhamChiTiets.Include("KichCo").Where(s => s.MaSP.Equals(id)).ToList();
            ViewBag.SPCT = list;
            ViewBag.Exitst = list[0];
            return View(sp);
        }

        [HttpPost]
        public JsonResult Index(int id)
        {
            SanPham sp = db.SanPhams.Include("DanhMuc").Include("SanPhamChiTiets").Where(s => s.MaSP.Equals(id)).FirstOrDefault();
            return Json(sp, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Detail(int id)
        {
            SanPhamChiTiet spct = db.SanPhamChiTiets.Where(sp => sp.IDCTSP == id).FirstOrDefault();
            return Json(spct, JsonRequestBehavior.AllowGet);
        }
    }
}
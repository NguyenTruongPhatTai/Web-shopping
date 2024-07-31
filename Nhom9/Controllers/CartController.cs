using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nhom9.Models;

namespace Nhom9.Controllers
{
    public class CartController : Controller
    {
        Nhom9DB db = new Nhom9DB();
        // GET: Cart
        //Giỏ hàng
        [HttpGet]
        public ActionResult Orders()
        {
            List<SanPhamChiTiet> list = new List<SanPhamChiTiet>();
            if(Session[Nhom9.Session.ConstainCart.CART] != null)
            {
                List<ChiTietHoaDon> ses = (List<ChiTietHoaDon>)Session[Nhom9.Session.ConstainCart.CART];
                foreach(ChiTietHoaDon item in ses)
                {
                    list.Add(db.SanPhamChiTiets.Include("SanPham").Include("KichCo").Where(s => s.IDCTSP == item.IDCTSP).FirstOrDefault());
                }
                for(int i = 0; i < list.Count; i++)
                {
                    list[i].ChiTietHoaDons.Add(ses[i]);
                }
            }
            return View(list);
        }

        [HttpPost]
        public JsonResult AddToCart(ChiTietHoaDon chiTiet)
        {
            if(chiTiet.SoLuongMua > db.SanPhamChiTiets.Where(x => x.IDCTSP == chiTiet.IDCTSP).FirstOrDefault().SoLuong)
            {
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            bool isExists = false;
            List<ChiTietHoaDon> list = new List<ChiTietHoaDon>();
            if (Session[Nhom9.Session.ConstainCart.CART] != null)
            {
                list = (List<ChiTietHoaDon>)Session[Nhom9.Session.ConstainCart.CART];
                foreach(ChiTietHoaDon item in list)
                {
                    if(item.IDCTSP == chiTiet.IDCTSP)
                    {
                        item.SoLuongMua += chiTiet.SoLuongMua;
                        isExists = true;
                    }
                }
                if (!isExists)
                {
                    list.Add(chiTiet);
                }
            }
            else
            {
                list = new List<ChiTietHoaDon>();
                list.Add(chiTiet);
            }
            list.RemoveAll((x) => x.SoLuongMua <= 0);
            foreach(ChiTietHoaDon item in list)
            {
                item.GiaMua = db.SanPhamChiTiets.Include("SanPham").Where(s => s.IDCTSP == item.IDCTSP).FirstOrDefault().SanPham.Gia;
            }
            Session[Nhom9.Session.ConstainCart.CART] = list;
            return Json(new { status = true,cart = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteFromCart(int idctsp)
        {
            List<ChiTietHoaDon> list = (List<ChiTietHoaDon>)Session[Nhom9.Session.ConstainCart.CART];
            list.RemoveAll((x) => x.IDCTSP == idctsp);
            Session[Nhom9.Session.ConstainCart.CART] = list;
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CheckOut()
        {
            TaiKhoanNguoiDung tk = (TaiKhoanNguoiDung)Session[Nhom9.Session.ConstaintUser.USER_SESSION];
            if(tk == null)
            {
                return RedirectToAction("Login", "Home");
            }
            List<SanPhamChiTiet> list = new List<SanPhamChiTiet>();
            List<ChiTietHoaDon> ses = (List<ChiTietHoaDon>)Session[Nhom9.Session.ConstainCart.CART];
            ViewBag.TaiKhoan = tk;
            foreach (ChiTietHoaDon item in ses)
            {
                list.Add(db.SanPhamChiTiets.Include("SanPham").Include("KichCo").Where(s => s.IDCTSP == item.IDCTSP).FirstOrDefault());
            }
            for (int i = 0; i < list.Count; i++)
            {
                list[i].ChiTietHoaDons.Add(ses[i]);
            }
            return View(list);
        }
    }
}
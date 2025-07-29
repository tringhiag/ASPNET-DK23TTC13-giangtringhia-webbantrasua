using WebTraSua.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebTraSua.Controllers
{
    public class NguoiDungController : Controller
    {
        // GET: NguoiDung
        DataClasses1DataContext data = new DataClasses1DataContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DangKi()
        {
            return View();
        }
        //POST :Hàm DangKi nhận dữ liệu từ trang DangKi và tạo mới dữ liệu
        [HttpPost]
        public ActionResult DangKi(FormCollection collection, KhachHang KH)
        {
            // Gán các giá trị đã nhập
            var hoten = collection["HoTenKH"];
            var taikhoan = collection["TaiKhoan"];
            var matkhau = collection["MatKhau"];
            //var nhaplaimatkhau = collection["NhapLaiMatKhau"];
            var email = collection["Email"];
            var diachi = collection["DiaChi"];
            var dienthoai = collection["DienThoai"];
            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "* Bắt buộc";
            }
            else if (String.IsNullOrEmpty(taikhoan))
            {
                ViewData["Loi2"] = "* Bắt buộc";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi3"] = "* Bắt buộc";
            }

            else if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi5"] = "* Bắt buộc";
            }
            else if (String.IsNullOrEmpty(diachi))
            {
                ViewData["Loi6"] = "* Bắt buộc";
            }
            else if (String.IsNullOrEmpty(dienthoai))
            {
                ViewData["Loi7"] = "* Chúng tôi sẽ liên hệ với bạn";
            }
            else
            {
                KH.HoTen = hoten;
                KH.TaiKhoan = taikhoan;
                KH.MatKhau = matkhau;
                KH.Email = email;
                KH.DiaChi_KH = diachi;
                KH.DienThoai_KH = dienthoai;

                data.KhachHangs.InsertOnSubmit(KH);
                data.SubmitChanges();
                return RedirectToAction("DangNhap");
            }
            return this.DangKi();
        }
        public ActionResult DangNhap(FormCollection collection)
        {
            var taikhoan = collection["TaiKhoan"];
            var matkhau = collection["MatKhau"];
            if (String.IsNullOrEmpty(taikhoan))
            {
                ViewData["Loi1"] = "* Bắt buộc";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "* Bắt buộc";
            }
            else
            {
                KhachHang KH = data.KhachHangs.SingleOrDefault(n => n.TaiKhoan == taikhoan && n.MatKhau == matkhau);
                if (KH != null)
                {
                    Session["TaiKhoan"] = KH;
                    Session["HoTen"] = KH.HoTen;
                    return RedirectToAction("GioHang", "GioHang");
                }
                else
                {
                    ViewBag.ThongBao = "Tài Khoản hoặc Mật khẩu không đúng";
                }
            }
            return View();
        }
        // đăng xuất
        public ActionResult DangXuat()
        {
            Session["TaiKhoan"] = null;
            return RedirectToAction("Index", "GuitarShop");
        }
        // tạo partipal view de hien thi lên màn hình chính
        public ActionResult NguoiDungPartial(FormCollection collection)
        {
            return PartialView();
        }
    }
}
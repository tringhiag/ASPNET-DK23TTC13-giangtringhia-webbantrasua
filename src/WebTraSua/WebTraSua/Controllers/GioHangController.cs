
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTraSua.Models;

namespace WebTraSua.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        // tạo đối tượng data chứa dữ liệu từ models dbQLBanSach đã tạo
        DataClasses1DataContext data = new DataClasses1DataContext();
        // lấy giỏ hàng
        public List<GioHang> LayGioHang()
        {
            List<GioHang> listGiohang = Session["Giohang"] as List<GioHang>;
            if (listGiohang == null)
            {
                // nếu giỏ hàng chưa tồn tại thì khởi tạo listGioHang
                listGiohang = new List<GioHang>();
                Session["Giohang"] = listGiohang;
            }
            return listGiohang;
        }
        // thêm vào giỏ hàng
        public ActionResult ThemGioHang(int iMaSP, string strURL)
        {
            // lấy ra session giohang
            List<GioHang> listGiohang = LayGioHang();
            // kiểm tra sách này tồn tại trong Session["GioHang"] chưa
            GioHang sanpham = listGiohang.Find(n => n.MaSP == iMaSP);
            if (sanpham == null)
            {
                sanpham = new GioHang(iMaSP);
                listGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSlMua++;
                return Redirect(strURL);
            }
        }
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> listGiohang = Session["Giohang"] as List<GioHang>;
            if (listGiohang != null)
            {
                iTongSoLuong = listGiohang.Sum(n => n.iSlMua);
            }
            return iTongSoLuong;
        }
        // tính tổng tiền
        private decimal TongTien()
        {
            decimal iTongTien = 0;
            List<GioHang> listGiohang = Session["Giohang"] as List<GioHang>;
            if (listGiohang != null)
            {
                iTongTien = listGiohang.Sum(n => n.dThanhTien);
            }
            return iTongTien;
        }
        // xây dựng trang giỏ hàng
        public ActionResult GioHang()
        {
            List<GioHang> listGiohang = LayGioHang();
            if (listGiohang.Count == 0)
            {
                return RedirectToAction("Index", "WebTraSua");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(listGiohang);
        }
        // tạo partipal view de hien thi lên màn hình chính
        public ActionResult GioHangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        //xóa Giohang
        public ActionResult XoaGiohang(int iMaSP)
        { //Lay gio hang tu Session
            List<GioHang> lstGiohang = LayGioHang();
            //kiem tra sach da co trong Session[ Giohang"
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.MaSP == iMaSP);
            //Neu ton tai thi cho sua Soluong
            if (sanpham != null)
            {
                lstGiohang.RemoveAll(n => n.MaSP == iMaSP);
                return RedirectToAction("GioHang");
            }

            if (lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "WebTraSua");
            }
            return RedirectToAction("GioHang");
        }
        //Cap nhat Giỏ hàng
        public ActionResult CapnhatGioHang(int iMaSP, FormCollection f)
        {
            //Lay gio hang tu Session
            List<GioHang> lstGiohang = LayGioHang();
            //kiem tra sach da co trong Session["Giohang"]
            GioHang sanpham = lstGiohang.SingleOrDefault(n => n.MaSP == iMaSP);
            //Neu ton tai thi cho sua Soluong
            if (sanpham != null)
            {
                sanpham.iSlMua = int.Parse(f["txtSlMua"].ToString());
            }
            return RedirectToAction("GioHang");
        }
        public ActionResult XoaTatCaGioHang()
        {
            // lấy giỏ hàng từ session
            List<GioHang> lstGiohang = LayGioHang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "WebTraSua");
        }
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "NguoiDung");

            }
            if (Session["Giohang"] == null)
            {
                return RedirectToAction("Index", "WebTraSua");
            }
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            DonDatHang ddh = new DonDatHang();
            KhachHang kh = (KhachHang)Session["TaiKhoan"];
            List<GioHang> giohang = LayGioHang();
            ddh.MaKH = kh.MaKH;
            ddh.NgayDat = DateTime.Now;
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["NgayGiao"]);
            ddh.NgayGiao = DateTime.Parse(ngaygiao);
            ddh.TinhTrangGiaoHang = false;
            ddh.DaThanhToan = false;
            data.DonDatHangs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            foreach (var item in giohang)
            {
                ChiTietDonHang ctdh = new ChiTietDonHang();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.MaSP = item.MaSP;
                ctdh.SlMua = item.iSlMua;
                ctdh.DonGia = (decimal)item.dGiaBan;
                data.ChiTietDonHangs.InsertOnSubmit(ctdh);

            }
            data.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("XacNhanDonHang", "GioHang");
        }
        public ActionResult XacNhanDonHang()
        {
            return View();
        }
        public ActionResult ThanhToan_ThanhCong()
        {
            return View();
        }
        public ActionResult ThanhToan_ThatBai()
        {
            return View();
        }
    }
}
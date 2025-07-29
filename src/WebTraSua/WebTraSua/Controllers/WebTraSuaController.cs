using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTraSua.Models;
using PagedList;
using PagedList.Mvc;

namespace WebTraSua.Controllers
{
    public class WebTraSuaController : Controller
    {
        // GET: WebTraSua
        DataClasses1DataContext data = new DataClasses1DataContext();
        private List<ChiTietSanPham> Laysanphammoi(int count)
        {
            // sắp xếp giảm dần theo ngày cập nhật, lấy count dòng đầu
            return data.ChiTietSanPhams.OrderByDescending(a => a.NgayCapNhat).Take(count).ToList();
        }
        public ActionResult Index(int? page)
        {
           
            // tạo bien quy dinh so san phẩm trên 1 trang
            int pageSize = 6;
            // tạo biến số trang
            int pageNum = (page ?? 1);
            // lấy  top  6 bán chạy nhất
            var danmoi = Laysanphammoi(15);
            return View(danmoi.ToPagedList(pageNum, pageSize));
            
        }
        public ActionResult PhanLoaiSanPham()
        {
            var phanloaisanpham = from phanloai in data.PhanLoaiSanPhams select phanloai;
            return PartialView(phanloaisanpham);
        }
        public ActionResult TheLoaiSanPham()
        {
            var theloaisanpham = from theloai in data.TheLoaiSanPhams select theloai;
            return PartialView(theloaisanpham);
        }
        public ActionResult SP_TheoPhanLoai(int id, int? page)
        {
            int pageSize = 6;
            int pageNum = (page ?? 1);
            // lấy  top  6 bán chạy nhất
            var phanloai = from s in data.ChiTietSanPhams where s.MaLoai == id select s;
            return View(phanloai.ToPagedList(pageNum, pageSize));
        }
        public ActionResult SP_TheoTheLoai(int id, int? page)
        {
            int pageSize = 6;
            int pageNum = (page ?? 1);
            var theloai = from s in data.ChiTietSanPhams where s.MaTheLoai == id select s;
            return View(theloai.ToPagedList(pageNum, pageSize));
        }
        public ActionResult ChiTietSanPham(int id)
        {
            var chitiet = from l in data.ChiTietSanPhams where l.MaSP == id select l;
            return View(chitiet.Single());
        }
        public ActionResult LienHe()
        {
            return View();
        }
    }
}
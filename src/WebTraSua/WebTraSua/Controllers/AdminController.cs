using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.IO;
using WebTraSua.Models;


namespace WebTraSua.Controllers
{
    public class AdminController : Controller
    {
        DataClasses1DataContext data = new DataClasses1DataContext();
        // GET: Admin
        private List<TheLoaiSanPham> Laytheloai(int count)
        {
            // sắp xếp giảm dần theo ngày cập nhật, lấy count dòng đầu
            return data.TheLoaiSanPhams.OrderByDescending(a => a.MaTheLoai).Take(count).ToList();
        }
        public ActionResult Index()
        {
            if (Session["Ten_Admin"] == null || Session["Ten_Admin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            var theloaisanpham = Laytheloai(6);
            return View(theloaisanpham);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var taikhoan_admin = collection["User_Admin"];
            var matkhau_admin = collection["Pass_Admin"];
            Admin ad = data.Admins.SingleOrDefault(n => n.User_Admin == taikhoan_admin && n.Pass_Admin == matkhau_admin);
            if (ad != null)
            {
                Session["Ten_Admin"] = ad.HoTen_Admin;
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                ViewBag.ThongBao = "* Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }
        public ActionResult SanPham(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(data.ChiTietSanPhams.ToList().OrderBy(n => n.MaSP).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult ThemMoiSanPham()
        {
            //Dua du lieu vao dropdownList
            //Lay ds tu tabke chu de, sắp xep tang dan trheo ten chu de, chon lay gia tri Ma CD, hien thi thi Tenchude
            ViewBag.MaLoai = new SelectList(data.PhanLoaiSanPhams.ToList().OrderBy(n => n.Loai), "MaLoai", "Loai");
            ViewBag.MaTheLoai = new SelectList(data.TheLoaiSanPhams.ToList().OrderBy(n => n.TenTheLoai), "MaTheLoai", "TenTheLoai");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemMoiSanPham(ChiTietSanPham sp, HttpPostedFileBase fileUpload, HttpPostedFileBase fileUpload_1, HttpPostedFileBase fileUpload_2, HttpPostedFileBase fileUpload_3)
        {
            //Dua du lieu vao dropdownload
            ViewBag.MaLoai = new SelectList(data.PhanLoaiSanPhams.ToList().OrderBy(n => n.Loai), "MaLoai", "Loai");
            ViewBag.MaTheLoai = new SelectList(data.TheLoaiSanPhams.ToList().OrderBy(n => n.TenTheLoai), "MaTheLoai", "TenTheLoai");
            //Kiem tra duong dan file
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            if (fileUpload_1 == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh 1";
                return View();
            }
            if (fileUpload_2 == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh 2";
                return View();
            }
            if (fileUpload_3 == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh 3";
                return View();
            }
            //Them vao CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //Luu ten fie, luu y bo sung thu vien using System.IO;
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var fileName_1 = Path.GetFileName(fileUpload_1.FileName);
                    var fileName_2 = Path.GetFileName(fileUpload_2.FileName);
                    var fileName_3 = Path.GetFileName(fileUpload_3.FileName);
                    //Luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/images"), fileName);
                    var path_1 = Path.Combine(Server.MapPath("~/images"), fileName_1);
                    var path_2 = Path.Combine(Server.MapPath("~/images"), fileName_2);
                    var path_3 = Path.Combine(Server.MapPath("~/images"), fileName_3);
                    //Kiem tra hình anh ton tai chua?
                    if (System.IO.File.Exists(path) || System.IO.File.Exists(path_1) || System.IO.File.Exists(path_2) || System.IO.File.Exists(path_3))
                    { ViewBag.Thongbao = "Hình ảnh đã tồn tại"; }
                    else
                    {
                        //Luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                        fileUpload_1.SaveAs(path_1);
                        fileUpload_2.SaveAs(path_2);
                        fileUpload_3.SaveAs(path_3);
                    }
                    sp.AnhBia = fileName;
                    sp.Anh_1 = fileName_1;
                    sp.Anh_2 = fileName_2;
                    sp.Anh_3 = fileName_3;
                    //Luu vao CSDL
                    data.ChiTietSanPhams.InsertOnSubmit(sp);
                    data.SubmitChanges();
                }
                return RedirectToAction("SanPham");
            }
        }
        //Hiển thị sản phẩm
        public ActionResult ChiTietSanPham(int id)
        {
            //Lay ra doi tuong sach theo ma
            ChiTietSanPham sp = data.ChiTietSanPhams.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sp.MaSP;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        //Xóa sản phẩm
        [HttpGet]
        public ActionResult XoaSanPham(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            ChiTietSanPham sp = data.ChiTietSanPhams.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sp.MaSP;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        [HttpPost, ActionName("XoaSanPham")]
        public ActionResult XacNhanXoa(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            ChiTietSanPham sp = data.ChiTietSanPhams.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sp.MaSP;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.ChiTietSanPhams.DeleteOnSubmit(sp);
            data.SubmitChanges();
            return RedirectToAction("SanPham");
        }
        //Chinh sửa sản phẩm
        [HttpGet]
        public ActionResult SuaSanPham(int id)
        {
            //Lay ra doi tuong sach theo ma
            ChiTietSanPham sp = data.ChiTietSanPhams.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sp.MaSP;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Dua du lieu vao dropdownList
            //Lay ds tu tabke chu de, sắp xep tang dan trheo ten chu de, chon lay gia tri Ma CD, hien thi thi Tenchude
            ViewBag.MaLoai = new SelectList(data.PhanLoaiSanPhams.ToList().OrderBy(n => n.Loai), "MaLoai", "Loai", sp.MaLoai);
            ViewBag.MaTheLoai = new SelectList(data.TheLoaiSanPhams.ToList().OrderBy(n => n.TenTheLoai), "MaTheLoai", "TenTheLoai", sp.MaTheLoai);
            return View(sp);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaSanPham(ChiTietSanPham sp, HttpPostedFileBase fileUpload, HttpPostedFileBase fileUpload_1, HttpPostedFileBase fileUpload_2, HttpPostedFileBase fileUpload_3)
        {
            //Dua du lieu vao dropdownload
            ViewBag.Maloai = new SelectList(data.PhanLoaiSanPhams.ToList().OrderBy(n => n.Loai), "MaLoai", "Loai");
            ViewBag.MaTheLoai = new SelectList(data.TheLoaiSanPhams.ToList().OrderBy(n => n.TenTheLoai), "MaTheLoai", "TenTheLoai");
            //Kiem tra duong dan file
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View(sp);
            }
            if (fileUpload_1 == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh 1";
                return View(sp);
            }
            if (fileUpload_2 == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh 2";
                return View(sp);
            }
            if (fileUpload_3 == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh 3";
                return View(sp);
            }
            //Them vao CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var fileName_1 = Path.GetFileName(fileUpload_1.FileName);
                    var fileName_2 = Path.GetFileName(fileUpload_2.FileName);
                    var fileName_3 = Path.GetFileName(fileUpload_3.FileName);
                    //Luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/images"), fileName);
                    var path_1 = Path.Combine(Server.MapPath("~/images"), fileName_1);
                    var path_2 = Path.Combine(Server.MapPath("~/images"), fileName_2);
                    var path_3 = Path.Combine(Server.MapPath("~/images"), fileName_3);
                    //Kiem tra hình anh ton tai chua?
                    if (System.IO.File.Exists(path) || System.IO.File.Exists(path_1) || System.IO.File.Exists(path_2) || System.IO.File.Exists(path_3))
                    { ViewBag.Thongbao = "Hình ảnh đã tồn tại"; }
                    else
                    {
                        //Luu hinh anh vao duong dan
                        fileUpload.SaveAs(path);
                        fileUpload_1.SaveAs(path_1);
                        fileUpload_2.SaveAs(path_2);
                        fileUpload_3.SaveAs(path_3);
                    }
                    sp.AnhBia = fileName;
                    sp.Anh_1 = fileName_1;
                    sp.Anh_2 = fileName_2;
                    sp.Anh_3 = fileName_3;
                    //Luu vao CSDL   
                    UpdateModel(sp);

                    data.SubmitChanges();
                }
                return RedirectToAction("SanPham");
            }
        }
        public ActionResult TheLoaiSanPham(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(data.TheLoaiSanPhams.ToList().OrderBy(n => n.MaTheLoai).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult ThemMoiTheLoai()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemMoiTheLoai(TheLoaiSanPham theloai, HttpPostedFileBase fileUpload_logo)
        {
            //Them vao CSDL  
            if (fileUpload_logo == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            //Them vao CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //Luu ten fie, luu y bo sung thu vien using System.IO;
                    var fileName_logo = Path.GetFileName(fileUpload_logo.FileName);
                    //Luu duong dan cua file
                    var path_logo = Path.Combine(Server.MapPath("~/images"), fileName_logo);
                    //Kiem tra hình anh ton tai chua?
                    if (System.IO.File.Exists(path_logo))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        //Luu hinh anh vao duong dan
                        fileUpload_logo.SaveAs(path_logo);
                    }
                    theloai.Logo = fileName_logo;
                    //Luu vao CSDL
                    data.TheLoaiSanPhams.InsertOnSubmit(theloai);
                    data.SubmitChanges();
                }
                return RedirectToAction("TheLoaiSanPham");
            }

        }
        public ActionResult ChiTietTheLoai(int id)
        {
            //Lay ra doi tuong sach theo ma
            TheLoaiSanPham theloai = data.TheLoaiSanPhams.SingleOrDefault(n => n.MaTheLoai == id);
            ViewBag.MaTheLoai = theloai.MaTheLoai;
            if (theloai == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(theloai);
        }
        //Xóa sản phẩm
        [HttpGet]
        public ActionResult XoaTheLoai(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            TheLoaiSanPham theloai = data.TheLoaiSanPhams.SingleOrDefault(n => n.MaTheLoai == id);
            ViewBag.MaTheLoai = theloai.MaTheLoai;
            if (theloai == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(theloai);
        }
        [HttpPost, ActionName("XoaTheLoai")]
        public ActionResult XacNhanXoa_TheLoai(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            TheLoaiSanPham theloai = data.TheLoaiSanPhams.SingleOrDefault(n => n.MaTheLoai == id);
            ViewBag.MaTheLoai = theloai.MaTheLoai;
            if (theloai == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.TheLoaiSanPhams.DeleteOnSubmit(theloai);
            data.SubmitChanges();
            return RedirectToAction("TheLoaiSanPham");
        }
        //Chinh sửa sản phẩm
        [HttpGet]
        public ActionResult SuaTheLoai(int id)
        {
            //Lay ra doi tuong sach theo ma
            TheLoaiSanPham theloai = data.TheLoaiSanPhams.SingleOrDefault(n => n.MaTheLoai == id);
            ViewBag.MaTheLoai = theloai.MaTheLoai;
            if (theloai == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(theloai);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaTheLoai(TheLoaiSanPham theloai, HttpPostedFileBase fileUpload_logo)
        {
            if (fileUpload_logo == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View(theloai);
            }
            //Them vao CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //Luu ten fie, luu y bo sung thu vien using System.IO;
                    var fileName_logo = Path.GetFileName(fileUpload_logo.FileName);
                    //Luu duong dan cua file
                    var path_logo = Path.Combine(Server.MapPath("~/images/"), fileName_logo);
                    //Kiem tra hình anh ton tai chua?
                    if (System.IO.File.Exists(path_logo))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        //Luu hinh anh vao duong dan
                        fileUpload_logo.SaveAs(path_logo);
                    }
                    theloai.Logo = fileName_logo;
                    //Luu vao CSDL
                    UpdateModel(theloai);
                    data.SubmitChanges();
                }
                return RedirectToAction("TheLoaiSanPham");
            }
        }
    }
}

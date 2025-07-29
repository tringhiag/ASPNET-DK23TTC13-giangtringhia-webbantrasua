
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebTraSua.Models;

namespace WebTraSua.Models
{
    public class GioHang
    {
        DataClasses1DataContext data = new DataClasses1DataContext();
        public int MaSP { set; get; }
        public String sTenSP { set; get; }
        public String sAnhBia { set; get; }
        public decimal dGiaBan { set; get; }
        public int iSlMua { set; get; }
        public decimal dThanhTien
        {
            get { return iSlMua * dGiaBan; }
        }
        // khỏi tạo giỏ hàng theo MAsach được truyền vào với sl mặc định là 1
        public GioHang(int MaSP)
        {
            this.MaSP = MaSP;
            ChiTietSanPham sp = data.ChiTietSanPhams.Single(n => n.MaSP == this.MaSP);
            sTenSP = sp.TenSP;
            sAnhBia = sp.AnhBia;
            dGiaBan = decimal.Parse(sp.GiaBan.ToString());
            iSlMua = 1;
        }
    }
}
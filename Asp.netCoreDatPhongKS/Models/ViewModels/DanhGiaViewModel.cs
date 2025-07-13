namespace Asp.netCoreDatPhongKS.Models.ViewModels
{
    public class DanhGiaViewModel
    {
        public int PhieuDatPhongId { get; set; }
        public string? MaPhieu { get; set; }
        public DateTime? NgayNhan { get; set; }
        public DateTime? NgayTra { get; set; }
        public List<PhongDanhGia> Phongs { get; set; } = new List<PhongDanhGia>();
    }

    public class PhongDanhGia
    {
        public int PhongId { get; set; }
        public string? SoPhong { get; set; }
        public int? DanhGiaId { get; set; }
        public int? Diem { get; set; }
        public string? NoiDung { get; set; }
        public int PhieuDatPhongId { get; set; } // Thêm thuộc tính này
    }

    public class DanhGiaDichVuViewModel
    {
        public int MaDonHangDv { get; set; }
        public DateTime? NgayDat { get; set; }
        public List<DichVuDanhGia> DichVus { get; set; } = new List<DichVuDanhGia>();
    }

    public class DichVuDanhGia
    {
        public int DichVuId { get; set; }
        public string? TenDichVu { get; set; }
        public int? SoLuong { get; set; }
        public int? DanhGiaId { get; set; }
        public int? Diem { get; set; }
        public string? NoiDung { get; set; }
        public int MaDonHangDv { get; set; } // Thêm thuộc tính này
    }
}
using System.ComponentModel.DataAnnotations;

namespace Asp.netCoreDatPhongKS.Models.ViewModels
{
    //dùng để thêm tk nv or kh
    public class TaiKhoanViewModel
    {
        public TaiKhoan TaiKhoan { get; set; } = new TaiKhoan();

        public NhanVien? NhanVien { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn vai trò.")]
        [Range(2, 3, ErrorMessage = "Vai trò không hợp lệ.")]
        public int VaiTroId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        public string MatKhau { get; set; } = string.Empty;
    }
}

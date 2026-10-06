// Họ và tên: Nguyễn Tiến Đạt
// Mã sinh viên: 23103100053
// Nội dung thực hiện: Module 3 – Thông tin nhân viên

using System.ComponentModel.DataAnnotations;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;
public class NhanVien
{
    [Key]
    public int MaNhanVien { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn tài khoản.")]
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn tài khoản.")]
    [Display(Name = "Tài khoản")]
    public int MaTaiKhoan { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự.")]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "Số điện thoại gồm 10 chữ số và bắt đầu bằng 0.")]
    [Display(Name = "Số điện thoại")]
    public string SoDienThoai { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn khu vực.")]
    [Display(Name = "Khu vực phụ trách")]
    public int MaKhuVuc { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn ngày vào làm.")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày vào làm")]
    public DateTime NgayVaoLam { get; set; } = DateTime.Today;

    [EnumDataType(typeof(TrangThaiNhanVien), ErrorMessage = "Trạng thái không hợp lệ.")]
    [Display(Name = "Trạng thái")]
    public TrangThaiNhanVien TrangThai { get; set; }
        = TrangThaiNhanVien.DangHoatDong;

    public TaiKhoan? TaiKhoan { get; set; }
    public KhuVuc? KhuVuc { get; set; }
}

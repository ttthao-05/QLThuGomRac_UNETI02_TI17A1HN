// Họ và tên: Vũ Đức Hải
// Mã sinh viên: 23103100008
// Nội dung thực hiện: Module 4 - ViewModel theo dõi kết quả thu gom
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QLThuGomRac_UNETI02_TI17A1HN.ViewModels;

public class LoaiRacKhoiLuongVM
{
    public string LoaiRac { get; set; } = "";
    public string DonViTinh { get; set; } = "";
    public decimal KhoiLuong { get; set; }
}

public class KetQuaRowVM
{
    public int MaYeuCau { get; set; }
    public string NguoiDan { get; set; } = "";
    public string KhuVuc { get; set; } = "";
    public string NhanVien { get; set; } = "";
    public DateTime NgayThuGom { get; set; }
    public string LoaiRac { get; set; } = "";
    public decimal KhoiLuong { get; set; }
}

public class TheoDoiKetQuaVM
{
    public YeuCauFilterVM Filter { get; set; } = new();
    public PagedList<KetQuaRowVM> Data { get; set; } = new();
    public List<LoaiRacKhoiLuongVM> TongTheoLoai { get; set; } = new();
    public List<SelectListItem> KhuVucs { get; set; } = new();
    public List<SelectListItem> NhanViens { get; set; } = new();
}

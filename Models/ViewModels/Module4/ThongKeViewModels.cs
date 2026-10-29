// Họ và tên: Vũ Đức Hải
// Mã sinh viên: 23103100008
// Nội dung thực hiện: Module 4 - ViewModel Dashboard và thống kê
namespace QLThuGomRac_UNETI02_TI17A1HN.ViewModels;

public class DashboardVM
{
    public int TongKhuVuc { get; set; }
    public int TongLoaiRac { get; set; }
    public int TongNguoiDan { get; set; }
    public int TongNhanVien { get; set; }
    public int TongYeuCau { get; set; }
    public int ChoXacNhan { get; set; }
    public int DangXuLy { get; set; }
    public int HoanThanh { get; set; }
    public int TuChoiHuy { get; set; }
    public decimal TongKhoiLuong { get; set; }
}

public class NhomSoLuongVM
{
    public string Ten { get; set; } = "";
    public int SoLuong { get; set; }
}

public class NhomKhoiLuongVM
{
    public string Ten { get; set; } = "";
    public decimal KhoiLuong { get; set; }
}

public class ThongKeVM
{
    public List<NhomSoLuongVM> TheoTrangThai { get; set; } = new();
    public List<NhomSoLuongVM> TheoKhuVuc { get; set; } = new();
    public List<NhomSoLuongVM> TheoThang { get; set; } = new();
    public List<NhomSoLuongVM> TheoNhanVien { get; set; } = new();
    public List<NhomKhoiLuongVM> KhoiLuongTheoLoai { get; set; } = new();
    public List<NhomKhoiLuongVM> KhoiLuongTheoKhuVuc { get; set; } = new();
    public List<NhomKhoiLuongVM> KhoiLuongTheoThang { get; set; } = new();
    public NhomKhoiLuongVM? LoaiRacNhieuNhat { get; set; }
}

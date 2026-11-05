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
    public int TongDiemTapKet { get; set; }
    public int TongYeuCau { get; set; }
    public int ChoXacNhan { get; set; }
    public int DangXuLy { get; set; }
    public int HoanThanh { get; set; }
    public int TuChoiHuy { get; set; }
    public int KhongThucHienDuoc { get; set; }
    public decimal TongKhoiLuong { get; set; } // Chỉ cộng các loại có đơn vị kg
    public List<string> NhanThang { get; set; } = new();
    public List<int> YeuCauTheoThang { get; set; } = new();
    public List<int> HoanThanhTheoThang { get; set; } = new();
    public List<NhomKhoiLuongVM> KhoiLuongTheoLoaiKg { get; set; } = new();
    public List<DashboardYeuCauVM> YeuCauGanDay { get; set; } = new();
    public List<DashboardYeuCauVM> LichSapToi { get; set; } = new();
}

public class DashboardYeuCauVM
{
    public int MaYeuCau { get; set; }
    public string NguoiDan { get; set; } = "";
    public string KhuVuc { get; set; } = "";
    public string DiaChi { get; set; } = "";
    public DateTime NgayDangKy { get; set; }
    public DateTime NgayMongMuon { get; set; }
    public string TrangThai { get; set; } = "";
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

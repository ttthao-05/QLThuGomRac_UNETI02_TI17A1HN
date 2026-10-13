// Họ và tên: [Vũ Đức Hải]
// Mã sinh viên: [23103100008]
// Nội dung thực hiện: Module 4 - ViewModel theo dõi yêu cầu
using Microsoft.AspNetCore.Mvc.Rendering;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.ViewModels;

public class YeuCauFilterVM
{
    public string? TuKhoa { get; set; }            
    public int? MaKhuVuc { get; set; }
    public TrangThaiYeuCau? TrangThai { get; set; }
    public int? MaNhanVien { get; set; }
    public DateTime? TuNgay { get; set; }
    public DateTime? DenNgay { get; set; }
    public int Page { get; set; } = 1;
}

public class YeuCauRowVM
{
    public int MaYeuCau { get; set; }
    public string NguoiDan { get; set; } = "";
    public string KhuVuc { get; set; } = "";
    public string? NhanVien { get; set; }
    public DateTime NgayMongMuon { get; set; }
    public TrangThaiYeuCau TrangThai { get; set; }
}

public class TheoDoiYeuCauVM
{
    public YeuCauFilterVM Filter { get; set; } = new();
    public PagedList<YeuCauRowVM> Data { get; set; } = new();
    public List<SelectListItem> KhuVucs { get; set; } = new();
    public List<SelectListItem> NhanViens { get; set; } = new();
    public List<SelectListItem> TrangThais { get; set; } = new();
}

public class YeuCauChiTietVM
{
    public YeuCauThuGom YeuCau { get; set; } = null!;
    public PhanCongThuGom? PhanCong { get; set; }
}

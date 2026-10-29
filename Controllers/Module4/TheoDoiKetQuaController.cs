// Họ và tên: Vũ Đức Hải
// Mã sinh viên: 23103100008
// Nội dung thực hiện: Module 4 - Theo dõi kết quả thu gom
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Filters;
using QLThuGomRac_UNETI02_TI17A1HN.ViewModels;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers;

[AdminOnly]
public class TheoDoiKetQuaController : Controller
{
    private const int PageSize = 15;
    private readonly ApplicationDbContext _db;
    public TheoDoiKetQuaController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(YeuCauFilterVM filter)
    {
        // Mỗi dòng = một loại rác trong kết quả của một yêu cầu
        var query = _db.ChiTietKetQuaThuGoms.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
        {
            var kw = filter.TuKhoa.Trim();
            if (int.TryParse(kw, out var ma))
                query = query.Where(c => c.KetQuaThuGom!.MaYeuCau == ma
                                      || c.KetQuaThuGom!.YeuCauThuGom!.NguoiDan.HoTen.Contains(kw));
            else
                query = query.Where(c => c.KetQuaThuGom!.YeuCauThuGom!.NguoiDan.HoTen.Contains(kw));
        }
        if (filter.MaKhuVuc.HasValue)
            query = query.Where(c => c.KetQuaThuGom!.YeuCauThuGom!.MaKhuVuc == filter.MaKhuVuc);
        if (filter.MaNhanVien.HasValue)
            query = query.Where(c => c.KetQuaThuGom!.MaNhanVien == filter.MaNhanVien);
        if (filter.TuNgay.HasValue)
            query = query.Where(c => c.KetQuaThuGom!.ThoiGianHoanThanh >= filter.TuNgay.Value.Date);
        if (filter.DenNgay.HasValue)
            query = query.Where(c => c.KetQuaThuGom!.ThoiGianHoanThanh < filter.DenNgay.Value.Date.AddDays(1));

        var total = await query.CountAsync();
        var page = Math.Max(1, filter.Page);

        var items = await query
            .OrderByDescending(c => c.KetQuaThuGom!.ThoiGianHoanThanh)
            .Skip((page - 1) * PageSize).Take(PageSize)
            .Select(c => new KetQuaRowVM
            {
                MaYeuCau = c.KetQuaThuGom!.MaYeuCau,
                NguoiDan = c.KetQuaThuGom!.YeuCauThuGom!.NguoiDan.HoTen,
                KhuVuc = c.KetQuaThuGom!.YeuCauThuGom!.KhuVuc.TenKhuVuc,
                NhanVien = c.KetQuaThuGom!.NhanVien!.HoTen,
                NgayThuGom = c.KetQuaThuGom!.ThoiGianHoanThanh,
                LoaiRac = c.LoaiRac!.TenLoaiRac,
                KhoiLuong = c.KhoiLuongThucTe
            }).ToListAsync();

        // Tổng khối lượng theo loại rác (theo đúng bộ lọc hiện tại)
        var tongTheoLoai = await query
            .GroupBy(c => new { c.LoaiRac!.TenLoaiRac, c.LoaiRac!.DonViTinh })
            .Select(g => new LoaiRacKhoiLuongVM
            {
                LoaiRac = g.Key.TenLoaiRac,
                DonViTinh = g.Key.DonViTinh,
                KhoiLuong = g.Sum(c => c.KhoiLuongThucTe)
            })
            .OrderByDescending(x => x.KhoiLuong).ToListAsync();

        filter.Page = page;
        var vm = new TheoDoiKetQuaVM
        {
            Filter = filter,
            Data = new PagedList<KetQuaRowVM> { Items = items, Page = page, PageSize = PageSize, TotalItems = total },
            TongTheoLoai = tongTheoLoai,
            KhuVucs = await _db.KhuVucs.OrderBy(k => k.TenKhuVuc)
                .Select(k => new SelectListItem(k.TenKhuVuc, k.MaKhuVuc.ToString(), k.MaKhuVuc == filter.MaKhuVuc))
                .ToListAsync(),
            NhanViens = await _db.NhanViens.OrderBy(n => n.HoTen)
                .Select(n => new SelectListItem(n.HoTen, n.MaNhanVien.ToString(), n.MaNhanVien == filter.MaNhanVien))
                .ToListAsync()
        };
        return View(vm);
    }
}

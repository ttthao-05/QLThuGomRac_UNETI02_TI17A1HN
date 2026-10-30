// Họ và tên: [Vũ Đức Hải]
// Mã sinh viên: [23103100008]
// Nội dung thực hiện: Module 4 - Theo dõi yêu cầu thu gom (tìm kiếm, lọc, phân trang)
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Controllers.Admin;
using QLThuGomRac_UNETI02_TI17A1HN.Helpers;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;
using QLThuGomRac_UNETI02_TI17A1HN.ViewModels;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers;

[AdminOnly]
public class TheoDoiYeuCauController : Controller
{
    private const int PageSize = 10;
    private readonly ApplicationDbContext _db;
    public TheoDoiYeuCauController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(YeuCauFilterVM filter)
    {
        var query = _db.YeuCauThuGoms.AsNoTracking().AsQueryable();

        // Tìm theo mã yêu cầu hoặc tên người dân
        if (!string.IsNullOrWhiteSpace(filter.TuKhoa))
        {
            var kw = filter.TuKhoa.Trim();
            if (int.TryParse(kw, out var ma))
                query = query.Where(x => x.MaYeuCau == ma || x.NguoiDan.HoTen.Contains(kw));
            else
                query = query.Where(x => x.NguoiDan.HoTen.Contains(kw));
        }
        if (filter.MaKhuVuc.HasValue)
            query = query.Where(x => x.MaKhuVuc == filter.MaKhuVuc);
        if (filter.TrangThai.HasValue)
            query = query.Where(x => x.TrangThai == filter.TrangThai.Value);
        // YeuCauThuGom không có navigation tới phân công -> dùng truy vấn con trên bảng PhanCongThuGom
        if (filter.MaNhanVien.HasValue)
            query = query.Where(x => _db.PhanCongThuGoms
                .Any(p => p.MaYeuCau == x.MaYeuCau && p.MaNhanVien == filter.MaNhanVien));
        if (filter.TuNgay.HasValue)
            query = query.Where(x => x.NgayMongMuonThuGom >= filter.TuNgay.Value.Date);
        if (filter.DenNgay.HasValue)
            query = query.Where(x => x.NgayMongMuonThuGom < filter.DenNgay.Value.Date.AddDays(1));

        var total = await query.CountAsync();
        var page = Math.Max(1, filter.Page);

        var items = await query
            .OrderByDescending(x => x.NgayDangKy)
            .Skip((page - 1) * PageSize).Take(PageSize)
            .Select(x => new YeuCauRowVM
            {
                MaYeuCau = x.MaYeuCau,
                NguoiDan = x.NguoiDan.HoTen,
                KhuVuc = x.KhuVuc.TenKhuVuc,
                NhanVien = _db.PhanCongThuGoms
                    .Where(p => p.MaYeuCau == x.MaYeuCau)
                    .Select(p => p.NhanVien!.HoTen).FirstOrDefault(),
                NgayMongMuon = x.NgayMongMuonThuGom,
                TrangThai = x.TrangThai
            }).ToListAsync();

        filter.Page = page;
        var vm = new TheoDoiYeuCauVM
        {
            Filter = filter,
            Data = new PagedList<YeuCauRowVM> { Items = items, Page = page, PageSize = PageSize, TotalItems = total },
            KhuVucs = await _db.KhuVucs.OrderBy(k => k.TenKhuVuc)
                .Select(k => new SelectListItem(k.TenKhuVuc, k.MaKhuVuc.ToString(), k.MaKhuVuc == filter.MaKhuVuc))
                .ToListAsync(),
            NhanViens = await _db.NhanViens.OrderBy(n => n.HoTen)
                .Select(n => new SelectListItem(n.HoTen, n.MaNhanVien.ToString(), n.MaNhanVien == filter.MaNhanVien))
                .ToListAsync(),
            TrangThais = Enum.GetValues<TrangThaiYeuCau>()
                .Select(t => new SelectListItem(t.TenHienThi(), ((int)t).ToString(), t == filter.TrangThai))
                .ToList()
        };
        return View(vm);
    }

    public async Task<IActionResult> Details(int id)
    {
        var yc = await _db.YeuCauThuGoms.AsNoTracking()
            .Include(x => x.NguoiDan).Include(x => x.KhuVuc)
            .Include(x => x.ChiTietYeuCaus).ThenInclude(c => c.LoaiRac)
            .FirstOrDefaultAsync(x => x.MaYeuCau == id);
        if (yc == null) return NotFound();

        var phanCong = await _db.PhanCongThuGoms.AsNoTracking()
            .Include(p => p.NhanVien)
            .FirstOrDefaultAsync(p => p.MaYeuCau == id);

        return View(new YeuCauChiTietVM { YeuCau = yc, PhanCong = phanCong });
    }
}

// Họ và tên: Nguyễn Tiến Đạt
// Mã sinh viên: 23103100053
// Nội dung: Module 3 – Xử lý yêu cầu thu gom (7.3)

using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;
using QLThuGomRac_UNETI02_TI17A1HN.Models.ViewModels.Module3;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers.Module3;

public class CongViecController : Controller
{
    private readonly ApplicationDbContext _context;

    public CongViecController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Lấy nhân viên từ tài khoản đăng nhập.
    // Cả hồ sơ nhân viên và tài khoản phải đang hoạt động.
    private async Task<int?> MaNhanVienAsync()
    {
        int? maTaiKhoan =
            HttpContext.Session.GetInt32("MaTaiKhoan");

        if (!maTaiKhoan.HasValue)
            return null;

        return await _context.NhanViens
            .Where(n =>
                n.MaTaiKhoan == maTaiKhoan.Value
                && n.TrangThai == TrangThaiNhanVien.DangHoatDong
                && _context.TaiKhoans.Any(t =>
                    t.MaTaiKhoan == n.MaTaiKhoan
                    && t.LoaiTaiKhoan == LoaiTaiKhoan.NhanVien
                    && t.TrangThai ==
                        TrangThaiTaiKhoan.DangHoatDong))
            .Select(n => (int?)n.MaNhanVien)
            .SingleOrDefaultAsync();
    }

    private IActionResult TuChoiTruyCap()
    {
        if (HttpContext.Session.GetInt32("MaTaiKhoan") == null)
            return RedirectToAction("Login", "Account");

        return StatusCode(
            403,
            "Bạn cần tài khoản và hồ sơ nhân viên đang hoạt động.");
    }

    private IQueryable<PhanCongThuGom> DanhSach()
    {
        return _context.PhanCongThuGoms
            .AsNoTracking()
            .Include(p => p.NhanVien)
            .Include(p => p.YeuCauThuGom)
                .ThenInclude(y => y!.KhuVuc);
    }

    // Nhân viên chỉ xem công việc của mình.
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var maNhanVien = await MaNhanVienAsync();

        if (maNhanVien == null)
            return TuChoiTruyCap();

        var congViecs = await DanhSach()
            .Where(p => p.MaNhanVien == maNhanVien.Value)
            .OrderBy(p => p.YeuCauThuGom!.NgayMongMuonThuGom)
            .ThenBy(p => p.MaPhanCong)
            .ToListAsync();

        return View(congViecs);
    }

    // Admin theo dõi trạng thái các công việc.
    [HttpGet]
    public async Task<IActionResult> TheoDoi()
    {
        int? maTaiKhoan =
            HttpContext.Session.GetInt32("MaTaiKhoan");

        if (!maTaiKhoan.HasValue)
            return RedirectToAction("Login", "Account");

        bool laAdmin = await _context.TaiKhoans.AnyAsync(t =>
            t.MaTaiKhoan == maTaiKhoan.Value
            && t.LoaiTaiKhoan == LoaiTaiKhoan.Admin
            && t.TrangThai == TrangThaiTaiKhoan.DangHoatDong);

        if (!laAdmin)
            return StatusCode(403, "Chỉ Admin được theo dõi công việc.");

        var congViecs = await DanhSach()
            .OrderByDescending(p => p.NgayPhanCong)
            .ToListAsync();

        return View(congViecs);
    }

    // Chi tiết công việc và các loại rác dự kiến.
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var maNhanVien = await MaNhanVienAsync();

        if (maNhanVien == null)
            return TuChoiTruyCap();

        var congViec = await DanhSach()
            .Include(p => p.YeuCauThuGom)
                .ThenInclude(y => y!.ChiTietYeuCaus)
                .ThenInclude(c => c.LoaiRac)
            .SingleOrDefaultAsync(p =>
                p.MaPhanCong == id
                && p.MaNhanVien == maNhanVien.Value);

        if (congViec == null)
            return NotFound();

        return View(congViec);
    }

    // Đã phân công → Đang thu gom.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> BatDau(int id)
    {
        return ChuyenTrangThaiAsync(
            id,
            TrangThaiYeuCau.DaPhanCong,
            TrangThaiYeuCau.DangThuGom,
            null);
    }

    // Đang thu gom → Hoàn thành.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> HoanThanh(int id)
    {
        return ChuyenTrangThaiAsync(
            id,
            TrangThaiYeuCau.DangThuGom,
            TrangThaiYeuCau.HoanThanh,
            null);
    }

    // Mở form nhập lý do không thực hiện được.
    [HttpGet]
    public async Task<IActionResult> KhongThucHien(int id)
    {
        var maNhanVien = await MaNhanVienAsync();

        if (maNhanVien == null)
            return TuChoiTruyCap();

        bool hopLe = await _context.PhanCongThuGoms.AnyAsync(p =>
            p.MaPhanCong == id
            && p.MaNhanVien == maNhanVien.Value
            && p.TrangThai == TrangThaiYeuCau.DaPhanCong
            && p.YeuCauThuGom!.TrangThai ==
                TrangThaiYeuCau.DaPhanCong);

        if (!hopLe)
            return NotFound();

        return View(new KhongThucHienViewModel
        {
            MaPhanCong = id
        });
    }

    // Lưu lý do không thực hiện được.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KhongThucHien(
        KhongThucHienViewModel model)
    {
        var maNhanVien = await MaNhanVienAsync();

        if (maNhanVien == null)
            return TuChoiTruyCap();

        bool laCongViecCuaMinh =
            await _context.PhanCongThuGoms.AnyAsync(p =>
                p.MaPhanCong == model.MaPhanCong
                && p.MaNhanVien == maNhanVien.Value);

        if (!laCongViecCuaMinh)
            return NotFound();

        model.LyDoKhongThucHien =
            model.LyDoKhongThucHien?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(model.LyDoKhongThucHien))
        {
            ModelState.AddModelError(
                nameof(model.LyDoKhongThucHien),
                "Vui lòng nhập lý do, không chỉ nhập khoảng trắng.");
        }

        if (!ModelState.IsValid)
            return View(model);

        return await ChuyenTrangThaiAsync(
            model.MaPhanCong,
            TrangThaiYeuCau.DaPhanCong,
            TrangThaiYeuCau.KhongThucHienDuoc,
            model.LyDoKhongThucHien);
    }

    // Kiểm tra quyền và trạng thái, sau đó cập nhật cả hai bảng.
    private async Task<IActionResult> ChuyenTrangThaiAsync(
        int id,
        TrangThaiYeuCau trangThaiCu,
        TrangThaiYeuCau trangThaiMoi,
        string? lyDo)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            var maNhanVien = await MaNhanVienAsync();

            if (maNhanVien == null)
                return TuChoiTruyCap();

            var congViec = await _context.PhanCongThuGoms
                .Include(p => p.YeuCauThuGom)
                .SingleOrDefaultAsync(p =>
                    p.MaPhanCong == id
                    && p.MaNhanVien == maNhanVien.Value);

            if (congViec == null)
                return NotFound();

            if (congViec.YeuCauThuGom == null
                || congViec.TrangThai != trangThaiCu
                || congViec.YeuCauThuGom.TrangThai != trangThaiCu)
            {
                TempData["Loi"] =
                    "Trạng thái đã thay đổi. Vui lòng tải lại công việc.";

                return RedirectToAction(nameof(Details), new { id });
            }

            congViec.TrangThai = trangThaiMoi;
            congViec.YeuCauThuGom.TrangThai = trangThaiMoi;

            if (trangThaiMoi == TrangThaiYeuCau.DangThuGom)
            {
                congViec.NgayBatDau = DateTime.Now;
            }
            else
            {
                congViec.NgayKetThuc = DateTime.Now;
            }

            if (trangThaiMoi == TrangThaiYeuCau.KhongThucHienDuoc)
            {
                congViec.LyDoKhongThucHien = lyDo;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["ThongBao"] = "Đã cập nhật trạng thái công việc.";

            return RedirectToAction(nameof(Details), new { id });
        }
        catch (SqlException ex) when (ex.Number == 1205)
        {
            await transaction.RollbackAsync();

            TempData["Loi"] =
                "Có thao tác đồng thời. Vui lòng tải lại và thử lại.";

            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is SqlException sql
                && sql.Number == 1205)
        {
            await transaction.RollbackAsync();

            TempData["Loi"] =
                "Có thao tác đồng thời. Vui lòng tải lại và thử lại.";

            return RedirectToAction(nameof(Index));
        }
    }
}   

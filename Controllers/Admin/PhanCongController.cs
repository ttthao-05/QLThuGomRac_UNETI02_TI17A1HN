// Họ và tên: Nguyễn Tiến Đạt
// Mã sinh viên: 23103100053
// Nội dung: Module 3 – Phân công thu gom (7.2)

using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;
using QLThuGomRac_UNETI02_TI17A1HN.Models.ViewModels.Module3;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers.Admin;

[AdminOnly]
public class PhanCongController : Controller
{
    private readonly ApplicationDbContext _context;

    public PhanCongController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Kiểm tra tài khoản Admin hiện tại trong database.
    // Vẫn chặn truy cập nếu AdminOnly đang bật cờ bỏ qua đăng nhập.
    private async Task<bool> LaAdminAsync()
    {
        int? maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");

        return maTaiKhoan.HasValue
            && await _context.TaiKhoans.AnyAsync(x =>
                x.MaTaiKhoan == maTaiKhoan.Value
                && x.LoaiTaiKhoan == LoaiTaiKhoan.Admin
                && x.TrangThai == TrangThaiTaiKhoan.DangHoatDong);
    }

    private IQueryable<NhanVien> NhanVienHopLe(int maKhuVuc)
    {
        return _context.NhanViens.Where(x =>
            x.MaKhuVuc == maKhuVuc
            && x.TrangThai == TrangThaiNhanVien.DangHoatDong
            && _context.TaiKhoans.Any(t =>
                t.MaTaiKhoan == x.MaTaiKhoan
                && t.LoaiTaiKhoan == LoaiTaiKhoan.NhanVien
                && t.TrangThai == TrangThaiTaiKhoan.DangHoatDong));
    }

    private async Task NapFormAsync(
        PhanCongCreateViewModel model,
        YeuCauThuGom yeuCau)
    {
        model.DiaChiThuGom = yeuCau.DiaChiThuGom;

        model.TenKhuVuc = await _context.KhuVucs
            .Where(x => x.MaKhuVuc == yeuCau.MaKhuVuc)
            .Select(x => x.TenKhuVuc)
            .FirstOrDefaultAsync();

        model.NhanViens = await NhanVienHopLe(yeuCau.MaKhuVuc)
            .OrderBy(x => x.HoTen)
            .Select(x => new SelectListItem
            {
                Value = x.MaNhanVien.ToString(),
                Text = x.HoTen + " - " + x.SoDienThoai
            })
            .ToListAsync();
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!await LaAdminAsync())
            return StatusCode(403, "Bạn cần đăng nhập bằng tài khoản Admin.");

        // Chỉ hiển thị yêu cầu đã xác nhận và chưa có phân công.
        var yeuCaus = await _context.YeuCauThuGoms
            .AsNoTracking()
            .Include(x => x.KhuVuc)
            .Where(x =>
                x.TrangThai == TrangThaiYeuCau.DaXacNhan
                && !_context.PhanCongThuGoms.Any(p =>
                    p.MaYeuCau == x.MaYeuCau))
            .OrderBy(x => x.NgayMongMuonThuGom)
            .ToListAsync();
        ViewBag.YeuCauChoXacNhan = await _context.YeuCauThuGoms
            .AsNoTracking()
            .Include(x => x.KhuVuc)
            .Where(x => x.TrangThai == TrangThaiYeuCau.ChoXacNhan)
            .OrderBy(x => x.NgayDangKy)
            .ToListAsync();
        return View(yeuCaus);
    }

    // Xác nhận yêu cầu trước khi phân công nhân viên.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XacNhan(int id)
    {
        if (!await LaAdminAsync())
            return StatusCode(
                403,
                "Bạn cần đăng nhập bằng tài khoản Admin.");

        int soDong = await _context.YeuCauThuGoms
            .Where(x =>
                x.MaYeuCau == id
                && x.TrangThai == TrangThaiYeuCau.ChoXacNhan)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(
                    x => x.TrangThai,
                    TrangThaiYeuCau.DaXacNhan));

        if (soDong == 1)
        {
            TempData["ThongBao"] =
                $"Đã xác nhận yêu cầu #{id}. Bạn có thể phân công nhân viên.";
        }
        else
        {
            TempData["Loi"] =
                "Yêu cầu không tồn tại hoặc không còn chờ xác nhận.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Create(int id)
    {
        if (!await LaAdminAsync())
            return StatusCode(403, "Bạn cần đăng nhập bằng tài khoản Admin.");

        var yeuCau = await _context.YeuCauThuGoms
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaYeuCau == id);

        if (yeuCau == null)
            return NotFound();

        if (yeuCau.TrangThai != TrangThaiYeuCau.DaXacNhan
            || await _context.PhanCongThuGoms.AnyAsync(x =>
                x.MaYeuCau == id))
        {
            TempData["Loi"] = "Yêu cầu chưa được xác nhận hoặc đã có phân công.";
            return RedirectToAction(nameof(Index));
        }

        var model = new PhanCongCreateViewModel
        {
            MaYeuCau = id
        };

        await NapFormAsync(model, yeuCau);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        PhanCongCreateViewModel model)
    {
        if (!await LaAdminAsync())
            return StatusCode(403, "Bạn cần đăng nhập bằng tài khoản Admin.");

        // Khóa việc kiểm tra và lưu trong cùng một transaction,
        // tránh hai Admin phân công đồng thời cùng một yêu cầu.
        await using var transaction = await _context.Database
            .BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            var yeuCau = await _context.YeuCauThuGoms
                .FirstOrDefaultAsync(x =>
                    x.MaYeuCau == model.MaYeuCau);

            if (yeuCau == null)
                return NotFound();

            bool daPhanCong = await _context.PhanCongThuGoms
                .AnyAsync(x => x.MaYeuCau == model.MaYeuCau);

            if (yeuCau.TrangThai != TrangThaiYeuCau.DaXacNhan
                || daPhanCong)
            {
                TempData["Loi"] =
                    "Yêu cầu chưa được xác nhận hoặc đã có phân công.";

                return RedirectToAction(nameof(Index));
            }

            bool nhanVienHopLe = await NhanVienHopLe(yeuCau.MaKhuVuc)
                .AnyAsync(x => x.MaNhanVien == model.MaNhanVien);

            if (!nhanVienHopLe)
            {
                ModelState.AddModelError(
                    nameof(model.MaNhanVien),
                    "Nhân viên phải đang hoạt động, đúng khu vực "
                    + "và có tài khoản nhân viên đang hoạt động.");
            }

            if (!ModelState.IsValid)
            {
                await NapFormAsync(model, yeuCau);
                return View(model);
            }

            var phanCong = new PhanCongThuGom
            {
                MaYeuCau = yeuCau.MaYeuCau,
                MaNhanVien = model.MaNhanVien,
                NgayPhanCong = DateTime.Now,
                GhiChu = model.GhiChu?.Trim(),
                TrangThai = TrangThaiYeuCau.DaPhanCong
            };

            _context.PhanCongThuGoms.Add(phanCong);

            // Cập nhật đồng thời trạng thái yêu cầu.
            yeuCau.TrangThai = TrangThaiYeuCau.DaPhanCong;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["ThongBao"] = "Phân công nhân viên thành công.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is SqlException sql
                && (sql.Number == 2601
                    || sql.Number == 2627
                    || sql.Number == 1205
                    || sql.Number == 547))
        {
            await transaction.RollbackAsync();

            TempData["Loi"] =
                "Dữ liệu đã thay đổi hoặc yêu cầu đã được phân công. "
                + "Vui lòng tải lại trang và thử lại.";

            return RedirectToAction(nameof(Index));
        }
        catch (SqlException ex) when (ex.Number == 1205)
        {
            await transaction.RollbackAsync();

            TempData["Loi"] =
                "Có thao tác đồng thời. Vui lòng thử phân công lại.";

            return RedirectToAction(nameof(Index));
        }
    }
}

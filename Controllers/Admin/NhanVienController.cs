// Họ và tên: Nguyễn Tiến Đạt
// Mã sinh viên: 23103100053
// Nội dung: Module 3 – Danh sách và tìm kiếm nhân viên

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Models.ViewModels.Module3;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers.Admin;

[AdminOnly]
public class NhanVienController : Controller
{
    private readonly ApplicationDbContext _context;

    public NhanVienController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? tuKhoa)
    {
        var query =
            from nv in _context.NhanViens.AsNoTracking()
            join tk in _context.TaiKhoans
                on nv.MaTaiKhoan equals tk.MaTaiKhoan
            join kv in _context.KhuVucs
                on nv.MaKhuVuc equals kv.MaKhuVuc
            select new NhanVienDanhSachViewModel
            {
                MaNhanVien = nv.MaNhanVien,
                TenDangNhap = tk.TenDangNhap,
                HoTen = nv.HoTen,
                SoDienThoai = nv.SoDienThoai,
                TenKhuVuc = kv.TenKhuVuc,
                NgayVaoLam = nv.NgayVaoLam,
                TrangThai = nv.TrangThai
            };

        tuKhoa = tuKhoa?.Trim();

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            query = query.Where(x =>
                x.HoTen.Contains(tuKhoa) ||
                x.SoDienThoai.Contains(tuKhoa) ||
                x.TenDangNhap.Contains(tuKhoa) ||
                x.TenKhuVuc.Contains(tuKhoa));
        }

        ViewData["TuKhoa"] = tuKhoa;

        var danhSach = await query
            .OrderBy(x => x.HoTen)
            .ThenBy(x => x.MaNhanVien)
            .ToListAsync();

        return View(danhSach);
    }
    // Mở form thêm nhân viên.
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new NhanVienCreateViewModel();

        await NapDanhSachChonAsync(model);

        return View(model);
    }

    // Nhận dữ liệu và lưu hồ sơ nhân viên.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NhanVienCreateViewModel model)
    {
        model.HoTen = model.HoTen?.Trim() ?? string.Empty;
        model.SoDienThoai = model.SoDienThoai?.Trim() ?? string.Empty;

        ModelState.Clear();
        TryValidateModel(model);

        var taiKhoanHopLe = await _context.TaiKhoans.AnyAsync(x =>
            x.MaTaiKhoan == model.MaTaiKhoan &&
            x.LoaiTaiKhoan == LoaiTaiKhoan.NhanVien &&
            x.TrangThai == TrangThaiTaiKhoan.DangHoatDong);

        if (!taiKhoanHopLe)
        {
            ModelState.AddModelError(
                nameof(model.MaTaiKhoan),
                "Phải chọn tài khoản nhân viên đang hoạt động.");
        }

        var daCoHoSo = await _context.NhanViens.AnyAsync(x =>
            x.MaTaiKhoan == model.MaTaiKhoan);

        if (daCoHoSo)
        {
            ModelState.AddModelError(
                nameof(model.MaTaiKhoan),
                "Tài khoản này đã có hồ sơ nhân viên.");
        }

        var khuVucHopLe = await _context.KhuVucs.AnyAsync(x =>
            x.MaKhuVuc == model.MaKhuVuc &&
            x.TrangThai == TrangThaiKhuVuc.DangHoatDong);

        if (!khuVucHopLe)
        {
            ModelState.AddModelError(
                nameof(model.MaKhuVuc),
                "Phải chọn khu vực đang hoạt động.");
        }

        if (model.NgayVaoLam.HasValue &&
            model.NgayVaoLam.Value.Year < 2000)
        {
            ModelState.AddModelError(
                nameof(model.NgayVaoLam),
                "Ngày vào làm phải từ năm 2000 trở đi.");
        }

        if (ModelState.IsValid)
        {
            var nhanVien = new NhanVien
            {
                MaTaiKhoan = model.MaTaiKhoan,
                HoTen = model.HoTen,
                SoDienThoai = model.SoDienThoai,
                MaKhuVuc = model.MaKhuVuc,
                NgayVaoLam = model.NgayVaoLam!.Value.Date,
                TrangThai = model.TrangThai
            };

            _context.NhanViens.Add(nhanVien);

            try
            {
                await _context.SaveChangesAsync();

                TempData["ThongBao"] = "Thêm nhân viên thành công.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is SqlException sql &&
                      (sql.Number == 2601 || sql.Number == 2627))
            {
                _context.Entry(nhanVien).State = EntityState.Detached;

                ModelState.AddModelError(
                    nameof(model.MaTaiKhoan),
                    "Tài khoản này đã được tạo hồ sơ. Vui lòng chọn tài khoản khác.");
            }
        }

        // Khi nhập sai, nạp lại danh sách để form vẫn hiển thị.
        await NapDanhSachChonAsync(model);

        return View(model);
    }

    // Chỉ lấy tài khoản nhân viên hoạt động, chưa có hồ sơ;
    // và khu vực đang hoạt động.
    private async Task NapDanhSachChonAsync(NhanVienCreateViewModel model)
    {
        model.TaiKhoans = await _context.TaiKhoans
            .AsNoTracking()
            .Where(x =>
                x.LoaiTaiKhoan == LoaiTaiKhoan.NhanVien &&
                x.TrangThai == TrangThaiTaiKhoan.DangHoatDong &&
                !_context.NhanViens.Any(nv =>
                    nv.MaTaiKhoan == x.MaTaiKhoan))
            .OrderBy(x => x.TenDangNhap)
            .Select(x => new SelectListItem
            {
                Value = x.MaTaiKhoan.ToString(),
                Text = x.TenDangNhap + " - " + x.HoTen
            })
            .ToListAsync();

        model.KhuVucs = await _context.KhuVucs
            .AsNoTracking()
            .Where(x => x.TrangThai == TrangThaiKhuVuc.DangHoatDong)
            .OrderBy(x => x.TenKhuVuc)
            .Select(x => new SelectListItem
            {
                Value = x.MaKhuVuc.ToString(),
                Text = x.TenKhuVuc
            })
            .ToListAsync();
    }
}

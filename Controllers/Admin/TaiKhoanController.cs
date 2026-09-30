// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung thực hiện: Module 1 – Quản lý tài khoản (§5.1 Quản lý tài khoản)

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;
using QLThuGomRac_UNETI02_TI17A1HN.Models.ViewModels.Module1;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers.Admin;

/// <summary>
/// §5.1 Quản lý tài khoản: xem danh sách, xem chi tiết, thêm, sửa, khóa/mở khóa tài khoản.
/// Mọi action đều được kiểm tra quyền qua <see cref="AdminOnlyAttribute"/> (chỉ Admin được dùng).
/// </summary>
[AdminOnly]
public class TaiKhoanController : Controller
{
    private readonly ApplicationDbContext _context;

    public TaiKhoanController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /TaiKhoan
    [HttpGet]
    public async Task<IActionResult> Index(string? tuKhoa, LoaiTaiKhoan? loaiTaiKhoan, TrangThaiTaiKhoan? trangThai)
    {
        IQueryable<TaiKhoan> query = _context.TaiKhoans.AsNoTracking();

        // Tìm kiếm theo tên đăng nhập hoặc họ tên.
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            string tuKhoaTrim = tuKhoa.Trim();
            query = query.Where(x =>
                x.TenDangNhap.Contains(tuKhoaTrim) || x.HoTen.Contains(tuKhoaTrim));
        }

        // Lọc theo loại tài khoản.
        if (loaiTaiKhoan.HasValue)
        {
            query = query.Where(x => x.LoaiTaiKhoan == loaiTaiKhoan.Value);
        }

        // Lọc theo trạng thái.
        if (trangThai.HasValue)
        {
            query = query.Where(x => x.TrangThai == trangThai.Value);
        }

        var model = new TaiKhoanIndexViewModel
        {
            TuKhoa = tuKhoa,
            LoaiTaiKhoan = loaiTaiKhoan,
            TrangThai = trangThai,
            DanhSach = await query.OrderBy(x => x.MaTaiKhoan).ToListAsync(),
            LoaiTaiKhoanItems = BuildEnumItems<LoaiTaiKhoan>(loaiTaiKhoan, "-- Tất cả loại tài khoản --"),
            TrangThaiItems = BuildEnumItems<TrangThaiTaiKhoan>(trangThai, "-- Tất cả trạng thái --")
        };

        // Giữ lại điều kiện tìm kiếm/lọc khi chọn lại các mục trong form.
        ViewBag.DaLoc = !string.IsNullOrWhiteSpace(tuKhoa) || loaiTaiKhoan.HasValue || trangThai.HasValue;

        return View(model);
    }

    // GET: /TaiKhoan/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        TaiKhoan? taiKhoan = await _context.TaiKhoans.AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaTaiKhoan == id);

        if (taiKhoan is null)
        {
            return NotFound();
        }

        return View(taiKhoan);
    }

    // GET: /TaiKhoan/Create
    [HttpGet]
    public IActionResult Create()
    {
        var model = new TaiKhoanCreateViewModel
        {
            LoaiTaiKhoanItems = BuildEnumItems<LoaiTaiKhoan>(LoaiTaiKhoan.NguoiDan),
            TrangThaiItems = BuildEnumItems<TrangThaiTaiKhoan>(TrangThaiTaiKhoan.DangHoatDong)
        };

        return View(model);
    }

    // POST: /TaiKhoan/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaiKhoanCreateViewModel model)
    {
        model.TenDangNhap = model.TenDangNhap?.Trim() ?? string.Empty;
        model.HoTen = model.HoTen?.Trim() ?? string.Empty;
        model.SoDienThoai = model.SoDienThoai?.Trim() ?? string.Empty;

        // §5.1: Tên đăng nhập không được trùng.
        bool tenDangNhapTrung = await _context.TaiKhoans
            .AnyAsync(x => x.TenDangNhap == model.TenDangNhap);

        if (tenDangNhapTrung)
        {
            ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã tồn tại, vui lòng chọn tên khác");
        }

        if (!ModelState.IsValid)
        {
            model.LoaiTaiKhoanItems = BuildEnumItems<LoaiTaiKhoan>(model.LoaiTaiKhoan);
            model.TrangThaiItems = BuildEnumItems<TrangThaiTaiKhoan>(model.TrangThai);
            return View(model);
        }

        var taiKhoan = new TaiKhoan
        {
            TenDangNhap = model.TenDangNhap,
            MatKhau = model.MatKhau,
            HoTen = model.HoTen,
            SoDienThoai = model.SoDienThoai,
            LoaiTaiKhoan = model.LoaiTaiKhoan,
            TrangThai = model.TrangThai
        };

        try
        {
            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (LaViPhamTenDangNhapTrung(ex))
        {
            // Hai request gửi cùng tên đăng nhập nên đã vượt qua bước kiểm tra phía trên.
            _context.Entry(taiKhoan).State = EntityState.Detached;
            ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã tồn tại, vui lòng chọn tên khác");
            model.LoaiTaiKhoanItems = BuildEnumItems<LoaiTaiKhoan>(model.LoaiTaiKhoan);
            model.TrangThaiItems = BuildEnumItems<TrangThaiTaiKhoan>(model.TrangThai);
            return View(model);
        }

        TempData["ThongBao"] = $"Đã thêm tài khoản \"{taiKhoan.TenDangNhap}\" thành công.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /TaiKhoan/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        TaiKhoan? taiKhoan = await _context.TaiKhoans.FindAsync(id);
        if (taiKhoan is null)
        {
            return NotFound();
        }

        var model = new TaiKhoanEditViewModel
        {
            MaTaiKhoan = taiKhoan.MaTaiKhoan,
            TenDangNhap = taiKhoan.TenDangNhap,
            HoTen = taiKhoan.HoTen,
            SoDienThoai = taiKhoan.SoDienThoai,
            LoaiTaiKhoan = taiKhoan.LoaiTaiKhoan,
            TrangThai = taiKhoan.TrangThai,
            LoaiTaiKhoanItems = BuildEnumItems<LoaiTaiKhoan>(taiKhoan.LoaiTaiKhoan),
            TrangThaiItems = BuildEnumItems<TrangThaiTaiKhoan>(taiKhoan.TrangThai)
        };

        return View(model);
    }

    // POST: /TaiKhoan/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TaiKhoanEditViewModel model)
    {
        if (id != model.MaTaiKhoan)
        {
            return NotFound();
        }

        model.TenDangNhap = model.TenDangNhap?.Trim() ?? string.Empty;
        model.HoTen = model.HoTen?.Trim() ?? string.Empty;
        model.SoDienThoai = model.SoDienThoai?.Trim() ?? string.Empty;

        // §5.1: Tên đăng nhập không được trùng (loại trừ chính tài khoản đang sửa).
        bool tenDangNhapTrung = await _context.TaiKhoans
            .AnyAsync(x => x.MaTaiKhoan != model.MaTaiKhoan && x.TenDangNhap == model.TenDangNhap);

        if (tenDangNhapTrung)
        {
            ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã tồn tại, vui lòng chọn tên khác");
        }

        if (!ModelState.IsValid)
        {
            model.LoaiTaiKhoanItems = BuildEnumItems<LoaiTaiKhoan>(model.LoaiTaiKhoan);
            model.TrangThaiItems = BuildEnumItems<TrangThaiTaiKhoan>(model.TrangThai);
            return View(model);
        }

        TaiKhoan? taiKhoan = await _context.TaiKhoans.FindAsync(model.MaTaiKhoan);
        if (taiKhoan is null)
        {
            return NotFound();
        }

        taiKhoan.TenDangNhap = model.TenDangNhap;
        taiKhoan.HoTen = model.HoTen;
        taiKhoan.SoDienThoai = model.SoDienThoai;
        taiKhoan.LoaiTaiKhoan = model.LoaiTaiKhoan;
        taiKhoan.TrangThai = model.TrangThai;

        // Mật khẩu để trống thì giữ nguyên mật khẩu cũ.
        if (!string.IsNullOrWhiteSpace(model.MatKhau))
        {
            taiKhoan.MatKhau = model.MatKhau;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (LaViPhamTenDangNhapTrung(ex))
        {
            // Hai request gửi cùng tên đăng nhập nên đã vượt qua bước kiểm tra phía trên.
            ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã tồn tại, vui lòng chọn tên khác");
            model.LoaiTaiKhoanItems = BuildEnumItems<LoaiTaiKhoan>(model.LoaiTaiKhoan);
            model.TrangThaiItems = BuildEnumItems<TrangThaiTaiKhoan>(model.TrangThai);
            return View(model);
        }

        TempData["ThongBao"] = $"Đã cập nhật tài khoản \"{taiKhoan.TenDangNhap}\" thành công.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /TaiKhoan/KhoaMoKhoa/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KhoaMoKhoa(int id)
    {
        TaiKhoan? taiKhoan = await _context.TaiKhoans.FindAsync(id);
        if (taiKhoan is null)
        {
            return NotFound();
        }

        if (taiKhoan.TrangThai == TrangThaiTaiKhoan.DangHoatDong)
        {
            taiKhoan.TrangThai = TrangThaiTaiKhoan.DaKhoa;
            TempData["ThongBao"] = $"Đã khóa tài khoản \"{taiKhoan.TenDangNhap}\".";
        }
        else
        {
            taiKhoan.TrangThai = TrangThaiTaiKhoan.DangHoatDong;
            TempData["ThongBao"] = $"Đã mở khóa tài khoản \"{taiKhoan.TenDangNhap}\".";
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private static bool LaViPhamTenDangNhapTrung(DbUpdateException ex)
    {
        if (ex.InnerException is not Microsoft.Data.SqlClient.SqlException sqlEx)
        {
            return false;
        }

        bool laLoiUnique = sqlEx.Number == 2601 || sqlEx.Number == 2627;
        return laLoiUnique
            && sqlEx.Message.Contains("IX_TaiKhoans_TenDangNhap", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Danh sách select cho Enum: hiển thị tên tiếng Việt từ [Display],
    /// giá trị gửi lên Form là số (int) để Model Binding chuyển về Enum.
    /// </summary>
    /// <param name="selected">Giá trị đang được chọn; null thì chọn mục "Tất cả" (nếu có).</param>
    /// <param name="nhanTatCa">Nhãn của mục để trống, dùng cho bộ lọc.</param>
    private static List<SelectListItem> BuildEnumItems<TEnum>(TEnum? selected, string? nhanTatCa = null)
        where TEnum : struct, Enum
    {
        var items = new List<SelectListItem>();

        if (nhanTatCa is not null)
        {
            items.Add(new SelectListItem
            {
                Value = string.Empty,
                Text = nhanTatCa,
                Selected = !selected.HasValue
            });
        }

        foreach (TEnum giaTri in Enum.GetValues<TEnum>())
        {
            items.Add(new SelectListItem
            {
                Value = Convert.ToInt32(giaTri).ToString(),
                Text = giaTri.GetDisplayName(),
                Selected = selected.HasValue && selected.Value.Equals(giaTri)
            });
        }

        return items;
    }
}

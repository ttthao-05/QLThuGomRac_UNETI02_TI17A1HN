// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung thực hiện: Module 1 – Đăng nhập và đăng xuất (§5.2, §5.3)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;
using QLThuGomRac_UNETI02_TI17A1HN.Models.ViewModels.Module1;
using QLThuGomRac_UNETI02_TI17A1HN.Services;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers.Account;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly MatKhauService _matKhauService;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
        _matKhauService = new MatKhauService();
    }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        // Bước 1: Kiểm tra dữ liệu nhập
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string tenDangNhap = model.TenDangNhap.Trim();

        // Bước 2: Tìm tài khoản
        var taiKhoan = await _context.TaiKhoans
            .FirstOrDefaultAsync(x => x.TenDangNhap == tenDangNhap);

        if (taiKhoan == null)
        {
            ModelState.AddModelError(
                string.Empty,
                "Tên đăng nhập hoặc mật khẩu không đúng.");

            return View(model);
        }

        // Bước 3: Kiểm tra tài khoản bị Admin khóa
        if (taiKhoan.TrangThai != TrangThaiTaiKhoan.DangHoatDong)
        {
            ModelState.AddModelError(
                string.Empty,
                "Tài khoản đang bị khóa.");

            return View(model);
        }

        // Bước 4: Kiểm tra khóa đăng nhập tạm thời
        DateTime hienTai = DateTime.Now;

        if (taiKhoan.KhoaDangNhapDen.HasValue &&
            taiKhoan.KhoaDangNhapDen.Value > hienTai)
        {
            int soGiayConLai = (int)Math.Ceiling(
                (taiKhoan.KhoaDangNhapDen.Value - hienTai).TotalSeconds);

            ModelState.AddModelError(
                string.Empty,
                $"Bạn đã nhập sai mật khẩu 3 lần. Vui lòng thử lại sau {soGiayConLai} giây.");

            return View(model);
        }

        // Bước 5: Nếu hết thời gian khóa thì đặt lại bộ đếm
        if (taiKhoan.KhoaDangNhapDen.HasValue &&
            taiKhoan.KhoaDangNhapDen.Value <= hienTai)
        {
            taiKhoan.SoLanDangNhapSai = 0;
            taiKhoan.KhoaDangNhapDen = null;
        }


        // Bước 6: Xác minh mật khẩu đã băm
        bool matKhauHopLe = false;

        try
        {
            matKhauHopLe = _matKhauService.KiemTraMatKhau(
                taiKhoan, model.MatKhau);
        }
        catch (FormatException)
        {
            matKhauHopLe = false;
        }
        catch (ArgumentException)
        {
            matKhauHopLe = false;
        }


        // Bước 7: Xử lý nhập sai mật khẩu
        if (!matKhauHopLe)
        {
            taiKhoan.SoLanDangNhapSai++;

            if (taiKhoan.SoLanDangNhapSai >= 3)
            {
                taiKhoan.KhoaDangNhapDen =
                    DateTime.Now.AddSeconds(30);

                await _context.SaveChangesAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "Bạn đã nhập sai mật khẩu 3 lần. Tài khoản tạm khóa trong 30 giây.");

                return View(model);
            }

            await _context.SaveChangesAsync();

            int soLanConLai = 3 - taiKhoan.SoLanDangNhapSai;

            ModelState.AddModelError(
                string.Empty,
                $"Tên đăng nhập hoặc mật khẩu không đúng. Bạn còn {soLanConLai} lần thử.");

            return View(model);
        }

        // Bước 8: Đăng nhập đúng
        taiKhoan.SoLanDangNhapSai = 0;
        taiKhoan.KhoaDangNhapDen = null;

        await _context.SaveChangesAsync();

        // Bước 9: Lưu thông tin vào Session
        HttpContext.Session.SetInt32(
            "MaTaiKhoan",
            taiKhoan.MaTaiKhoan);

        HttpContext.Session.SetString(
            "HoTen",
            taiKhoan.HoTen);

        HttpContext.Session.SetString(
            "VaiTro",
            taiKhoan.LoaiTaiKhoan.ToString());

        TempData["ThongBao"] =
            $"Bạn đã đăng nhập thành công. Xin chào {taiKhoan.HoTen}!";

        // Bước 10: Điều hướng theo vai trò
        if (taiKhoan.LoaiTaiKhoan == LoaiTaiKhoan.Admin)
        {
            return RedirectToAction("Index", "QuanTri");
        }

        if (taiKhoan.LoaiTaiKhoan == LoaiTaiKhoan.NguoiDan)
        {
            return RedirectToAction("Index", "Home");
        }

        return RedirectToAction("Index", "CongViec");
    }

    // GET: /Account/Logout
    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        TempData["ThongBao"] = "Đăng xuất thành công.";

        return RedirectToAction("Login", "Account");
    }
}

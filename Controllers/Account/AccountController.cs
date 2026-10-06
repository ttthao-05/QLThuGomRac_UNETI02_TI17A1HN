// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung thực hiện: Module 1 – Đăng nhập và đăng xuất (§5.2, §5.3)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;
using QLThuGomRac_UNETI02_TI17A1HN.Models.ViewModels.Module1;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers.Account;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
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
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string tenDangNhap = model.TenDangNhap.Trim();

        // Dùng EF Core + LINQ để tìm tài khoản.
        var taiKhoan = await _context.TaiKhoans
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.TenDangNhap == tenDangNhap &&
                x.MatKhau == model.MatKhau);

        // Sai tên đăng nhập hoặc mật khẩu.
        if (taiKhoan == null)
        {
            ModelState.AddModelError(
                string.Empty,
                "Tên đăng nhập hoặc mật khẩu không đúng.");

            return View(model);
        }

        // Tài khoản đã bị khóa.
        if (taiKhoan.TrangThai != TrangThaiTaiKhoan.DangHoatDong)
        {
            ModelState.AddModelError(
                string.Empty,
                "Tài khoản đã bị khóa.");

            return View(model);
        }

        // Lưu thông tin đăng nhập vào Session.
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
            $"Đăng nhập thành công. Xin chào {taiKhoan.HoTen}!";

        // Admin vào quản lý tài khoản.
        if (taiKhoan.LoaiTaiKhoan == LoaiTaiKhoan.Admin)
        {
            return RedirectToAction(
                "Index",
                "TaiKhoan");
        }

        if (taiKhoan.LoaiTaiKhoan == LoaiTaiKhoan.NguoiDan)
            return RedirectToAction("Index", "YeuCauThuGom");

        // Nhân viên tạm về trang chủ
        // vì các module tương ứng chưa hoàn thiện.
        return RedirectToAction(
            "Index",
            "Home");
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        TempData["ThongBao"] = "Đăng xuất thành công.";

        return RedirectToAction("Login", "Account");
    }
}

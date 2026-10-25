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

        var taiKhoan = await _context.TaiKhoans
            .FirstOrDefaultAsync(x => x.TenDangNhap == tenDangNhap);

        if (taiKhoan == null)
        {
            ModelState.AddModelError(
                string.Empty,
                "Tên đăng nhập hoặc mật khẩu không đúng.");

            return View(model);
        }

        // Admin khóa tài khoản thì vẫn không được đăng nhập
        if (taiKhoan.TrangThai != TrangThaiTaiKhoan.DangHoatDong)
        {
            ModelState.AddModelError(
                string.Empty,
                "Tài khoản đang bị khóa.");

            return View(model);
        }

        // Đang trong thời gian khóa tạm
        if (taiKhoan.KhoaDangNhapDen.HasValue &&
            taiKhoan.KhoaDangNhapDen.Value > DateTime.Now)
        {
            int soGiayConLai = (int)Math.Ceiling(
                (taiKhoan.KhoaDangNhapDen.Value - DateTime.Now).TotalSeconds);

            ModelState.AddModelError(
                string.Empty,
                $"Bạn đã nhập sai quá 3 lần. Vui lòng thử lại sau {soGiayConLai} giây.");

            return View(model);
        }

        // Hết 30 giây thì tự mở khóa
        if (taiKhoan.KhoaDangNhapDen.HasValue &&
            taiKhoan.KhoaDangNhapDen.Value <= DateTime.Now)
        {
            taiKhoan.KhoaDangNhapDen = null;
            taiKhoan.SoLanDangNhapSai = 0;

            await _context.SaveChangesAsync();
        }

        // Sai mật khẩu
        if (taiKhoan.MatKhau != model.MatKhau)
        {
            taiKhoan.SoLanDangNhapSai++;

            // Sai lần thứ 3
            if (taiKhoan.SoLanDangNhapSai >= 3)
            {
                taiKhoan.KhoaDangNhapDen = DateTime.Now.AddSeconds(30);

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

        // Đăng nhập đúng thì reset bộ đếm
        taiKhoan.SoLanDangNhapSai = 0;
        taiKhoan.KhoaDangNhapDen = null;

        await _context.SaveChangesAsync();

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

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        TempData["ThongBao"] = "Đăng xuất thành công.";

        return RedirectToAction("Login", "Account");
    }
}

// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung thực hiện: Module 1 – Trang chủ phía người dân

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QLThuGomRac_UNETI02_TI17A1HN.Models;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
        var vaiTro = HttpContext.Session.GetString("VaiTro");

        if (maTaiKhoan == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (vaiTro != "NguoiDan")
        {
            return StatusCode(403);
        }

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

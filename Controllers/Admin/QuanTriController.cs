// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung thực hiện: Module 1 – Trang quản trị Admin

using Microsoft.AspNetCore.Mvc;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers.Admin;

[AdminOnly]
public class QuanTriController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

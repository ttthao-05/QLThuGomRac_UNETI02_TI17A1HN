// Họ và tên: Phạm Văn Hào
// Mã sinh viên: 23103100041
// Nội dung thực hiện: Module 2 – Phân quyền người dân (§6.1–§6.5)

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers.Module2;

// Đọc lại tài khoản mỗi request để khóa/đổi vai trò có hiệu lực ngay với session cũ.
public sealed class Module2AccessAttribute(LoaiTaiKhoan role) : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var id = context.HttpContext.Session.GetInt32("MaTaiKhoan");
        if (!id.HasValue)
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }
        var db = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
        var account = await db.TaiKhoans.AsNoTracking().SingleOrDefaultAsync(x => x.MaTaiKhoan == id);
        if (account == null || account.TrangThai != TrangThaiTaiKhoan.DangHoatDong || account.LoaiTaiKhoan != role)
            context.Result = new ContentResult { StatusCode = 403, Content = "Tài khoản không có quyền truy cập hoặc đã bị khóa." };
    }
}

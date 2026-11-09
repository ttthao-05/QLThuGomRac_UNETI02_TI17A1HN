// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// ND: Module 1-Quản lí tài khoản, đăng nhập và phân quyền (§5.1,§5.4)

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers.Admin;

/// <summary>
/// Kiểm tra quyền Admin tại Controller.
/// Kiểm tra Session, vai trò và trạng thái tài khoản trong CSDL.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class AdminOnlyAttribute : Attribute, IAsyncAuthorizationFilter
{
    public const string KhoaVaiTro = "VaiTro";

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        //b1 Lấy thông tin đăng nhập từ Session
        var session = context.HttpContext.Session;

        int? maTaiKhoan = session.GetInt32("MaTaiKhoan");
        string? vaiTro = session.GetString(KhoaVaiTro);

        //b2 Chưa đăng nhập thì chuyển về Login
        if (maTaiKhoan == null || string.IsNullOrWhiteSpace(vaiTro))
        {
            context.Result = new RedirectToActionResult(
                "Login", "Account", null);
            return;
        }

        //b3 Không phải Admin thì từ chối truy cập
        if (!string.Equals(vaiTro,
            LoaiTaiKhoan.Admin.ToString(),
            StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new ContentResult
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Content = "Bạn không có quyền sử dụng chức năng quản trị"
            };
            return;
        }

        // b4 Lấy DbContext để kiểm tra tài khoản
        var db = context.HttpContext.RequestServices
            .GetRequiredService<ApplicationDbContext>();

        var taiKhoan = await db.TaiKhoans
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaTaiKhoan == maTaiKhoan.Value);

        // b5 Kiểm tra tài khoản có tồn tại và đang hoạt động
        if (taiKhoan == null ||
            taiKhoan.TrangThai != TrangThaiTaiKhoan.DangHoatDong)
        {
            session.Clear();

            context.Result = new RedirectToActionResult(
                "Login", "Account", null);
            return;
        }

        //b6 Kiểm tra vai trò thực tế trong CSDL
        if (taiKhoan.LoaiTaiKhoan != LoaiTaiKhoan.Admin)
        {
            session.Clear();

            context.Result = new ContentResult
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Content = "Bạn không có quyền sử dụng chức năng quản trị"
            };
            return;
        }

        // Hợp lệ thì cho phép truy cập Controller
    }
}

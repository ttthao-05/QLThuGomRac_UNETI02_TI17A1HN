// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// ND: Module 1-Quản lí tài khoản, đăng nhập và phân quyền (§5.1,§5.4)

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers.Admin;

/// <summary>
/// Kiểm tra quyền ở phía Controller (§5.1 yêu cầu chỉ Admin được dùng chức năng quản lý tài khoản).
/// Quyền được đọc từ Session theo key <see cref="KhoaVaiTro"/> – chính là "Vai trò" mà §5.2 Đăng nhập lưu vào Session.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class AdminOnlyAttribute : Attribute, IAuthorizationFilter
{
    /// <summary>Key Session chứa vai trò, do §5.2 Đăng nhập ghi vào (giá trị: tên của enum LoaiTaiKhoan).</summary>
    public const string KhoaVaiTro = "VaiTro";

    /// <summary>
    /// Chỉ cho phép tkhoan có vtro Admin truy cập chức năng quản trị
    /// Nếu chưa đnhập thì chuyển về trang đăng nhập
    /// Nếu đã đnhập nhưng k phải Admin thì từ chối truy cập
    /// </summary>
    public static bool ChoPhepChuaDangNhap { get; set; } = false;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        string? vaiTro = context.HttpContext.Session.GetString(KhoaVaiTro);

        if (string.IsNullOrWhiteSpace(vaiTro))
        {
            // Chưa có phiên đăng nhập thì chuyển về trang đăng nhập
            if (ChoPhepChuaDangNhap)
            {
                return;
            }

            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        // Đã đăng nhập nhưng không phải Admin thì chặn ngay tại Controller.
        if (!string.Equals(vaiTro, LoaiTaiKhoan.Admin.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new ContentResult
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Content = "Bạn không có quyền sử dụng chức năng quản trị"
            };
        }
    }
}

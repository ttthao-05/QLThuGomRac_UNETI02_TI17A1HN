// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung thực hiện: Module 1 – Quản lý tài khoản (§5.1 Quản lý tài khoản)

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
    /// TODO (§5.2): hiện Section 5.2 Đăng nhập chưa thực hiện nên chưa có phiên đăng nhập,
    /// đặt cờ này = true để phần còn lại của §5.1 vẫn chạy được.
    /// Sau khi hoàn thành §5.2 và §5.4 thì đặt = false để chỉ Admin mới vào được.
    /// </summary>
    public static bool ChoPhepChuaDangNhap { get; set; } = true;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        string? vaiTro = context.HttpContext.Session.GetString(KhoaVaiTro);

        if (string.IsNullOrWhiteSpace(vaiTro))
        {
            // Chưa có phiên đăng nhập (§5.2 chưa làm).
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
                Content = "Bạn không có quyền sử dụng chức năng quản lý tài khoản."
            };
        }
    }
}

// Họ và tên: [Vũ Đức Hải]
// Mã sinh viên: [23103100008]
// Nội dung thực hiện: Module 4 - Kiểm tra quyền Admin ở Controller
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QLThuGomRac_UNETI02_TI17A1HN.Filters;

// Đọc Session do Module 1 lưu khi đăng nhập.
// TODO: đối chiếu tên key ("VaiTro") và giá trị ("Admin") với controller đăng nhập của Module 1.
// Nếu Module 1 đã có filter phân quyền thì dùng filter đó và xóa file này.
public class AdminOnlyAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var vaiTro = context.HttpContext.Session.GetString("VaiTro");
        if (string.IsNullOrEmpty(vaiTro))
            context.Result = new RedirectToActionResult("Login", "TaiKhoan", null);
        else if (!string.Equals(vaiTro, "Admin", StringComparison.OrdinalIgnoreCase))
            context.Result = new ForbidResult();
    }
}

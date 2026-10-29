// Họ và tên: [Vũ Đức Hải]
// Mã sinh viên: [23103100008]
// Nội dung thực hiện: Module 4 - Tên hiển thị của enum
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace QLThuGomRac_UNETI02_TI17A1HN.Helpers;

public static class EnumExtensions
{
    // Lấy tên từ [Display(Name = "...")] nếu enum có, không thì dùng tên thành viên enum.
    public static string TenHienThi(this Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? value.ToString();
    }
}

// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung thực hiện: Module 1 – Quản lý tài khoản (§5.1 Quản lý tài khoản)

using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

public static class EnumExtensions
{
    /// <summary>
    /// Lấy tên hiển thị tiếng Việt của giá trị Enum từ attribute [Display(Name = "...")].
    /// Nếu không khai báo Display thì trả về tên phần tử của Enum.
    /// </summary>
    public static string GetDisplayName(this Enum value)
    {
        FieldInfo? field = value.GetType().GetField(value.ToString());
        if (field is null)
        {
            return value.ToString();
        }

        DisplayAttribute? attr = field.GetCustomAttribute<DisplayAttribute>(inherit: false);
        return attr?.GetName() ?? value.ToString();
    }
}

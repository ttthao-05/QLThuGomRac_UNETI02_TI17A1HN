// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung: Module 1 – Quản lý loại rác (5.6)

using System.ComponentModel.DataAnnotations;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

public enum TrangThaiLoaiRac
{
    [Display(Name = "Đang phục vụ")]
    DangPhucVu = 1,

    [Display(Name = "Ngừng phục vụ")]
    NgungPhucVu = 2
}

// Họ và tên: [Vũ Đức Hải]
// Mã sinh viên: [23103100008]
// Nội dung thực hiện: Module 4 - Lớp phân trang dùng chung
namespace QLThuGomRac_UNETI02_TI17A1HN.ViewModels;

public class PagedList<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
}

using Microsoft.EntityFrameworkCore;

namespace QLThuGomRac_UNETI02_TI17A1HN.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Khai báo DbSet và cấu hình quan hệ sau khi các Entity được xây dựng.
}

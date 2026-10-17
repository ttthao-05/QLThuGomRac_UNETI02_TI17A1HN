using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLThuGomRac_UNETI02_TI17A1HN.Data.Migrations
{
    /// <inheritdoc />
    public partial class ThemDiemTapKet_Module1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiemTapKets",
                columns: table => new
                {
                    MaDiemTapKet = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDiemTapKet = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaKhuVuc = table.Column<int>(type: "int", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiemTapKets", x => x.MaDiemTapKet);
                    table.ForeignKey(
                        name: "FK_DiemTapKets_KhuVucs_MaKhuVuc",
                        column: x => x.MaKhuVuc,
                        principalTable: "KhuVucs",
                        principalColumn: "MaKhuVuc",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiemTapKets_MaKhuVuc",
                table: "DiemTapKets",
                column: "MaKhuVuc");

            migrationBuilder.CreateIndex(
                name: "IX_DiemTapKets_TenDiemTapKet",
                table: "DiemTapKets",
                column: "TenDiemTapKet",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiemTapKets");
        }
    }
}

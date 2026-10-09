using Microsoft.EntityFrameworkCore.Migrations;

namespace QLThuGomRac_UNETI02_TI17A1HN.Data.Migrations
{
    public partial class TichHopModule1Module2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChiTietYeuCaus",
                columns: table => new
                {
                    MaChiTiet = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    MaYeuCau = table.Column<int>(
                        type: "int",
                        nullable: false),

                    MaLoaiRac = table.Column<int>(
                        type: "int",
                        nullable: false),

                    SoLuongDuKien = table.Column<decimal>(
                        type: "decimal(12,3)",
                        precision: 12,
                        scale: 3,
                        nullable: false),

                    GhiChu = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ChiTietYeuCaus",
                        x => x.MaChiTiet);

                    table.CheckConstraint(
                        "CK_ChiTietYeuCau_SoLuong",
                        "[SoLuongDuKien] > 0");

                    table.ForeignKey(
                        name: "FK_ChiTietYeuCaus_LoaiRacs_MaLoaiRac",
                        column: x => x.MaLoaiRac,
                        principalTable: "LoaiRacs",
                        principalColumn: "MaLoaiRac",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_ChiTietYeuCaus_YeuCauThuGoms_MaYeuCau",
                        column: x => x.MaYeuCau,
                        principalTable: "YeuCauThuGoms",
                        principalColumn: "MaYeuCau",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietYeuCaus_MaLoaiRac",
                table: "ChiTietYeuCaus",
                column: "MaLoaiRac");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietYeuCaus_MaYeuCau_MaLoaiRac",
                table: "ChiTietYeuCaus",
                columns: new[] { "MaYeuCau", "MaLoaiRac" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietYeuCaus");
        }
    }
}

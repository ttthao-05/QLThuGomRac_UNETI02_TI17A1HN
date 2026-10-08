using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLThuGomRac_UNETI02_TI17A1HN.Data.Migrations
{
    public partial class TichHopModule1Module2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // LoaiRacs is created by the preceding migration. Add this FK here
            // because the original Module 2 migration runs before that table
            // exists on a fresh database.
            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietYeuCaus_LoaiRacs_MaLoaiRac",
                table: "ChiTietYeuCaus",
                column: "MaLoaiRac",
                principalTable: "LoaiRacs",
                principalColumn: "MaLoaiRac",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietYeuCaus_LoaiRacs_MaLoaiRac",
                table: "ChiTietYeuCaus");
        }
    }
}

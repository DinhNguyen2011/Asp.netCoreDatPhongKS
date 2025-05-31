using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asp.netCoreDatPhongKS.Migrations
{
    public partial class dn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__NhanVien__TaiKho__5EBF139D",
                table: "NhanVien");

            migrationBuilder.DropForeignKey(
                name: "FK__TaiKhoan__VaiTro__59063A47",
                table: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "HinhAnhPhong");

            migrationBuilder.DropTable(
                name: "PhanQuyen");

            migrationBuilder.RenameColumn(
                name: "TenDangNhap",
                table: "TaiKhoan",
                newName: "Hoten");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "TaiKhoan",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "TaiKhoan",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayTao",
                table: "TaiKhoan",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<decimal>(
                name: "GiaPhong1Dem",
                table: "Phong",
                type: "money",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "Phong",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HinhAnh1",
                table: "Phong",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HinhAnh2",
                table: "Phong",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HinhAnh3",
                table: "Phong",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayTao",
                table: "Phong",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<int>(
                name: "SoLuongKhach",
                table: "Phong",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VnpTransactionId",
                table: "PhieuDatPhong",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CCCD",
                table: "NhanVien",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiaChi",
                table: "NhanVien",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "NhanVien",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayTao",
                table: "NhanVien",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<string>(
                name: "AnhDemo",
                table: "LoaiPhong",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayTao",
                table: "LoaiPhong",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<int>(
                name: "SoluongNguoi",
                table: "LoaiPhong",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayTao",
                table: "KhuyenMai",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<bool>(
                name: "TrangThai",
                table: "KhuyenMai",
                type: "bit",
                nullable: false,
                defaultValueSql: "((1))");

            migrationBuilder.AddColumn<string>(
                name: "CCCD",
                table: "KhachHang",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhiChu",
                table: "KhachHang",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayTao",
                table: "KhachHang",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())");

            migrationBuilder.CreateTable(
                name: "DichVu",
                columns: table => new
                {
                    DichVuId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDichVu = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    NgayTao = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVu", x => x.DichVuId);
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    HoaDonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhieuDatPhongId = table.Column<int>(type: "int", nullable: false),
                    TongTienPhong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TongTienDichVu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GiamGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayLap = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhuongThucThanhToan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDon", x => x.HoaDonId);
                    table.ForeignKey(
                        name: "FK__HoaDon__PhieuDat__3864608B",
                        column: x => x.PhieuDatPhongId,
                        principalTable: "PhieuDatPhong",
                        principalColumn: "PhieuDatPhongID");
                });

            migrationBuilder.CreateTable(
                name: "Quyen",
                columns: table => new
                {
                    QuyenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaQuyen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenQuyen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quyen", x => x.QuyenId);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDichVu",
                columns: table => new
                {
                    ChiTietDichVuId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhieuDatPhongId = table.Column<int>(type: "int", nullable: false),
                    DichVuId = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDichVu", x => x.ChiTietDichVuId);
                    table.ForeignKey(
                        name: "FK__ChiTietDi__DichV__3493CFA7",
                        column: x => x.DichVuId,
                        principalTable: "DichVu",
                        principalColumn: "DichVuId");
                    table.ForeignKey(
                        name: "FK__ChiTietDi__Phieu__339FAB6E",
                        column: x => x.PhieuDatPhongId,
                        principalTable: "PhieuDatPhong",
                        principalColumn: "PhieuDatPhongID");
                });

            migrationBuilder.CreateTable(
                name: "QuyenTaiKhoan",
                columns: table => new
                {
                    TaiKhoanId = table.Column<int>(type: "int", nullable: false),
                    QuyenId = table.Column<int>(type: "int", nullable: false),
                    Mota = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TaiKhoan__4B3B6DA39AADDB30", x => new { x.TaiKhoanId, x.QuyenId });
                    table.ForeignKey(
                        name: "FK__TaiKhoan___Quyen__19DFD96B",
                        column: x => x.QuyenId,
                        principalTable: "Quyen",
                        principalColumn: "QuyenId");
                    table.ForeignKey(
                        name: "FK__TaiKhoan___TaiKh__18EBB532",
                        column: x => x.TaiKhoanId,
                        principalTable: "TaiKhoan",
                        principalColumn: "TaiKhoanID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDichVu_DichVuId",
                table: "ChiTietDichVu",
                column: "DichVuId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDichVu_PhieuDatPhongId",
                table: "ChiTietDichVu",
                column: "PhieuDatPhongId");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_PhieuDatPhongId",
                table: "HoaDon",
                column: "PhieuDatPhongId");

            migrationBuilder.CreateIndex(
                name: "UQ__Quyen__1D4B7ED5D71DC5F4",
                table: "Quyen",
                column: "MaQuyen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuyenTaiKhoan_QuyenId",
                table: "QuyenTaiKhoan",
                column: "QuyenId");

            migrationBuilder.AddForeignKey(
                name: "FK__NhanVien__TaiKho__7A672E12",
                table: "NhanVien",
                column: "TaiKhoanID",
                principalTable: "TaiKhoan",
                principalColumn: "TaiKhoanID");

            migrationBuilder.AddForeignKey(
                name: "FK__TaiKhoan__VaiTro__778AC167",
                table: "TaiKhoan",
                column: "VaiTroID",
                principalTable: "VaiTro",
                principalColumn: "VaiTroID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__NhanVien__TaiKho__7A672E12",
                table: "NhanVien");

            migrationBuilder.DropForeignKey(
                name: "FK__TaiKhoan__VaiTro__778AC167",
                table: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "ChiTietDichVu");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "QuyenTaiKhoan");

            migrationBuilder.DropTable(
                name: "DichVu");

            migrationBuilder.DropTable(
                name: "Quyen");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "NgayTao",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "GiaPhong1Dem",
                table: "Phong");

            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "Phong");

            migrationBuilder.DropColumn(
                name: "HinhAnh1",
                table: "Phong");

            migrationBuilder.DropColumn(
                name: "HinhAnh2",
                table: "Phong");

            migrationBuilder.DropColumn(
                name: "HinhAnh3",
                table: "Phong");

            migrationBuilder.DropColumn(
                name: "NgayTao",
                table: "Phong");

            migrationBuilder.DropColumn(
                name: "SoLuongKhach",
                table: "Phong");

            migrationBuilder.DropColumn(
                name: "VnpTransactionId",
                table: "PhieuDatPhong");

            migrationBuilder.DropColumn(
                name: "CCCD",
                table: "NhanVien");

            migrationBuilder.DropColumn(
                name: "DiaChi",
                table: "NhanVien");

            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "NhanVien");

            migrationBuilder.DropColumn(
                name: "NgayTao",
                table: "NhanVien");

            migrationBuilder.DropColumn(
                name: "AnhDemo",
                table: "LoaiPhong");

            migrationBuilder.DropColumn(
                name: "NgayTao",
                table: "LoaiPhong");

            migrationBuilder.DropColumn(
                name: "SoluongNguoi",
                table: "LoaiPhong");

            migrationBuilder.DropColumn(
                name: "NgayTao",
                table: "KhuyenMai");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "KhuyenMai");

            migrationBuilder.DropColumn(
                name: "CCCD",
                table: "KhachHang");

            migrationBuilder.DropColumn(
                name: "GhiChu",
                table: "KhachHang");

            migrationBuilder.DropColumn(
                name: "NgayTao",
                table: "KhachHang");

            migrationBuilder.RenameColumn(
                name: "Hoten",
                table: "TaiKhoan",
                newName: "TenDangNhap");

            migrationBuilder.CreateTable(
                name: "HinhAnhPhong",
                columns: table => new
                {
                    HinhAnhID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhongID = table.Column<int>(type: "int", nullable: true),
                    DuongDan = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HinhAnhP__8EF32B7B597EE560", x => x.HinhAnhID);
                    table.ForeignKey(
                        name: "FK__HinhAnhPh__Phong__3C69FB99",
                        column: x => x.PhongID,
                        principalTable: "Phong",
                        principalColumn: "PhongID");
                });

            migrationBuilder.CreateTable(
                name: "PhanQuyen",
                columns: table => new
                {
                    PhanQuyenID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VaiTroID = table.Column<int>(type: "int", nullable: true),
                    CoQuyen = table.Column<bool>(type: "bit", nullable: true),
                    TenChucNang = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanQuyen", x => x.PhanQuyenID);
                    table.ForeignKey(
                        name: "FK__PhanQuyen__VaiTr__5BE2A6F2",
                        column: x => x.VaiTroID,
                        principalTable: "VaiTro",
                        principalColumn: "VaiTroID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HinhAnhPhong_PhongID",
                table: "HinhAnhPhong",
                column: "PhongID");

            migrationBuilder.CreateIndex(
                name: "IX_PhanQuyen_VaiTroID",
                table: "PhanQuyen",
                column: "VaiTroID");

            migrationBuilder.AddForeignKey(
                name: "FK__NhanVien__TaiKho__5EBF139D",
                table: "NhanVien",
                column: "TaiKhoanID",
                principalTable: "TaiKhoan",
                principalColumn: "TaiKhoanID");

            migrationBuilder.AddForeignKey(
                name: "FK__TaiKhoan__VaiTro__59063A47",
                table: "TaiKhoan",
                column: "VaiTroID",
                principalTable: "VaiTro",
                principalColumn: "VaiTroID");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asp.netCoreDatPhongKS.Migrations
{
    public partial class ThiemDinh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    KhachHangID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHang", x => x.KhachHangID);
                });

            migrationBuilder.CreateTable(
                name: "KhuyenMai",
                columns: table => new
                {
                    KhuyenMaiID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhuyenMai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhanTramGiam = table.Column<int>(type: "int", nullable: true),
                    NgayBatDau = table.Column<DateTime>(type: "date", nullable: true),
                    NgayKetThuc = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhuyenMai", x => x.KhuyenMaiID);
                });

            migrationBuilder.CreateTable(
                name: "LoaiPhong",
                columns: table => new
                {
                    LoaiPhongID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaCoBan = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiPhong", x => x.LoaiPhongID);
                });

            migrationBuilder.CreateTable(
                name: "VaiTro",
                columns: table => new
                {
                    VaiTroID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTro", x => x.VaiTroID);
                });

            migrationBuilder.CreateTable(
                name: "PhieuDatPhong",
                columns: table => new
                {
                    PhieuDatPhongID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhieu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    KhachHangID = table.Column<int>(type: "int", nullable: true),
                    NgayDat = table.Column<DateTime>(type: "datetime", nullable: true),
                    NgayNhan = table.Column<DateTime>(type: "date", nullable: true),
                    NgayTra = table.Column<DateTime>(type: "date", nullable: true),
                    KhuyenMaiID = table.Column<int>(type: "int", nullable: true),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuDatPhong", x => x.PhieuDatPhongID);
                    table.ForeignKey(
                        name: "FK__PhieuDatP__Khach__4AB81AF0",
                        column: x => x.KhachHangID,
                        principalTable: "KhachHang",
                        principalColumn: "KhachHangID");
                    table.ForeignKey(
                        name: "FK__PhieuDatP__Khuye__4BAC3F29",
                        column: x => x.KhuyenMaiID,
                        principalTable: "KhuyenMai",
                        principalColumn: "KhuyenMaiID");
                });

            migrationBuilder.CreateTable(
                name: "Phong",
                columns: table => new
                {
                    PhongID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoPhong = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    LoaiPhongID = table.Column<int>(type: "int", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TinhTrang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phong", x => x.PhongID);
                    table.ForeignKey(
                        name: "FK__Phong__LoaiPhong__398D8EEE",
                        column: x => x.LoaiPhongID,
                        principalTable: "LoaiPhong",
                        principalColumn: "LoaiPhongID");
                });

            migrationBuilder.CreateTable(
                name: "PhanQuyen",
                columns: table => new
                {
                    PhanQuyenID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VaiTroID = table.Column<int>(type: "int", nullable: true),
                    TenChucNang = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CoQuyen = table.Column<bool>(type: "bit", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    TaiKhoanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    VaiTroID = table.Column<int>(type: "int", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.TaiKhoanID);
                    table.ForeignKey(
                        name: "FK__TaiKhoan__VaiTro__59063A47",
                        column: x => x.VaiTroID,
                        principalTable: "VaiTro",
                        principalColumn: "VaiTroID");
                });

            migrationBuilder.CreateTable(
                name: "ThanhToan",
                columns: table => new
                {
                    ThanhToanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhieuDatPhongID = table.Column<int>(type: "int", nullable: true),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime", nullable: true),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PhuongThuc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhToan", x => x.ThanhToanID);
                    table.ForeignKey(
                        name: "FK__ThanhToan__Phieu__5441852A",
                        column: x => x.PhieuDatPhongID,
                        principalTable: "PhieuDatPhong",
                        principalColumn: "PhieuDatPhongID");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPhieuPhong",
                columns: table => new
                {
                    ChiTietID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhieuDatPhongID = table.Column<int>(type: "int", nullable: true),
                    PhongID = table.Column<int>(type: "int", nullable: true),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietP__B117E9EAD34FC6E4", x => x.ChiTietID);
                    table.ForeignKey(
                        name: "FK__ChiTietPh__Phieu__4F7CD00D",
                        column: x => x.PhieuDatPhongID,
                        principalTable: "PhieuDatPhong",
                        principalColumn: "PhieuDatPhongID");
                    table.ForeignKey(
                        name: "FK__ChiTietPh__Phong__5070F446",
                        column: x => x.PhongID,
                        principalTable: "Phong",
                        principalColumn: "PhongID");
                });

            migrationBuilder.CreateTable(
                name: "DanhGia",
                columns: table => new
                {
                    DanhGiaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KhachHangID = table.Column<int>(type: "int", nullable: true),
                    PhongID = table.Column<int>(type: "int", nullable: true),
                    Diem = table.Column<int>(type: "int", nullable: true),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayDanhGia = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DanhGia__52C0CA2556B8202C", x => x.DanhGiaID);
                    table.ForeignKey(
                        name: "FK__DanhGia__KhachHa__412EB0B6",
                        column: x => x.KhachHangID,
                        principalTable: "KhachHang",
                        principalColumn: "KhachHangID");
                    table.ForeignKey(
                        name: "FK__DanhGia__PhongID__4222D4EF",
                        column: x => x.PhongID,
                        principalTable: "Phong",
                        principalColumn: "PhongID");
                });

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
                name: "NhanVien",
                columns: table => new
                {
                    NhanVienID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TaiKhoanID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVien", x => x.NhanVienID);
                    table.ForeignKey(
                        name: "FK__NhanVien__TaiKho__5EBF139D",
                        column: x => x.TaiKhoanID,
                        principalTable: "TaiKhoan",
                        principalColumn: "TaiKhoanID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuPhong_PhieuDatPhongID",
                table: "ChiTietPhieuPhong",
                column: "PhieuDatPhongID");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuPhong_PhongID",
                table: "ChiTietPhieuPhong",
                column: "PhongID");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_KhachHangID",
                table: "DanhGia",
                column: "KhachHangID");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_PhongID",
                table: "DanhGia",
                column: "PhongID");

            migrationBuilder.CreateIndex(
                name: "IX_HinhAnhPhong_PhongID",
                table: "HinhAnhPhong",
                column: "PhongID");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_TaiKhoanID",
                table: "NhanVien",
                column: "TaiKhoanID");

            migrationBuilder.CreateIndex(
                name: "IX_PhanQuyen_VaiTroID",
                table: "PhanQuyen",
                column: "VaiTroID");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuDatPhong_KhachHangID",
                table: "PhieuDatPhong",
                column: "KhachHangID");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuDatPhong_KhuyenMaiID",
                table: "PhieuDatPhong",
                column: "KhuyenMaiID");

            migrationBuilder.CreateIndex(
                name: "IX_Phong_LoaiPhongID",
                table: "Phong",
                column: "LoaiPhongID");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_VaiTroID",
                table: "TaiKhoan",
                column: "VaiTroID");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToan_PhieuDatPhongID",
                table: "ThanhToan",
                column: "PhieuDatPhongID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietPhieuPhong");

            migrationBuilder.DropTable(
                name: "DanhGia");

            migrationBuilder.DropTable(
                name: "HinhAnhPhong");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "PhanQuyen");

            migrationBuilder.DropTable(
                name: "ThanhToan");

            migrationBuilder.DropTable(
                name: "Phong");

            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "PhieuDatPhong");

            migrationBuilder.DropTable(
                name: "LoaiPhong");

            migrationBuilder.DropTable(
                name: "VaiTro");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "KhuyenMai");
        }
    }
}

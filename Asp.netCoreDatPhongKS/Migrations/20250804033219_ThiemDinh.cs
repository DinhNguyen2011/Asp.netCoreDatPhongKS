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
                name: "KhuyenMai",
                columns: table => new
                {
                    KhuyenMaiID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhuyenMai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhanTramGiam = table.Column<int>(type: "int", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))")
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
                    TenLoai = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaCoBan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoluongNguoi = table.Column<int>(type: "int", nullable: false),
                    AnhDemo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiPhong", x => x.LoaiPhongID);
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
                name: "Phong",
                columns: table => new
                {
                    PhongID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoPhong = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LoaiPhongID = table.Column<int>(type: "int", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TinhTrang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaPhong1Dem = table.Column<decimal>(type: "money", nullable: false),
                    SoLuongKhach = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    HinhAnh1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    HinhAnh2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    HinhAnh3 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phong", x => x.PhongID);
                    table.ForeignKey(
                        name: "FK__Phong__LoaiPhong__398D8EEE",
                        column: x => x.LoaiPhongID,
                        principalTable: "LoaiPhong",
                        principalColumn: "LoaiPhongID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    TaiKhoanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    VaiTroID = table.Column<int>(type: "int", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: true),
                    HinhAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Hoten = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.TaiKhoanID);
                    table.ForeignKey(
                        name: "FK__TaiKhoan__VaiTro__778AC167",
                        column: x => x.VaiTroID,
                        principalTable: "VaiTro",
                        principalColumn: "VaiTroID");
                });

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    KhachHangID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CCCD = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    TaiKhoanId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHang", x => x.KhachHangID);
                    table.ForeignKey(
                        name: "FK_KhachHang_TaiKhoan",
                        column: x => x.TaiKhoanId,
                        principalTable: "TaiKhoan",
                        principalColumn: "TaiKhoanID");
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    NhanVienID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TaiKhoanID = table.Column<int>(type: "int", nullable: true),
                    HinhAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVien", x => x.NhanVienID);
                    table.ForeignKey(
                        name: "FK__NhanVien__TaiKho__7A672E12",
                        column: x => x.TaiKhoanID,
                        principalTable: "TaiKhoan",
                        principalColumn: "TaiKhoanID");
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

            migrationBuilder.CreateTable(
                name: "DonHangDichVu",
                columns: table => new
                {
                    MaDonHangDv = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KhachHangId = table.Column<int>(type: "int", nullable: true),
                    NgayDat = table.Column<DateTime>(type: "datetime", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DonHangD__A906C724482980CC", x => x.MaDonHangDv);
                    table.ForeignKey(
                        name: "FK_DonHangDichVu_KhachHang",
                        column: x => x.KhachHangId,
                        principalTable: "KhachHang",
                        principalColumn: "KhachHangID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    MaHoaDon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NgayLap = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    KhachHangID = table.Column<int>(type: "int", nullable: true),
                    TongTienPhong = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValueSql: "((0))"),
                    TongTienDichVu = table.Column<decimal>(type: "decimal(18,2)", nullable: true, defaultValueSql: "((0))"),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HinhThucThanhToan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValueSql: "(N'Chưa thanh toán')"),
                    IsKhachVangLai = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoTienConNo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NguoiLapDH = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HoaDon__835ED13BD5B92EFA", x => x.MaHoaDon);
                    table.ForeignKey(
                        name: "FK__HoaDon__KhachHan__2610A626",
                        column: x => x.KhachHangID,
                        principalTable: "KhachHang",
                        principalColumn: "KhachHangID");
                });

            migrationBuilder.CreateTable(
                name: "LienHeVoiCToi",
                columns: table => new
                {
                    LienHeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KhachHangId = table.Column<int>(type: "int", nullable: true),
                    TaiKhoanId = table.Column<int>(type: "int", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayGui = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LienHeVo__53D769847D2D6696", x => x.LienHeId);
                    table.ForeignKey(
                        name: "FK__LienHeVoi__Khach__5CA1C101",
                        column: x => x.KhachHangId,
                        principalTable: "KhachHang",
                        principalColumn: "KhachHangID");
                    table.ForeignKey(
                        name: "FK__LienHeVoi__TaiKh__5D95E53A",
                        column: x => x.TaiKhoanId,
                        principalTable: "TaiKhoan",
                        principalColumn: "TaiKhoanID");
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
                    NgayNhan = table.Column<DateTime>(type: "datetime", nullable: true),
                    NgayTra = table.Column<DateTime>(type: "datetime", nullable: true),
                    KhuyenMaiID = table.Column<int>(type: "int", nullable: true),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VnpTransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoTienCoc = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TinhTrangSuDung = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SoTienDaThanhToan = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MoMoTransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "ChiTietDonHangDichVu",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDonHangDV = table.Column<int>(type: "int", nullable: false),
                    DichVuId = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(29,2)", nullable: true, computedColumnSql: "([SoLuong]*[DonGia])", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDonHangDichVu", x => x.ID);
                    table.ForeignKey(
                        name: "FK__ChiTietDo__DichV__3EDC53F0",
                        column: x => x.DichVuId,
                        principalTable: "DichVu",
                        principalColumn: "DichVuId");
                    table.ForeignKey(
                        name: "FK__ChiTietDo__MDHDV__3DE82FB7",
                        column: x => x.MaDonHangDV,
                        principalTable: "DonHangDichVu",
                        principalColumn: "MaDonHangDv");
                });

            migrationBuilder.CreateTable(
                name: "HoaDonDichVu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDonHangDv = table.Column<int>(type: "int", nullable: false),
                    TrangThaiThanhToan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValueSql: "('Chua thanh toán')"),
                    MaHoaDonTong = table.Column<int>(type: "int", nullable: true),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime", nullable: true),
                    HinhThucThanhToan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDonDichVu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoaDonDichVu_DonHangDichVu",
                        column: x => x.MaDonHangDv,
                        principalTable: "DonHangDichVu",
                        principalColumn: "MaDonHangDv",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoaDonDichVu_HoaDon",
                        column: x => x.MaHoaDonTong,
                        principalTable: "HoaDon",
                        principalColumn: "MaHoaDon",
                        onDelete: ReferentialAction.SetNull);
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
                    PhongID = table.Column<int>(type: "int", nullable: true),
                    DichVuId = table.Column<int>(type: "int", nullable: true),
                    PhieuDatPhongId = table.Column<int>(type: "int", nullable: true),
                    DonHangDichVuId = table.Column<int>(type: "int", nullable: true),
                    Diem = table.Column<int>(type: "int", nullable: true),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayDanhGia = table.Column<DateTime>(type: "datetime", nullable: true),
                    TaiKhoanId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DanhGia__52C0CA2556B8202C", x => x.DanhGiaID);
                    table.ForeignKey(
                        name: "FK__DanhGia__PhongID__4222D4EF",
                        column: x => x.PhongID,
                        principalTable: "Phong",
                        principalColumn: "PhongID");
                    table.ForeignKey(
                        name: "FK_DanhGia_DonHangDichVu_DonHangDichVuId",
                        column: x => x.DonHangDichVuId,
                        principalTable: "DonHangDichVu",
                        principalColumn: "MaDonHangDv");
                    table.ForeignKey(
                        name: "FK_DanhGia_PhieuDatPhong_PhieuDatPhongId",
                        column: x => x.PhieuDatPhongId,
                        principalTable: "PhieuDatPhong",
                        principalColumn: "PhieuDatPhongID");
                    table.ForeignKey(
                        name: "FK_DanhGium_DichVu",
                        column: x => x.DichVuId,
                        principalTable: "DichVu",
                        principalColumn: "DichVuId");
                    table.ForeignKey(
                        name: "FK_DanhGium_TaiKhoan",
                        column: x => x.TaiKhoanId,
                        principalTable: "TaiKhoan",
                        principalColumn: "TaiKhoanID");
                });

            migrationBuilder.CreateTable(
                name: "HoaDonPDP",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHoaDon = table.Column<int>(type: "int", nullable: true),
                    PhieuDatPhongID = table.Column<int>(type: "int", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDonPDP", x => x.ID);
                    table.ForeignKey(
                        name: "FK__HoaDonPDP__MaHoa__55BFB948",
                        column: x => x.MaHoaDon,
                        principalTable: "HoaDon",
                        principalColumn: "MaHoaDon");
                    table.ForeignKey(
                        name: "FK__HoaDonPDP__Phieu__56B3DD81",
                        column: x => x.PhieuDatPhongID,
                        principalTable: "PhieuDatPhong",
                        principalColumn: "PhieuDatPhongID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHangDichVu_DichVuId",
                table: "ChiTietDonHangDichVu",
                column: "DichVuId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHangDichVu_MaDonHangDV",
                table: "ChiTietDonHangDichVu",
                column: "MaDonHangDV");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuPhong_PhieuDatPhongID",
                table: "ChiTietPhieuPhong",
                column: "PhieuDatPhongID");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuPhong_PhongID",
                table: "ChiTietPhieuPhong",
                column: "PhongID");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_DichVuId",
                table: "DanhGia",
                column: "DichVuId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_DonHangDichVuId",
                table: "DanhGia",
                column: "DonHangDichVuId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_PhieuDatPhongId",
                table: "DanhGia",
                column: "PhieuDatPhongId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_PhongID",
                table: "DanhGia",
                column: "PhongID");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_TaiKhoanId",
                table: "DanhGia",
                column: "TaiKhoanId");

            migrationBuilder.CreateIndex(
                name: "IX_DonHangDichVu_KhachHangId",
                table: "DonHangDichVu",
                column: "KhachHangId");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_KhachHangID",
                table: "HoaDon",
                column: "KhachHangID");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonDichVu_MaDonHangDv",
                table: "HoaDonDichVu",
                column: "MaDonHangDv");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonDichVu_MaHoaDonTong",
                table: "HoaDonDichVu",
                column: "MaHoaDonTong");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDonPDP_MaHoaDon",
                table: "HoaDonPDP",
                column: "MaHoaDon");

            migrationBuilder.CreateIndex(
                name: "UQ__HoaDonPD__53A722215C43A14A",
                table: "HoaDonPDP",
                column: "PhieuDatPhongID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KhachHang_TaiKhoanId",
                table: "KhachHang",
                column: "TaiKhoanId");

            migrationBuilder.CreateIndex(
                name: "IX_LienHeVoiCToi_KhachHangId",
                table: "LienHeVoiCToi",
                column: "KhachHangId");

            migrationBuilder.CreateIndex(
                name: "IX_LienHeVoiCToi_TaiKhoanId",
                table: "LienHeVoiCToi",
                column: "TaiKhoanId");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_TaiKhoanID",
                table: "NhanVien",
                column: "TaiKhoanID");

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
                name: "UQ__Quyen__1D4B7ED5D71DC5F4",
                table: "Quyen",
                column: "MaQuyen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuyenTaiKhoan_QuyenId",
                table: "QuyenTaiKhoan",
                column: "QuyenId");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_VaiTroID",
                table: "TaiKhoan",
                column: "VaiTroID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietDonHangDichVu");

            migrationBuilder.DropTable(
                name: "ChiTietPhieuPhong");

            migrationBuilder.DropTable(
                name: "DanhGia");

            migrationBuilder.DropTable(
                name: "HoaDonDichVu");

            migrationBuilder.DropTable(
                name: "HoaDonPDP");

            migrationBuilder.DropTable(
                name: "LienHeVoiCToi");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "QuyenTaiKhoan");

            migrationBuilder.DropTable(
                name: "Phong");

            migrationBuilder.DropTable(
                name: "DichVu");

            migrationBuilder.DropTable(
                name: "DonHangDichVu");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "PhieuDatPhong");

            migrationBuilder.DropTable(
                name: "Quyen");

            migrationBuilder.DropTable(
                name: "LoaiPhong");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "KhuyenMai");

            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "VaiTro");
        }
    }
}

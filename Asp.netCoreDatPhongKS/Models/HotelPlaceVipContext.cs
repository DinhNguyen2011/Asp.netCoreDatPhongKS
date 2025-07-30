using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Asp.netCoreDatPhongKS.Models
{
    public partial class HotelPlaceVipContext : DbContext
    {
        public HotelPlaceVipContext()
        {
        }

        public HotelPlaceVipContext(DbContextOptions<HotelPlaceVipContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ChiTietDonHangDichVu> ChiTietDonHangDichVus { get; set; } = null!;
        public virtual DbSet<ChiTietPhieuPhong> ChiTietPhieuPhongs { get; set; } = null!;
        public virtual DbSet<DanhGium> DanhGia { get; set; } = null!;
        public virtual DbSet<DichVu> DichVus { get; set; } = null!;
        public virtual DbSet<DonHangDichVu> DonHangDichVus { get; set; } = null!;
        public virtual DbSet<HoaDon> HoaDons { get; set; } = null!;
        public virtual DbSet<HoaDonDichVu> HoaDonDichVus { get; set; } = null!;
        public virtual DbSet<HoaDonPdp> HoaDonPdps { get; set; } = null!;
        public virtual DbSet<KhachHang> KhachHangs { get; set; } = null!;
        public virtual DbSet<KhuyenMai> KhuyenMais { get; set; } = null!;
        public virtual DbSet<LienHeVoiCtoi> LienHeVoiCtois { get; set; } = null!;
        public virtual DbSet<LoaiPhong> LoaiPhongs { get; set; } = null!;
        public virtual DbSet<NhanVien> NhanViens { get; set; } = null!;
        public virtual DbSet<PhieuDatPhong> PhieuDatPhongs { get; set; } = null!;
        public virtual DbSet<Phong> Phongs { get; set; } = null!;
        public virtual DbSet<Quyen> Quyens { get; set; } = null!;
        public virtual DbSet<QuyenTaiKhoan> QuyenTaiKhoans { get; set; } = null!;
        public virtual DbSet<TaiKhoan> TaiKhoans { get; set; } = null!;
        public virtual DbSet<VaiTro> VaiTros { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DINHNGUYEN;Database=HotelPlaceVip;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChiTietDonHangDichVu>(entity =>
            {
                entity.ToTable("ChiTietDonHangDichVu");

                entity.HasIndex(e => e.DichVuId, "IX_ChiTietDonHangDichVu_DichVuId");

                entity.HasIndex(e => e.MaDonHangDv, "IX_ChiTietDonHangDichVu_MaDonHangDV");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MaDonHangDv).HasColumnName("MaDonHangDV");

                entity.Property(e => e.ThanhTien)
                    .HasColumnType("decimal(29, 2)")
                    .HasComputedColumnSql("([SoLuong]*[DonGia])", true);

                entity.HasOne(d => d.DichVu)
                    .WithMany(p => p.ChiTietDonHangDichVus)
                    .HasForeignKey(d => d.DichVuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChiTietDo__DichV__3EDC53F0");

                entity.HasOne(d => d.MaDonHangDvNavigation)
                    .WithMany(p => p.ChiTietDonHangDichVus)
                    .HasForeignKey(d => d.MaDonHangDv)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChiTietDo__MDHDV__3DE82FB7");
            });

            modelBuilder.Entity<ChiTietPhieuPhong>(entity =>
            {
                entity.HasKey(e => e.ChiTietId)
                    .HasName("PK__ChiTietP__B117E9EAD34FC6E4");

                entity.ToTable("ChiTietPhieuPhong");

                entity.HasIndex(e => e.PhieuDatPhongId, "IX_ChiTietPhieuPhong_PhieuDatPhongID");

                entity.HasIndex(e => e.PhongId, "IX_ChiTietPhieuPhong_PhongID");

                entity.Property(e => e.ChiTietId).HasColumnName("ChiTietID");

                entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PhieuDatPhongId).HasColumnName("PhieuDatPhongID");

                entity.Property(e => e.PhongId).HasColumnName("PhongID");

                entity.HasOne(d => d.PhieuDatPhong)
                    .WithMany(p => p.ChiTietPhieuPhongs)
                    .HasForeignKey(d => d.PhieuDatPhongId)
                    .HasConstraintName("FK__ChiTietPh__Phieu__4F7CD00D");

                entity.HasOne(d => d.Phong)
                    .WithMany(p => p.ChiTietPhieuPhongs)
                    .HasForeignKey(d => d.PhongId)
                    .HasConstraintName("FK__ChiTietPh__Phong__5070F446");
            });

            modelBuilder.Entity<DanhGium>(entity =>
            {
                entity.HasKey(e => e.DanhGiaId)
                    .HasName("PK__DanhGia__52C0CA2556B8202C");

                entity.HasIndex(e => e.DichVuId, "IX_DanhGia_DichVuId");

                entity.HasIndex(e => e.DonHangDichVuId, "IX_DanhGia_DonHangDichVuId");

                entity.HasIndex(e => e.PhieuDatPhongId, "IX_DanhGia_PhieuDatPhongId");

                entity.HasIndex(e => e.PhongId, "IX_DanhGia_PhongID");

                entity.HasIndex(e => e.TaiKhoanId, "IX_DanhGia_TaiKhoanId");

                entity.Property(e => e.DanhGiaId).HasColumnName("DanhGiaID");

                entity.Property(e => e.NgayDanhGia).HasColumnType("datetime");

                entity.Property(e => e.PhongId).HasColumnName("PhongID");

                entity.HasOne(d => d.DichVu)
                    .WithMany(p => p.DanhGia)
                    .HasForeignKey(d => d.DichVuId)
                    .HasConstraintName("FK_DanhGium_DichVu");

                entity.HasOne(d => d.DonHangDichVu)
                    .WithMany(p => p.DanhGia)
                    .HasForeignKey(d => d.DonHangDichVuId);

                entity.HasOne(d => d.PhieuDatPhong)
                    .WithMany(p => p.DanhGia)
                    .HasForeignKey(d => d.PhieuDatPhongId);

                entity.HasOne(d => d.Phong)
                    .WithMany(p => p.DanhGia)
                    .HasForeignKey(d => d.PhongId)
                    .HasConstraintName("FK__DanhGia__PhongID__4222D4EF");

                entity.HasOne(d => d.TaiKhoan)
                    .WithMany(p => p.DanhGia)
                    .HasForeignKey(d => d.TaiKhoanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DanhGium_TaiKhoan");
            });

            modelBuilder.Entity<DichVu>(entity =>
            {
                entity.ToTable("DichVu");

                entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.HinhAnh).HasMaxLength(255);

                entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");

                entity.Property(e => e.NgayTao)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TenDichVu).HasMaxLength(255);

                entity.Property(e => e.TrangThai)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<DonHangDichVu>(entity =>
            {
                entity.HasKey(e => e.MaDonHangDv)
                    .HasName("PK__DonHangD__A906C724482980CC");

                entity.ToTable("DonHangDichVu");

                entity.HasIndex(e => e.KhachHangId, "IX_DonHangDichVu_KhachHangId");

                entity.Property(e => e.NgayDat).HasColumnType("datetime");

                entity.Property(e => e.TrangThai).HasMaxLength(50);

                entity.HasOne(d => d.KhachHang)
                    .WithMany(p => p.DonHangDichVus)
                    .HasForeignKey(d => d.KhachHangId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_DonHangDichVu_KhachHang");
            });

            modelBuilder.Entity<HoaDon>(entity =>
            {
                entity.HasKey(e => e.MaHoaDon)
                    .HasName("PK__HoaDon__835ED13BD5B92EFA");

                entity.ToTable("HoaDon");

                entity.HasIndex(e => e.KhachHangId, "IX_HoaDon_KhachHangID");

                entity.Property(e => e.HinhThucThanhToan).HasMaxLength(50);

                entity.Property(e => e.IsKhachVangLai).HasDefaultValueSql("((0))");

                entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");

                entity.Property(e => e.NgayLap)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.NguoiLapDh).HasColumnName("NguoiLapDH");

                entity.Property(e => e.SoTienConNo).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TongTienDichVu)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.TongTienPhong)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.TrangThai)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("(N'Chưa thanh toán')");

                entity.HasOne(d => d.KhachHang)
                    .WithMany(p => p.HoaDons)
                    .HasForeignKey(d => d.KhachHangId)
                    .HasConstraintName("FK__HoaDon__KhachHan__2610A626");
            });

            modelBuilder.Entity<HoaDonDichVu>(entity =>
            {
                entity.ToTable("HoaDonDichVu");

                entity.HasIndex(e => e.MaDonHangDv, "IX_HoaDonDichVu_MaDonHangDv");

                entity.HasIndex(e => e.MaHoaDonTong, "IX_HoaDonDichVu_MaHoaDonTong");

                entity.Property(e => e.HinhThucThanhToan).HasMaxLength(100);

                entity.Property(e => e.NgayThanhToan).HasColumnType("datetime");

                entity.Property(e => e.TrangThaiThanhToan)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("('Chua thanh toán')");

                entity.HasOne(d => d.MaDonHangDvNavigation)
                    .WithMany(p => p.HoaDonDichVus)
                    .HasForeignKey(d => d.MaDonHangDv)
                    .HasConstraintName("FK_HoaDonDichVu_DonHangDichVu");

                entity.HasOne(d => d.MaHoaDonTongNavigation)
                    .WithMany(p => p.HoaDonDichVus)
                    .HasForeignKey(d => d.MaHoaDonTong)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_HoaDonDichVu_HoaDon");
            });

            modelBuilder.Entity<HoaDonPdp>(entity =>
            {
                entity.ToTable("HoaDonPDP");

                entity.HasIndex(e => e.MaHoaDon, "IX_HoaDonPDP_MaHoaDon");

                entity.HasIndex(e => e.PhieuDatPhongId, "UQ__HoaDonPD__53A722215C43A14A")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PhieuDatPhongId).HasColumnName("PhieuDatPhongID");

                entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TrangThai).HasMaxLength(50);

                entity.HasOne(d => d.MaHoaDonNavigation)
                    .WithMany(p => p.HoaDonPdps)
                    .HasForeignKey(d => d.MaHoaDon)
                    .HasConstraintName("FK__HoaDonPDP__MaHoa__55BFB948");

                entity.HasOne(d => d.PhieuDatPhong)
                    .WithOne(p => p.HoaDonPdp)
                    .HasForeignKey<HoaDonPdp>(d => d.PhieuDatPhongId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HoaDonPDP__Phieu__56B3DD81");
            });

            modelBuilder.Entity<KhachHang>(entity =>
            {
                entity.ToTable("KhachHang");

                entity.HasIndex(e => e.TaiKhoanId, "IX_KhachHang_TaiKhoanId");

                entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");

                entity.Property(e => e.Cccd)
                    .HasMaxLength(255)
                    .HasColumnName("CCCD");

                entity.Property(e => e.DiaChi).HasMaxLength(200);

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.HoTen).HasMaxLength(100);

                entity.Property(e => e.NgayTao)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.SoDienThoai).HasMaxLength(20);

                entity.HasOne(d => d.TaiKhoan)
                    .WithMany(p => p.KhachHangs)
                    .HasForeignKey(d => d.TaiKhoanId)
                    .HasConstraintName("FK_KhachHang_TaiKhoan");
            });

            modelBuilder.Entity<KhuyenMai>(entity =>
            {
                entity.ToTable("KhuyenMai");

                entity.Property(e => e.KhuyenMaiId).HasColumnName("KhuyenMaiID");

                entity.Property(e => e.MaKhuyenMai).HasMaxLength(50);

                entity.Property(e => e.NgayBatDau).HasColumnType("datetime");

                entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");

                entity.Property(e => e.NgayTao)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TrangThai)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<LienHeVoiCtoi>(entity =>
            {
                entity.HasKey(e => e.LienHeId)
                    .HasName("PK__LienHeVo__53D769847D2D6696");

                entity.ToTable("LienHeVoiCToi");

                entity.HasIndex(e => e.KhachHangId, "IX_LienHeVoiCToi_KhachHangId");

                entity.HasIndex(e => e.TaiKhoanId, "IX_LienHeVoiCToi_TaiKhoanId");

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.HoTen).HasMaxLength(100);

                entity.Property(e => e.NgayGui)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.SoDienThoai).HasMaxLength(20);

                entity.HasOne(d => d.KhachHang)
                    .WithMany(p => p.LienHeVoiCtois)
                    .HasForeignKey(d => d.KhachHangId)
                    .HasConstraintName("FK__LienHeVoi__Khach__5CA1C101");

                entity.HasOne(d => d.TaiKhoan)
                    .WithMany(p => p.LienHeVoiCtois)
                    .HasForeignKey(d => d.TaiKhoanId)
                    .HasConstraintName("FK__LienHeVoi__TaiKh__5D95E53A");
            });

            modelBuilder.Entity<LoaiPhong>(entity =>
            {
                entity.ToTable("LoaiPhong");

                entity.Property(e => e.LoaiPhongId).HasColumnName("LoaiPhongID");

                entity.Property(e => e.AnhDemo).HasMaxLength(250);

                entity.Property(e => e.GiaCoBan).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NgayTao)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TenLoai).HasMaxLength(100);
            });

            modelBuilder.Entity<NhanVien>(entity =>
            {
                entity.ToTable("NhanVien");

                entity.HasIndex(e => e.TaiKhoanId, "IX_NhanVien_TaiKhoanID");

                entity.Property(e => e.NhanVienId).HasColumnName("NhanVienID");

                entity.Property(e => e.Cccd)
                    .HasMaxLength(20)
                    .HasColumnName("CCCD");

                entity.Property(e => e.DiaChi).HasMaxLength(100);

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.HinhAnh).HasMaxLength(255);

                entity.Property(e => e.HoTen).HasMaxLength(100);

                entity.Property(e => e.NgayTao)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.SoDienThoai).HasMaxLength(20);

                entity.Property(e => e.TaiKhoanId).HasColumnName("TaiKhoanID");

                entity.HasOne(d => d.TaiKhoan)
                    .WithMany(p => p.NhanViens)
                    .HasForeignKey(d => d.TaiKhoanId)
                    .HasConstraintName("FK__NhanVien__TaiKho__7A672E12");
            });

            modelBuilder.Entity<PhieuDatPhong>(entity =>
            {
                entity.ToTable("PhieuDatPhong");

                entity.HasIndex(e => e.KhachHangId, "IX_PhieuDatPhong_KhachHangID");

                entity.HasIndex(e => e.KhuyenMaiId, "IX_PhieuDatPhong_KhuyenMaiID");

                entity.Property(e => e.PhieuDatPhongId).HasColumnName("PhieuDatPhongID");

                entity.Property(e => e.KhachHangId).HasColumnName("KhachHangID");

                entity.Property(e => e.KhuyenMaiId).HasColumnName("KhuyenMaiID");

                entity.Property(e => e.MaPhieu).HasMaxLength(50);

                entity.Property(e => e.NgayDat).HasColumnType("datetime");

                entity.Property(e => e.NgayNhan).HasColumnType("datetime");

                entity.Property(e => e.NgayTra).HasColumnType("datetime");

                entity.Property(e => e.SoTienCoc).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SoTienDaThanhToan).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TinhTrangSuDung).HasMaxLength(50);

                entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TrangThai).HasMaxLength(50);

                entity.HasOne(d => d.KhachHang)
                    .WithMany(p => p.PhieuDatPhongs)
                    .HasForeignKey(d => d.KhachHangId)
                    .HasConstraintName("FK__PhieuDatP__Khach__4AB81AF0");

                entity.HasOne(d => d.KhuyenMai)
                    .WithMany(p => p.PhieuDatPhongs)
                    .HasForeignKey(d => d.KhuyenMaiId)
                    .HasConstraintName("FK__PhieuDatP__Khuye__4BAC3F29");
            });

            modelBuilder.Entity<Phong>(entity =>
            {
                entity.ToTable("Phong");

                entity.HasIndex(e => e.LoaiPhongId, "IX_Phong_LoaiPhongID");

                entity.Property(e => e.PhongId).HasColumnName("PhongID");

                entity.Property(e => e.GiaPhong1Dem).HasColumnType("money");

                entity.Property(e => e.HinhAnh1).HasMaxLength(255);

                entity.Property(e => e.HinhAnh2).HasMaxLength(255);

                entity.Property(e => e.HinhAnh3).HasMaxLength(255);

                entity.Property(e => e.LoaiPhongId).HasColumnName("LoaiPhongID");

                entity.Property(e => e.NgayTao)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.SoPhong).HasMaxLength(10);

                entity.Property(e => e.TinhTrang).HasMaxLength(50);

                entity.HasOne(d => d.LoaiPhong)
                    .WithMany(p => p.Phongs)
                    .HasForeignKey(d => d.LoaiPhongId)
                    .HasConstraintName("FK__Phong__LoaiPhong__398D8EEE");
            });

            modelBuilder.Entity<Quyen>(entity =>
            {
                entity.ToTable("Quyen");

                entity.HasIndex(e => e.MaQuyen, "UQ__Quyen__1D4B7ED5D71DC5F4")
                    .IsUnique();

                entity.Property(e => e.MaQuyen).HasMaxLength(50);

                entity.Property(e => e.MoTa).HasMaxLength(255);

                entity.Property(e => e.TenQuyen).HasMaxLength(100);
            });

            modelBuilder.Entity<QuyenTaiKhoan>(entity =>
            {
                entity.HasKey(e => new { e.TaiKhoanId, e.QuyenId })
                    .HasName("PK__TaiKhoan__4B3B6DA39AADDB30");

                entity.ToTable("QuyenTaiKhoan");

                entity.HasIndex(e => e.QuyenId, "IX_QuyenTaiKhoan_QuyenId");

                entity.HasOne(d => d.Quyen)
                    .WithMany(p => p.QuyenTaiKhoans)
                    .HasForeignKey(d => d.QuyenId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TaiKhoan___Quyen__19DFD96B");

                entity.HasOne(d => d.TaiKhoan)
                    .WithMany(p => p.QuyenTaiKhoans)
                    .HasForeignKey(d => d.TaiKhoanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TaiKhoan___TaiKh__18EBB532");
            });

            modelBuilder.Entity<TaiKhoan>(entity =>
            {
                entity.ToTable("TaiKhoan");

                entity.HasIndex(e => e.VaiTroId, "IX_TaiKhoan_VaiTroID");

                entity.Property(e => e.TaiKhoanId).HasColumnName("TaiKhoanID");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.HinhAnh).HasMaxLength(255);

                entity.Property(e => e.Hoten).HasMaxLength(50);

                entity.Property(e => e.MatKhau).HasMaxLength(255);

                entity.Property(e => e.NgayTao)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.VaiTroId).HasColumnName("VaiTroID");

                entity.HasOne(d => d.VaiTro)
                    .WithMany(p => p.TaiKhoans)
                    .HasForeignKey(d => d.VaiTroId)
                    .HasConstraintName("FK__TaiKhoan__VaiTro__778AC167");
            });

            modelBuilder.Entity<VaiTro>(entity =>
            {
                entity.ToTable("VaiTro");

                entity.Property(e => e.VaiTroId).HasColumnName("VaiTroID");

                entity.Property(e => e.TenVaiTro).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

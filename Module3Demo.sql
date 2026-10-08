SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    -- Kiểm tra dữ liệu cần thiết.
    IF NOT EXISTS (
        SELECT 1 FROM KhuVucs WHERE TrangThai = 1
    )
        THROW 50001, N'Cần có ít nhất một khu vực đang hoạt động.', 1;

    IF NOT EXISTS (
        SELECT 1 FROM NguoiDans
    )
        THROW 50002, N'Cần có ít nhất một hồ sơ người dân.', 1;

    IF NOT EXISTS (
        SELECT 1 FROM LoaiRacs WHERE TrangThai = 1
    )
        THROW 50003, N'Cần có ít nhất một loại rác đang hoạt động.', 1;

    -- Tạo hồ sơ cho tài khoản nhân viên chưa có hồ sơ.
    -- TrangThaiNhanVien.DangHoatDong = 0 theo enum hiện tại.
    ;WITH TaiKhoanChuaCoHoSo AS
    (
        SELECT
            t.MaTaiKhoan,
            t.HoTen,
            t.SoDienThoai,
            ROW_NUMBER() OVER (
                ORDER BY t.MaTaiKhoan
            ) AS STT
        FROM TaiKhoans t
        WHERE t.LoaiTaiKhoan = 2
          AND t.TrangThai = 1
          AND NOT EXISTS (
              SELECT 1
              FROM NhanViens n
              WHERE n.MaTaiKhoan = t.MaTaiKhoan
          )
    ),
    KhuVucHoatDong AS
    (
        SELECT
            MaKhuVuc,
            ROW_NUMBER() OVER (
                ORDER BY MaKhuVuc
            ) AS STT
        FROM KhuVucs
        WHERE TrangThai = 1
    )
    INSERT INTO NhanViens
    (
        MaTaiKhoan,
        HoTen,
        SoDienThoai,
        MaKhuVuc,
        NgayVaoLam,
        TrangThai
    )
    SELECT
        t.MaTaiKhoan,
        t.HoTen,
        t.SoDienThoai,
        k.MaKhuVuc,
        CAST(GETDATE() AS date),
        0
    FROM TaiKhoanChuaCoHoSo t
    JOIN KhuVucHoatDong k
        ON k.STT = (
            (t.STT - 1)
            % (SELECT COUNT(*) FROM KhuVucHoatDong)
        ) + 1;

    -- Ưu tiên khu vực có nhân viên đủ điều kiện phân công.
    DECLARE @MaNhanVien int;
    DECLARE @MaKhuVuc int;

    SELECT TOP (1)
        @MaNhanVien = n.MaNhanVien,
        @MaKhuVuc = n.MaKhuVuc
    FROM NhanViens n
    JOIN TaiKhoans t ON t.MaTaiKhoan = n.MaTaiKhoan
    JOIN KhuVucs k ON k.MaKhuVuc = n.MaKhuVuc
    WHERE n.TrangThai = 0
      AND t.LoaiTaiKhoan = 2
      AND t.TrangThai = 1
      AND k.TrangThai = 1
    ORDER BY n.MaNhanVien;

    IF @MaKhuVuc IS NULL
        SELECT TOP (1) @MaKhuVuc = MaKhuVuc
        FROM KhuVucs
        WHERE TrangThai = 1
        ORDER BY MaKhuVuc;

    DECLARE @MaLoaiRac int;

    SELECT TOP (1) @MaLoaiRac = MaLoaiRac
    FROM LoaiRacs
    WHERE TrangThai = 1
    ORDER BY MaLoaiRac;

    DECLARE @i int = 1;
    DECLARE @MaNguoiDan int;
    DECLARE @SoDienThoai nvarchar(15);
    DECLARE @MaYeuCau int;
    DECLARE @TrangThai int;
    DECLARE @Khoa nvarchar(64);

    WHILE @i <= 12
    BEGIN
        SET @Khoa = CONCAT(N'DAT_MODULE3_DEMO_V1_', @i);

        IF NOT EXISTS (
            SELECT 1 FROM YeuCauThuGoms
            WHERE KhoaChongTrung = @Khoa
        )
        BEGIN
            -- Luân phiên lấy các hồ sơ người dân.
            ;WITH DanhSach AS
            (
                SELECT
                    MaNguoiDan,
                    SoDienThoai,
                    ROW_NUMBER() OVER (
                        ORDER BY MaNguoiDan
                    ) AS STT
                FROM NguoiDans
            )
            SELECT
                @MaNguoiDan = MaNguoiDan,
                @SoDienThoai = SoDienThoai
            FROM DanhSach
            WHERE STT = (
                (@i - 1)
                % (SELECT COUNT(*) FROM NguoiDans)
            ) + 1;

            -- 1: Chờ xác nhận
            -- 2: Đã xác nhận
            -- 3: Đã phân công
            SET @TrangThai =
                CASE
                    WHEN @i <= 4 THEN 1
                    WHEN @i <= 8 THEN 2
                    WHEN @MaNhanVien IS NOT NULL THEN 3
                    ELSE 2
                END;

            INSERT INTO YeuCauThuGoms
            (
                MaNguoiDan,
                MaKhuVuc,
                DiaChiThuGom,
                SoDienThoaiLienHe,
                NgayDangKy,
                NgayMongMuonThuGom,
                KhungGio,
                GhiChu,
                TrangThai,
                KhoaChongTrung
            )
            VALUES
            (
                @MaNguoiDan,
                @MaKhuVuc,
                CONCAT(N'Địa chỉ thử nghiệm số ', @i),
                @SoDienThoai,
                GETDATE(),
                DATEADD(day, @i, CAST(GETDATE() AS date)),
                N'08:00–10:00',
                N'Dữ liệu mẫu Module 3. Gọi trước khi đến.',
                @TrangThai,
                @Khoa
            );

            SET @MaYeuCau =
                CONVERT(int, SCOPE_IDENTITY());

            INSERT INTO ChiTietYeuCaus
            (
                MaYeuCau,
                MaLoaiRac,
                SoLuongDuKien,
                GhiChu
            )
            VALUES
            (
                @MaYeuCau,
                @MaLoaiRac,
                @i + 2,
                N'Rác đã phân loại, dữ liệu thử nghiệm.'
            );

            IF @TrangThai = 3
            BEGIN
                INSERT INTO PhanCongThuGoms
                (
                    MaYeuCau,
                    MaNhanVien,
                    NgayPhanCong,
                    GhiChu,
                    TrangThai
                )
                VALUES
                (
                    @MaYeuCau,
                    @MaNhanVien,
                    GETDATE(),
                    N'Phân công mẫu để kiểm tra Module 3.',
                    3
                );
            END;
        END;

        SET @i = @i + 1;
    END;

    COMMIT TRANSACTION;

    -- Hiển thị kết quả tạo dữ liệu.
    SELECT
        n.MaNhanVien,
        t.TenDangNhap,
        n.HoTen,
        k.TenKhuVuc,
        n.TrangThai
    FROM NhanViens n
    JOIN TaiKhoans t ON t.MaTaiKhoan = n.MaTaiKhoan
    JOIN KhuVucs k ON k.MaKhuVuc = n.MaKhuVuc;

    SELECT
        y.MaYeuCau,
        y.DiaChiThuGom,
        y.TrangThai,
        p.MaNhanVien
    FROM YeuCauThuGoms y
    LEFT JOIN PhanCongThuGoms p
        ON p.MaYeuCau = y.MaYeuCau
    WHERE y.KhoaChongTrung LIKE N'DAT_MODULE3_DEMO_V1_%'
    ORDER BY y.MaYeuCau;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
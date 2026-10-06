SELECT
    COLUMN_NAME AS TenCot,
    DATA_TYPE AS KieuDuLieu,
    CHARACTER_MAXIMUM_LENGTH AS DoDai,
    IS_NULLABLE AS ChoPhepNull
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'dbo'
  AND TABLE_NAME = 'NhanViens'
ORDER BY ORDINAL_POSITION;

-- Kiểm tra nhân viên có mã khu vực không tồn tại.
SELECT nv.MaNhanVien, nv.MaKhuVuc
FROM dbo.NhanViens nv
LEFT JOIN dbo.KhuVucs kv ON kv.MaKhuVuc = nv.MaKhuVuc
WHERE kv.MaKhuVuc IS NULL;
# Thiết kế CSDL

## Entity tối thiểu

- `TaiKhoan`
- `KhuVuc`
- `LoaiRac`
- `DiemTapKet`
- `NguoiDan`
- `NhanVien`
- `YeuCauThuGom`
- `ChiTietYeuCau`
- `PhanCongThuGom`
- `KetQuaThuGom`

## Quan hệ chính

- `KhuVuc` 1–n `NhanVien`.
- `KhuVuc` 1–n `YeuCauThuGom`.
- `TaiKhoan` 1–1 `NguoiDan`.
- `TaiKhoan` 1–1 `NhanVien`.
- `NguoiDan` 1–n `YeuCauThuGom`.
- `YeuCauThuGom` 1–n `ChiTietYeuCau`.
- `LoaiRac` 1–n `ChiTietYeuCau`.
- `YeuCauThuGom` 1–1 `PhanCongThuGom`.
- `NhanVien` 1–n `PhanCongThuGom`.
- `YeuCauThuGom` 1–1 `KetQuaThuGom`.

Thiết kế Primary Key, Foreign Key, Navigation Property, kiểu dữ liệu và ràng buộc sẽ được hoàn thiện trong phần Model.

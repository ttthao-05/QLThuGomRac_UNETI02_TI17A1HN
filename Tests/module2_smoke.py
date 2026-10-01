"""Họ và tên: Phạm Văn Hào
Mã sinh viên: 23103100041
Nội dung thực hiện: Module 2 – Kiểm thử Module 2 (§6.1–§6.5)
"""

"""HTTP + SQL Server integration checks. Run only against the dedicated test database.
Start app as documented in Docs/Module2.md, then: python Tests/module2_smoke.py
Uses only Python's standard library and sqlcmd. Does not touch the default app database.
"""
import concurrent.futures
import datetime
import html
import http.cookiejar
import re
import subprocess
import urllib.error
import urllib.parse
import urllib.request
import uuid

BASE = "http://127.0.0.1:5187"
DB = "QLThuGomRac_Module2_Test_20261001"


def sql(query):
    return subprocess.check_output(["sqlcmd", "-S", r"(localdb)\MSSQLLocalDB", "-d", DB, "-E", "-I", "-b", "-h", "-1", "-W", "-Q", "SET NOCOUNT ON; " + query], text=True).strip()


class Browser:
    def __init__(self):
        self.client = urllib.request.build_opener(urllib.request.HTTPCookieProcessor(http.cookiejar.CookieJar()))

    def request(self, path, data=None):
        body = urllib.parse.urlencode(data).encode() if data is not None else None
        try:
            response = self.client.open(BASE + path, body)
        except urllib.error.HTTPError as error:
            response = error
        return response.status, response.geturl(), html.unescape(response.read().decode())

    def token(self, path):
        status, _, body = self.request(path)
        assert status == 200, (path, status)
        return re.search(r'name="__RequestVerificationToken" type="hidden" value="([^"]+)"', body).group(1)

    def post(self, path, data, token_path=None):
        return self.request(path, {**data, "__RequestVerificationToken": self.token(token_path or path)})

    def login(self, name):
        status, url, _ = self.post("/Account/Login", {"TenDangNhap": name, "MatKhau": "123456"})
        assert status == 200 and not url.endswith("/Account/Login"), (status, url)


def check(name, condition):
    assert condition, name
    print("PASS", name)


def main():
    # Guard: seed data must exist in the explicitly named disposable DB.
    assert int(sql("SELECT COUNT(*) FROM NguoiDans")) >= 10
    guest, citizen, other, admin = Browser(), Browser(), Browser(), Browser()
    check("anonymous redirected to login", guest.request("/YeuCauThuGom/Create")[1].endswith("/Account/Login"))
    citizen.login("nd01")
    other.login("nd02")
    admin.login("admin")
    check("citizen cannot access admin list", citizen.request("/NguoiDan")[0] == 403)
    check("admin list/search/details render", admin.request("/NguoiDan?tuKhoa=0903000001")[0] == 200 and admin.request("/NguoiDan/Details/1")[0] == 200)
    check("admin cannot register as citizen", admin.request("/YeuCauThuGom/Create")[0] == 403)
    check("CSRF required", citizen.request("/YeuCauThuGom/Create", {})[0] == 400)

    marker = "module2-" + uuid.uuid4().hex
    form = {"MaKhuVuc": "1", "DiaChiThuGom": marker, "SoDienThoaiLienHe": "0903000001",
            "NgayMongMuonThuGom": str(datetime.date.today() + datetime.timedelta(days=2)), "KhungGio": "08:00 - 10:00",
            "ChiTiet[0].MaLoaiRac": "1", "ChiTiet[0].SoLuongDuKien": "2.5",
            "ChiTiet[1].MaLoaiRac": "2", "ChiTiet[1].SoLuongDuKien": "3"}
    count_before = int(sql("SELECT COUNT(*) FROM YeuCauThuGoms"))
    invalid_cases = [
        ("past date", {"NgayMongMuonThuGom": "2000-01-01"}),
        ("zero quantity", {"ChiTiet[0].SoLuongDuKien": "0"}),
        ("negative quantity", {"ChiTiet[0].SoLuongDuKien": "-1"}),
        ("unknown waste", {"ChiTiet[0].MaLoaiRac": "999999"}),
        ("unknown area", {"MaKhuVuc": "999999"}),
        ("duplicate waste", {"ChiTiet[1].MaLoaiRac": "1"}),
        ("invalid slot", {"KhungGio": "midnight"}),
        ("invalid phone", {"SoDienThoaiLienHe": "abc"}),
        ("blank address", {"DiaChiThuGom": "   "}),
    ]
    for name, changes in invalid_cases:
        status, url, body = citizen.post("/YeuCauThuGom/Create", {**form, **changes})
        check(name + " rejected", status == 200 and url.endswith("/Create") and "validation-summary-errors" in body)
    no_waste = {k: v for k, v in form.items() if not k.startswith("ChiTiet")}
    check("empty waste rejected", "ít nhất một loại rác" in citizen.post("/YeuCauThuGom/Create", no_waste)[2])
    check("invalid submissions did not save", int(sql("SELECT COUNT(*) FROM YeuCauThuGoms")) == count_before)

    sql("UPDATE LoaiRacs SET DangThuGom=0 WHERE MaLoaiRac=1")
    try:
        check("inactive waste rejected", "ngừng thu gom" in citizen.post("/YeuCauThuGom/Create", form)[2])
    finally:
        sql("UPDATE LoaiRacs SET DangThuGom=1 WHERE MaLoaiRac=1")
    sql("UPDATE KhuVucs SET TrangThai=2 WHERE MaKhuVuc=1")
    try:
        check("inactive area rejected", "ngừng hoạt động" in citizen.post("/YeuCauThuGom/Create", form)[2])
    finally:
        sql("UPDATE KhuVucs SET TrangThai=1 WHERE MaKhuVuc=1")

    status, url, body = citizen.post("/YeuCauThuGom/Create", {**form, "MaNguoiDan": "2", "TrangThai": "5"})
    check("valid multi-waste registration", status == 200 and "/Details/" in url and "Chờ xác nhận" in body)
    request_id = int(url.rsplit("/", 1)[1])
    check("two details saved atomically", sql(f"SELECT COUNT(*) FROM ChiTietYeuCaus WHERE MaYeuCau={request_id}") == "2")
    check("owner and status cannot be forged", sql(f"SELECT COUNT(*) FROM YeuCauThuGoms y JOIN NguoiDans n ON y.MaNguoiDan=n.MaNguoiDan JOIN TaiKhoans t ON n.MaTaiKhoan=t.MaTaiKhoan WHERE y.MaYeuCau={request_id} AND t.TenDangNhap='nd01' AND y.TrangThai=1") == "1")
    check("other citizen cannot read details", other.request(f"/YeuCauThuGom/Details/{request_id}")[0] == 404)
    check("other citizen list excludes request", f"/Details/{request_id}\"" not in other.request("/YeuCauThuGom")[2])
    check("normalized duplicate rejected", "đã đăng ký" in citizen.post("/YeuCauThuGom/Create", {**form, "DiaChiThuGom": "  " + marker.upper() + "  "})[2])
    check("referenced area cannot be deleted", admin.post("/KhuVuc/Delete/1", {}, "/KhuVuc/Index")[0] == 200 and sql("SELECT COUNT(*) FROM KhuVucs WHERE MaKhuVuc=1") == "1")

    for state in range(1, 8):
        sql(f"UPDATE YeuCauThuGoms SET TrangThai={state} WHERE MaYeuCau={request_id}")
        check(f"history filter status {state}", f"/Details/{request_id}\"" in citizen.request(f"/YeuCauThuGom?trangThai={state}")[2])
    check("cancelled request may be registered again", "/Details/" in citizen.post("/YeuCauThuGom/Create", form)[1])

    profile = {"HoTen": "Người dân 01", "NgaySinh": "2000-01-01", "GioiTinh": "1", "SoDienThoai": "0903000001", "Email": "nd01@example.com", "DiaChi": "Địa chỉ thử nghiệm"}
    for name, change in [("blank name", {"HoTen": "  "}), ("future birth", {"NgaySinh": "2999-01-01"}), ("email", {"Email": "bad"}), ("phone", {"SoDienThoai": "bad"}), ("gender", {"GioiTinh": "99"})]:
        check("profile rejects " + name, "validation-summary-errors" in citizen.post("/HoSo", {**profile, **change})[2])
    check("profile update works", "Đã cập nhật" in citizen.post("/HoSo", {**profile, "MaTaiKhoan": "9", "TrangThai": "2"})[2])
    check("profile status cannot be forged", sql("SELECT TrangThai FROM NguoiDans WHERE MaTaiKhoan=(SELECT MaTaiKhoan FROM TaiKhoans WHERE TenDangNhap='nd01')") == "1")
    person_id = sql("SELECT MaNguoiDan FROM NguoiDans WHERE MaTaiKhoan=(SELECT MaTaiKhoan FROM TaiKhoans WHERE TenDangNhap='nd01')")
    check("invalid admin status rejected", admin.post(f"/NguoiDan/CapNhatTrangThai/{person_id}", {"trangThai": "99"}, f"/NguoiDan/Details/{person_id}")[0] == 400)
    admin.post(f"/NguoiDan/CapNhatTrangThai/{person_id}", {"trangThai": "2"}, f"/NguoiDan/Details/{person_id}")
    try:
        check("locked profile cannot register", citizen.request("/YeuCauThuGom/Create")[0] == 403)
        check("locked profile cannot edit", citizen.post("/HoSo", profile)[0] == 403)
    finally:
        admin.post(f"/NguoiDan/CapNhatTrangThai/{person_id}", {"trangThai": "1"}, f"/NguoiDan/Details/{person_id}")
    sql("UPDATE TaiKhoans SET TrangThai=2 WHERE TenDangNhap='nd01'")
    try:
        check("existing session blocked after account lock", citizen.request("/YeuCauThuGom")[0] == 403)
    finally:
        sql("UPDATE TaiKhoans SET TrangThai=1 WHERE TenDangNhap='nd01'")

    # Two independent sessions submit the same request concurrently.
    second = Browser()
    second.login("nd01")
    concurrent_form = {**form, "DiaChiThuGom": marker + "-parallel"}
    tokens = [b.token("/YeuCauThuGom/Create") for b in (citizen, second)]
    with concurrent.futures.ThreadPoolExecutor() as pool:
        results = list(pool.map(lambda item: item[0].request("/YeuCauThuGom/Create", {**concurrent_form, "__RequestVerificationToken": item[1]}), zip((citizen, second), tokens)))
    check("concurrent submissions return usable responses", all(result[0] == 200 for result in results))
    check("concurrent duplicate saved only once", sql(f"SELECT COUNT(*) FROM YeuCauThuGoms WHERE DiaChiThuGom='{marker}-parallel'") == "1")
    new_login = "test_" + uuid.uuid4().hex[:16]
    status, url, _ = admin.post("/TaiKhoan/Create", {"TenDangNhap": new_login, "MatKhau": "123456", "HoTen": "Test Module 2", "SoDienThoai": "0903000099", "LoaiTaiKhoan": "3", "TrangThai": "1"})
    check("account creation creates citizen profile", status == 200 and not url.endswith("/Create") and sql(f"SELECT COUNT(*) FROM NguoiDans n JOIN TaiKhoans t ON n.MaTaiKhoan=t.MaTaiKhoan WHERE t.TenDangNhap='{new_login}'") == "1")
    newcomer = Browser()
    newcomer.login(new_login)
    check("new citizen can open registration", newcomer.request("/YeuCauThuGom/Create")[0] == 200)
    sql(f"UPDATE TaiKhoans SET LoaiTaiKhoan=2 WHERE TenDangNhap='{new_login}'")
    check("role changes invalidate old citizen access", newcomer.request("/YeuCauThuGom")[0] == 403)
    sql(f"UPDATE TaiKhoans SET LoaiTaiKhoan=3 WHERE TenDangNhap='{new_login}'; DELETE FROM NguoiDans WHERE MaTaiKhoan=(SELECT MaTaiKhoan FROM TaiKhoans WHERE TenDangNhap='{new_login}')")
    check("missing citizen profile cannot register", newcomer.request("/YeuCauThuGom/Create")[0] == 403)
    sql(f"DELETE FROM TaiKhoans WHERE TenDangNhap='{new_login}'")
    check("deleted account cannot use old session", newcomer.request("/YeuCauThuGom")[0] == 403)
    print("All Module 2 integration checks passed.")


if __name__ == "__main__":
    main()

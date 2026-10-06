// Họ và tên: Phạm Văn Hào
// Mã sinh viên: 23103100041
// Nội dung thực hiện: Module 2 – Thao tác loại rác khi đăng ký (§6.3)

(() => {
    const rows = document.getElementById('waste-rows');
    function reindex() {
        rows.querySelectorAll('.waste-row').forEach((row, index) => {
            row.querySelectorAll('input, select').forEach(input => {
                const field = input.dataset.field || input.name.split('.').pop();
                input.name = `ChiTiet[${index}].${field}`;
                input.id = `ChiTiet_${index}__${field}`;
                input.parentElement.querySelector('label').htmlFor = input.id;
            });
        });
    }
    document.getElementById('add-waste').addEventListener('click', () => {
        rows.append(document.getElementById('waste-template').content.cloneNode(true));
        reindex();
    });
    rows.addEventListener('click', event => {
        if (event.target.closest('.remove-waste')) {
            event.target.closest('.waste-row').remove();
            reindex();
        }
    });
})();

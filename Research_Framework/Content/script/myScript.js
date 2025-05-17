// เมื่อเอกสารโหลดเสร็จแล้ว
document.addEventListener('DOMContentLoaded', function () {
    // เรียกใช้ฟังก์ชันเพื่อไฮไลท์เมนูปัจจุบัน
    highlightCurrentMenu();
});

// ฟังก์ชันสำหรับไฮไลท์เมนูที่ตรงกับหน้าปัจจุบัน
function highlightCurrentMenu() {
    // หาชื่อไฟล์ของหน้าปัจจุบัน
    var currentPage = window.location.pathname.split('/').pop();
    
    // หาลิงก์เมนูทั้งหมด
    var menuLinks = document.querySelectorAll('.nav-link.sidebarItem');
    
    // ตรวจสอบแต่ละลิงก์
    menuLinks.forEach(function(link) {
        // ดึง href ของลิงก์
        var href = link.getAttribute('href');
        
        // ถ้า href ตรงกับหน้าปัจจุบัน
        if (href === currentPage) {
            // เพิ่ม class active
            link.classList.add('active');
        } else {
            // ลบ class active
            link.classList.remove('active');
        }
    });
}

// ฟังก์ชันสำหรับเปลี่ยนสถานะของปุ่มเมนู (สำหรับรองรับโค้ดเดิม)
function activateButton(element) {
    // ลบ class active จากเมนูทั้งหมด
    document.querySelectorAll('.nav-link.sidebarItem').forEach(function(item) {
        item.classList.remove('active');
    });
    
    // เพิ่ม class active ให้กับปุ่มที่ถูกคลิก
    element.classList.add('active');
    
    return true;
}
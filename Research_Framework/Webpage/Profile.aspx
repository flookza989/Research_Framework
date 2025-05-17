<%@ Page Title="" Language="C#" MasterPageFile="~/Webpage/Layout.Master" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeBehind="Profile.aspx.cs" Inherits="Research_Framework.Webpage.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="contentHead">
        <asp:Label runat="server" Text="ข้อมูลส่วนตัว"></asp:Label>
    </div>
    <div class="row contentBody">
        <div class="col-12 d-flex flex-column align-items-center text-center">
            <div>
                <asp:Image ID="ProfileImage" runat="server" Style="width: 250px; border-radius: 50px;" />
            </div>
            <div>
                <input type="file" id="fileUpload" accept="image/*" class="d-none" onchange="uploadImage(this.files)" />
                <asp:LinkButton ID="Btn_uploadImage" CssClass="buttonNormal btn mt-4" runat="server" OnClientClick="document.getElementById('fileUpload').click(); return false;">
            <span class="fa-solid fa-image"/> อัพโหลดรูป
                </asp:LinkButton>
            </div>
        </div>
        <div class="col-6 mt-4">
            <asp:Label runat="server" CssClass="labelContent" Text="ชื่อ"></asp:Label>
            <input id="Tb_name" type="text" runat="server" class="form-control textboxContent" placeholder="ชื่อ" onkeypress="return isThaiLanguage(event)"/>
        </div>
        <div class="col-6 mt-4">
            <asp:Label runat="server" CssClass="labelContent" Text="นามสกุล"></asp:Label>
            <input id="Tb_lname" type="text" runat="server" class="form-control textboxContent" placeholder="นามสกุล" onkeypress="return isThaiLanguage(event)"/>

        </div>
        <div class="col-6 mt-2">
            <asp:Label runat="server" CssClass="labelContent" Text="รหัสนักศึกษา"></asp:Label>
            <asp:TextBox ID="Tb_username" runat="server" CssClass="form-control textboxContent" placeholder="รหัสนักศึกษา" Enabled="false"></asp:TextBox>
        </div>
        <div class="col-6 mt-2">
            <asp:Label runat="server" CssClass="labelContent" Text="รหัสผ่าน"></asp:Label>
            <input id="Tb_password" type="password" runat="server" class="form-control textboxContent" placeholder="รหัสผ่าน" />
        </div>
        <div class="col-6 mt-2">
            <asp:Label runat="server" CssClass="labelContent" Text="คณะ"></asp:Label>
            <asp:TextBox ID="Tb_faculty" runat="server" CssClass="form-control textboxContent" placeholder="คณะ" Enabled="false"></asp:TextBox>
        </div>
        <div class="col-6 mt-2">
            <asp:Label runat="server" CssClass="labelContent" Text="สาขา"></asp:Label>
            <asp:TextBox ID="Tb_branch" runat="server" CssClass="form-control textboxContent" placeholder="สาขา" Enabled="false"></asp:TextBox>
        </div>
        <div class="col-12 d-flex flex-column align-items-end text-center">
            <asp:LinkButton ID="Btn_save" CssClass="buttonNormal btn mt-4" runat="server" OnClientClick="SaveProfile(); return false;">
                <span class="fa-solid fa-floppy-disk"/> บันทึก
            </asp:LinkButton>
        </div>
    </div>

    <script type="text/javascript">

        window.onload = function () {
            localStorage.clear();
        };

        function isValidPassword(password) {
            // กำหนดเงื่อนไขสำหรับรหัสผ่านที่ถูกต้อง
            var passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
            return passwordRegex.test(password);
        }

        function isThaiLanguage(event) {
            var charCode = event.which || event.keyCode;
            var charValue = String.fromCharCode(charCode);
            var thaiRegex = /^[\u0E00-\u0E7F]+$/; // Regular Expression สำหรับตัวอักษรภาษาไทย

            if (!thaiRegex.test(charValue)) {
                event.preventDefault();
                return false;
            }

            return true;
        }

        function uploadImage(files) {
            if (files.length === 0) {
                alert("กรุณาเลือกไฟล์ภาพก่อน");
                return;
            }

            var reader = new FileReader();

            reader.onload = function (e) {
                var imageURL = e.target.result;
                localStorage.setItem("profileImage", imageURL);
                $("#<%= ProfileImage.ClientID %>").attr("src", imageURL);
            };

            reader.readAsDataURL(files[0]);
        }

        function SaveProfile() {
            var profileData = {
                Name: $("#<%= Tb_name.ClientID %>").val(),
        LastName: $("#<%= Tb_lname.ClientID %>").val(),
        Password: $("#<%= Tb_password.ClientID %>").val(),
        Image: null
    };

    // ตรวจสอบความถูกต้องของรหัสผ่าน
    if (!isValidPassword(profileData.Password)) {
        Swal.fire({
            icon: 'error',
            title: 'รหัสผ่านต้องมีความยาวอย่างน้อย 8 ตัวอักษร และประกอบด้วยตัวอักษรพิมพ์ใหญ่ ตัวอักษรพิมพ์เล็ก ตัวเลข และอักขระพิเศษ',
            showConfirmButton: true
        });
        return;
    }

    // สร้างฟังก์ชันสำหรับส่งข้อมูลไปยัง server
    function sendDataToServer(profileData) {
        $.ajax({
            type: "POST",
            url: "Profile.aspx/SaveProfile",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ profileData: profileData }),
            dataType: "json",
            success: function (response) {
                if (response.d) {
                    Swal.fire({
                        icon: 'success',
                        title: 'บันทึกข้อมูลสำเร็จ',
                        showConfirmButton: true,
                        didClose: () => {
                            location.reload();
                        }
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'เกิดข้อผิดพลาดในการบันทึกข้อมูล',
                        showConfirmButton: false,
                        timer: 1500
                    });
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", error);
                console.log("Response Text:", xhr.responseText);
                Swal.fire({
                    icon: 'error',
                    title: 'เกิดข้อผิดพลาดในการเชื่อมต่อ',
                    text: 'กรุณาลองใหม่อีกครั้ง',
                    showConfirmButton: true
                });
            }
        });
    }

    var fileInput = document.getElementById('fileUpload');
    var file = fileInput.files[0];

    if (file) {
        // ถ้ามีการเลือกไฟล์
        var reader = new FileReader();
        reader.onload = function (event) {
            var arrayBuffer = event.target.result;
            var byteArray = new Uint8Array(arrayBuffer);
            var base64String = btoa(String.fromCharCode.apply(null, byteArray));
            
                    // อัพเดทค่า Image ใน profileData
                    profileData.Image = base64String;

                    console.log('Image converted to base64');
                    console.log('Base64 length:', base64String.length);
                    console.log('Sample:', base64String.substring(0, 50) + '...');

                    // ส่งข้อมูลหลังจากแปลงรูปภาพเสร็จแล้ว
                    sendDataToServer(profileData);
                };

                reader.onerror = function (error) {
                    console.error('Error reading file:', error);
                    Swal.fire({
                        icon: 'error',
                        title: 'เกิดข้อผิดพลาดในการอ่านไฟล์',
                        text: 'กรุณาลองใหม่อีกครั้ง',
                        showConfirmButton: true
                    });
                };

                // เริ่มอ่านไฟล์
                reader.readAsArrayBuffer(file);
            } else {
                // ถ้าไม่มีการเลือกไฟล์ใหม่ ส่งข้อมูลทันที
                sendDataToServer(profileData);
            }
        }

        // ฟังก์ชันตรวจสอบรหัสผ่าน (ถ้ายังไม่มี)
        function isValidPassword(password) {
            // ตรวจสอบความยาวอย่างน้อย 8 ตัวอักษร
            if (password.length < 8) return false;

            // ตรวจสอบว่ามีตัวอักษรพิมพ์ใหญ่
            if (!/[A-Z]/.test(password)) return false;

            // ตรวจสอบว่ามีตัวอักษรพิมพ์เล็ก
            if (!/[a-z]/.test(password)) return false;

            // ตรวจสอบว่ามีตัวเลข
            if (!/[0-9]/.test(password)) return false;

            // ตรวจสอบว่ามีอักขระพิเศษ
            if (!/[!@#$%^&*(),.?":{}|<>]/.test(password)) return false;

            return true;
        }




    </script>
</asp:Content>

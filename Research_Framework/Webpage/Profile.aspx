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
                Image: null // กำหนดให้ Image เป็น null ก่อน
            };

            var fileInput = document.getElementById('fileUpload');
            var file = fileInput.files[0];

            // ตรวจสอบความถูกต้องของรหัสผ่าน
            if (!isValidPassword(profileData.Password)) {
                Swal.fire({
                    icon: 'error',
                    title: 'รหัสผ่านต้องมีความยาวอย่างน้อย 8 ตัวอักษร และประกอบด้วยตัวอักษรพิมพ์ใหญ่ ตัวอักษรพิมพ์เล็ก ตัวเลข และอักขระพิเศษ',
                    showConfirmButton: true
                });
                return;
            }

            if (file) {
                var reader = new FileReader();
                reader.readAsArrayBuffer(file);

                reader.onload = function (event) {
                    var arrayBuffer = event.target.result;
                    var byteArray = new Uint8Array(arrayBuffer);
                    var base64String = btoa(String.fromCharCode.apply(null, byteArray));
                    profileData.Image = base64String;

                    // ส่งข้อมูลไปยังเซิร์ฟเวอร์ด้วย Ajax ตามโค้ดที่มีอยู่

                   
                };
            } else {

            }

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
                            showConfirmButton: true, // เปลี่ยนเป็น true เพื่อให้แสดงปุ่มยืนยัน
                            didClose: () => {
                                location.reload(); // โหลดหน้าเว็บใหม่หลังจากการบันทึกข้อมูลเรียบร้อยแล้วเมื่อผู้ใช้คลิกปิดเอง
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
                    alert("เกิดข้อผิดพลาดในการบันทึกข้อมูล: " + xhr.responseText);
                    console.log(xhr.responseText);
                }
            });
        }




    </script>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Webpage/Layout.Master" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeBehind="Profile.aspx.cs" Inherits="Research_Framework.Webpage.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="contentHead">
        <asp:Label runat="server" Text="ข้อมูลส่วนตัว"></asp:Label>
    </div>
    <div class="row contentBody">
        <!-- โซนรูปโปรไฟล์ -->
        <div class="col-12 d-flex flex-column align-items-center text-center">
            <div>
                <asp:Image ID="ProfileImage" runat="server" CssClass="profile-image" />
            </div>
            <div>
                <input type="file" id="fileUpload" accept="image/*" class="d-none" onchange="uploadImage(this.files)" />
                <asp:LinkButton ID="Btn_uploadImage" CssClass="buttonNormal btn mt-4" runat="server" OnClientClick="document.getElementById('fileUpload').click(); return false;">
                    <span class="fa-solid fa-image"/> อัพโหลดรูป
                </asp:LinkButton>
            </div>
        </div>

        <!-- โซนข้อมูลส่วนตัว -->
        <div class="col-12">
            <div class="card mt-4">
                <div class="card-header">
                    <h5 class="mb-0">แก้ไขข้อมูลส่วนตัว</h5>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-6">
                            <asp:Label runat="server" CssClass="labelContent" Text="ชื่อ"></asp:Label>
                            <input id="Tb_name" type="text" runat="server" class="form-control textboxContent" placeholder="ชื่อ" onkeypress="return isThaiLanguage(event)"/>
                        </div>
                        <div class="col-6">
                            <asp:Label runat="server" CssClass="labelContent" Text="นามสกุล"></asp:Label>
                            <input id="Tb_lname" type="text" runat="server" class="form-control textboxContent" placeholder="นามสกุล" onkeypress="return isThaiLanguage(event)"/>
                        </div>
                        <div class="col-6 mt-2">
                            <asp:Label runat="server" CssClass="labelContent" Text="รหัสนักศึกษา"></asp:Label>
                            <asp:TextBox ID="Tb_username" runat="server" CssClass="form-control textboxContent" placeholder="รหัสนักศึกษา" Enabled="false"></asp:TextBox>
                        </div>
                        <div class="col-6 mt-2">
                            <asp:Label runat="server" CssClass="labelContent" Text="คณะ"></asp:Label>
                            <asp:TextBox ID="Tb_faculty" runat="server" CssClass="form-control textboxContent" placeholder="คณะ" Enabled="false"></asp:TextBox>
                        </div>
                        <div class="col-6 mt-2">
                            <asp:Label runat="server" CssClass="labelContent" Text="สาขา"></asp:Label>
                            <asp:TextBox ID="Tb_branch" runat="server" CssClass="form-control textboxContent" placeholder="สาขา" Enabled="false"></asp:TextBox>
                        </div>
                    </div>
                    <div class="text-end mt-3">
                        <asp:LinkButton ID="Btn_save" CssClass="buttonNormal btn" runat="server" OnClientClick="SaveProfile(); return false;">
                            <span class="fa-solid fa-floppy-disk"/> บันทึกข้อมูล
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>

        <!-- โซนเปลี่ยนรหัสผ่าน -->
        <div class="col-12">
            <div class="card mt-4">
                <div class="card-header">
                    <h5 class="mb-0">เปลี่ยนรหัสผ่าน</h5>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-4">
                            <asp:Label runat="server" CssClass="labelContent" Text="รหัสผ่านปัจจุบัน"></asp:Label>
                            <input id="Tb_currentPassword" type="password" runat="server" class="form-control textboxContent" placeholder="รหัสผ่านปัจจุบัน"/>
                        </div>
                        <div class="col-md-4">
                            <asp:Label runat="server" CssClass="labelContent" Text="รหัสผ่านใหม่"></asp:Label>
                            <input id="Tb_newPassword" type="password" runat="server" class="form-control textboxContent" placeholder="รหัสผ่านใหม่"/>
                        </div>
                        <div class="col-md-4">
                            <asp:Label runat="server" CssClass="labelContent" Text="ยืนยันรหัสผ่านใหม่"></asp:Label>
                            <input id="Tb_confirmPassword" type="password" runat="server" class="form-control textboxContent" placeholder="ยืนยันรหัสผ่านใหม่"/>
                        </div>
                    </div>
                    <div class="text-end mt-3">
                        <asp:LinkButton ID="Btn_changePassword" CssClass="buttonNormal btn" runat="server" OnClientClick="ChangePassword(); return false;">
                            <span class="fa-solid fa-key"/> เปลี่ยนรหัสผ่าน
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        // ฟังก์ชันสำหรับอัพโหลดรูปและแก้ไขข้อมูลส่วนตัว
        function SaveProfile() {
            // ตรวจสอบข้อมูลให้ครบถ้วน
            var name = $("#<%= Tb_name.ClientID %>").val().trim();
            var lastName = $("#<%= Tb_lname.ClientID %>").val().trim();
            
            if (!name || !lastName) {
                Swal.fire({
                    icon: 'warning',
                    title: 'กรุณากรอกข้อมูลให้ครบถ้วน',
                    text: 'กรุณากรอกชื่อและนามสกุล',
                    showConfirmButton: true
                });
                return;
            }
            
            var profileData = {
                Name: name,
                LastName: lastName,
                Image: null
            };

            var fileInput = document.getElementById('fileUpload');
            var file = fileInput.files[0];

            if (file) {
                // แสดงข้อความกำลังอัพโหลด
                Swal.fire({
                    title: 'กำลังประมวลผลรูปภาพ...',
                    text: 'โปรดรอสักครู่',
                    didOpen: () => {
                        Swal.showLoading();
                    },
                    allowOutsideClick: false,
                    allowEscapeKey: false,
                    showConfirmButton: false
                });
                
                var reader = new FileReader();
                reader.onload = function (event) {
                    try {
                        var arrayBuffer = event.target.result;
                        var byteArray = new Uint8Array(arrayBuffer);
                        
                        // แปลงข้อมูลเป็น base64 โดยไม่ใช้ String.fromCharCode.apply เพื่อหลีกเลี่ยง stack overflow
                        // ใช้การแปลงเป็นชุดๆ แทน
                        var base64String = '';
                        var chunk = 8192; // ขนาด chunk ที่เหมาะสม
                        
                        for (var i = 0; i < byteArray.length; i += chunk) {
                            var slice = byteArray.subarray(i, Math.min(i + chunk, byteArray.length));
                            var tempArray = Array.from(slice);
                            base64String += String.fromCharCode.apply(null, tempArray);
                        }
                        
                        profileData.Image = btoa(base64String);
                        sendProfileData(profileData);
                    } catch (error) {
                        console.error("Error processing image:", error);
                        Swal.close(); // ปิด loading dialog
                        
                        Swal.fire({
                            icon: 'error',
                            title: 'เกิดข้อผิดพลาดในการประมวลผลรูปภาพ',
                            text: 'รูปภาพมีขนาดใหญ่เกินไป โปรดลองใช้รูปที่มีขนาดเล็กกว่านี้',
                            showConfirmButton: true
                        });
                    }
                };
                reader.readAsArrayBuffer(file);
            } else {
                // แสดงสถานะกำลังดำเนินการ
                Swal.fire({
                    title: 'กำลังบันทึกข้อมูล...',
                    text: 'โปรดรอสักครู่',
                    didOpen: () => {
                        Swal.showLoading();
                    },
                    allowOutsideClick: false,
                    allowEscapeKey: false,
                    showConfirmButton: false
                });
                
                sendProfileData(profileData);
            }
        }

        function sendProfileData(profileData) {
            $.ajax({
                type: "POST",
                url: "Profile.aspx/SaveProfile",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({ profileData: profileData }),
                dataType: "json",
                success: function (response) {
                    // ปิด loading dialog
                    Swal.close();
                    
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
                            showConfirmButton: true
                        });
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Error:", error);
                    
                    // ปิด loading dialog
                    Swal.close();
                    
                    Swal.fire({
                        icon: 'error',
                        title: 'เกิดข้อผิดพลาดในการบันทึกข้อมูล',
                        text: 'กรุณาลองใหม่อีกครั้งหรือติดต่อผู้ดูแลระบบ',
                        showConfirmButton: true
                    });
                }
            });
        }

        // ฟังก์ชันสำหรับเปลี่ยนรหัสผ่าน
        function ChangePassword() {
            var currentPassword = $("#<%= Tb_currentPassword.ClientID %>").val();
            var newPassword = $("#<%= Tb_newPassword.ClientID %>").val();
            var confirmPassword = $("#<%= Tb_confirmPassword.ClientID %>").val();

            if (!currentPassword || !newPassword || !confirmPassword) {
                Swal.fire({
                    icon: 'warning',
                    title: 'กรุณากรอกข้อมูลให้ครบถ้วน',
                    showConfirmButton: true
                });
                return false;
            }

            if (newPassword !== confirmPassword) {
                Swal.fire({
                    icon: 'error',
                    title: 'รหัสผ่านใหม่ไม่ตรงกัน',
                    showConfirmButton: true
                });
                return false;
            }

            if (!isValidPassword(newPassword)) {
                Swal.fire({
                    icon: 'error',
                    title: 'รหัสผ่านไม่ถูกต้องตามเงื่อนไข',
                    text: 'รหัสผ่านต้องมีความยาวอย่างน้อย 8 ตัวอักษร และประกอบด้วยตัวอักษรพิมพ์ใหญ่ ตัวอักษรพิมพ์เล็ก ตัวเลข และอักขระพิเศษ',
                    showConfirmButton: true
                });
                return false;
            }

            // แสดงสถานะกำลังดำเนินการ
            Swal.fire({
                title: 'กำลังเปลี่ยนรหัสผ่าน...',
                text: 'โปรดรอสักครู่',
                didOpen: () => {
                    Swal.showLoading();
                },
                allowOutsideClick: false,
                allowEscapeKey: false,
                showConfirmButton: false
            });

            $.ajax({
                type: "POST",
                url: "Profile.aspx/ChangePassword",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify({
                    currentPassword: currentPassword,
                    newPassword: newPassword
                }),
                dataType: "json",
                success: function (response) {
                    // ปิด loading dialog
                    Swal.close();
                    
                    if (response.d) {
                        Swal.fire({
                            icon: 'success',
                            title: 'เปลี่ยนรหัสผ่านสำเร็จ',
                            showConfirmButton: true,
                            didClose: function() {
                                document.getElementById('<%= Tb_currentPassword.ClientID %>').value = '';
                                document.getElementById('<%= Tb_newPassword.ClientID %>').value = '';
                                document.getElementById('<%= Tb_confirmPassword.ClientID %>').value = '';
                            }
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'รหัสผ่านปัจจุบันไม่ถูกต้อง',
                            showConfirmButton: true
                        });
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Error:", error);
                    
                    // ปิด loading dialog
                    Swal.close();
                    
                    Swal.fire({
                        icon: 'error',
                        title: 'เกิดข้อผิดพลาดในการเปลี่ยนรหัสผ่าน',
                        text: 'กรุณาลองใหม่อีกครั้งหรือติดต่อผู้ดูแลระบบ',
                        showConfirmButton: true
                    });
                }
            });

            return false;
        }

        function isValidPassword(password) {
            var passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
            return passwordRegex.test(password);
        }

        // ฟังก์ชันสำหรับตรวจสอบการป้อนภาษาไทย
        function isThaiLanguage(event) {
            var keyCode = event.keyCode || event.which;
            
            // อนุญาตให้ใช้ปุ่ม Backspace, Delete, Arrow keys, Space
            if (keyCode == 8 || keyCode == 46 || keyCode == 37 || keyCode == 39 || keyCode == 32) {
                return true;
            }
            
            // ช่วงรหัสภาษาไทย (Unicode: 0E00-0E7F)
            if (keyCode >= 0x0E00 && keyCode <= 0x0E7F) {
                return true;
            }
            
            // อนุญาตให้ใช้ตัวอักษรภาษาอังกฤษด้วย
            if ((keyCode >= 65 && keyCode <= 90) || (keyCode >= 97 && keyCode <= 122)) {
                return true;
            }

            // เพิ่มค่า return false สำหรับกรณีอื่นๆ
            return false;
        }

        function uploadImage(files) {
            if (files.length === 0) {
                Swal.fire({
                    icon: 'warning',
                    title: 'กรุณาเลือกไฟล์ภาพก่อน',
                    showConfirmButton: true
                });
                return;
            }
            
            var file = files[0];
            var maxSizeInMB = 2;
            var maxSizeInBytes = maxSizeInMB * 1024 * 1024; // 2MB
            
            // ตรวจสอบขนาดไฟล์
            if (file.size > maxSizeInBytes) {
                Swal.fire({
                    icon: 'error',
                    title: 'ขนาดไฟล์ใหญ่เกินไป',
                    text: 'ขนาดไฟล์ต้องไม่เกิน ' + maxSizeInMB + ' MB',
                    showConfirmButton: true
                });
                return;
            }
            
            // ตรวจสอบประเภทไฟล์
            var allowedTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/jpg'];
            if (!allowedTypes.includes(file.type)) {
                Swal.fire({
                    icon: 'error',
                    title: 'ประเภทไฟล์ไม่ถูกต้อง',
                    text: 'กรุณาอัพโหลดไฟล์ภาพเท่านั้น (JPEG, PNG, GIF)',
                    showConfirmButton: true
                });
                return;
            }

            var reader = new FileReader();
            reader.onload = function (e) {
                var imageURL = e.target.result;
                $("#<%= ProfileImage.ClientID %>").attr("src", imageURL);
            };
            reader.readAsDataURL(file);
        }
    </script>
</asp:Content>

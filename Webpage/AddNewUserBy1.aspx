<%@ Page Title="" Language="C#" MasterPageFile="~/Webpage/Layout.Master" AutoEventWireup="true" CodeBehind="AddNewUserBy1.aspx.cs" Inherits="Research_Framework.Webpage.AddNewUserBy1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="contentHead">
        <asp:Label runat="server" Text="รายละเอียดวิทยานิพนธ์"></asp:Label>
    </div>
    <div class="row contentBody">
        <div class="col-6">
            <asp:Label runat="server" CssClass="labelContent" Text="รหัสผู้ใช้"></asp:Label>
            <asp:TextBox ID="Tb_username" runat="server" CssClass="form-control textboxContent" placeholder="รหัสผู้ใช้"></asp:TextBox>
        </div>
        <div class="col-6">
            <asp:Label runat="server" CssClass="labelContent" Text="รหัสผ่าน"></asp:Label>
            <asp:TextBox ID="Tb_password" runat="server" CssClass="form-control textboxContent" placeholder="รหัสผ่าน"></asp:TextBox>
        </div>
        <div class="col-6 mt-2">
            <asp:Label runat="server" CssClass="labelContent" Text="ชื่อ"></asp:Label>
            <asp:TextBox ID="Tb_name" runat="server" CssClass="form-control textboxContent" placeholder="ชื่อ"></asp:TextBox>
        </div>
        <div class="col-6 mt-2">
            <asp:Label runat="server" CssClass="labelContent" Text="นามสกุล"></asp:Label>
            <asp:TextBox ID="Tb_lname" runat="server" CssClass="form-control textboxContent" placeholder="นามสกุล"></asp:TextBox>
        </div>

        <div class="col-6 mt-2">
            <asp:Label runat="server" CssClass="labelContent" Text="คณะ"></asp:Label>
            <select id="Ddl_faculty" runat="server" class="form-select"></select>
        </div>
        <div class="col-6 mt-2">
            <asp:Label runat="server" CssClass="labelContent" Text="สาขา"></asp:Label>
            <select id="Ddl_branch" runat="server" class="form-select"></select>
        </div>
        <div class="col-6 mt-2">
            <asp:Label runat="server" CssClass="labelContent" Text="การอนุญาติ"></asp:Label>
            <select id="Ddl_permission" runat="server" class="form-select">
                <option value="admin">admin</option>
                <option value="teacher">teacher</option>
                <option value="student">student</option>
            </select>
        </div>

        <div class="col-12 mt-3">
            <button id="BtnSave" type="button" class="btn btn-primary buttonNormal" onclick="return saveUser();">
                <i class="fa-solid fa-floppy-disk me-1"></i>บันทึก
            </button>
        </div>
    </div>
    <script>
        $(document).ready(function () {
            // โหลดข้อมูลคณะ
            loadFaculties();

            // เพิ่ม event listener สำหรับ dropdown ของคณะ
            $("#<%= Ddl_faculty.ClientID %>").change(function () {
                var selectedFacultyId = $(this).val();
                loadBranches(selectedFacultyId);
            });

            function loadFaculties() {
                $.ajax({
                    type: "POST",
                    url: "AddNewUserBy1.aspx/GetFaculties",
                    data: '{}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var Ddl_faculty = document.getElementById("<%= Ddl_faculty.ClientID %>");
                        var faculties = JSON.parse(response.d);
                        for (var i = 0; i < faculties.length; i++) {
                            var option = document.createElement("option");
                            option.value = faculties[i].id;
                            option.text = faculties[i].name;
                            Ddl_faculty.appendChild(option);
                        }
                        // โหลดสาขาทั้งหมดเมื่อโหลดคณะเสร็จ
                        loadAllBranches();
                    },
                    failure: function (response) {
                        alert(response.d);
                    }
                });
            }

            function loadAllBranches() {
                $.ajax({
                    type: "POST",
                    url: "AddNewUserBy1.aspx/GetAllBranches",
                    data: '{}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var Ddl_branch = document.getElementById("<%= Ddl_branch.ClientID %>");
                        Ddl_branch.innerHTML = ''; // ล้างตัวเลือกที่มีอยู่
                        var branches = JSON.parse(response.d);
                        for (var i = 0; i < branches.length; i++) {
                            var option = document.createElement("option");
                            option.value = branches[i].id;
                            option.text = branches[i].name;
                            Ddl_branch.appendChild(option);
                        }
                    },
                    failure: function (response) {
                        alert(response.d);
                    }
                });
            }

            function loadBranches(facultyId) {
                $.ajax({
                    type: "POST",
                    url: "AddNewUserBy1.aspx/GetBranches",
                    data: JSON.stringify({ facultyId: facultyId }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var Ddl_branch = document.getElementById("<%= Ddl_branch.ClientID %>");
                        Ddl_branch.innerHTML = ''; // ล้างตัวเลือกที่มีอยู่
                        var branches = JSON.parse(response.d);
                        for (var i = 0; i < branches.length; i++) {
                            var option = document.createElement("option");
                            option.value = branches[i].id;
                            option.text = branches[i].name;
                            Ddl_branch.appendChild(option);
                        }
                    },
                    failure: function (response) {
                        alert(response.d);
                    }
                });
            }

            $(document).ready(function () {
                $("#BtnSave").click(function (e) {
                    e.preventDefault();
                    console.log("Button clicked");
                    saveUser();
                });
            });

            function saveUser() {
                console.log("saveUser function called");

                var username = $("#<%= Tb_username.ClientID %>").val();
                var password = $("#<%= Tb_password.ClientID %>").val();
                var name = $("#<%= Tb_name.ClientID %>").val();
                var lname = $("#<%= Tb_lname.ClientID %>").val();
                var branchId = $("#<%= Ddl_branch.ClientID %>").val();
                var permission = $("#<%= Ddl_permission.ClientID %>").val();

                console.log("User data:", { username, password, name, lname, branchId, permission });

                if (!username || !password || !name || !lname || !branchId || !permission) {
                    alert("กรุณากรอกข้อมูลให้ครบทุกช่อง");
                    return false;
                }

                var userData = {
                    username: username,
                    password: password,
                    name: name,
                    lname: lname,
                    branch_id: parseInt(branchId),
                    permission: permission
                };

                console.log("Sending data to server:", userData);

                $.ajax({
                    type: "POST",
                    url: "AddNewUserBy1.aspx/SaveUser",
                    data: JSON.stringify({ userData: userData }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        console.log("Server response:", response);
                        if (response.d === "success") {
                            alert("บันทึกข้อมูลสำเร็จ");
                            // ล้างข้อมูลในฟอร์ม
                            $("#Tb_username").val("");
                            $("#Tb_password").val("");
                            $("#Tb_name").val("");
                            $("#Tb_lname").val("");
                            $("#Ddl_branch").val("");
                            $("#Ddl_permission").val("");
                        } else {
                            alert("เกิดข้อผิดพลาดในการบันทึกข้อมูล: " + response.d);
                        }
                    },
                    error: function (xhr, status, error) {
                        console.log("AJAX error:", xhr, status, error);
                        alert("เกิดข้อผิดพลาดในการส่งข้อมูล: " + error);
                    }
                });

                return false; // ป้องกันการ submit form แบบปกติ
            }
        });
    </script>
</asp:Content>

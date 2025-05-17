<%@ Page Title="" Language="C#" MasterPageFile="~/Webpage/Layout.Master" AutoEventWireup="true" CodeBehind="MangeUser.aspx.cs" Inherits="Research_Framework.Webpage.MangeUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="contentHead">
        จัดการผู้ใช้
    </div>
    <div class="row contentBody">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div class="col-6 d-flex align-items-center">
            <label class="">ค้นหา</label>
            <input id="Tb_search" type="text" class="form-control textboxContent ms-2" placeholder="ค้นหา" onkeypress="HandleKeyPress(event);" />
        </div>
        <div class="mt-2">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <asp:GridView ID="Gv_User" runat="server" CssClass="custom-gridview" AutoGenerateColumns="False">
                        <Columns>
                            <asp:BoundField DataField="user_id" HeaderText="user_id" SortExpression="user_od" Visible="false" />
                            <asp:BoundField DataField="name" HeaderText="ชื่อ" SortExpression="name" />
                            <asp:BoundField DataField="lname" HeaderText="นามสกุล" SortExpression="lname" />
                            <asp:BoundField DataField="username" HeaderText="รหัสนักศึกษา" SortExpression="username" />
                            <asp:BoundField DataField="password" HeaderText="รหัสผ่าน" SortExpression="password" />
                            <asp:BoundField DataField="faculty_name" HeaderText="คณะ" SortExpression="faculty_name" />
                            <asp:BoundField DataField="branch_name" HeaderText="สาขา" SortExpression="branch_name" />
                            <asp:BoundField DataField="permission" HeaderText="การอนุญาต" SortExpression="permission" />
                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <div class="text-end">
                                        <asp:LinkButton ID="btn_edit" runat="server" CommandName="EditClick" CssClass="buttonNormal btn me-2 px-3" OnClientClick='<%# "ShowEditForm(" + Eval("user_id") + ")" %>'>
                                            <span class="fa-solid fa-pen-to-square me-1"></span> แก้ไข
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btn_remove" runat="server" CommandName="RemoveClick" CssClass="buttonRemove btn me-2 px-3" OnClientClick='<%# "RemoveUser(" + Eval("user_id") + ")" %>'>
                                            <span class="fa-solid fa-trash me-1"></span> ลบ
                                        </asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <div id="overlay" class="overlay"></div>

    <div id="editForm" class="editForm">
        <div class="contentHead">
            แก้ไขผู้ใช้
        </div>
        <div class="contentBody">
            <asp:HiddenField ID="UserIdHiddenField" runat="server" />
            <asp:HiddenField ID="HiddenEditBranchDropDownList" runat="server" />
            <label>ชื่อ:</label>
            <asp:TextBox ID="EditNameTextBox" runat="server" CssClass="form-control" /><br />
            <label>นามสกุล:</label>
            <asp:TextBox ID="EditLastNameTextBox" runat="server" CssClass="form-control" /><br />
            <label>รหัสนักศึกษา:</label>
            <asp:TextBox ID="EditUsernameTextBox" runat="server" CssClass="form-control" /><br />
            <label>รหัสผ่าน:</label>
            <asp:TextBox ID="EditPasswordTextBox" runat="server" CssClass="form-control" /><br />
            <label>คณะ:</label>
            <select id="EditFacultyDropDownList" runat="server" class="form-select"></select>
            <br />
            <label>สาขา:</label>
            <select id="EditBranchDropDownList" runat="server" class="form-select"></select>
            <br />
            <label>การอนุญาต:</label>
            <select id="EditPermissionDropDownList" runat="server" class="form-select">
                <option value="admin">admin</option>
                <option value="teacher">teacher</option>
                <option value="student">student</option>
            </select>
            <br />
            <asp:LinkButton ID="btn_save" runat="server" CssClass="buttonNormal btn me-2 px-3" OnClick="btn_save_Click">
                <span class="fa-solid fa-floppy-disk me-1"></span> บันทึก
            </asp:LinkButton>

            <button type="button" onclick="HideEditForm();" class="buttonWarning btn me-2 px-3">
                <span class="fa-solid fa-xmark me-1"></span>ยกเลิก</button>
        </div>
    </div>


    <script>
        window.onload = function () {
            // Add onchange event handler to the faculty dropdown list
            var facultyDropDownList = document.getElementById("<%= EditFacultyDropDownList.ClientID %>");
            if (facultyDropDownList) {
                facultyDropDownList.onchange = function () {
                    LoadBranches();
                };
            }
        }

        var branchDropDownList = document.getElementById("<%= EditBranchDropDownList.ClientID %>");
        if (branchDropDownList) {
            branchDropDownList.onchange = function () {
                updateHiddenValueFromDropDown();
            };
        }

        function HandleKeyPress(e) {
            if (e.keyCode === 13) { // Enter key
                SearchUsers();
                e.preventDefault(); // Prevent form submission
            }
        }

        function SearchUsers() {
            var searchText = document.getElementById("Tb_search").value;
            $.ajax({
                type: "POST",
                url: "MangeUser.aspx/SearchUsers",
                data: "{searchText: '" + searchText + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var userData = JSON.parse(response.d);
                    UpdateGridView(userData);
                },
                failure: function (response) {
                    alert("Failed to search users.");
                }
            });
        }

        function GetUpdatedUserData() {
            $.ajax({
                type: "POST",
                url: "MangeUser.aspx/GetUpdatedUserData",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var userData = JSON.parse(response.d);
                    UpdateGridView(userData);
                }
            });
        }


        function UpdateGridView(userData) {
            var gvUser = document.getElementById('<%= Gv_User.ClientID %>');
            var gvRows = gvUser.getElementsByTagName('tr');

            // Clear existing rows
            for (var i = gvRows.length - 1; i > 0; i--) {
                gvUser.deleteRow(i);
            }

            // Populate GridView with fetched data
            for (var i = 0; i < userData.length; i++) {
                var row = gvUser.insertRow(-1);
                var data = userData[i];
                row.insertCell(0).innerText = data.name;
                row.insertCell(1).innerText = data.lname;
                row.insertCell(2).innerText = data.username;
                row.insertCell(3).innerText = data.password;
                row.insertCell(4).innerText = data.faculty_name;
                row.insertCell(5).innerText = data.branch_name;
                row.insertCell(6).innerText = data.permission;
                var editCell = row.insertCell(7);
                editCell.innerHTML = '<div class="text-end">' +
                    '<a class="buttonNormal btn me-2 px-3" onclick="ShowEditForm(' + data.user_id + ');">' +
                    '<span class="fa-solid fa-pen-to-square me-1">' +
                    '</span> แก้ไข</a>' +
                    '<a class="buttonRemove btn me-2 px-3" onclick="RemoveUser(' + data.user_id + ');">' +
                    '<span class="fa-solid fa-solid fa-trash me-1 me-1">' +
                    '</span> ลบ</a>' +
                    '</div>';
            }
        }

        function updateHiddenValueFromDropDown() {
            var branchDropDownList = document.getElementById("<%= EditBranchDropDownList.ClientID %>");
                document.getElementById("<%= HiddenEditBranchDropDownList.ClientID %>").value = branchDropDownList.value;
        }

        function RemoveUser(userId) {
            // Use SweetAlert for confirmation
            Swal.fire({
                title: 'คุณแน่ใจว่าต้องการลบ?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'ลบ',
                cancelButtonText: 'ยกเลิก'
            }).then((result) => {
                if (result.isConfirmed) {
                    // If confirmed, make AJAX call to remove the user
                    $.ajax({
                        type: "POST",
                        url: "MangeUser.aspx/RemoveUser",
                        data: "{userId: '" + userId + "'}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            // Reload the GridView after successful removal
                            GetUpdatedUserData();
                            Swal.fire(
                                'ลบสำเร็จ',
                                '',
                                'success'
                            );
                        },
                        failure: function (response) {
                            alert("Failed to remove user.");
                        }
                    });
                }
            });
        }

        function HideEditForm() {
            // Hide the overlay
            document.getElementById("overlay").style.display = "none";
            // Hide the edit form
            document.getElementById("editForm").style.display = "none";
        }

        function ShowEditForm(userId) {
            // Show the overlay
            document.getElementById("overlay").style.display = "block";
            // Show the edit form
            document.getElementById("editForm").style.display = "block";

            // Make AJAX call to fetch user details
            $.ajax({
                type: "POST",
                url: "MangeUser.aspx/GetUserDetails",
                data: "{userId: '" + userId + "'}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var userDetails = JSON.parse(response.d);
                    document.getElementById("<%= UserIdHiddenField.ClientID %>").value = userDetails.user_id;
                        document.getElementById("<%= EditNameTextBox.ClientID %>").value = userDetails.name;
                        document.getElementById("<%= EditLastNameTextBox.ClientID %>").value = userDetails.lname;
                        document.getElementById("<%= EditUsernameTextBox.ClientID %>").value = userDetails.username;
                        document.getElementById("<%= EditPasswordTextBox.ClientID %>").value = userDetails.password;

                        var facultyDropDownList = document.getElementById("<%= EditFacultyDropDownList.ClientID %>");
                        var options = facultyDropDownList.getElementsByTagName("option");

                        let newOpitons = [];
                        for (var i = 0; i < options.length; i++) {
                            var faculty = options[i];
                            var newOpiton = document.createElement("option");
                            newOpiton.value = faculty.value;
                            newOpiton.text = faculty.value;
                            newOpitons.push(newOpiton);

                        }

                        // Clear existing options
                        facultyDropDownList.innerHTML = "";

                        // Create and add new options
                        for (var i = 0; i < newOpitons.length; i++) {
                            var faculty = newOpitons[i];
                            var option = document.createElement("option");
                            option.value = faculty.value;
                            option.text = faculty.text; // ใช้ text ที่ได้จากการสร้าง option ใหม่

                            // Select the current user's faculty
                            if (faculty.value === userDetails.faculty_name) {
                                option.selected = true;
                            }

                            facultyDropDownList.appendChild(option);
                        }
                        LoadBranches(userDetails.branch_name);

                        document.getElementById("<%= EditPermissionDropDownList.ClientID %>").value = userDetails.permission;
                    },
                    failure: function (response) {
                        alert("Failed to fetch user details.");
                    }
                });
        }

        function LoadBranches(branch_name) {
            // Get the selected faculty name
            var selectedFaculty = document.getElementById("<%= EditFacultyDropDownList.ClientID %>").value;

                $.ajax({
                    type: "POST",
                    url: "MangeUser.aspx/GetBranchesByFaculty",
                    data: "{facultyName: '" + selectedFaculty + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var branches = JSON.parse(response.d);
                        var branchDropDownList = document.getElementById("<%= EditBranchDropDownList.ClientID %>");
                        var hiddenBranchDropDownList = document.getElementById("<%= HiddenEditBranchDropDownList.ClientID %>");


                        // Clear existing options
                        branchDropDownList.innerHTML = "";

                        // Populate the branch DropDownList with fetched branches
                        for (var i = 0; i < branches.length; i++) {
                            var branch = branches[i];
                            var option = document.createElement("option");
                            option.value = branch;
                            option.text = branch;

                            branchDropDownList.appendChild(option);
                        }

                        branchDropDownList.value = branch_name;
                        hiddenBranchDropDownList.value = branch_name;
                    },
                    failure: function (response) {
                        alert("Failed to fetch branches.");
                    }
                });

        }
    </script>
</asp:Content>

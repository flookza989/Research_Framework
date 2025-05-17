<%@ Page Title="" Language="C#" MasterPageFile="~/Webpage/Layout.Master" AutoEventWireup="true" CodeBehind="MangeUser.aspx.cs" Inherits="Research_Framework.Webpage.MangeUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="contentHead">
        จัดการผู้ใช้
    </div>
    <div class="row contentBody">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
        </asp:ScriptManager>
        <div class="col-6 d-flex align-items-center">
            <label class="">ค้นหา</label>
            <asp:TextBox ID="TbSearch" runat="server" CssClass="form-control textboxContent ms-2" placeholder="ค้นหา"></asp:TextBox>
            <asp:Button ID="BtnSearch" runat="server" Text="ค้นหา" CssClass="btn btn-primary ms-2" OnClick="BtnSearch_Click" />
        </div>
        <div class="mt-2">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <asp:GridView ID="GvUser" runat="server" CssClass="table table-striped table-hover"
                        AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
                        OnPageIndexChanging="GvUser_PageIndexChanging"
                        OnRowCommand="GvUser_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="id" HeaderText="ID" Visible="false" />
                            <asp:BoundField DataField="username" HeaderText="รหัสผู้ใช้" />
                            <asp:BoundField DataField="first_name" HeaderText="ชื่อ" />
                            <asp:BoundField DataField="last_name" HeaderText="นามสกุล" />
                            <asp:BoundField DataField="faculty_name" HeaderText="คณะ" />
                            <asp:BoundField DataField="branch_name" HeaderText="สาขา" />
                            <asp:BoundField DataField="user_type" HeaderText="ประเภทผู้ใช้" />
                            <asp:TemplateField HeaderText="สถานะ">
                                <ItemTemplate>
                                    <div class="form-check form-switch">
                                        <input type="checkbox" class="form-check-input"
                                            <%# Convert.ToBoolean(Eval("is_active")) ? "checked" : "" %>
                                            <%# Eval("user_type").ToString() == "ADMIN" ? "disabled" : "" %>
                                            onchange="toggleUserStatus(<%# Eval("id") %>, this.checked)"
                                            <%# Eval("user_type").ToString() == "ADMIN" ? "style='cursor: not-allowed;'" : "" %> />
                                        <label class="form-check-label">
                                            <%# Convert.ToBoolean(Eval("is_active")) ? "เปิดใช้งาน" : "ปิดใช้งาน" %>
                                        </label>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <%# Eval("user_type").ToString() != "ADMIN" ? "<div class='btn-group'>" : "" %>
                                    <asp:LinkButton ID="btnEdit" runat="server" CssClass="buttonNormal btn"
                                        CommandName="EditUser" CommandArgument='<%# Eval("id") %>'
                                        Visible='<%# Eval("user_type").ToString() != "ADMIN" %>'>
                                        <i class="fas fa-edit"></i> แก้ไข
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnDelete" runat="server" CssClass="buttonRemove btn"
                                        OnClientClick='<%# "return confirmDelete(" + Eval("id") + ");" %>'
                                        Visible='<%# Eval("user_type").ToString() != "ADMIN" %>'>
                                        <i class="fas fa-trash"></i> ลบ
                                    </asp:LinkButton>
                                    <%# Eval("user_type").ToString() != "ADMIN" ? "</div>" : "" %>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <%-- Modal Edit User --%>
    <div class="modal fade" id="modalEditUser" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">แก้ไขผู้ใช้</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <asp:UpdatePanel ID="UpdatePanelEdit" runat="server">
                        <ContentTemplate>
                            <asp:HiddenField ID="HfUserId" runat="server" />
                            <div class="mb-3">
                                <label class="form-label">ชื่อ</label>
                                <asp:TextBox ID="TbEditName" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RfvName" runat="server" ControlToValidate="TbEditName"
                                    ValidationGroup="EditUser" CssClass="text-danger" Display="Dynamic"
                                    ErrorMessage="กรุณากรอกชื่อ"></asp:RequiredFieldValidator>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">นามสกุล</label>
                                <asp:TextBox ID="TbEditLname" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RfvLname" runat="server" ControlToValidate="TbEditLname"
                                    ValidationGroup="EditUser" CssClass="text-danger" Display="Dynamic"
                                    ErrorMessage="กรุณากรอกนามสกุล"></asp:RequiredFieldValidator>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">รหัสผู้ใช้</label>
                                <asp:TextBox ID="TbEditUsername" runat="server" CssClass="form-control" MaxLength="20"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RfvUsername" runat="server" ControlToValidate="TbEditUsername"
                                    ValidationGroup="EditUser" CssClass="text-danger" Display="Dynamic"
                                    ErrorMessage="กรุณากรอกรหัสนักศึกษา"></asp:RequiredFieldValidator>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">รหัสผ่าน</label>
                                <asp:TextBox ID="TbEditPassword" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                                <small class="text-muted">เว้นว่างไว้หากไม่ต้องการเปลี่ยนรหัสผ่าน</small>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">คณะ</label>
                                <asp:DropDownList ID="DdlEditFaculty" runat="server" CssClass="form-select" AutoPostBack="true"
                                    OnSelectedIndexChanged="DdlEditFaculty_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">สาขา</label>
                                <asp:DropDownList ID="DdlEditBranch" runat="server" CssClass="form-select">
                                </asp:DropDownList>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">ยกเลิก</button>
                    <asp:Button ID="BtnSave" runat="server" Text="บันทึก" CssClass="btn btn-primary"
                        ValidationGroup="EditUser" OnClick="BtnSave_Click" />
                </div>
            </div>
        </div>
    </div>

    <script>
        function confirmDelete(userId) {
            Swal.fire({
                title: 'ยืนยันการลบ?',
                text: "คุณต้องการลบผู้ใช้นี้ใช่หรือไม่?",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'ยืนยันการลบ',
                cancelButtonText: 'ยกเลิก'
            }).then((result) => {
                if (result.isConfirmed) {
                    PageMethods.DeleteUserMethod(userId,
                        function (result) {
                            if (result) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'สำเร็จ',
                                    text: 'ลบข้อมูลผู้ใช้เรียบร้อยแล้ว',
                                    showConfirmButton: false,
                                    timer: 1500
                                }).then(() => {
                                    // รีโหลดหน้าหลังจากลบสำเร็จ
                                    window.location.reload();
                                });
                            } else {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'ผิดพลาด',
                                    text: 'ไม่สามารถลบข้อมูลผู้ใช้ได้'
                                });
                            }
                        },
                        function (error) {
                            Swal.fire({
                                icon: 'error',
                                title: 'ผิดพลาด',
                                text: 'เกิดข้อผิดพลาดในการลบข้อมูล: ' + error.get_message()
                            });
                        }
                    );
                }
            });
            return false;
        }

        function showEditModal() {
            var modal = new bootstrap.Modal(document.getElementById('modalEditUser'));
            modal.show();
        }

        // ถ้ามี validation error ให้แสดง modal
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function () {
            if (document.getElementById('<%= RfvName.ClientID %>').style.display === 'inline' ||
                document.getElementById('<%= RfvLname.ClientID %>').style.display === 'inline' ||
                document.getElementById('<%= RfvUsername.ClientID %>').style.display === 'inline') {
                showEditModal();
            }
        });

        function toggleUserStatus(userId, isActive) {
            PageMethods.ToggleUserStatus(userId, isActive,
                function (result) {
                    if (result) {
                        Swal.fire({
                            icon: 'success',
                            title: 'สำเร็จ',
                            text: 'อัพเดทสถานะผู้ใช้เรียบร้อยแล้ว'
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'ผิดพลาด',
                            text: 'ไม่สามารถอัพเดทสถานะผู้ใช้ได้'
                        });
                    }
                },
                function (error) {
                    Swal.fire({
                        icon: 'error',
                        title: 'ผิดพลาด',
                        text: 'เกิดข้อผิดพลาดในการอัพเดทสถานะ: ' + error.get_message()
                    });
                }
            );
        }
    </script>
</asp:Content>

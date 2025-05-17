<%@ Page Title="" Language="C#" MasterPageFile="~/Webpage/Layout.Master" AutoEventWireup="true" CodeBehind="AddReserch.aspx.cs" Inherits="Research_Framework.Webpage.AddReserch"  EnableEventValidation="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="contentHead">
        <asp:Label runat="server" Text="รายละเอียดวิทยานิพนธ์"></asp:Label>
    </div>
    <div class="row contentBody">
        <div class="col-6">
            <asp:Label runat="server" CssClass="labelContent" Text="ชื่อวิทยานิพนธ์"></asp:Label>
            <asp:TextBox ID="Tb_reserch" runat="server" CssClass="form-control textboxContent" placeholder="ชื่อวิทยานิพนธ์"></asp:TextBox>
            <asp:HiddenField ID="hdn_reserch" runat="server" />
        </div>
        <div class="col-6">
            <asp:Label runat="server" CssClass="labelContent" Text="อาจารย์ที่ปรึกษา"></asp:Label>
            <asp:TextBox ID="Tb_teacher" runat="server" CssClass="form-control textboxContent" placeholder="อาจารย์ที่ปรึกษา"></asp:TextBox>



        </div>
    </div>

    <div class="contentHead mt-3">
        <asp:Label runat="server" Text="สมาชิก"></asp:Label>
    </div>
    <div class="row contentBody">
        <asp:GridView ID="Dgv_std" runat="server" CssClass="custom-gridview" AutoGenerateColumns="false" DataKeyNames="stdID" OnRowDeleting="Dgv_std_RowDeleting">
            <Columns>
                <asp:BoundField DataField="stdID" HeaderText="รหัสนักศึกษา" SortExpression="stdID" />
                <asp:BoundField DataField="name" HeaderText="ชื่อ" SortExpression="name" />
                <asp:BoundField DataField="lname" HeaderText="นามสกุล" SortExpression="lname" />
                <asp:BoundField DataField="faculty" HeaderText="คณะ" SortExpression="faculty" />
                <asp:BoundField DataField="branch" HeaderText="สาขา" SortExpression="branch" />
<asp:TemplateField HeaderText="จัดการ">
    <ItemTemplate>
        <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" 
            CssClass="buttonNormal btn px-2" 
            OnClientClick="return confirmDelete(this);">
            <i class="fa fa-trash" aria-hidden="true"></i> ลบ
        </asp:LinkButton>
    </ItemTemplate>
</asp:TemplateField>
            </Columns>
        </asp:GridView>
        <div class="col-6 mt-2">
            <asp:Label runat="server" CssClass="labelContent" Text="ชื่อนักศึกษา"></asp:Label>
            <asp:TextBox ID="Tb_student" runat="server" CssClass="form-control textboxContent" placeholder="ชื่อนักศึกษา"></asp:TextBox>
        </div>
        <div class="col-6 mt-2">
            <asp:LinkButton ID="Btn_add" CssClass="buttonNormal btn mt-4" runat="server" OnClick="Btn_add_Click">
                <span class="fa-solid fa-plus"/> เพิ่มสมาชิก
            </asp:LinkButton>
        </div>
        <div class="col-12 d-flex flex-column align-items-end text-center">
            <asp:LinkButton ID="Btn_save" CssClass="buttonNormal btn mt-4" runat="server" OnClick="Btn_save_Click">
                <span class="fa-solid fa-floppy-disk"/> บันทึก
            </asp:LinkButton>
        </div>
    </div>
<script type="text/javascript">
    function confirmDelete(button) {
        var uniqueID = button.id;
        swal({
            title: "คุณแน่ใจหรือไม่?",
            text: "คุณจะไม่สามารถกู้คืนข้อมูลนี้ได้!",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "ใช่, ลบเลย!",
            cancelButtonText: "ยกเลิก"
        },
            function (isConfirm) {
                if (isConfirm) {
                    var postBackForm = document.forms[0];
                    if (postBackForm) {
                        var eventTarget = document.createElement('input');
                        eventTarget.type = 'hidden';
                        eventTarget.name = '__EVENTTARGET';
                        eventTarget.value = uniqueID;
                        postBackForm.appendChild(eventTarget);

                        postBackForm.submit();
                    }
                }
            });
        return false;
    }
</script>
</asp:Content>

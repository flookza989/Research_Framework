<%@ Page Title="" Language="C#" MasterPageFile="~/Webpage/Layout.Master" AutoEventWireup="true" MaintainScrollPositionOnPostback="true" CodeBehind="Profile.aspx.cs" Inherits="Research_Framework.Webpage.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="contentHead">
        <asp:Label runat="server" Text="ข้อมูลส่วนตัว"></asp:Label>
    </div>
    <div class="row contentBody">
        <div class="col-12 d-flex flex-column align-items-center text-center">
            <div>
                <asp:Image ID="ProfileImage" runat="server" ImageUrl="~/Images/NewUser.png" Style="width: 250px; border-radius: 50px;" />
            </div>
            <div>
                <asp:LinkButton ID="Btn_uploadImage" CssClass="buttonNormal btn mt-4" runat="server" OnClick="UploadImageButton_Click">
                <span class="fa-solid fa-image"/> อัพโหลดรูป
                </asp:LinkButton>
            </div>
        </div>
        <div class="col-6 mt-4">
            <asp:Label runat="server" CssClass="labelContent" Text="ชื่อ"></asp:Label>
            <asp:TextBox ID="Tb_name" runat="server" CssClass="form-control textboxContent" placeholder="ชื่อ"></asp:TextBox>
        </div>
        <div class="col-6 mt-4">
            <asp:Label runat="server" CssClass="labelContent" Text="นามสกุล"></asp:Label>
            <asp:TextBox ID="Tb_lname" runat="server" CssClass="form-control textboxContent" placeholder="นามสกุล"></asp:TextBox>
        </div>
        <div class="col-6 mt-2">
            <asp:Label runat="server" CssClass="labelContent" Text="รหัสนักศึกษา"></asp:Label>
            <asp:TextBox ID="Tb_username" runat="server" CssClass="form-control textboxContent" placeholder="รหัสนักศึกษา" Enabled="false"></asp:TextBox>
        </div>
        <div class="col-6 mt-2">
            <asp:Label runat="server" CssClass="labelContent" Text="รหัสผ่าน"></asp:Label>
            <asp:TextBox ID="Tb_password" runat="server" CssClass="form-control textboxContent" placeholder="รหัสผ่าน"></asp:TextBox>
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
            <asp:LinkButton ID="Btn_save" CssClass="buttonNormal btn mt-4" runat="server" OnClick="SaveButton_Click">
                <span class="fa-solid fa-floppy-disk"/> บันทึก
            </asp:LinkButton>
        </div>
    </div>
</asp:Content>

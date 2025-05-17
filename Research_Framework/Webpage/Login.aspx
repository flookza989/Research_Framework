<%@ Page Title="เข้าสู่ระบบ" Language="C#" MasterPageFile="~/Webpage/Layout.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Research_Framework.Webpage.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="containerLogin container">
        <div class="contentHead text-center">
            <h2 class="mb-2 mt-2">ระบบจัดการวิทยานิพนธ์</h2>
        </div>
        <div class="contentBody">
            <div class="form-group">
                <label class="form-label">ชื่อผู้ใช้</label>
                <asp:TextBox ID="Tb_user" runat="server" CssClass="form-control" placeholder="กรุณากรอกชื่อผู้ใช้" required="true"></asp:TextBox>
            </div>
            <div class="form-group mt-3">
                <label class="form-label">รหัสผ่าน</label>
                <asp:TextBox ID="Tb_pass" runat="server" CssClass="form-control" TextMode="Password" placeholder="กรุณากรอกรหัสผ่าน" required="true"></asp:TextBox>
            </div>
            <asp:Button ID="Btn_login" runat="server" CssClass="btn btn-primary w-100 mt-4 buttonNormal" Text="เข้าสู่ระบบ" OnClick="Btn_login_Click" />
        </div>
    </div>
</asp:Content>
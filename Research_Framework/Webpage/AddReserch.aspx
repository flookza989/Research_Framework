<%@ Page Title="" Language="C#" MasterPageFile="~/Webpage/Layout.Master" AutoEventWireup="true" CodeBehind="AddReserch.aspx.cs" Inherits="Research_Framework.Webpage.AddReserch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="contentHead">
        <asp:Label runat="server" Text="รายละเอียดวิทยานิพนธ์"></asp:Label>
    </div>
    <div class="row contentBody">
        <div class="col-6">
            <asp:Label runat="server" CssClass="labelContent" Text="ชื่อวิทยานิพนธ์"></asp:Label>
            <asp:TextBox ID="Tb_reserch" runat="server" CssClass="form-control textboxContent" placeholder="ชื่อวิทยานิพนธ์"></asp:TextBox>
            <asp:HiddenField ID="hdn_reserch" runat="server"/>
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
        <asp:GridView ID="Dgv_std" runat="server" CssClass="custom-gridview" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField DataField="stdID" HeaderText="รหัสนักศึกษา" SortExpression="stdID" />
                <asp:BoundField DataField="name" HeaderText="ชื่อ" SortExpression="name" />
                <asp:BoundField DataField="lname" HeaderText="นามสกุล" SortExpression="lname" />
                <asp:BoundField DataField="faculty" HeaderText="คณะ" SortExpression="faculty" />
                <asp:BoundField DataField="branch" HeaderText="สาขา" SortExpression="branch" />
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
</asp:Content>

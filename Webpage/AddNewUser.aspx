<%@ Page Title="" Language="C#" MasterPageFile="~/Webpage/Layout.Master" AutoEventWireup="true" CodeBehind="AddNewUser.aspx.cs" Inherits="Research_Framework.Webpage.AddnewUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="contentHead">
        สถานะวิทยานิพนธ์
    </div>
    <div class="row contentBody">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div class="col-12 d-flex align-items-center">
            <label class="">คลิกเพื่อดาวน์โหลดไฟล์ต้นแบบ</label>
            <div class="d-flex flex-column align-items-start text-center">
                <asp:LinkButton ID="Btn_Download" CssClass="buttonNormal btn ms-2" runat="server" OnClick="Btn_Download_Click">
            <span class="fa-solid fa-download" /> &nbsp;ดาวน์โหลด 
                </asp:LinkButton>
            </div>
        </div>
        <div class="col-12 d-flex align-items-center mt-2">
            <label class="">คลิกเพื่ออัพโหลดไฟล์</label>
            <asp:FileUpload ID="FileUploadControl" runat="server" CssClass="buttonUpload me-1 ms-2" />
            <div class="d-flex flex-column align-items-start text-center">

                <asp:LinkButton ID="Btn_upload" CssClass="buttonNormal btn ms-2" runat="server" OnClick="Btn_upload_Click">
    <span class="fa-solid fa-upload" />  &nbsp;อัพโหลด
                </asp:LinkButton>
            </div>
            <div id="div_Btn_save" runat="server" visible="false" class="d-flex flex-column align-items-end text-center ms-auto">

                <asp:LinkButton ID="Btn_save" CssClass="buttonNormal btn ms-2" runat="server" OnClick="Btn_save_Click">
    <span class="fa-solid fa-save" />  &nbsp;บันทึก
                </asp:LinkButton>
            </div>
        </div>
        <div class="mt-2">
            <asp:GridView ID="Gv_newSTD" runat="server" CssClass="custom-gridview" AutoGenerateColumns="False">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderText="ชื่อ" SortExpression="Name" />
                    <asp:BoundField DataField="SurName" HeaderText="นามสกุล" SortExpression="SurName" />
                    <asp:BoundField DataField="UserName" HeaderText="รหัสนักศึกษา" SortExpression="UserName" />
                    <asp:BoundField DataField="Password" HeaderText="รหัสผ่าน" SortExpression="Password" />
                    <asp:BoundField DataField="Faculty" HeaderText="คณะ" SortExpression="Faculty" />
                    <asp:BoundField DataField="Branch" HeaderText="สาขา" SortExpression="Branch" />
                </Columns>
            </asp:GridView>
        </div>

    </div>
</asp:Content>

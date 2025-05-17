<%@ Page Title="" Language="C#" MasterPageFile="~/Webpage/Layout.Master" AutoEventWireup="true" CodeBehind="AddNewUser.aspx.cs" Inherits="Research_Framework.Webpage.AddnewUser" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="contentHead">
        เพิ่มผู้ใช้งานใหม่
    </div>
    <div class="row contentBody">
    <div class="col-12">
        <div class="card">
            <div class="card-body">
                <div class="row mb-4">
                    <!-- ส่วนดาวน์โหลดเทมเพลต -->
                    <div class="col-md-6">
                        <div class="d-flex align-items-center">
                            <label class="me-2">1. ดาวน์โหลดไฟล์ต้นแบบ</label>
                            <asp:LinkButton ID="Btn_Download" CssClass="buttonNormal btn" runat="server" OnClick="Btn_Download_Click">
                                <span class="fa-solid fa-download"></span> ดาวน์โหลด
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>

                <div class="row mb-4">
                    <!-- ส่วนอัพโหลดไฟล์ -->
                    <div class="col-md-8">
                        <div class="d-flex align-items-center">
                            <label class="me-2">2. อัพโหลดไฟล์ข้อมูล</label>
                            <asp:FileUpload ID="FileUploadControl" runat="server" CssClass="form-control w-50 me-2" />
                            <asp:LinkButton ID="Btn_upload" CssClass="buttonNormal btn" runat="server" OnClick="Btn_upload_Click">
                                <span class="fa-solid fa-upload"></span> อัพโหลด
                            </asp:LinkButton>
                            <!-- ปุ่มบันทึก -->
                            <div id="div_Btn_save" runat="server" class="ms-2">
                                <asp:LinkButton ID="Btn_save" runat="server" CssClass="buttonNormal btn" 
                                    OnClick="Btn_save_Click">
                                    <span class="fa-solid fa-save"></span> บันทึก
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- ตารางแสดงข้อมูล -->
                <div class="table-responsive">
                    <asp:GridView ID="Gv_newSTD" runat="server" CssClass="table table-striped table-hover"
                        AutoGenerateColumns="False">
                        <Columns>
                            <asp:BoundField DataField="Name" HeaderText="ชื่อ" />
                            <asp:BoundField DataField="SurName" HeaderText="นามสกุล" />
                            <asp:BoundField DataField="UserName" HeaderText="รหัสนักศึกษา" />
                            <asp:BoundField DataField="Password" HeaderText="รหัสผ่าน" />
                            <asp:BoundField DataField="Faculty" HeaderText="คณะ" />
                            <asp:BoundField DataField="Branch" HeaderText="สาขา" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>
</div>
</asp:Content>

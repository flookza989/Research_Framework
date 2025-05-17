<%@ Page Title="" Language="C#" MasterPageFile="~/Webpage/Layout.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="Research_Framework.Webpage.Contact" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid p-0">
        <div class="row">
            <div class="col-12">
                <div class="contentHead">
                    <h3><i class="fas fa-file-download me-2"></i> เอกสารดาวน์โหลด</h3>
                </div>
                <div class="contentBody">
                    <!-- ส่วนจัดการไฟล์เอกสาร -->
                    <div class="row mt-2">
                        <div class="col-12">                            
                            <!-- ส่วนอัพโหลดไฟล์ (แสดงเฉพาะ Admin) -->
                            <asp:Panel ID="AdminPanel" runat="server" Visible="false" CssClass="mb-4 p-3 bg-light rounded border">
                                <h5 class="mb-3"><i class="fas fa-upload me-2"></i> เพิ่มเอกสารใหม่</h5>
                                <div class="row g-3 align-items-center">
                                    <div class="col-md-4">
                                        <asp:TextBox ID="TxtFileName" runat="server" CssClass="form-control" placeholder="ชื่อเอกสาร"></asp:TextBox>
                                    </div>
                                    <div class="col-md-5">
                                        <asp:FileUpload ID="FileUploadControl" runat="server" CssClass="form-control" />
                                    </div>
                                    <div class="col-md-3">
                                        <asp:Button ID="BtnUpload" runat="server" Text="อัพโหลดไฟล์" CssClass="btn btn-primary" OnClick="BtnUpload_Click" />
                                    </div>
                                </div>
                                <small class="text-muted mt-2 d-block">* รองรับไฟล์ประเภท PDF, Word, Excel, PowerPoint, ZIP (ขนาดไม่เกิน 10MB)</small>
                            </asp:Panel>

                            <!-- แสดงข้อความผลการอัพโหลด -->
                            <asp:Label ID="StatusLabel" runat="server" Text="" CssClass="alert alert-info mt-3 d-none"></asp:Label>

                            <!-- ตารางแสดงไฟล์ -->
                            <asp:GridView ID="FilesGridView" runat="server" AutoGenerateColumns="False" 
                                CssClass="table table-striped table-bordered custom-gridview mt-3" 
                                EmptyDataText="ไม่มีเอกสารให้ดาวน์โหลด" 
                                OnRowCommand="FilesGridView_RowCommand"
                                OnRowDataBound="FilesGridView_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="FileId" HeaderText="ลำดับ" HeaderStyle-Width="5%" />
                                    <asp:BoundField DataField="FileName" HeaderText="ชื่อเอกสาร" HeaderStyle-Width="40%" />
                                    <asp:BoundField DataField="FileType" HeaderText="ประเภทไฟล์" HeaderStyle-Width="15%" />
                                    <asp:BoundField DataField="UploadDate" HeaderText="วันที่อัพโหลด" HeaderStyle-Width="15%" DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" />
                                    <asp:BoundField DataField="FileSize" HeaderText="ขนาดไฟล์" HeaderStyle-Width="10%" />
                                    <asp:TemplateField HeaderText="ดาวน์โหลด" HeaderStyle-Width="10%">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="DownloadButton" runat="server" Text="ดาวน์โหลด" CssClass="btn btn-sm btn-primary" 
                                                CommandName="DownloadFile" CommandArgument='<%# Eval("FilePath") %>'>
                                                <i class="fas fa-download me-2"></i> ดาวน์โหลด
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ลบ" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="DeleteButton" runat="server" Text="ลบ" CssClass="btn btn-sm btn-danger" 
                                                CommandName="DeleteFile" CommandArgument='<%# Eval("FilePath") %>'
                                                OnClientClick="return confirm('คุณต้องการลบไฟล์นี้ใช่หรือไม่?');">
                                                <i class="fas fa-trash-alt"></i>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

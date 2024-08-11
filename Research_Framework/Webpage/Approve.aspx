<%@ Page Title="" Language="C#" MasterPageFile="~/Webpage/Layout.Master" AutoEventWireup="true" CodeBehind="Approve.aspx.cs" Inherits="Research_Framework.Webpage.Approve" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="contentHead">
        สถานะวิทยานิพนธ์
    </div>
    <div class="row contentBody">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div class="col-6">
            <label class="labelContent">ชื่อวิทยานิพนธ์</label>
            <asp:TextBox ID="Tb_researchName" runat="server" CssClass="form-control textboxContent mb-2" Enabled="false" placeholder="ชื่องานวิจัย"></asp:TextBox>
            <asp:DropDownList ID="Ddl_research" runat="server" CssClass="dropdownlist mb-2" Visible="false" OnSelectedIndexChanged="Gv_status_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
        </div>
        <asp:GridView ID="Gv_status" runat="server" CssClass="custom-gridview" AutoGenerateColumns="False" OnRowDataBound="Gv_status_RowDataBound" OnRowCommand="Gv_status_RowCommand">
            <Columns>
                <asp:BoundField DataField="step" HeaderText="ขั้นตอน" SortExpression="step" />
                <asp:BoundField DataField="status" HeaderText="สถานะ" SortExpression="status" />
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <div class="text-end">
                            <asp:FileUpload ID="FileUploadControl" runat="server" CssClass="buttonUpload me-2" />
                            <asp:LinkButton ID="btnUpload" runat="server" CommandName="UploadClick" CssClass="buttonNormal btn me-2">
                            <span class="fa-solid fa-file-pdf me-1"></span> อัพโหลด
                            </asp:LinkButton>
                            <asp:LinkButton ID="btnDownload" runat="server" CommandName="DownloadClick" CssClass="buttonNormal btn me-2">
                            <span class="fa-solid fa-file-pdf me-1"></span> ดาวน์โหลด
                            </asp:LinkButton>
                            <asp:TextBox ID="TbDatetime" runat="server" TextMode="DateTimeLocal" CssClass="form-control d-inline-block w-auto me-2" Visible="false"></asp:TextBox>
                            <asp:LinkButton ID="btnApprove" runat="server" CommandName="ApproveClick" CssClass="buttonNormal btn me-2">
                            <span class="fa-solid fa-check me-1"></span> อนุมัติ
                            </asp:LinkButton>
                            <asp:LinkButton ID="btnRequest" runat="server" CommandName="RequestClick" CssClass="buttonNormal btn me-2">
                            <span class="fa-solid fa-bell me-1"></span> ยื่นสอบ
                            </asp:LinkButton>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

    </div>


</asp:Content>



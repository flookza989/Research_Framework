<%@ Page Title="" Language="C#" MasterPageFile="~/Webpage/Layout.Master" AutoEventWireup="true" CodeBehind="ResearchProcess.aspx.cs" Inherits="Research_Framework.Webpage.ResearchProcess" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    <div class="contentHead d-flex justify-content-between align-items-center">
        <h3>ขั้นตอนการดำเนินงานวิจัย</h3>
        <asp:Label ID="LbResearchStatus" runat="server" CssClass="badge bg-primary"></asp:Label>
    </div>

<!-- ส่วนของ DropDownList สำหรับเลือกงานวิจัย -->
<div class="mb-3" id="researchSelector" runat="server" visible='<%# IsTeacher() %>'>
            <div class="form-group">
                <label class="form-label">เลือกงานวิจัย</label>
                <asp:DropDownList ID="DdlResearch" runat="server" 
                    CssClass="form-select" 
                    AutoPostBack="true"
                    OnSelectedIndexChanged="DdlResearch_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
</div>

    <div class="contentBody">
        <!-- ส่วนข้อมูลงานวิจัย -->
        <div class="card mb-4">
            <div class="card-header">
                <h5 class="card-title mb-0">ข้อมูลงานวิจัย</h5>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-8">
                        <label class="labelContent">ชื่องานวิจัย</label>
                        <asp:TextBox ID="TbResearchName" runat="server" CssClass="form-control textboxContent" ReadOnly="true"></asp:TextBox>
                    </div>
                    <div class="col-md-4">
                        <label id="LbAdvisor" runat="server" class="labelContent">อาจารย์ที่ปรึกษา</label>
                        <asp:TextBox ID="TbAdvisor" runat="server" CssClass="form-control textboxContent" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </div>

        <!-- ส่วนแสดงขั้นตอน -->
        <div class="card">
            <div class="card-body">
                <asp:Repeater ID="RptProcesses" runat="server" OnItemDataBound="RptProcesses_ItemDataBound">
                    <ItemTemplate>
                        <div class="process-item mb-4">
                            <div class="d-flex justify-content-between align-items-center">
                                <h5><%# Eval("ProcessName") %></h5>
                                <asp:Label ID="LbStatus" runat="server"
                                    CssClass='<%# "badge " + GetStatusClass(Eval("Status").ToString()) %>'
                                    Text='<%# GetStatusText(Eval("Status").ToString()) %>'>
                                </asp:Label>
                            </div>

                            <!-- แสดงความคิดเห็น -->
                            <asp:Panel ID="CommentPanel" runat="server"
                                CssClass="alert alert-info mt-2"
                                Visible='<%# !string.IsNullOrEmpty(Eval("Comments")?.ToString()) %>'>
                                <i class="fas fa-comment me-2"></i><%# Eval("Comments") %>
                            </asp:Panel>

                            <!-- ส่วนอัพโหลดเอกสาร -->
                            <div class="upload-section mt-3" runat="server" id="UploadSection" visible='<%# IsUploadAllowed(Eval("Status")) %>'>
                                <asp:FileUpload ID="FileUpload" runat="server" CssClass="buttonUpload" />
                                <asp:LinkButton ID="BtnUpload" runat="server" CssClass="buttonNormal btn"
                                    OnClick="BtnUpload_Click" CommandArgument='<%# Eval("ProcessId") %>'>
                                    <i class="fas fa-upload"></i> อัพโหลด
                                </asp:LinkButton>
                            </div>

                            <!-- ส่วนดาวน์โหลดเอกสาร -->
                            <div class="document-list mt-3">
                                <!-- ส่วนแสดงรายการเอกสาร -->
                                <asp:Repeater ID="RptDocuments" runat="server">
                                    <ItemTemplate>
                                        <div class="document-item d-flex align-items-center justify-content-between mt-2">
                                            <div>
                                                <i class="fas fa-file-pdf text-danger me-2"></i>
                                                <asp:LinkButton ID="BtnDownload" runat="server"
                                                    CssClass="btn btn-link p-0"
                                                    OnClick="BtnDownload_Click"
                                                    CommandArgument='<%# Eval("Id") %>'>
                                                <%# Eval("FileName") %>
                                                </asp:LinkButton>
                                                <small class="text-muted ms-2">(<%# ((DateTime)Eval("UploadDate")).ToString("dd/MM/yyyy HH:mm") %>)
                                                </small>
                                                <span class='<%# GetDocumentStatusClass(Eval("Status").ToString()) %>'>
                                                    <%# GetDocumentStatusText(Eval("Status").ToString()) %>
                                                </span>
                                            </div>

                                            <!-- ส่วนของอาจารย์ -->
                                            <div class="teacher-actions" runat="server" visible='<%# IsTeacher() %>'>
                                                <div class="input-group">
                                                    <asp:FileUpload ID="FileUploadFeedback" runat="server" CssClass="form-control" />
                                                    <asp:TextBox ID="TbFeedback" runat="server" CssClass="form-control"
                                                        placeholder="ความเห็น"></asp:TextBox>
                                                    <asp:LinkButton ID="BtnSendFeedback" runat="server"
                                                        CssClass="btn btn-warning"
                                                        OnClick="BtnSendFeedback_Click"
                                                        CommandArgument='<%# Eval("Id") %>'>
                                                    <i class="fas fa-reply"></i> ส่งกลับแก้ไข
                                                    </asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>

                            <!-- ส่วนการอนุมัติ (สำหรับอาจารย์) -->
                            <div class="approval-section mt-3" runat="server" id="ApprovalSection" visible='<%# IsTeacher() %>'>
                                <asp:TextBox ID="TbComments" runat="server" CssClass="form-control textboxContent mb-2"
                                    TextMode="MultiLine" Rows="2" placeholder="ความคิดเห็น"></asp:TextBox>

                                <asp:LinkButton ID="BtnApprove" runat="server" CssClass="buttonNormal btn"
                                    OnClick="BtnApprove_Click" CommandArgument='<%# Eval("ProcessId") + ";" + GetSelectedResearchId() %>'>
                                    <i class="fas fa-check"></i> อนุมัติ
                                </asp:LinkButton>

                                <asp:LinkButton ID="BtnReject" runat="server" CssClass="buttonRemove btn"
                                    OnClick="BtnReject_Click" CommandArgument='<%# Eval("ProcessId") + ";" + GetSelectedResearchId() %>'>
                                    <i class="fas fa-times"></i> ไม่อนุมัติ
                                </asp:LinkButton>
                            </div>
                        </div>
                        <hr />
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>

</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/Webpage/Layout.Master" AutoEventWireup="true" CodeBehind="ResearchList.aspx.cs" Inherits="Research_Framework.Webpage.ResearchList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    
    <div class="contentHead">
        <h3>รายการวิทยานิพนธ์ทั้งหมด</h3>
    </div>
    
    <div class="contentBody">
        <!-- ส่วนค้นหา -->
        <div class="card mb-4">
            <div class="card-header">
                <h5 class="card-title mb-0">ค้นหาวิทยานิพนธ์</h5>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group mb-3">
                            <label class="form-label">ชื่อวิทยานิพนธ์</label>
                            <asp:TextBox ID="TbSearchName" runat="server" CssClass="form-control" placeholder="ค้นหาตามชื่อวิทยานิพนธ์"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group mb-3">
                            <label class="form-label">สถานะ</label>
                            <asp:DropDownList ID="DdlStatus" runat="server" CssClass="form-select">
                                <asp:ListItem Value="">-- ทั้งหมด --</asp:ListItem>
                                <asp:ListItem Value="PENDING">รอดำเนินการ</asp:ListItem>
                                <asp:ListItem Value="IN_PROGRESS">กำลังดำเนินการ</asp:ListItem>
                                <asp:ListItem Value="WAITING_APPROVAL">รออนุมัติ</asp:ListItem>
                                <asp:ListItem Value="APPROVED">อนุมัติแล้ว</asp:ListItem>
                                <asp:ListItem Value="REJECTED">ไม่อนุมัติ</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <label class="form-label">&nbsp;</label>
                            <div class="d-grid">
                                <asp:Button ID="BtnSearch" runat="server" Text="ค้นหา" CssClass="btn btn-primary" OnClick="BtnSearch_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- ส่วนตารางแสดงข้อมูล -->
        <div class="card">
            <div class="card-body">
                <div class="table-responsive">
                    <asp:GridView ID="GvResearch" runat="server" AutoGenerateColumns="false" CssClass="table table-striped table-hover"
                        EmptyDataText="ไม่พบรายการวิทยานิพนธ์" AllowPaging="true" PageSize="10" OnPageIndexChanging="GvResearch_PageIndexChanging"
                        OnRowCommand="GvResearch_RowCommand" DataKeyNames="Id">
                        <Columns>
                            <asp:BoundField DataField="Id" HeaderText="รหัส" HeaderStyle-CssClass="d-none" ItemStyle-CssClass="d-none" />
                            <asp:TemplateField HeaderText="ลำดับ">
                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1 + (GvResearch.PageIndex * GvResearch.PageSize) %>
                                </ItemTemplate>
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="Name" HeaderText="ชื่อวิทยานิพนธ์" />
                            <asp:BoundField DataField="StudentName" HeaderText="หัวหน้ากลุ่ม" />
                            <asp:BoundField DataField="AdvisorName" HeaderText="อาจารย์ที่ปรึกษา" />
                            <asp:TemplateField HeaderText="สถานะ">
                                <ItemTemplate>
                                    <span class='<%# "badge " + GetStatusClass(Eval("Status").ToString()) %>'>
                                        <%# GetStatusText(Eval("Status").ToString()) %>
                                    </span>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:LinkButton ID="BtnViewProcess" runat="server" CssClass="btn btn-sm btn-primary"
                                        CommandName="ViewProcess" CommandArgument='<%# Eval("Id") %>'>
                                        <i class="fas fa-folder-open"></i> ขั้นตอนการดำเนินงาน
                                    </asp:LinkButton>
                                </ItemTemplate>
                                <ItemStyle Width="150px" HorizontalAlign="Center" />
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="pagination-container" HorizontalAlign="Center" />
                        <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" FirstPageText="« หน้าแรก" LastPageText="หน้าสุดท้าย »" />
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>

    <style>
        .pagination-container {
            display: flex;
            justify-content: center;
            margin-top: 1rem;
        }
        
        .pagination-container table {
            margin: 0 auto;
        }
        
        .pagination-container a, .pagination-container span {
            position: relative;
            display: block;
            padding: 0.5rem 0.75rem;
            margin-left: -1px;
            line-height: 1.25;
            color: #007bff;
            background-color: #fff;
            border: 1px solid #dee2e6;
            text-decoration: none;
        }
        
        .pagination-container span.current {
            z-index: 3;
            color: #fff;
            background-color: #304F6D;
            border-color: #304F6D;
        }
        
        .pagination-container a:hover {
            z-index: 2;
            color: #0056b3;
            text-decoration: none;
            background-color: #e9ecef;
            border-color: #dee2e6;
        }
    </style>
</asp:Content>

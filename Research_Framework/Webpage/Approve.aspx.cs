using Research_Framework.ApplicationDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Research_Framework.Webpage
{
    public partial class Approve : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Application["userID"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                View_user _User = (View_user)Application["userID"];

                using (var _db = new ResearchDBEntities())
                {
                    int researchID;
                    if (_User.permission == "student")
                    {
                        View_research_member _researchMember = _db.View_research_member.FirstOrDefault(c => c.user_id == _User.user_id);

                        if (_researchMember == null)
                            return;

                        Tb_researchName.Text = _researchMember.research_name;
                        researchID = _researchMember.research_id;
                    }
                    else
                    {
                        Tb_researchName.Visible = false;
                        Ddl_research.Visible = true;

                        List<research> _research = _db.researches.Where(c => c.teacher_id == _User.user_id).ToList();

                        if (_research == null)
                            return;

                        _research.ForEach(c =>
                        {
                            Ddl_research.Items.Add(new ListItem(c.research_name, c.research_id.ToString()));
                        });

                        researchID = _research.First().research_id;
                    }

                    LoadDataToGridview(researchID);
                }
            }
        }

        protected void Gv_status_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "UploadClick")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);

                FileUpload fileUploadControl = Gv_status.Rows[rowIndex].FindControl("FileUploadControl") as FileUpload;

                if (fileUploadControl.HasFile)
                {
                    string fileExtension = Path.GetExtension(fileUploadControl.FileName).ToLower();

                    if (fileExtension == ".pdf")
                    {
                        byte[] fileData = fileUploadControl.FileBytes;

                        View_user _User = (View_user)Application["userID"];
                        List<process_path> _process = Application["process"] as List<process_path>;
                        process_path processUpload = _process.OrderBy(c => c.process_id)
                                                                 .FirstOrDefault(c => c.status == false);

                        if(_User.permission == "student")
                            processUpload.path_student = fileData;
                        else
                            processUpload.path_teacher = fileData;

                        using (var _db = new ResearchDBEntities())
                        {
                            _db.process_path.AddOrUpdate(processUpload);
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        string script = "swal('พบข้อผิดพลาด', 'เฉพาะไฟล์นามสกุล pdf เท่านั้น', 'error');";
                        ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert", script, true);
                        return;
                    }
                }
                else
                {
                    string script = "swal('พบข้อผิดพลาด', 'กรุณาเลือกไฟล์อัพโหลด', 'error');";
                    ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert", script, true);
                    return;
                }
            }

            if (e.CommandName == "DownloadClick")
            {
                int rowIndex = Convert.ToInt32(Convert.ToString(e.CommandArgument));

                using (var _db = new ResearchDBEntities())
                {
                    View_user _User = (View_user)Application["userID"];
                    List<process_path> _process = Application["process"] as List<process_path>;
                    process_path process_Path = _process.FirstOrDefault(c => c.process_id == rowIndex + 1);
                    View_process processApprove = _db.View_process.FirstOrDefault(c => c.path_id == process_Path.path_id);

                    byte[] pdfBytes;

                    if (_User.permission == "student")
                    {
                        pdfBytes = processApprove.path_teacher;
                    }
                    else
                    {
                        pdfBytes = processApprove.path_student;
                    }

                    Response.Clear();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", $"attachment;filename={processApprove.research_name}_{processApprove.processResearch}.pdf");
                    Response.Buffer = true;
                    Response.BinaryWrite(pdfBytes);
                    Response.End();
                }
                
            }

            if (e.CommandName == "ApproveClick")
            {
                using (var _db = new ResearchDBEntities())
                {
                    int rowIndex = Convert.ToInt32(Convert.ToString(e.CommandArgument)[0]);

                    List<process_path> _process = Application["process"] as List<process_path>;
                    process_path processApprove;
                    if (e.CommandArgument.ToString().Length == 2 && Convert.ToString(e.CommandArgument)[1] == '0')
                    {
                        processApprove = _process.OrderByDescending(c => c.process_id)
                                                 .FirstOrDefault(c => c.status == true);
                        processApprove.status = false;
                    }
                    else
                    {
                        processApprove = _process.OrderBy(c => c.process_id)
                                                 .FirstOrDefault(c => c.status == false);
                        processApprove.status = true;
                    }
                    _db.process_path.AddOrUpdate(processApprove);
                    _db.SaveChanges();
                }

                Response.Redirect(Request.RawUrl);

            }
        }


        protected void Gv_status_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                View_user _User = (View_user)Application["userID"];

                LinkButton btnUpload = e.Row.FindControl("btnUpload") as LinkButton;
                LinkButton btnApprove = e.Row.FindControl("btnApprove") as LinkButton;
                LinkButton btnDownload = e.Row.FindControl("btnDownload") as LinkButton;

                if (_User.permission == "student")
                {
                    btnApprove = e.Row.FindControl("btnApprove") as LinkButton;
                    btnApprove.Visible = false;
                }
                
                btnUpload.CommandArgument = btnApprove.CommandArgument = btnDownload.CommandArgument = Convert.ToString(e.Row.RowIndex);
                HandleDataRow(e);
            }
        }

        private void HandleDataRow(GridViewRowEventArgs e)
        {
            string value = e.Row.Cells[1].Text;

            if (value == "อนุมัติแล้ว")
            {
                HandleApprovedRow(e);
            }
            else
            {
                HandleUnapprovedRow(e);
            }
        }

        private void HandleApprovedRow(GridViewRowEventArgs e)
        {
            LinkButton btnApprove = e.Row.FindControl("btnApprove") as LinkButton;
            btnApprove.Text = btnApprove.Text.Replace("อนุมัติ", "ยกเลิก").Replace("fa-check", "fa-xmark");
            btnApprove.Visible = true;
            btnApprove.CommandArgument += "0";

            LinkButton btnUpload = e.Row.FindControl("btnUpload") as LinkButton;
            btnUpload.Visible = false;

            LinkButton btnDownload = e.Row.FindControl("btnDownload") as LinkButton;
            btnDownload.Visible = false;

            FileUpload FileUploadControl = e.Row.FindControl("FileUploadControl") as FileUpload;
            FileUploadControl.Visible = false;

            DataTable dataSource = Gv_status.DataSource as DataTable;

            if (e.Row.RowIndex < dataSource.Rows.Count - 1)
            {
                string nextValue = Convert.ToString(dataSource.Rows[e.Row.RowIndex + 1][1]);
                if (nextValue == "อนุมัติแล้ว")
                {
                    btnApprove.Visible = false;
                }
            }
        }

        private void HandleUnapprovedRow(GridViewRowEventArgs e)
        {
            DataTable dataSource = Gv_status.DataSource as DataTable;
            string prevValue = Convert.ToString(dataSource.Rows[e.Row.RowIndex][1]);

            if (prevValue == "ยังไม่อนุมัติ")
            {
                View_user _User = (View_user)Application["userID"];
                List<process_path> _process = Application["process"] as List<process_path>;
                process_path processUpload = _process.OrderBy(c => c.process_id)
                                                     .FirstOrDefault(c => c.status == false);

                if (_User.permission == "student" && processUpload.path_teacher == null)
                {
                    LinkButton btnDownload = e.Row.FindControl("btnDownload") as LinkButton;
                    btnDownload.Visible = false;
                }

                if (_User.permission != "student" && processUpload.path_student == null)
                {
                    LinkButton btnDownload = e.Row.FindControl("btnDownload") as LinkButton;
                    btnDownload.Visible = false;
                }
            }

            if (e.Row.RowIndex == 0)
            {
                return;
            }
            else
            {
                prevValue = Gv_status.Rows[e.Row.RowIndex - 1].Cells[1].Text;
                if (prevValue == "ยังไม่อนุมัติ")
                {
                    FileUpload FileUploadControl = e.Row.FindControl("FileUploadControl") as FileUpload;
                    FileUploadControl.Visible = false;

                    LinkButton btnUpload = e.Row.FindControl("btnUpload") as LinkButton;
                    btnUpload.Visible = false;

                    LinkButton btnDownload = e.Row.FindControl("btnDownload") as LinkButton;
                    btnDownload.Visible = false;

                    LinkButton btnApprove = e.Row.FindControl("btnApprove") as LinkButton;
                    btnApprove.Visible = false;
                }
            }
        }

        private void LoadDataToGridview(int _researchID)
        {
            using (var _db = new ResearchDBEntities())
            {
                List<View_process> _viewProcess = _db.View_process.Where(c => c.research_id == _researchID)
                                                  .OrderBy(c => c.process_id)
                                                  .ToList();

                List<process_path> _process = _db.process_path.Where(c => c.research_id == _researchID)
                                                                  .OrderBy(c => c.process_id)
                                                                  .ToList();

                if (_viewProcess.Count == 0)
                    return;

                Application["process"] = _process;
                // Create sample data
                DataTable _dt = new DataTable();
                _dt.Columns.Add("step", typeof(string));
                _dt.Columns.Add("status", typeof(string));
                _dt.Columns.Add("buttons", typeof(string));


                _viewProcess.ForEach(c =>
                {
                    _dt.Rows.Add(c.processResearch, c.status ? "อนุมัติแล้ว" : "ยังไม่อนุมัติ", "");
                });

                Gv_status.DataSource = _dt;
                Gv_status.DataBind();
            }
                
        }

        protected void Gv_status_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (var _db = new ResearchDBEntities())
            {
                int ddl_select = Convert.ToInt32(Ddl_research.SelectedValue);
                View_user _User = (View_user)Application["userID"];
                research research = _db.researches.FirstOrDefault(c => c.research_id == ddl_select);
                LoadDataToGridview(ddl_select);
            }
                
        }
    }
}
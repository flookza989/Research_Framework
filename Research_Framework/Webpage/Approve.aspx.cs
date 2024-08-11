using Research_Framework.ApplicationDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Research_Framework.Webpage
{
    public partial class Approve : System.Web.UI.Page
    {
        private ResearchDBEntities _db; // Database context instance
        private List<process_path> _processes; // List of processes
        private user _currentUser; // Current user

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if user is logged in
                _currentUser = GetCurrentUser();
                if (_currentUser == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                _db = new ResearchDBEntities();
                LoadResearchAndProcesses();
                LoadDataToGridview();
            }
        }

        private user GetCurrentUser()
        {
            return Session["userID"] as user;
        }

        private void LoadResearchAndProcesses()
        {
            if (_currentUser.permission == "student")
            {

                research_member _researchMember = _db.research_member.FirstOrDefault(c => c.user_id == _currentUser.user_id);
                if (_researchMember != null)
                {
                    Tb_researchName.Text = _researchMember.research.research_name;
                    _processes = _db.process_path.Where(c => c.research_id == _researchMember.research_id)
                                                 .OrderBy(c => c.process_id)
                                                 .ToList();
                }
            }
            else
            {
                Tb_researchName.Visible = false;
                Ddl_research.Visible = true;

                List<research> _researches = _db.researches.Where(c => c.teacher_id == _currentUser.user_id).ToList();
                _researches.ForEach(c =>
                {
                    Ddl_research.Items.Add(new ListItem(c.research_name, c.research_id.ToString()));
                });

                if (_researches.Count > 0)
                {
                    int researchId = _researches.First().research_id;
                    _processes = _db.process_path.Where(c => c.research_id == researchId)
                                                 .OrderBy(c => c.process_id)
                                                 .ToList();
                }
            }
        }

        protected void Gv_status_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            if (_db == null)
            {
                _db = new ResearchDBEntities();
            }

            if (_currentUser == null)
            {
                _currentUser = GetCurrentUser();
            }

            if (_processes == null)
            {
                LoadResearchAndProcesses();
            }



            process_path processToUpdate = _processes.OrderBy(c => c.process_id).ElementAt(rowIndex);

            switch (e.CommandName)
            {
                case "UploadClick":
                    HandleUploadClick(rowIndex, processToUpdate);
                    break;
                case "DownloadClick":
                    HandleDownloadClick(processToUpdate);
                    break;
                case "ApproveClick":
                    HandleApproveClick(rowIndex, processToUpdate);
                    break;
                case "RequestClick":
                    HandleRequestClick(rowIndex, processToUpdate);
                    break;
            }

            Response.Redirect(Request.RawUrl);
        }

        private void HandleUploadClick(int rowIndex, process_path processToUpdate)
        {
            LinkButton btnUpload = Gv_status.Rows[rowIndex].FindControl("btnUpload") as LinkButton;
            string btnUploadText = btnUpload.Text;
            int index = btnUploadText.IndexOf("ยกเลิกอัพโหลด");

            if (index != -1)
            {
                if (_currentUser.permission == "student")
                    processToUpdate.path_student = null;
                else
                    processToUpdate.path_teacher = null;

                _db.process_path.AddOrUpdate(processToUpdate);
                _db.SaveChanges();
            }
            else
            {
                FileUpload fileUploadControl = Gv_status.Rows[rowIndex].FindControl("FileUploadControl") as FileUpload;

                if (fileUploadControl.HasFile)
                {
                    string fileExtension = System.IO.Path.GetExtension(fileUploadControl.FileName).ToLower();

                    if (fileExtension == ".pdf")
                    {
                        byte[] fileData = fileUploadControl.FileBytes;

                        if (_currentUser.permission == "student")
                            processToUpdate.path_student = fileData;
                        else
                            processToUpdate.path_teacher = fileData;

                        _db.process_path.AddOrUpdate(processToUpdate);
                        _db.SaveChanges();
                    }
                    else
                    {
                        ShowSweetAlert("พบข้อผิดพลาด", "เฉพาะไฟล์นามสกุล pdf เท่านั้น", "error");
                    }
                }
                else
                {
                    ShowSweetAlert("พบข้อผิดพลาด", "กรุณาเลือกไฟล์อัพโหลด", "error");
                }
            }
        }

        private void HandleDownloadClick(process_path processToUpdate)
        {
            byte[] pdfBytes = _currentUser.permission == "student" ? processToUpdate.path_teacher : processToUpdate.path_student;

            if (pdfBytes != null)
            {
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", $"attachment;filename={processToUpdate.research.research_name}_{processToUpdate.process.processResearch}.pdf");
                Response.Buffer = true;
                Response.BinaryWrite(pdfBytes);
                Response.End();
            }
        }

        private void HandleApproveClick(int rowIndex, process_path processToUpdate)
        {
            LinkButton btnApprove = Gv_status.Rows[rowIndex].FindControl("btnApprove") as LinkButton;
            string btnApproveText = btnApprove.Text;
            string wordToFind = "อนุมัติ";

            Regex regex = new Regex("\\b" + wordToFind + "\\b", RegexOptions.IgnoreCase);
            Match match = regex.Match(btnApproveText);

            if (match.Success)
            {
                processToUpdate.status = "อนุมัติแล้ว";
            }
            else
            {
                if (processToUpdate.process_id == 2 || processToUpdate.process_id == 4)
                {
                    processToUpdate.status = "รออนุมัติ";
                }
                else
                {
                    processToUpdate.status = "ยังไม่อนุมัติ";

                    
                }
            }
            _db.process_path.AddOrUpdate(processToUpdate);
            _db.SaveChanges();
        }

        private void HandleRequestClick(int rowIndex, process_path processToUpdate)
        {
            if (processToUpdate.status == "รออนุมัติ")
            {
                processToUpdate.status = "ยังไม่อนุมัติ";
            }
            else if (processToUpdate.status == "ยังไม่อนุมัติ")
            {
                processToUpdate.status = "รออนุมัติ";
            }

            _db.process_path.AddOrUpdate(processToUpdate);
            _db.SaveChanges();
        }

        protected void Gv_status_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (_processes == null)
            {
                LoadResearchAndProcesses();
            }

            if (_currentUser == null)
            {
                _currentUser = GetCurrentUser();
            }

            if (_db == null)
            {
                _db = new ResearchDBEntities();
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int rowIndex = e.Row.RowIndex;
                process_path currentProcess = _processes.OrderBy(c => c.process_id).ElementAt(rowIndex);

                FileUpload fileUploadControl = e.Row.FindControl("FileUploadControl") as FileUpload;
                LinkButton btnUpload = e.Row.FindControl("btnUpload") as LinkButton;
                LinkButton btnApprove = e.Row.FindControl("btnApprove") as LinkButton;
                LinkButton btnDownload = e.Row.FindControl("btnDownload") as LinkButton;
                LinkButton btnRequest = e.Row.FindControl("btnRequest") as LinkButton;

                SetControlVisibility(currentProcess, fileUploadControl, btnUpload, btnApprove, btnDownload, btnRequest);

                btnUpload.CommandArgument = btnApprove.CommandArgument = btnDownload.CommandArgument = btnRequest.CommandArgument = Convert.ToString(rowIndex);

                if (currentProcess.status != "อนุมัติแล้ว")
                {
                    SetButtonTextAndVisibility(currentProcess, fileUploadControl, btnUpload, btnDownload);
                }

                HandleDataRow(e, currentProcess);
            }
        }

        private void SetControlVisibility(process_path currentProcess, FileUpload fileUploadControl, LinkButton btnUpload, LinkButton btnApprove, LinkButton btnDownload, LinkButton btnRequest)
        {
            if (_currentUser.permission == "student")
            {
                btnApprove.Visible = false;
            }

            if (_currentUser.permission == "teacher")
            {
                btnRequest.Visible = false;
            }

            if (currentProcess.process_id == 1 || currentProcess.process_id == 3 || currentProcess.process_id == 5)
            {
                btnUpload.Visible = true;
                btnApprove.Visible = true;
                btnDownload.Visible = true;
                fileUploadControl.Visible = true;
            }
            else
            {
                btnUpload.Visible = false;
                btnApprove.Visible = false;
                btnDownload.Visible = false;
                fileUploadControl.Visible = false;
            }
        }

        private void SetButtonTextAndVisibility(process_path currentProcess, FileUpload fileUploadControl, LinkButton btnUpload, LinkButton btnDownload)
        {
            if (_currentUser.permission == "student")
            {
                if (currentProcess.path_teacher == null)
                {
                    btnDownload.Visible = false;
                }
                if (currentProcess.path_student != null)
                {
                    fileUploadControl.Visible = false;
                    btnUpload.Text = btnUpload.Text.Replace("อัพโหลด", "ยกเลิกอัพโหลด").Replace("fa-file-pdf", "fa-xmark");
                }
            }
            else
            {
                if (currentProcess.path_student == null)
                {
                    btnDownload.Visible = false;
                }
                if (currentProcess.path_teacher != null)
                {
                    fileUploadControl.Visible = false;
                    btnUpload.Text = btnUpload.Text.Replace("อัพโหลด", "ยกเลิกอัพโหลด").Replace("fa-file-pdf", "fa-xmark");
                }
            }
        }

        private void HandleDataRow(GridViewRowEventArgs e, process_path currentProcess)
        {
            if (currentProcess.status == "อนุมัติแล้ว")
            {
                HandleApprovedRow(e);
            }
            else
            {
                HandleUnapprovedRow(e, currentProcess);
            }
        }

        private void HandleApprovedRow(GridViewRowEventArgs e)
        {
            LinkButton btnApprove = e.Row.FindControl("btnApprove") as LinkButton;
            btnApprove.Text = btnApprove.Text.Replace("อนุมัติ", "ยกเลิก").Replace("fa-check", "fa-xmark");
            btnApprove.Visible = true;
            //btnApprove.CommandArgument += "0";

            LinkButton btnUpload = e.Row.FindControl("btnUpload") as LinkButton;
            btnUpload.Visible = false;

            LinkButton btnDownload = e.Row.FindControl("btnDownload") as LinkButton;
            btnDownload.Visible = false;

            LinkButton btnRequest = e.Row.FindControl("btnRequest") as LinkButton;
            btnRequest.Visible = false;

            FileUpload fileUploadControl = e.Row.FindControl("FileUploadControl") as FileUpload;
            fileUploadControl.Visible = false;

            TextBox tbDatetime = e.Row.FindControl("TbDatetime") as TextBox;
            tbDatetime.Visible = false;

            DataTable dataSource = Gv_status.DataSource as DataTable;

            if (e.Row.RowIndex < dataSource.Rows.Count - 1)
            {
                string nextValue = Convert.ToString(dataSource.Rows[e.Row.RowIndex + 1][1]);
                if (nextValue == "อนุมัติแล้ว")
                {
                    btnApprove.Visible = false;
                }
            }

            if (_currentUser.permission == "student")
                btnApprove.Visible = false;
        }

        private void HandleUnapprovedRow(GridViewRowEventArgs e, process_path currentProcess)
        {
            DataTable dataSource = Gv_status.DataSource as DataTable;
            string prevValue = Convert.ToString(dataSource.Rows[e.Row.RowIndex][1]);
            string processes = Convert.ToString(dataSource.Rows[e.Row.RowIndex][0]);

            TextBox tbDatetime = e.Row.FindControl("TbDatetime") as TextBox;
            if (currentProcess.process_id == 2 || currentProcess.process_id == 4)
            {
                if (_currentUser.permission == "student")
                {

                }
                else if (_currentUser.permission == "teacher")
                {

                }
            }
            else
            {
                tbDatetime.Visible = false;
            }

            LinkButton btnRequest = e.Row.FindControl("btnRequest") as LinkButton;

            if (prevValue == "รออนุมัติ")
            {
                if (_currentUser.permission == "student")
                {
                    btnRequest.Text = btnRequest.Text.Replace("ยื่นสอบ", "ยกเลิกยื่นสอบ").Replace("fa-bell", "fa-xmark");
                    btnRequest.Visible = true;
                }
            }

            LinkButton btnApprove = e.Row.FindControl("btnApprove") as LinkButton;

            if (_currentUser.permission == "student")
                btnApprove.Visible = false;

            if (_currentUser.permission == "teacher")
            {
                if (currentProcess.path_student != null)
                {
                    btnApprove.Visible = true;
                }
                else
                {
                    FileUpload fileUploadControl = e.Row.FindControl("FileUploadControl") as FileUpload;
                    LinkButton btnUpload = e.Row.FindControl("btnUpload") as LinkButton;
                    fileUploadControl.Visible = false;
                    btnUpload.Visible = false;
                    btnApprove.Visible = false;
                }
            }

            if (processes == "สอบหัวข้อ" || processes == "สอบจบ")
            {
                if (prevValue == "ยังไม่อนุมัติ" && _currentUser.permission == "teacher")
                    btnApprove.Visible = false;

                if (prevValue == "รออนุมัติ" && _currentUser.permission == "teacher")
                    btnApprove.Visible = true;
            }
            else
            {
                btnRequest.Visible = false;
            }

            LinkButton btnDownload = e.Row.FindControl("btnDownload") as LinkButton;

            if (_currentUser.permission == "student" && currentProcess.path_teacher == null)
                btnDownload.Visible = false;

            if (_currentUser.permission == "teacher" && currentProcess.path_student == null)
                btnDownload.Visible = false;

            if (e.Row.RowIndex == 0)
            {
                return;
            }
            else
            {
                prevValue = Gv_status.Rows[e.Row.RowIndex - 1].Cells[1].Text;
                if (prevValue != "อนุมัติแล้ว")
                {
                    FileUpload fileUploadControl = e.Row.FindControl("FileUploadControl") as FileUpload;
                    fileUploadControl.Visible = false;

                    LinkButton btnUpload = e.Row.FindControl("btnUpload") as LinkButton;
                    btnUpload.Visible = false;

                    btnDownload.Visible = false;

                    btnApprove.Visible = false;

                    btnRequest.Visible = false;
                }
            }
        }

        private void LoadDataToGridview()
        {
            if (_processes ==null || _processes.Count == 0)
                return;

            // Create sample data
            DataTable _dt = new DataTable();
            _dt.Columns.Add("step", typeof(string));
            _dt.Columns.Add("status", typeof(string));
            _dt.Columns.Add("buttons", typeof(string));

            _processes.ForEach(c =>
            {
                _dt.Rows.Add(c.process.processResearch, c.status, "");
            });

            Gv_status.DataSource = _dt;
            Gv_status.DataBind();
        }

        protected void Gv_status_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_currentUser == null)
            {
                _currentUser = GetCurrentUser();
            }

            if (_db == null)
            {
                _db = new ResearchDBEntities();
            }

            if (_currentUser.permission == "teacher")
            {
                int selectedResearchId = Convert.ToInt32(Ddl_research.SelectedValue);
                _processes = _db.process_path.Where(c => c.research_id == selectedResearchId)
                                             .OrderBy(c => c.process_id)
                                             .ToList();
                LoadDataToGridview();
            }
        }

        private void ShowSweetAlert(string title, string message, string type)
        {
            string script = $"swal('{title}', '{message}', '{type}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert", script, true);
        }
    }
}
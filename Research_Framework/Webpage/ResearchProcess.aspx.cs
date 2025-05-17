using Research_Framework.ApplicationDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Research_Framework.Webpage
{
    public partial class ResearchProcess : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (Session["UserID"] == null)
                {
                    Response.Redirect("~/Webpage/Login.aspx");
                    return;
                }

                string userType = Session["UserType"]?.ToString();

                // แสดง/ซ่อน dropdown ตามประเภทผู้ใช้
                researchSelector.Visible = (userType == "TEACHER");

                if (userType == "TEACHER")
                {
                    LoadResearchList();
                }
                
                LoadResearchInfo(); // โหลดข้อมูลงานวิจัย (ทั้งกรณีนักศึกษาและอาจารย์)
                LoadProcesses(); // โหลดข้อมูลขั้นตอนการวิจัย
            }
        }

        protected string GetSelectedResearchId()
        {
            if (DdlResearch.Visible && !string.IsNullOrEmpty(DdlResearch.SelectedValue))
            {
                return DdlResearch.SelectedValue;
            }

            // กรณีเป็นนักศึกษาหรือไม่ได้เลือกงานวิจัย
            using (var db = new ResearchDBEntities())
            {
                int currentUserId = Convert.ToInt32(Session["UserID"]);
                var student = db.students.FirstOrDefault(s => s.user_id == currentUserId);
                if (student != null)
                {
                    var researchGroup = db.research_groups.FirstOrDefault(rg => rg.student_id == student.id);
                    return researchGroup?.research_id.ToString() ?? "0";
                }
            }
            return "0";
        }

        protected void BtnApprove_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserType"]?.ToString() != "TEACHER")
                {
                    ShowError("ไม่มีสิทธิ์ในการอนุมัติ");
                    return;
                }

                var btn = (LinkButton)sender;
                string[] args = btn.CommandArgument.Split(';');
                if (args.Length != 2)
                {
                    ShowError("ข้อมูลไม่ถูกต้อง");
                    return;
                }

                int processId = Convert.ToInt32(args[0]);
                int researchId = Convert.ToInt32(args[1]);
                var comments = ((TextBox)btn.NamingContainer.FindControl("TbComments")).Text;

                using (var db = new ResearchDBEntities())
                {
                    var process = db.research_process.FirstOrDefault(p =>
                        p.process_id == processId &&
                        p.research_id == researchId);

                    if (process == null)
                    {
                        ShowError("ไม่พบข้อมูลขั้นตอนที่ต้องการอนุมัติ");
                        return;
                    }

                    process.status = "APPROVED";
                    process.approved_by = Convert.ToInt32(Session["UserID"]);
                    process.approved_date = DateTime.Now;
                    process.comments = comments;

                    db.SaveChanges();
                    ShowSuccess("อนุมัติเรียบร้อยแล้ว");
                    LoadProcesses();
                }
            }
            catch (Exception ex)
            {
                ShowError($"เกิดข้อผิดพลาด: {ex.Message}");
            }
        }

        protected void BtnReject_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["UserType"]?.ToString() != "TEACHER")
                {
                    ShowError("ไม่มีสิทธิ์ในการไม่อนุมัติ");
                    return;
                }

                var btn = (LinkButton)sender;
                string[] args = btn.CommandArgument.Split(';');
                if (args.Length != 2)
                {
                    ShowError("ข้อมูลไม่ถูกต้อง");
                    return;
                }

                int processId = Convert.ToInt32(args[0]);
                int researchId = Convert.ToInt32(args[1]);
                var comments = ((TextBox)btn.NamingContainer.FindControl("TbComments")).Text;

                if (string.IsNullOrWhiteSpace(comments))
                {
                    ShowError("กรุณาระบุเหตุผลที่ไม่อนุมัติ");
                    return;
                }

                using (var db = new ResearchDBEntities())
                {
                    var process = db.research_process.FirstOrDefault(p =>
                        p.process_id == processId &&
                        p.research_id == researchId);

                    if (process == null)
                    {
                        ShowError("ไม่พบข้อมูลขั้นตอนที่ต้องการไม่อนุมัติ");
                        return;
                    }

                    process.status = "REJECTED";
                    process.approved_by = Convert.ToInt32(Session["UserID"]);
                    process.approved_date = DateTime.Now;
                    process.comments = comments;

                    db.SaveChanges();
                    ShowSuccess("บันทึกการไม่อนุมัติเรียบร้อยแล้ว");
                    LoadProcesses();
                }
            }
            catch (Exception ex)
            {
                ShowError($"เกิดข้อผิดพลาด: {ex.Message}");
            }
        }

        protected void BtnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                var btn = (LinkButton)sender;
                int processId = Convert.ToInt32(btn.CommandArgument);
                var fileUpload = (FileUpload)btn.NamingContainer.FindControl("FileUpload");

                if (!fileUpload.HasFile)
                {
                    ShowError("กรุณาเลือกไฟล์ที่ต้องการอัพโหลด");
                    return;
                }

                // ตรวจสอบนามสกุลไฟล์
                string extension = System.IO.Path.GetExtension(fileUpload.FileName).ToLower();
                if (extension != ".pdf" && extension != ".doc" && extension != ".docx")
                {
                    ShowError("รองรับเฉพาะไฟล์ PDF และ Word เท่านั้น");
                    return;
                }

                using (var db = new ResearchDBEntities())
                {
                    // หา research_id ของผู้ใช้ปัจจุบัน
                    int currentUserId = Convert.ToInt32(Session["UserID"]);
                    var student = db.students.FirstOrDefault(s => s.user_id == currentUserId);
                    var researchGroup = db.research_groups.FirstOrDefault(rg => rg.student_id == student.id);

                    if (researchGroup == null)
                    {
                        ShowError("ไม่พบข้อมูลงานวิจัย");
                        return;
                    }

                    // ตรวจสอบว่ามี research_process แล้วหรือยัง
                    var process = db.research_process.FirstOrDefault(rp =>
                        rp.research_id == researchGroup.research_id &&
                        rp.process_id == processId);

                    // ถ้ายังไม่มีให้สร้างใหม่
                    if (process == null)
                    {
                        process = new research_process
                        {
                            research_id = researchGroup.research_id,
                            process_id = processId,
                            status = "IN_PROGRESS",
                            create_date = DateTime.Now
                        };
                        db.research_process.Add(process);
                        db.SaveChanges();
                    }

                    // สร้างโฟลเดอร์ถ้ายังไม่มี
                    string uploadPath = Server.MapPath($"~/Uploads/ResearchDocs/{researchGroup.research_id}/{processId}/");
                    if (!System.IO.Directory.Exists(uploadPath))
                    {
                        System.IO.Directory.CreateDirectory(uploadPath);
                    }

                    // สร้างชื่อไฟล์
                    string filename = $"{DateTime.Now:yyyyMMddHHmmss}_{fileUpload.FileName}";
                    string filepath = System.IO.Path.Combine(uploadPath, filename);

                    // บันทึกไฟล์
                    fileUpload.SaveAs(filepath);

                    // บันทึกข้อมูลลงฐานข้อมูล
                    var document = new process_document
                    {
                        research_process_id = process.id, // ใช้ id ของ process ที่สร้างใหม่
                        document_type = extension,
                        file_path = $"~/Uploads/ResearchDocs/{researchGroup.research_id}/{processId}/{filename}",
                        upload_date = DateTime.Now,
                        upload_by = currentUserId
                    };

                    db.process_document.Add(document);

                    // อัพเดทสถานะ
                    process.status = "WAITING_APPROVAL";
                    db.SaveChanges();

                    // รีโหลดข้อมูลใหม่
                    LoadProcesses();

                    // แสดง SweetAlert
                    ShowSuccess("อัพโหลดเอกสารเรียบร้อยแล้ว");
                }
                }
    catch (Exception ex)
            {
                ShowError($"เกิดข้อผิดพลาด: {ex.Message}");
            }
        }

        protected void BtnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                var btn = (LinkButton)sender;
                int documentId = Convert.ToInt32(btn.CommandArgument);

                using (var db = new ResearchDBEntities())
                {
                    var document = db.process_document.Find(documentId);
                    if (document != null)
                    {
                        string filepath = Server.MapPath(document.file_path);
                        if (System.IO.File.Exists(filepath))
                        {
                            Response.Clear();
                            Response.ContentType = GetContentType(System.IO.Path.GetExtension(document.file_path));
                            Response.AppendHeader("Content-Disposition",
                                $"attachment; filename={System.IO.Path.GetFileName(document.file_path)}");
                            Response.TransmitFile(filepath);
                            Response.End();
                        }
                        else
                        {
                            ShowError("ไม่พบไฟล์ที่ต้องการดาวน์โหลด");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"เกิดข้อผิดพลาด: {ex.Message}");
            }
        }

        private string GetContentType(string extension)
        {
            switch (extension.ToLower())
            {
                case ".pdf":
                    return "application/pdf";
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                default:
                    return "application/octet-stream";
            }
        }

        public string GetStatusText(string status)
        {
            switch (status?.ToUpper())
            {
                case "PENDING": return "รอดำเนินการ";
                case "IN_PROGRESS": return "กำลังดำเนินการ";
                case "WAITING_APPROVAL": return "รออนุมัติ";
                case "NEED_REVISION": return "ต้องแก้ไข";
                case "APPROVED": return "อนุมัติแล้ว";
                case "REJECTED": return "ไม่อนุมัติ";
                default: return "ไม่ระบุ";
            }
        }

        private string GetStatusColorClass(string status)
        {
            switch (status?.ToUpper())
            {
                case "PENDING": return "bg-secondary";
                case "IN_PROGRESS": return "bg-primary";
                case "WAITING_APPROVAL": return "bg-warning";
                case "NEED_REVISION": return "badge bg-warning";
                case "APPROVED": return "bg-success";
                case "REJECTED": return "bg-danger";
                default: return "bg-secondary";
            }
        }

        private void LoadResearchSchedule(int researchId)
        {
            try
            {
                // โหลดข้อมูลกำหนดการ/ตารางเวลา
                using (var db = new ResearchDBEntities())
                {
                    // โค้ดสำหรับโหลดกำหนดการ
                }
            }
            catch (Exception ex)
            {
                ShowError($"เกิดข้อผิดพลาดในการโหลดกำหนดการ: {ex.Message}");
            }
        }

        private void LoadResearchProcesses(int researchId)
        {
            try
            {
                using (var db = new ResearchDBEntities())
                {
                    var processes = (from mp in db.master_process
                                     join rp in db.research_process
                                     on new { ProcessId = mp.id, ResearchId = researchId }
                                     equals new { ProcessId = rp.process_id, ResearchId = rp.research_id }
                                     into rpGroup
                                     from rp in rpGroup.DefaultIfEmpty()
                                     orderby mp.sequence_no
                                     select new
                                     {
                                         ProcessId = mp.id,
                                         ProcessName = mp.name,
                                         Status = rp != null ? rp.status : "PENDING",
                                         Comments = rp != null ? rp.comments : "",
                                         ApprovedDate = rp != null ? rp.approved_date : (DateTime?)null
                                     }).ToList();

                    // Bind ข้อมูลไปที่ control ที่ใช้แสดงผล
                    RptProcesses.DataSource = processes;
                    RptProcesses.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShowError($"เกิดข้อผิดพลาดในการโหลดขั้นตอน: {ex.Message}");
            }
        }

        private void ShowNoResearchMessage()
        {
            string userType = Session["UserType"]?.ToString();
            string message = userType == "STUDENT"
                ? "คุณยังไม่มีงานวิจัย กรุณาสร้างงานวิจัยใหม่"
                : "ไม่พบงานวิจัยที่รับเป็นที่ปรึกษา";

            ScriptManager.RegisterStartupScript(this, GetType(), "noResearch",
                $"Swal.fire({{ " +
                $"  title: 'แจ้งเตือน', " +
                $"  text: '{message}', " +
                $"  icon: 'info', " +
                $"  confirmButtonText: 'ตกลง' " +
                $"}}).then((result) => {{ " +
                $"  if (result.isConfirmed) {{ " +
                $"    window.location = 'AddResearch.aspx'; " +
                $"  }} " +
                $"}});", true);
        }

        private void LoadResearchInfo(int? specificResearchId = null)
        {
            try
            {
                if (Session["UserID"] == null)
                {
                    Response.Redirect("~/Webpage/Login.aspx");
                    return;
                }

                int currentUserId = Convert.ToInt32(Session["UserID"]);

                using (var db = new ResearchDBEntities())
                {
                    var user = db.users.Find(currentUserId);
                    if (user == null) return;

                    researches currentResearch = null;

                    // กรณีเป็นนักศึกษา
                    if (user.user_type == "STUDENT")
                    {
                        var student = db.students.FirstOrDefault(s => s.user_id == currentUserId);
                        if (student != null)
                        {
                            var researchGroup = db.research_groups
                                .FirstOrDefault(rg => rg.student_id == student.id);

                            if (researchGroup != null)
                            {
                                // ดึงข้อมูลงานวิจัยพร้อมข้อมูลที่เกี่ยวข้อง
                                var research = db.researches.Find(researchGroup.research_id);
                                
                                if (research != null)
                                {
                                    // ดึงข้อมูลอาจารย์ที่ปรึกษา
                                    var advisor = db.users.Find(research.advisor_id);
                                    
                                    if (advisor != null)
                                    {
                                        currentResearch = research;
                                        
                                        // แสดงชื่องานวิจัยและอาจารย์ที่ปรึกษา
                                        TbResearchName.Text = research.name;
                                        TbAdvisor.Text = $"อ.{advisor.first_name} {advisor.last_name}";
                                        
                                        // แสดงสถานะงานวิจัย
                                        LbResearchStatus.Text = GetStatusText(research.status);
                                        LbResearchStatus.CssClass = $"badge {GetStatusClass(research.status)}";
                                    }
                                }
                            }
                        }
                    }
                    // กรณีเป็นอาจารย์
                    else if (user.user_type == "TEACHER")
                    {
                        // กรณีระบุงานวิจัยเฉพาะ
                        if (specificResearchId.HasValue)
                        {
                            var research = db.researches.Find(specificResearchId.Value);
                            if (research != null && research.advisor_id == currentUserId)
                            {
                                currentResearch = research;
                                
                                // ดึงข้อมูลหัวหน้ากลุ่ม
                                var leaderGroup = db.research_groups
                                    .FirstOrDefault(rg => rg.research_id == research.id && rg.is_leader);
                                    
                                if (leaderGroup != null)
                                {
                                    var student = db.students.Find(leaderGroup.student_id);
                                    if (student != null)
                                    {
                                        var leaderUser = db.users.Find(student.user_id);
                                        if (leaderUser != null)
                                        {
                                            // แสดงชื่องานวิจัยและหัวหน้ากลุ่ม
                                            TbResearchName.Text = research.name;
                                            TbAdvisor.Text = $"หัวหน้ากลุ่ม: {leaderUser.first_name} {leaderUser.last_name} ({leaderUser.username})";
                                            LbAdvisor.InnerText = "หัวหน้ากลุ่ม";
                                            
                                            // แสดงสถานะงานวิจัย
                                            LbResearchStatus.Text = GetStatusText(research.status);
                                            LbResearchStatus.CssClass = $"badge {GetStatusClass(research.status)}";
                                        }
                                    }
                                }
                            }
                        }
                        // กรณีไม่ได้ระบุงานวิจัยเฉพาะ แต่มีงานวิจัยที่เป็นที่ปรึกษา
                        else
                        {
                            // ค้นหางานวิจัยล่าสุดที่เป็นที่ปรึกษา
                            var research = db.researches
                                .Where(r => r.advisor_id == currentUserId)
                                .OrderByDescending(r => r.id)
                                .FirstOrDefault();
                                
                            if (research != null)
                            {
                                currentResearch = research;
                                
                                // ดึงข้อมูลหัวหน้ากลุ่ม
                                var leaderGroup = db.research_groups
                                    .FirstOrDefault(rg => rg.research_id == research.id && rg.is_leader);
                                    
                                if (leaderGroup != null)
                                {
                                    var student = db.students.Find(leaderGroup.student_id);
                                    if (student != null)
                                    {
                                        var leaderUser = db.users.Find(student.user_id);
                                        if (leaderUser != null)
                                        {
                                            // แสดงชื่องานวิจัยและหัวหน้ากลุ่ม
                                            TbResearchName.Text = research.name;
                                            TbAdvisor.Text = $"หัวหน้ากลุ่ม: {leaderUser.first_name} {leaderUser.last_name} ({leaderUser.username})";
                                            LbAdvisor.InnerText = "หัวหน้ากลุ่ม";
                                            
                                            // แสดงสถานะงานวิจัย
                                            LbResearchStatus.Text = GetStatusText(research.status);
                                            LbResearchStatus.CssClass = $"badge {GetStatusClass(research.status)}";
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (currentResearch == null)
                    {
                        // กรณีไม่พบข้อมูลงานวิจัย
                        TbResearchName.Text = "";
                        TbAdvisor.Text = "";
                        LbResearchStatus.Text = "";
                        
                        if (!IsPostBack)  // แสดงข้อความเตือนเฉพาะตอนโหลดหน้าแรก
                        {
                            ShowNoResearchMessage();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"เกิดข้อผิดพลาดในการโหลดข้อมูล: {ex.Message}");
            }
        }

        //private research GetCurrentResearch(int userId)
        //{
        //    using (var db = new ResearchDBEntities())
        //    {
        //        // หาว่าเป็นนักศึกษาหรืออาจารย์
        //        var user = db.users.Find(userId);
        //        if (user == null) return null;

        //        if (user.user_type == "STUDENT")
        //        {
        //            // กรณีเป็นนักศึกษา หางานวิจัยจาก research_groups
        //            var student = db.students.FirstOrDefault(s => s.user_id == userId);
        //            if (student == null) return null;

        //            var researchGroup = db.research_groups
        //                .FirstOrDefault(rg => rg.student_id == student.id);
        //            if (researchGroup == null) return null;

        //            return db.researches.Find(researchGroup.research_id);
        //        }
        //        else if (user.user_type == "TEACHER")
        //        {
        //            // กรณีเป็นอาจารย์ หางานวิจัยที่เป็นที่ปรึกษา
        //            var teacher = db.teachers.FirstOrDefault(t => t.user_id == userId);
        //            if (teacher == null) return null;

        //            return db.researches
        //                .FirstOrDefault(r => r.advisor_id == userId);
        //        }

        //        return null;
        //    }
        //}

        protected bool IsUploadAllowed(object statusObj)
        {
            if (statusObj == null) return false;

            string status = statusObj.ToString();
            return status != "APPROVED" && status != "REJECTED";
        }

        protected bool IsTeacher()
        {
            if (Session["UserType"] == null) return false;
            return Session["UserType"].ToString() == "TEACHER";
        }

        protected bool HasPermission(string action, object processObj = null)
        {
            if (Session["UserID"] == null) return false;
            int userId = Convert.ToInt32(Session["UserID"]);

            using (var db = new ResearchDBEntities())
            {
                var user = db.users.Find(userId);
                if (user == null) return false;

                switch (action.ToUpper())
                {
                    case "UPLOAD":
                        // นักศึกษาอัพโหลดได้ถ้าเป็นหัวหน้ากลุ่ม
                        if (user.user_type == "STUDENT")
                        {
                            var student = db.students.FirstOrDefault(s => s.user_id == userId);
                            if (student == null) return false;

                            return db.research_groups.Any(rg =>
                                rg.student_id == student.id &&
                                rg.is_leader);
                        }
                        return false;

                    case "APPROVE":
                        // อาจารย์ที่ปรึกษาอนุมัติได้
                        return user.user_type == "TEACHER";

                    case "VIEW":
                        // ทุกคนที่เกี่ยวข้องดูได้
                        return true;

                    default:
                        return false;
                }
            }
        }

        private void LoadResearchList()
        {
            try
            {
                int currentUserId = Convert.ToInt32(Session["UserID"]);
                using (var db = new ResearchDBEntities())
                {
                    var researchList = (from r in db.researches
                                        join rg in db.research_groups on r.id equals rg.research_id
                                        join s in db.students on rg.student_id equals s.id
                                        join u in db.users on s.user_id equals u.id
                                        where r.advisor_id == currentUserId
                                        && rg.is_leader
                                        select new
                                        {
                                            r.id,
                                            r.name,
                                            StudentName = u.first_name + " " + u.last_name,
                                            u.username
                                        })
                                      .ToList() // ดึงข้อมูลมาก่อน
                                      .Select(x => new // จัดรูปแบบข้อความหลังจากดึงข้อมูลแล้ว
                                      {
                                          id = x.id,
                                          DisplayText = $"{x.name} - {x.StudentName} ({x.username})"
                                      })
                                      .ToList();

                    DdlResearch.DataSource = researchList;
                    DdlResearch.DataTextField = "DisplayText";
                    DdlResearch.DataValueField = "id";
                    DdlResearch.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShowError($"เกิดข้อผิดพลาดในการโหลดรายการงานวิจัย: {ex.Message}");
            }
        }

        protected void DdlResearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(DdlResearch.SelectedValue))
            {
                int researchId = Convert.ToInt32(DdlResearch.SelectedValue);
                LoadResearchInfo(researchId);
                LoadProcesses();
            }
            else
            {
                // กรณีเลือก "-- เลือกงานวิจัย --"
                TbResearchName.Text = "";
                TbAdvisor.Text = "";
                RptProcesses.DataSource = null;
                RptProcesses.DataBind();
            }
        }

        private void LoadProcesses()
        {
            try
            {
                using (var db = new ResearchDBEntities())
                {
                    int currentUserId = Convert.ToInt32(Session["UserID"]);
                    int? selectedResearchId = null;

                    var user = db.users.Find(currentUserId);
                    if (user == null) return;
                    
                    if (user.user_type == "TEACHER")
                    {
                        // กรณีเป็นอาจารย์ ตรวจสอบการเลือกจาก dropdown ก่อน
                        if (!string.IsNullOrEmpty(DdlResearch.SelectedValue))
                        {
                            selectedResearchId = Convert.ToInt32(DdlResearch.SelectedValue);
                        }
                        else
                        {
                            // ถ้าไม่ได้เลือก ให้ใช้งานวิจัยล่าสุด
                            var research = db.researches
                                .Where(r => r.advisor_id == currentUserId)
                                .OrderByDescending(r => r.id)
                                .FirstOrDefault();
                                
                            if (research != null)
                            {
                                selectedResearchId = research.id;
                            }
                        }
                    }
                    else if (user.user_type == "STUDENT")
                    {
                        // กรณีเป็นนักศึกษา ใช้งานวิจัยของตนเอง
                        var student = db.students.FirstOrDefault(s => s.user_id == currentUserId);
                        if (student != null)
                        {
                            var researchGroup = db.research_groups.FirstOrDefault(rg => rg.student_id == student.id);
                            if (researchGroup != null)
                            {
                                selectedResearchId = researchGroup.research_id;
                            }
                        }
                    }

                    if (!selectedResearchId.HasValue)
                    {
                        RptProcesses.DataSource = null;
                        RptProcesses.DataBind();
                        return;
                    }

                    // โหลดขั้นตอนการดำเนินงานวิจัย
                    var processes = from mp in db.master_process
                                    join rp in db.research_process
                                    on new { ProcessId = mp.id, ResearchId = selectedResearchId.Value }
                                    equals new { ProcessId = rp.process_id, ResearchId = rp.research_id }
                                    into rpGroup
                                    from rp in rpGroup.DefaultIfEmpty()
                                    orderby mp.sequence_no
                                    select new
                                    {
                                        ProcessId = mp.id,
                                        ProcessName = mp.name,
                                        Status = rp != null ? rp.status : "PENDING",
                                        Comments = rp != null ? rp.comments : "",
                                        ApprovedDate = rp != null ? rp.approved_date : (DateTime?)null,
                                        ResearchProcessId = rp != null ? rp.id : (int?)null
                                    };

                    // ดึงข้อมูลมาก่อน
                    var processResults = processes.ToList();

                    // แปลงข้อมูลและจัดการ Path.GetFileName หลังจากดึงข้อมูลแล้ว
                    var processViewModels = processResults.Select(p => new ProcessViewModel
                    {
                        ProcessId = p.ProcessId,
                        ProcessName = p.ProcessName,
                        Status = p.Status,
                        Comments = p.Comments,
                        ApprovedDate = p.ApprovedDate,
                        Documents = p.ResearchProcessId.HasValue ?
                            db.process_document
                                .Where(pd => pd.research_process_id == p.ResearchProcessId)
                                .ToList() // ดึงข้อมูลมาก่อน
                                .Select(pd => new DocumentViewModel
                                {
                                    Id = pd.id,
                                    FileName = System.IO.Path.GetFileName(pd.file_path), // ใช้ Path.GetFileName หลังจากดึงข้อมูลแล้ว
                                    FilePath = pd.file_path,
                                    UploadDate = pd.upload_date,
                                    Status = pd.document_status ?? "PENDING"
                                })
                                .ToList()
                            : new List<DocumentViewModel>()
                    }).ToList();

                    RptProcesses.DataSource = processViewModels;
                    RptProcesses.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShowError($"เกิดข้อผิดพลาดในการโหลดข้อมูล: {ex.Message}");
            }
        }

        protected void RptProcesses_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var documentRepeater = e.Item.FindControl("RptDocuments") as Repeater;
                var processData = e.Item.DataItem as ProcessViewModel;

                if (documentRepeater != null && processData != null)
                {
                    documentRepeater.DataSource = processData.Documents;
                    documentRepeater.DataBind();
                }
            }
        }

        protected void BtnSendFeedback_Click(object sender, EventArgs e)
        {
            try
            {
                var btn = (LinkButton)sender;
                int documentId = Convert.ToInt32(btn.CommandArgument);

                // หา control
                var container = btn.NamingContainer;
                var fileUpload = (FileUpload)container.FindControl("FileUploadFeedback");
                var txtFeedback = (TextBox)container.FindControl("TbFeedback");

                if (!fileUpload.HasFile)
                {
                    ShowError("กรุณาเลือกไฟล์ที่ต้องการอัพโหลด");
                    return;
                }

                using (var db = new ResearchDBEntities())
                {
                    var document = db.process_document.Find(documentId);
                    if (document != null)
                    {
                        // อัพเดทสถานะเอกสารเก่า
                        document.document_status = "NEED_REVISION";

                        // บันทึกไฟล์ feedback
                        string uploadPath = Server.MapPath($"~/Uploads/Feedback/{document.research_process_id}/");
                        if (!System.IO.Directory.Exists(uploadPath))
                        {
                            System.IO.Directory.CreateDirectory(uploadPath);
                        }

                        string filename = $"feedback_{DateTime.Now:yyyyMMddHHmmss}_{fileUpload.FileName}";
                        string filepath = System.IO.Path.Combine(uploadPath, filename);
                        fileUpload.SaveAs(filepath);

                        // เพิ่มเอกสาร feedback
                        var feedback = new process_document
                        {
                            research_process_id = document.research_process_id,
                            document_type = System.IO.Path.GetExtension(filename),
                            file_path = $"~/Uploads/Feedback/{document.research_process_id}/{filename}",
                            upload_date = DateTime.Now,
                            upload_by = Convert.ToInt32(Session["UserID"]),
                            document_status = "FEEDBACK"
                        };

                        db.process_document.Add(feedback);

                        // อัพเดทสถานะ research_process
                        var process = db.research_process.Find(document.research_process_id);
                        if (process != null)
                        {
                            process.status = "NEED_REVISION";
                            process.comments = txtFeedback.Text;
                        }

                        db.SaveChanges();
                        ShowSuccess("ส่งผลการตรวจสอบเรียบร้อยแล้ว");
                        LoadProcesses();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"เกิดข้อผิดพลาด: {ex.Message}");
            }
        }

        protected string GetDocumentStatusClass(object statusObj)
        {
            string status = statusObj?.ToString() ?? "PENDING";
            switch (status.ToUpper())
            {
                case "PENDING":
                    return "badge bg-secondary";
                case "APPROVED":
                    return "badge bg-success";
                case "NEED_REVISION":
                    return "badge bg-warning";
                case "FEEDBACK":
                    return "badge bg-info";
                default:
                    return "badge bg-secondary";
            }
        }

        protected string GetDocumentStatusText(object statusObj)
        {
            string status = statusObj?.ToString() ?? "PENDING";
            switch (status.ToUpper())
            {
                case "PENDING":
                    return "รอตรวจสอบ";
                case "APPROVED":
                    return "ผ่านการตรวจสอบ";
                case "NEED_REVISION":
                    return "ต้องแก้ไข";
                case "FEEDBACK":
                    return "ความเห็นจากอาจารย์";
                default:
                    return "ไม่ระบุ";
            }
        }

        public string GetStatusClass(string status)
        {
            switch (status)
            {
                case "PENDING": return "bg-secondary";
                case "IN_PROGRESS": return "bg-primary";
                case "WAITING_APPROVAL": return "bg-warning";
                case "APPROVED": return "bg-success";
                case "REJECTED": return "bg-danger";
                default: return "bg-secondary"; 
            }
        }

        private void ShowError(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "error",
                $"Swal.fire('ข้อผิดพลาด', '{message}', 'error');", true);
        }

        private void ShowSuccess(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "success",
                $"Swal.fire('สำเร็จ', '{message}', 'success');", true);
        }

    }

    public class ProcessViewModel
    {
        public ProcessViewModel()
        {
            Documents = new List<DocumentViewModel>();
        }

        public int ProcessId { get; set; }
        public string ProcessName { get; set; }
        public string Status { get; set; }
        public string Comments { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public List<DocumentViewModel> Documents { get; set; }
    }

    public class DocumentViewModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadDate { get; set; }
        public string Status { get; set; }
    }
}
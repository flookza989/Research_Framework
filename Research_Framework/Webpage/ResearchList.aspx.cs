using Research_Framework.ApplicationDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Research_Framework.Webpage
{
    public partial class ResearchList : System.Web.UI.Page
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

                // ตรวจสอบว่าเป็นอาจารย์หรือไม่
                if (Session["UserType"]?.ToString() != "TEACHER")
                {
                    // ถ้าไม่ใช่อาจารย์ ให้ redirect ไปหน้าหลัก
                    Response.Redirect("~/index.aspx");
                    return;
                }

                LoadResearchList();
            }
        }

        private void LoadResearchList()
        {
            try
            {
                int currentUserId = Convert.ToInt32(Session["UserID"]);
                string searchName = TbSearchName.Text.Trim();
                string searchStatus = DdlStatus.SelectedValue;

                using (var db = new ResearchDBEntities())
                {
                    // ดึงข้อมูลวิทยานิพนธ์ที่อาจารย์คนนี้เป็นที่ปรึกษา
                    var query = from r in db.researches
                                join t in db.users
                                on r.advisor_id equals t.id
                                where r.advisor_id == currentUserId
                                select new
                                {
                                    r.id,
                                    r.name,
                                    r.status,
                                    AdvisorId = r.advisor_id,
                                    AdvisorName = t.first_name + " " + t.last_name
                                };

                    // กรองตามชื่อวิทยานิพนธ์ (ถ้ามี)
                    if (!string.IsNullOrEmpty(searchName))
                    {
                        query = query.Where(r => r.name.Contains(searchName));
                    }

                    // กรองตามสถานะ (ถ้ามี)
                    if (!string.IsNullOrEmpty(searchStatus))
                    {
                        query = query.Where(r => r.status == searchStatus);
                    }

                    var researches = query.ToList();

                    // ดึงข้อมูลหัวหน้ากลุ่มวิจัย (ข้อมูลนักศึกษา)
                    var researchLeaders = (from rg in db.research_groups
                                          join s in db.students on rg.student_id equals s.id
                                          join u in db.users on s.user_id equals u.id
                                          where rg.is_leader
                                          select new
                                          {
                                              ResearchId = rg.research_id,
                                              StudentId = s.id,
                                              StudentName = u.first_name + " " + u.last_name,
                                              Username = u.username
                                          }).ToList();

                    // เชื่อมข้อมูลวิทยานิพนธ์กับข้อมูลหัวหน้ากลุ่ม
                    var result = researches.Select(r => new
                    {
                        Id = r.id,
                        Name = r.name,
                        Status = r.status,
                        AdvisorName = r.AdvisorName,
                        StudentName = GetStudentNameFromLeaders(r.id, researchLeaders)
                    }).OrderByDescending(r => r.Id).ToList();

                    GvResearch.DataSource = result;
                    GvResearch.DataBind();
                }
            }
            catch (Exception ex)
            {
                ShowError($"เกิดข้อผิดพลาดในการโหลดข้อมูล: {ex.Message}");
            }
        }

        private string GetStudentNameFromLeaders(int researchId, IEnumerable<object> leaders)
        {
            dynamic leader = leaders.FirstOrDefault(l => ((dynamic)l).ResearchId == researchId);
            if (leader != null)
            {
                return $"{leader.StudentName} ({leader.Username})";
            }
            return "ไม่พบข้อมูลหัวหน้ากลุ่ม";
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadResearchList();
        }

        protected void GvResearch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GvResearch.PageIndex = e.NewPageIndex;
            LoadResearchList();
        }

        protected void GvResearch_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "ViewProcess")
                {
                    int researchId = Convert.ToInt32(e.CommandArgument);
                    Response.Redirect($"~/Webpage/ResearchProcess.aspx?id={researchId}");
                }
            }
            catch (Exception ex)
            {
                ShowError($"เกิดข้อผิดพลาด: {ex.Message}");
            }
        }

        protected string GetStatusText(string status)
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

        protected string GetStatusClass(string status)
        {
            switch (status?.ToUpper())
            {
                case "PENDING": return "bg-secondary";
                case "IN_PROGRESS": return "bg-primary";
                case "WAITING_APPROVAL": return "bg-warning";
                case "NEED_REVISION": return "bg-warning";
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
}
using Research_Framework.ApplicationDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.DynamicData;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Research_Framework.Webpage
{
    public partial class AddReserch : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // ตรวจสอบการเข้าสู่ระบบ
                if (Session["UserID"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                int currentStudentId = Convert.ToInt32(Session["StudentID"]);
                int currentUserId = Convert.ToInt32(Session["UserID"]);

                using (var db = new ResearchDBEntities())
                {
                    // ตรวจสอบว่ามีงานวิจัยไหม
                    var research = (from rg in db.research_groups
                                    join r in db.researches on rg.research_id equals r.id
                                    join u in db.users on r.advisor_id equals u.id
                                    where rg.student_id == currentStudentId
                                    select new
                                    {
                                        r.id,
                                        r.name,
                                        r.description,
                                        advisorId = r.advisor_id,
                                        advisorName = u.first_name + " " + u.last_name,
                                        advisorImage = u.profile_img,
                                        r.status
                                    }).FirstOrDefault();

                    if (research != null)
                    {
                        // ตรวจสอบว่าเป็นหัวหน้ากลุ่ม
                        bool isLeader = db.research_groups
                            .Any(rg => rg.research_id == research.id && rg.student_id == currentStudentId && rg.is_leader);

                        if (!isLeader)
                        {
                            // ทำให้ fields เป็น readonly
                            Tb_reserch.ReadOnly = true;
                            Tb_description.ReadOnly = true;

                            // ซ่อนปุ่มที่ไม่จำเป็น
                            BtnSave.Visible = false;

                            btnSelectTeacher.Visible = false;
                            BtnAddMember.Visible = false;
                        }


                        LbResearchStatus.Text = GetStatusText(research.status);
                        LbResearchStatus.CssClass = $"badge {GetStatusColorClass(research.status)}";

                        // แสดงข้อมูลงานวิจัย
                        Tb_reserch.Text = research.name;
                        Tb_description.Text = research.description;

                        // แสดงข้อมูลอาจารย์ที่ปรึกษา
                        HdnAdvisorId.Value = research.advisorId.ToString();
                        LtAdvisorName.InnerText = research.advisorName;

                        if (research.advisorImage != null)
                        {
                            ImgAdvisor.ImageUrl = $"data:image/jpeg;base64,{Convert.ToBase64String(research.advisorImage)}";
                        }
                        else
                        {
                            ImgAdvisor.ImageUrl = VirtualPathUtility.ToAbsolute("~/Images/NewUser.png");
                        }

                        // โหลดข้อมูลสมาชิก
                        LoadResearchMembers(research.id);
                    }
                    else
                    {
                        CheckAndRequireResearchCreation(currentStudentId);
                    }
                }
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


        // เพิ่มเมธอดใหม่สำหรับโหลดข้อมูลสมาชิก
        private void LoadResearchMembers(int researchId)
        {
            try
            {
                using (var db = new ResearchDBEntities())
                {
                    // ตรวจสอบว่า researchId ถูกต้อง
                    if (researchId <= 0)
                    {
                        throw new ArgumentException("Invalid research ID");
                    }

                    var members = from rg in db.research_groups
                                  join s in db.students on rg.student_id equals s.id
                                  join u in db.users on s.user_id equals u.id
                                  join f in db.facultys on s.faculty_id equals f.faculty_id
                                  join b in db.branchs on s.branch_id equals b.branch_id
                                  where rg.research_id == researchId
                                  select new
                                  {
                                      id = rg.id,
                                      stdID = u.username,
                                      name = u.first_name + " " + u.last_name,
                                      faculty = f.faculty_name,
                                      branch = b.branch_name,
                                      user_id = u.id,
                                      is_leader = rg.is_leader
                                  };

                    var membersList = members.ToList();
                    Dgv_std.DataSource = membersList;
                    Dgv_std.DataBind();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    $"Swal.fire('Error', '{ex.Message}', 'error');", true);
            }
        }

        [WebMethod]
        public static object GetResearchMembers()
        {
            try
            {
                int currentUserId = Convert.ToInt32(HttpContext.Current.Session["UserID"]);

                using (var db = new ResearchDBEntities())
                {
                    var student = db.students.FirstOrDefault(s => s.user_id == currentUserId);
                    if (student == null) return new { success = false, message = "ไม่พบข้อมูลนักศึกษา" };

                    var researchGroup = db.research_groups.FirstOrDefault(rg => rg.student_id == student.id);
                    if (researchGroup == null) return new { success = false, message = "ไม่พบข้อมูลงานวิจัย" };

                    var members = from rg in db.research_groups
                                  join s in db.students on rg.student_id equals s.id
                                  join u in db.users on s.user_id equals u.id
                                  join f in db.facultys on s.faculty_id equals f.faculty_id
                                  join b in db.branchs on s.branch_id equals b.branch_id
                                  where rg.research_id == researchGroup.research_id
                                  select new
                                  {
                                      id = rg.id,
                                      stdID = u.username,
                                      name = u.first_name + " " + u.last_name,
                                      faculty = f.faculty_name,
                                      branch = b.branch_name,
                                      user_id = u.id,
                                      is_leader = rg.is_leader
                                  };

                    return new { success = true, data = members.ToList() };
                }
            }
            catch (Exception ex)
            {
                return new { success = false, message = ex.Message };
            }
        }


        private void CheckAndRequireResearchCreation(int currentStudentId)
        {
            using (var db = new ResearchDBEntities())
            {
                // ตรวจสอบว่ามีงานวิจัยหรือไม่
                var hasResearch = db.research_groups
                    .Any(rg => rg.student_id == currentStudentId);

                if (!hasResearch)
                {
                    // บังคับให้สร้างงานวิจัย
                    ScriptManager.RegisterStartupScript(this, GetType(), "forceResearchCreation",
                        "$(document).ready(function() { " +
                        "   $('#modalForceResearch').modal('show'); " +
                        "   loadTeachers(); " + // โหลดรายชื่ออาจารย์
                        "});", true);
                }
            }
        }


        [WebMethod]
        public static object SearchTeachers(string searchText)
        {
            try
            {
                int currentUserId = Convert.ToInt32(HttpContext.Current.Session["UserID"]);

                using (var db = new ResearchDBEntities())
                {
                    // หา research_id ของผู้ใช้ปัจจุบัน
                    var student = db.students.FirstOrDefault(s => s.user_id == currentUserId);
                    var researchGroup = db.research_groups.FirstOrDefault(rg => rg.student_id == student.id);
                    int? currentAdvisorId = null;

                    // ถ้ามีงานวิจัย ให้ดึง advisor_id 
                    if (researchGroup != null)
                    {
                        var research = db.researches.Find(researchGroup.research_id);
                        if (research != null)
                        {
                            currentAdvisorId = research.advisor_id;
                        }
                    }

                    var query = from u in db.users
                                join t in db.teachers on u.id equals t.user_id
                                join b in db.branchs on t.branch_id equals b.branch_id
                                where u.user_type == "TEACHER"
                                && u.is_active
                                && u.id != currentAdvisorId  // กรองอาจารย์ที่ปรึกษาคนปัจจุบันออก
                                select new
                                {
                                    id = u.id,
                                    name = u.first_name + " " + u.last_name,
                                    department = b.branch_name,
                                    profileImg = u.profile_img
                                };

                    // กรองตามคำค้นหา
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        searchText = searchText.ToLower();
                        query = query.Where(t =>
                            t.name.ToLower().Contains(searchText) ||
                            t.department.ToLower().Contains(searchText)
                        );
                    }

                    var teachers = query
                        .OrderBy(t => t.name)
                        .Take(50)
                        .ToList()
                        .Select(t => new
                        {
                            t.id,
                            t.name,
                            t.department,
                            imageUrl = t.profileImg != null
                                ? $"data:image/jpeg;base64,{Convert.ToBase64String(t.profileImg)}"
                                : VirtualPathUtility.ToAbsolute("~/Images/NewUser.png")
                        })
                        .ToList();

                    return new
                    {
                        teachers = teachers,
                        totalCount = teachers.Count
                    };
                }
            }
            catch
            {
                return new
                {
                    teachers = new List<object>(),
                    totalCount = 0
                };
            }
        }

        protected bool IsCurrentUserLeader()
        {
            int currentUserId = Convert.ToInt32(Session["UserID"]);
            using (var db = new ResearchDBEntities())
            {
                var student = db.students.FirstOrDefault(s => s.user_id == currentUserId);
                var researchGroup = db.research_groups.FirstOrDefault(rg =>
                    rg.student_id == student.id);

                return researchGroup?.is_leader ?? false;
            }
        }

        [WebMethod]
        public static object SearchStudents(string searchText)
        {
            try
            {
                using (var db = new ResearchDBEntities())
                {
                    // ดึงรายการนักศึกษาที่มีงานวิจัยแล้ว
                    var studentsWithResearch = db.research_groups
                        .Select(rg => rg.student_id)
                        .Distinct()
                        .ToList();

                    // ดึงรายการนักศึกษาที่ยังไม่มีงานวิจัย
                    var query = from u in db.users
                                join s in db.students on u.id equals s.user_id
                                join b in db.branchs on s.branch_id equals b.branch_id
                                join f in db.facultys on s.faculty_id equals f.faculty_id
                                where u.user_type == "STUDENT"
                                && u.is_active
                                && !studentsWithResearch.Contains(s.id) // เพิ่มเงื่อนไขนี้
                                select new
                                {
                                    id = u.id,
                                    studentId = u.username,
                                    name = u.first_name + " " + u.last_name,
                                    department = b.branch_name,
                                    faculty = f.faculty_name,
                                    imageUrl = u.profile_img
                                };

                    // กรองตามคำค้นหา
                    if (!string.IsNullOrEmpty(searchText))
                    {
                        searchText = searchText.ToLower();
                        query = query.Where(s =>
                            s.studentId.ToLower().Contains(searchText) ||
                            s.name.ToLower().Contains(searchText) ||
                            s.department.ToLower().Contains(searchText) ||
                            s.faculty.ToLower().Contains(searchText)
                        );
                    }

                    var students = query
                        .OrderBy(s => s.name)
                        .Take(50)
                        .ToList()
                        .Select(s => new
                        {
                            s.id,
                            s.studentId,
                            s.name,
                            s.department,
                            s.faculty,
                            imageUrl = s.imageUrl != null
                                ? $"data:image/jpeg;base64,{Convert.ToBase64String(s.imageUrl)}"
                                : VirtualPathUtility.ToAbsolute("~/Images/NewUser.png")
                        });

                    return students;
                }
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine($"Error in SearchStudents: {ex.Message}");
                return new List<object>();
            }
        }

        [WebMethod]
        public static object LoadAdvisors()
        {
            using (var db = new ResearchDBEntities())
            {
                var query = from u in db.users
                            join t in db.teachers on u.id equals t.user_id
                            join b in db.branchs on t.branch_id equals b.branch_id
                            where u.user_type == "TEACHER" && u.is_active
                            select new
                            {
                                id = u.id,
                                name = u.first_name + " " + u.last_name,
                                department = b.branch_name,
                            };

                var teachers = query
                    .OrderBy(t => t.name)
                    .Take(50)
                    .ToList()
                    .Select(t => new
                    {
                        t.id,
                        t.name,
                        t.department
                    })
                    .ToList();

                return teachers;
            }
        }

        [WebMethod]
        public static object SelectTeacher(int teacherId)
        {
            try
            {
                int currentUserId = Convert.ToInt32(HttpContext.Current.Session["UserID"]);

                using (var db = new ResearchDBEntities())
                {
                    // หาข้อมูลอาจารย์
                    var query = from u in db.users
                                join t in db.teachers on u.id equals t.user_id
                                join b in db.branchs on t.branch_id equals b.branch_id
                                where u.id == teacherId && u.user_type == "TEACHER"
                                select new
                                {
                                    id = u.id,
                                    name = u.first_name + " " + u.last_name,
                                    department = b.branch_name,
                                    profileImg = u.profile_img
                                };

                    var teacher = query.FirstOrDefault();
                    if (teacher == null)
                    {
                        return new { success = false, message = "ไม่พบข้อมูลอาจารย์" };
                    }

                    // หางานวิจัยของนักศึกษาปัจจุบัน
                    var student = db.students.FirstOrDefault(s => s.user_id == currentUserId);
                    var researchGroup = db.research_groups.FirstOrDefault(rg => rg.student_id == student.id);

                    if (researchGroup != null)
                    {
                        // อัพเดทอาจารย์ที่ปรึกษาในงานวิจัย
                        var research = db.researches.Find(researchGroup.research_id);
                        if (research != null)
                        {
                            research.advisor_id = teacherId;
                            db.SaveChanges();
                        }
                    }

                    return new
                    {
                        success = true,
                        id = teacher.id,
                        name = teacher.name,
                        department = teacher.department,
                        imageUrl = teacher.profileImg != null
                            ? $"data:image/jpeg;base64,{Convert.ToBase64String(teacher.profileImg)}"
                            : VirtualPathUtility.ToAbsolute("~/Images/NewUser.png")
                    };
                }
            }
            catch (Exception ex)
            {
                return new { success = false, message = "เกิดข้อผิดพลาดในการเลือกอาจารย์: " + ex.Message };
            }
        }



        [WebMethod]
        public static object AddStudent(int studentId)
        {
            try
            {
                int currentUserId = Convert.ToInt32(HttpContext.Current.Session["UserID"]);

                using (var db = new ResearchDBEntities())
                {
                    // หา research_id ของผู้ใช้ปัจจุบัน
                    var currentStudent = db.students.FirstOrDefault(s => s.user_id == currentUserId);
                    var researchGroup = db.research_groups.FirstOrDefault(rg =>
                        rg.student_id == currentStudent.id);

                    if (researchGroup == null)
                    {
                        return new { success = false, message = "ไม่พบข้อมูลงานวิจัย กรุณาสร้างงานวิจัยก่อน" };
                    }

                    // ตรวจสอบว่านักศึกษาที่จะเพิ่มมีอยู่ในระบบไหม
                    var newStudent = db.students.FirstOrDefault(s => s.user_id == studentId);
                    if (newStudent == null)
                    {
                        return new { success = false, message = "ไม่พบข้อมูลนักศึกษา" };
                    }

                    // ตรวจสอบว่านักศึกษาคนนี้มีงานวิจัยอยู่แล้วหรือไม่
                    var existingMember = db.research_groups.Any(rg =>
                        rg.student_id == newStudent.id);
                    if (existingMember)
                    {
                        return new { success = false, message = "นักศึกษาคนนี้มีงานวิจัยอยู่แล้ว" };
                    }

                    // เพิ่มสมาชิกใหม่
                    var newMember = new research_groups
                    {
                        research_id = researchGroup.research_id,
                        student_id = newStudent.id,
                        created_date = DateTime.Now,
                        is_leader = false
                    };

                    db.research_groups.Add(newMember);
                    db.SaveChanges();

                    // ส่งข้อมูลกลับเพื่อใช้อัพเดท UI
                    return new
                    {
                        success = true,
                        researchId = researchGroup.research_id
                    };
                }
            }
            catch (Exception ex)
            {
                return new
                {
                    success = false,
                    message = "เกิดข้อผิดพลาดในการเพิ่มสมาชิก: " + ex.Message
                };
            }
        }

        [WebMethod]
        public static object ReloadResearchMembers()
        {
            try
            {
                int currentUserId = Convert.ToInt32(HttpContext.Current.Session["UserID"]);

                using (var db = new ResearchDBEntities())
                {
                    var student = db.students.FirstOrDefault(s => s.user_id == currentUserId);
                    var researchGroup = db.research_groups.FirstOrDefault(rg => rg.student_id == student.id);

                    if (researchGroup == null)
                    {
                        return new { success = false, message = "ไม่พบข้อมูลงานวิจัย" };
                    }

                    var members = from rg in db.research_groups
                                  join s in db.students on rg.student_id equals s.id
                                  join u in db.users on s.user_id equals u.id
                                  join f in db.facultys on s.faculty_id equals f.faculty_id
                                  join b in db.branchs on s.branch_id equals b.branch_id
                                  where rg.research_id == researchGroup.research_id
                                  select new
                                  {
                                      id = rg.id,
                                      stdID = u.username,
                                      name = u.first_name + " " + u.last_name,
                                      faculty = f.faculty_name,
                                      branch = b.branch_name,
                                      user_id = u.id,
                                      is_leader = rg.is_leader
                                  };

                    return new { success = true, data = members.ToList() };
                }
            }
            catch (Exception ex)
            {
                return new { success = false, message = ex.Message };
            }
        }

        protected void Dgv_std_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (!IsCurrentUserLeader())
            {
                ShowErrorMessage("คุณไม่มีสิทธิ์ดำเนินการนี้");
                return;
            }

            try
            {
                int groupId = Convert.ToInt32(e.CommandArgument);

                using (var db = new ResearchDBEntities())
                {
                    var member = db.research_groups.Find(groupId);

                    if (e.CommandName == "DeleteMember")
                    {
                        // เก็บ research_id ไว้ก่อนลบข้อมูล
                        var researchId = member.research_id;

                        // ลบสมาชิก
                        db.research_groups.Remove(member);
                        db.SaveChanges();

                        // โหลดข้อมูลสมาชิกใหม่ทันที
                        LoadResearchMembers(researchId);

                        // แจ้งเตือนสำเร็จ
                        ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                            "Swal.fire({" +
                            "   icon: 'success'," +
                            "   title: 'สำเร็จ'," +
                            "   text: 'ลบสมาชิกเรียบร้อยแล้ว'," +
                            "   showConfirmButton: false," +
                            "   timer: 1500" +
                            "});", true);
                    }
                    else if (e.CommandName == "TransferLeader")
                    {
                        // หา leader คนปัจจุบัน
                        var currentLeader = db.research_groups.First(rg =>
                            rg.research_id == member.research_id &&
                            rg.is_leader);

                        // โอนสิทธิ์
                        currentLeader.is_leader = false;
                        member.is_leader = true;
                        db.SaveChanges();

                        ShowSuccessMessage("โอนสิทธิ์หัวหน้ากลุ่มเรียบร้อยแล้ว");
                    }

                    // รีโหลดข้อมูล
                    //LoadResearchMembers(member.research_id);

                    Response.Redirect(Request.RawUrl);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"เกิดข้อผิดพลาด: {ex.Message}");
            }
        }

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int currentUserId = Convert.ToInt32(Session["UserID"]);

                using (var db = new ResearchDBEntities())
                {
                    // หา research_id ของผู้ใช้ปัจจุบัน
                    var student = db.students.FirstOrDefault(s => s.user_id == currentUserId);
                    var researchGroup = db.research_groups.FirstOrDefault(rg => rg.student_id == student.id);

                    if (researchGroup == null)
                    {
                        ShowErrorMessage("ไม่พบข้อมูลงานวิจัย");
                        return;
                    }

                    // อัพเดทข้อมูลงานวิจัย
                    var research = db.researches.Find(researchGroup.research_id);
                    if (research != null)
                    {
                        research.name = Tb_reserch.Text.Trim();
                        research.description = Tb_description.Text.Trim();
                        db.SaveChanges();

                        ShowSuccessMessage("บันทึกข้อมูลสำเร็จ");
                    }
                    else
                    {
                        ShowErrorMessage("ไม่พบข้อมูลงานวิจัย");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"เกิดข้อผิดพลาด: {ex.Message}");
            }
        }

        // Helper methods สำหรับแสดงข้อความ
        private void ShowSuccessMessage(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                $@"Swal.fire({{
            icon: 'success',
            title: 'สำเร็จ',
            text: '{message}',
            timer: 1500,
            showConfirmButton: false
        }}).then(() => {{
        }});", true);
        }

        private void ShowErrorMessage(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                $@"Swal.fire({{
            icon: 'error',
            title: 'ข้อผิดพลาด',
            text: '{message}'
        }});", true);
        }

        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            // Redirect or clear form
            Response.Redirect("~/Webpage/AddReserch.aspx");
        }

        public string GetStudentImage(object userId)
        {
            if (userId == null) return "~/Images/NewUser.png";

            try
            {
                using (var db = new ResearchDBEntities())
                {
                    var user = db.users.Find(Convert.ToInt32(userId));
                    return user?.profile_img != null
                        ? $"data:image/jpeg;base64,{Convert.ToBase64String(user.profile_img)}"
                        : "~/Images/NewUser.png";
                }
            }
            catch
            {
                return "~/Images/NewUser.png";
            }
        }



        //[WebMethod]
        //public static object CreateInitialResearch(string researchName, int advisorId)
        //{
        //    //try
        //    //{
        //    //    int currentUserId = Convert.ToInt32(HttpContext.Current.Session["UserID"]);

        //    //    using (var db = new ResearchDBEntities())
        //    //    {
        //    //        // ค้นหาข้อมูลนักศึกษา
        //    //        var student = db.students
        //    //            .FirstOrDefault(s => s.user_id == currentUserId);

        //    //        if (student == null)
        //    //        {
        //    //            return new { success = false, message = "ไม่พบข้อมูลนักศึกษา" };
        //    //        }

        //    //        // สร้างงานวิจัยใหม่
        //    //        var newResearch = new research
        //    //        {
        //    //            name = researchName,
        //    //            description = "งานวิจัยเริ่มแรก",
        //    //            advisor_id = advisorId,
        //    //            status = "IN_PROGRESS",
        //    //            is_approved = false,
        //    //            created_date = DateTime.Now
        //    //        };

        //    //        db.researches.Add(newResearch);
        //    //        db.SaveChanges();

        //    //        // เพิ่มนักศึกษาเป็นหัวหน้ากลุ่ม
        //    //        var researchGroup = new research_groups
        //    //        {
        //    //            research_id = newResearch.id,
        //    //            student_id = student.id,
        //    //            created_date = DateTime.Now,
        //    //            is_leader = true
        //    //        };

        //    //        db.research_groups.Add(researchGroup);
        //    //        db.SaveChanges();

        //    //        return new { success = true };
        //    //    }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    return new { success = false, message = "เกิดข้อผิดพลาดในการสร้างงานวิจัย" };
        //    //}
        //}
    }

    [Serializable]
    public class InitialResearchData
    {
        public string Name { get; set; }
        public int StudentId { get; set; }
        public int FacultyId { get; set; }
        public int BranchId { get; set; }
        public bool IsLeader { get; set; }
    }
}
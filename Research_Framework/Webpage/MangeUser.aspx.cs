using Research_Framework.ApplicationDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Data.Entity.Migrations;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Services;

namespace Research_Framework.Webpage
{
    public partial class MangeUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                if (Session["UserID"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                LoadUsers();
                LoadFaculties();
            }
        }

        private void LoadUsers(string searchText = "")
        {
            using (var db = new ResearchDBEntities())
            {
                var query = from u in db.users
                            join s in db.students on u.id equals s.user_id into studentGroup
                            from student in studentGroup.DefaultIfEmpty()
                            join t in db.teachers on u.id equals t.user_id into teacherGroup
                            from teacher in teacherGroup.DefaultIfEmpty()
                            join f in db.facultys
                                on (student != null ? student.faculty_id :
                                    teacher != null ? teacher.faculty_id : 0) equals f.faculty_id into facultyGroup
                            from faculty in facultyGroup.DefaultIfEmpty()
                            join b in db.branchs
                                on (student != null ? student.branch_id :
                                    teacher != null ? teacher.branch_id : 0) equals b.branch_id into branchGroup
                            from branch in branchGroup.DefaultIfEmpty()
                            select new
                            {
                                u.id,
                                u.first_name,
                                u.last_name,
                                u.username,
                                faculty_name = faculty.faculty_name ?? "",
                                branch_name = branch.branch_name ?? "",
                                u.user_type,
                                u.is_active
                            };

                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.ToLower();
                    query = query.Where(u =>
                        u.first_name.ToLower().Contains(searchText) ||
                        u.last_name.ToLower().Contains(searchText) ||
                        u.username.ToLower().Contains(searchText)
                    );
                }

                GvUser.DataSource = query.ToList();
                GvUser.DataBind();
            }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = false)]
        public static bool ToggleUserStatus(int userId, bool isActive)
        {
            try
            {
                using (var db = new ResearchDBEntities())
                {
                    var user = db.users.Find(userId);
                    if (user != null && user.user_type != "ADMIN")
                    {
                        user.is_active = isActive;
                        db.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private void LoadFaculties()
        {
            using (var db = new ResearchDBEntities())
            {
                var faculties = db.facultys.OrderBy(f => f.faculty_name).ToList();
                DdlEditFaculty.DataSource = faculties;
                DdlEditFaculty.DataTextField = "faculty_name";
                DdlEditFaculty.DataValueField = "faculty_id";
                DdlEditFaculty.DataBind();
            }
                

            LoadBranches();
        }

        private void LoadBranches()
        {
            using (var db = new ResearchDBEntities())
            {
                int facultyId;
                if (int.TryParse(DdlEditFaculty.SelectedValue, out facultyId))
                {
                    var branches = db.branchs
                        .Where(b => b.faculty_id == facultyId)
                        .OrderBy(b => b.branch_name)
                        .ToList();

                    DdlEditBranch.DataSource = branches;
                    DdlEditBranch.DataTextField = "branch_name";
                    DdlEditBranch.DataValueField = "branch_id";
                    DdlEditBranch.DataBind();
                }
            }
        }

        protected void DdlEditFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadBranches();
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadUsers(TbSearch.Text.Trim());
        }

        protected void GvUser_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GvUser.PageIndex = e.NewPageIndex;
            LoadUsers(TbSearch.Text.Trim());
        }

        protected void GvUser_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditUser" || e.CommandName == "DeleteUser")
            {
                int userId = Convert.ToInt32(e.CommandArgument);

                using (var db = new ResearchDBEntities())
                {
                    var user = db.users.Find(userId);
                    if (user != null && user.user_type == "ADMIN")
                    {
                        ShowMessage("ไม่สามารถแก้ไขหรือลบผู้ดูแลระบบได้", true);
                        return;
                    }
                }

                if (e.CommandName == "EditUser")
                {
                    LoadUserForEdit(userId);
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showEditModal();", true);
                }
                else if (e.CommandName == "DeleteUser")
                {
                    DeleteUser(userId);
                }
            }
        }

        private void LoadUserForEdit(int userId)
        {
            using (var db = new ResearchDBEntities())
            {
                var user = db.users.Find(userId);
                if (user != null)
                {
                    HfUserId.Value = user.id.ToString();
                    TbEditName.Text = user.first_name;
                    TbEditLname.Text = user.last_name;
                    TbEditUsername.Text = user.username;

                    // หาข้อมูลคณะและสาขา
                    if (user.user_type == "STUDENT")
                    {
                        var student = db.students.FirstOrDefault(s => s.user_id == userId);
                        if (student != null)
                        {
                            DdlEditFaculty.SelectedValue = student.faculty_id.ToString();
                            LoadBranches(); // โหลดสาขาตามคณะที่เลือก
                            DdlEditBranch.SelectedValue = student.branch_id.ToString();
                        }
                    }
                    else if (user.user_type == "TEACHER")
                    {
                        var teacher = db.teachers.FirstOrDefault(t => t.user_id == userId);
                        if (teacher != null)
                        {
                            DdlEditFaculty.SelectedValue = teacher.faculty_id.ToString();
                            LoadBranches();
                            DdlEditBranch.SelectedValue = teacher.branch_id.ToString();
                        }
                    }
                }
            }
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = false)]
        public static bool DeleteUserMethod(int userId)
        {
            try
            {
                using (var db = new ResearchDBEntities())
                {
                    var user = db.users.Find(userId);
                    if (user != null && user.user_type != "ADMIN")
                    {
                        if (user.user_type == "STUDENT")
                        {
                            var student = db.students.FirstOrDefault(s => s.user_id == userId);
                            if (student != null)
                            {
                                db.students.Remove(student);
                            }
                        }
                        else if (user.user_type == "TEACHER")
                        {
                            var teacher = db.teachers.FirstOrDefault(t => t.user_id == userId);
                            if (teacher != null)
                            {
                                db.teachers.Remove(teacher);
                            }
                        }

                        db.users.Remove(user);
                        db.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private void DeleteUser(int userId)
        {
            try
            {
                using (var db = new ResearchDBEntities())
                {
                    var user = db.users.Find(userId);
                    if (user != null && user.user_type != "ADMIN")
                    {
                        // ลบข้อมูลที่เกี่ยวข้องก่อน
                        if (user.user_type == "STUDENT")
                        {
                            var student = db.students.FirstOrDefault(s => s.user_id == userId);
                            if (student != null)
                            {
                                db.students.Remove(student);
                            }
                        }
                        else if (user.user_type == "TEACHER")
                        {
                            var teacher = db.teachers.FirstOrDefault(t => t.user_id == userId);
                            if (teacher != null)
                            {
                                db.teachers.Remove(teacher);
                            }
                        }

                        // ลบ user
                        db.users.Remove(user);
                        db.SaveChanges();

                        // แสดง SweetAlert แจ้งสำเร็จ
                        ScriptManager.RegisterStartupScript(this, GetType(), "success", @"
                    Swal.fire({
                        icon: 'success',
                        title: 'ลบข้อมูลสำเร็จ',
                        text: 'ลบข้อมูลผู้ใช้เรียบร้อยแล้ว',
                        showConfirmButton: false,
                        timer: 1500
                    });", true);

                        // โหลดข้อมูลใหม่
                        LoadUsers(TbSearch.Text.Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                // แสดง SweetAlert แจ้งเตือนเมื่อเกิดข้อผิดพลาด
                ScriptManager.RegisterStartupScript(this, GetType(), "error", $@"
            Swal.fire({{
                icon: 'error',
                title: 'เกิดข้อผิดพลาด',
                text: 'ไม่สามารถลบข้อมูลได้: {ex.Message}',
                confirmButtonText: 'ตกลง'
            }});", true);
            }
        }

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            using (var db = new ResearchDBEntities())
            {
                try
                {
                    int userId = int.Parse(HfUserId.Value);
                    var user = db.users.Find(userId);
                    if (user != null)
                    {
                        user.first_name = TbEditName.Text.Trim();
                        user.last_name = TbEditLname.Text.Trim();
                        user.username = TbEditUsername.Text.Trim();

                        if (!string.IsNullOrEmpty(TbEditPassword.Text))
                        {
                            user.password = TbEditPassword.Text;
                        }

                        // อัพเดทข้อมูลคณะและสาขา
                        if (user.user_type == "STUDENT")
                        {
                            var student = db.students.First(s => s.user_id == userId);
                            student.faculty_id = int.Parse(DdlEditFaculty.SelectedValue);
                            student.branch_id = int.Parse(DdlEditBranch.SelectedValue);
                        }
                        else if (user.user_type == "TEACHER")
                        {
                            var teacher = db.teachers.First(t => t.user_id == userId);
                            teacher.faculty_id = int.Parse(DdlEditFaculty.SelectedValue);
                            teacher.branch_id = int.Parse(DdlEditBranch.SelectedValue);
                        }

                        db.SaveChanges();
                        LoadUsers(TbSearch.Text.Trim());
                        ShowMessage("บันทึกการเปลี่ยนแปลงสำเร็จ");
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage($"เกิดข้อผิดพลาดในการบันทึกข้อมูล: {ex.Message}", true);
                }
            }
        }

        private void ShowMessage(string message, bool isError = false)
        {
            string script = $@"Swal.fire({{
                title: '{message}',
                icon: '{(isError ? "error" : "success")}',
                confirmButtonText: 'ตกลง'
            }});";

            ScriptManager.RegisterStartupScript(this, GetType(),
                "ShowMessage", script, true);
        }
    }
}

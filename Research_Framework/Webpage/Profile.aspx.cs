using Research_Framework.ApplicationDB;
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;

namespace Research_Framework.Webpage
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] == null)
                {
                    Response.Redirect("~/Webpage/Login.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                LoadUserProfile();
            }
        }

        private void LoadUserProfile()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserID"]);
                using (var db = new ResearchDBEntities())
                {
                    var user = db.users.Find(userId);
                    if (user != null)
                    {
                        Tb_name.Value = user.first_name;
                        Tb_lname.Value = user.last_name;
                        Tb_username.Text = user.username;

                        // แสดงรูปโปรไฟล์
                        if (user.profile_img != null)
                        {
                            string base64Image = Convert.ToBase64String(user.profile_img);
                            ProfileImage.ImageUrl = $"data:image/jpeg;base64,{base64Image}";
                        }
                        else
                        {
                            // ใส่รูป default ถ้าไม่มีรูปโปรไฟล์
                            ProfileImage.ImageUrl = "~/Images/NewUser.png";
                        }

                        if (user.user_type == "STUDENT")
                        {
                            var student = db.students.FirstOrDefault(s => s.user_id == userId);
                            if (student != null)
                            {
                                var branch = db.branchs.Find(student.branch_id);
                                if (branch != null)
                                {
                                    Tb_branch.Text = branch.branch_name;
                                    var faculty = db.facultys.Find(branch.faculty_id);
                                    if (faculty != null)
                                    {
                                        Tb_faculty.Text = faculty.faculty_name;
                                    }
                                }
                            }
                        }
                        if (user.user_type == "TEACHER")
                        {
                            var student = db.teachers.FirstOrDefault(s => s.user_id == userId);
                            if (student != null)
                            {
                                var branch = db.branchs.Find(student.branch_id);
                                if (branch != null)
                                {
                                    Tb_branch.Text = branch.branch_name;
                                    var faculty = db.facultys.Find(branch.faculty_id);
                                    if (faculty != null)
                                    {
                                        Tb_faculty.Text = faculty.faculty_name;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                    $"Swal.fire('Error!', 'เกิดข้อผิดพลาดในการโหลดข้อมูล: {ex.Message}', 'error');", true);
            }
        }

        [WebMethod]
        public static bool SaveProfile(ProfileData profileData)
        {
            try
            {
                int userId = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                using (var db = new ResearchDBEntities())
                {
                    var user = db.users.Find(userId);
                    if (user != null)
                    {
                        user.first_name = profileData.Name;
                        user.last_name = profileData.LastName;

                        if (!string.IsNullOrEmpty(profileData.Image))
                        {
                            user.profile_img = Convert.FromBase64String(profileData.Image);
                        }

                        db.SaveChanges();
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        [WebMethod]
        public static bool ChangePassword(string currentPassword, string newPassword)
        {
            try
            {
                int userId = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                using (var db = new ResearchDBEntities())
                {
                    var user = db.users.Find(userId);
                    if (user != null && user.password == currentPassword)
                    {
                        user.password = newPassword;
                        db.SaveChanges();
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public class ProfileData
        {
            public string Name { get; set; }
            public string LastName { get; set; }
            public string Image { get; set; }
        }
    }
}
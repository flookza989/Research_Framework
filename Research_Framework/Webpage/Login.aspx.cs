using Research_Framework.ApplicationDB;
using System;
using System.Linq;
using System.Web.UI;

namespace Research_Framework.Webpage
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // ถ้ามี Session อยู่แล้วให้ redirect ไปหน้า Profile
            if (Session["UserID"] != null)
            {
                Response.Redirect("~/Webpage/Profile.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
        }

        protected void Btn_login_Click(object sender, EventArgs e)
        {
            try
            {
                string username = Tb_user.Text.Trim();
                string password = Tb_pass.Text.Trim();

                // ตรวจสอบว่ากรอกข้อมูลครบหรือไม่
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                        "Swal.fire('แจ้งเตือน!', 'กรุณากรอกข้อมูลให้ครบถ้วน', 'warning');", true);
                    return;
                }

                using (ResearchDBEntities db = new ResearchDBEntities())
                {
                    // ค้นหาผู้ใช้ในฐานข้อมูล
                    var user = db.users
                        .FirstOrDefault(u => u.username == username && 
                                           u.password == password && 
                                           u.is_active == true);

                    if (user != null)
                    {
                        // อัพเดท last_login
                        user.last_login = DateTime.Now;
                        db.SaveChanges();

                        // บันทึกข้อมูลลงใน Session
                        Session["UserID"] = user.id;
                        Session["Username"] = user.username;
                        Session["UserType"] = user.user_type;
                        Session["FullName"] = user.first_name + " " + user.last_name;

                        if (user.user_type == "STUDENT")
                        {
                            // ดึงข้อมูลนักศึกษา
                            var student = db.students.FirstOrDefault(s => s.user_id == user.id);
                            if (student != null)
                            {
                                Session["StudentID"] = student.id;
                            }
                            
                            // ถ้าเป็นนักศึกษาให้ไปหน้า AddResearch
                            Response.Redirect("~/Webpage/AddResearch.aspx", false);
                            Context.ApplicationInstance.CompleteRequest();
                            return;
                        }
                        else if (user.user_type == "TEACHER")
                        {
                            // ดึงข้อมูลอาจารย์
                            var teacher = db.teachers.FirstOrDefault(t => t.user_id == user.id);
                            if (teacher != null)
                            {
                                Session["TeacherID"] = teacher.id;
                            }
                            
                            // ถ้าเป็นอาจารย์ให้ไปหน้า ResearchList
                            Response.Redirect("~/Webpage/ResearchList.aspx", false);
                            Context.ApplicationInstance.CompleteRequest();
                            return;
                        }
                        else
                        {
                            // ถ้าเป็น Admin ให้ไปหน้า Profile ตามเดิม
                            Response.Redirect("~/Webpage/Profile.aspx", false);
                            Context.ApplicationInstance.CompleteRequest();
                            return;
                        }
                    }
                    else
                    {
                        // แจ้งเตือนเมื่อ login ไม่สำเร็จ
                        ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                            "Swal.fire('แจ้งเตือน!', 'ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง หรือบัญชีถูกระงับ', 'error');", true);
                        Tb_pass.Text = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                // จัดการกรณีเกิด error
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                    $"Swal.fire('Error!', 'เกิดข้อผิดพลาด: {ex.Message}', 'error');", true);
            }
        }
    }
}
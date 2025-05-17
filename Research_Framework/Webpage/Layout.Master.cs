using Research_Framework.ApplicationDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Research_Framework.Webpage
{
    public partial class Layout : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string currentPage = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
                bool isLoginPage = currentPage.Equals("Login.aspx", StringComparison.OrdinalIgnoreCase);

                navAddReserch.Visible = !isLoginPage;
                navApprove.Visible = !isLoginPage;
                navAddNewUser.Visible = !isLoginPage;
                navMangeUser.Visible = !isLoginPage;
                BtnLogout.Visible = !isLoginPage;
                navProfile.Visible = !isLoginPage;

                mainContent.Attributes["class"] = isLoginPage ? "content-full" : "content";

                if (!isLoginPage && Session["UserID"] == null)
                {
                    Response.Redirect("~/Webpage/Login.aspx");
                }

                if (Session["UserID"] != null)
                {
                    LoadUserInfo();
                }
            }
        }

        private void LoadUserInfo()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserID"]);
                using (var db = new ResearchDBEntities())
                {
                    var user = db.users.Find(userId);
                    if (user != null)
                    {
                        var userType = user.user_type;
                        navAddReserch.Visible = userType == "STUDENT";
                        navApprove.Visible = userType != "ADMIN";
                        navAddNewUser.Visible = userType == "ADMIN";
                        navMangeUser.Visible = userType == "ADMIN";

                        // แสดงชื่อผู้ใช้
                        LbUsername.Text = $"{user.first_name} {user.last_name}";

                        // แสดงรูปโปรไฟล์
                        if (user.profile_img != null)
                        {
                            string base64Image = Convert.ToBase64String(user.profile_img);
                            NavProfileImage.ImageUrl = $"data:image/jpeg;base64,{base64Image}";
                        }
                        else
                        {
                            NavProfileImage.ImageUrl = "~/Images/NewUser.png";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // จัดการข้อผิดพลาด
                Console.WriteLine($"Error loading user info: {ex.Message}");
            }
        }

        protected void BtnLogout_Click(object sender, EventArgs e)
        {
            // ล้าง Session ทั้งหมด
            Session.Clear();
            Session.Abandon();

            // ลบ Cookie ที่เกี่ยวข้อง (ถ้ามี)
            if (Request.Cookies["UserSettings"] != null)
            {
                Response.Cookies["UserSettings"].Expires = DateTime.Now.AddDays(-1);
            }

            // แสดง SweetAlert2 แล้วค่อย Redirect
            string script = @"
        Swal.fire({
            title: 'ออกจากระบบสำเร็จ',
            text: 'กำลังนำท่านไปยังหน้าเข้าสู่ระบบ',
            icon: 'success',
            timer: 2000,
            showConfirmButton: false
        }).then(function() {
            window.location = 'Login.aspx';
        });";

            ScriptManager.RegisterStartupScript(this, GetType(),
                "logout", script, true);
        }
    }
}
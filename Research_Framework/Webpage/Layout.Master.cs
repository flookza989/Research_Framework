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

                // กำหนด CSS class ให้กับ content container
                if (isLoginPage)
                {
                    // หน้า Login
                    mainContent.Attributes["class"] = "content-login";
                }
                else if (Session["UserID"] == null)
                {
                    // ยังไม่ได้ login แต่ไม่ใช่หน้า Login
                    mainContent.Attributes["class"] = "content-no-sidebar";
                }
                else
                {
                    // login แล้ว มี sidebar
                    mainContent.Attributes["class"] = "content";
                }
                
                // ซ่อน sidebar และ navbar เมื่อยังไม่ได้ login หรือเป็นหน้า Login
                sidebarContainer.Visible = !isLoginPage && Session["UserID"] != null;
                navbarContainer.Visible = !isLoginPage && Session["UserID"] != null;

                if (!isLoginPage && Session["UserID"] == null)
                {
                    Response.Redirect("~/Webpage/Login.aspx");
                }

                if (Session["UserID"] != null)
                {
                    LoadUserInfo();
                    
                    // ไฮไลท์เมนูปัจจุบัน
                    HighlightCurrentMenu();
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

        private void HighlightCurrentMenu()
        {
            try
            {
                // หาชื่อไฟล์ของหน้าปัจจุบัน
                string currentPage = System.IO.Path.GetFileName(Request.Url.AbsolutePath).ToLower();

                // ลบ class active จากเมนูทั้งหมดก่อน
                navAddReserchLink.Attributes["class"] = "nav-link sidebarItem";
                navApproveLink.Attributes["class"] = "nav-link sidebarItem";
                navAddNewUserLink.Attributes["class"] = "nav-link sidebarItem";
                navMangeUserLink.Attributes["class"] = "nav-link sidebarItem";

                // กำหนด class active ให้กับเมนูที่ตรงกับหน้าปัจจุบัน
                if (currentPage == "addreserch.aspx" && navAddReserch.Visible)
                {
                    navAddReserchLink.Attributes["class"] = "nav-link sidebarItem active";
                }
                else if ((currentPage == "researchprocess.aspx" || currentPage == "approve.aspx") && navApprove.Visible)
                {
                    navApproveLink.Attributes["class"] = "nav-link sidebarItem active";
                }
                else if (currentPage == "addnewuser.aspx" && navAddNewUser.Visible)
                {
                    navAddNewUserLink.Attributes["class"] = "nav-link sidebarItem active";
                }
                else if (currentPage == "mangeuser.aspx" && navMangeUser.Visible)
                {
                    navMangeUserLink.Attributes["class"] = "nav-link sidebarItem active";
                }

                // เพิ่มสคริปต์ JavaScript เพื่อเรียกใช้ฟังก์ชัน highlightCurrentMenu จาก myScript.js
                ScriptManager.RegisterStartupScript(this, GetType(), "highlightMenu", "highlightCurrentMenu();", true);
            }
            catch (Exception ex)
            {
                // จัดการข้อผิดพลาด
                Console.WriteLine($"Error highlighting current menu: {ex.Message}");
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
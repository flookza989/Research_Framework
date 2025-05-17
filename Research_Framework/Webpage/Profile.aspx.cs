using Research_Framework.ApplicationDB;
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Research_Framework.Webpage
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userID"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                user getUser = (user)Session["userID"];

                using (var _db = new ResearchDBEntities())
                {
                    user newUser = _db.users.FirstOrDefault(c => c.user_id == getUser.user_id);

                    if (getUser == null)
                        return;

                    Tb_name.Value = newUser.name;
                    Tb_lname.Value = newUser.lname;
                    Tb_username.Text = newUser.username;
                    Tb_password.Value = newUser.password;
                    Tb_faculty.Text = newUser.branch.faculty.faculty_name;
                    Tb_branch.Text = newUser.branch.branch_name;

                    // ตรวจสอบว่ารูปภาพในฐานข้อมูลเป็นค่าว่างหรือไม่
                    if (newUser.img != null)
                    {
                        // แสดงรูปภาพจากฐานข้อมูล
                        ProfileImage.ImageUrl = "data:image;base64," + Convert.ToBase64String(newUser.img);
                    }
                    else
                    {
                        // แสดงรูปภาพเริ่มต้น
                        ProfileImage.ImageUrl = "~/Images/NewUser.png";
                    }
                }

            }
        }

        [WebMethod]
        public static bool SaveProfile(ProfileData profileData)
        {
            bool status = false;
            try
            {
                string name = profileData.Name;
                string lastName = profileData.LastName;
                string password = profileData.Password;
                string base64String = profileData.Image;

                user getUser = (user)HttpContext.Current.Session["userID"];
                using (var _db = new ResearchDBEntities())
                {
                    user userUpdate = _db.users.FirstOrDefault(c => c.user_id == getUser.user_id);
                    if (userUpdate != null)
                    {
                        userUpdate.name = name;
                        userUpdate.lname = lastName;
                        userUpdate.password = password;

                        if (!string.IsNullOrEmpty(base64String))
                        {
                            try
                            {
                                // ตัด data URI scheme ออก (เช่น "data:image/jpeg;base64,")
                                string base64Data = base64String;
                                if (base64String.Contains(","))
                                {
                                    base64Data = base64String.Split(',')[1];
                                }

                                // แปลง base64 เป็น byte array
                                byte[] imageBytes = Convert.FromBase64String(base64Data);
                                userUpdate.img = imageBytes;
                            }
                            catch (Exception ex)
                            {
                                // Log error
                                System.Diagnostics.Debug.WriteLine($"Error processing image: {ex.Message}");
                                return false;
                            }
                        }

                        //_db.users.AddOrUpdate(userUpdate);


                        int count = _db.SaveChanges();
                        status = true;
                        HttpContext.Current.Session["userID"] = userUpdate;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine($"Error saving profile: {ex.Message}");
                status = false;
            }
            return status;
        }
    }

    public class ProfileData
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
    }
}
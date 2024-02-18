using Research_Framework.ApplicationDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Research_Framework.Webpage
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.RemoveAll();
        }

        protected void Btn_login_Click(object sender, EventArgs e)
        {
            using(var _db = new ResearchDBEntities())
            {
                string user = Tb_user.Text.Trim();
                string pass = Tb_pass.Text.Trim();

                View_user getUser = _db.View_user.FirstOrDefault(c => c.username == user && c.password == pass);

                if(getUser == null)
                {
                    // User not found, show sweet alert
                    string script = "swal('พบข้อผิดพลาด', 'รหัสนักศึกษา หรือ รหัสผ่านไม่ถูกต้อง', 'error');";
                    ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert", script, true);
                    return;
                }

                Application["fullName"] = $"{getUser.name} {getUser.lname}";
                Application["userID"] = getUser;

                Response.Redirect("Profile.aspx");
            }
        }
    }
}
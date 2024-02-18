using Research_Framework.ApplicationDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Research_Framework.Webpage
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Application["userID"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                    View_user getUser = (View_user)Application["userID"];

                    if (getUser == null)
                        return;

                    Tb_name.Text = getUser.name;
                    Tb_lname.Text = getUser.lname;
                    Tb_username.Text = getUser.username;
                    Tb_password.Text = getUser.password;
                    Tb_faculty.Text = getUser.faculty_name;
                    Tb_branch.Text = getUser.branch_name;
            }
        }

        protected void UploadImageButton_Click(object sender, EventArgs e)
        {

        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {

        }
    }
}
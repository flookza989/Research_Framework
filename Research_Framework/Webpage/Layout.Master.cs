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
                string currentUrl = Request.Url.AbsolutePath;

                switch (currentUrl)
                {
                    case "/Webpage/AddReserch.aspx":
                        break;
                    case "/Webpage/Approve.aspx":
                        break;
                }

                if (Application["userID"] == null)
                    return;

                View_user getUser = (View_user)Application["userID"];
                Lb_name.Text = $"{getUser.name} {getUser.lname}";

                if(getUser.permission != "student")
                {
                    navAdd.Visible = false;
                }
            }

            
        }
    }
}
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

                if (Session["userID"] == null)
                    return;

                user getUser = (user)Session["userID"];
                Lb_name.Text = $"{getUser.name} {getUser.lname}";

                // ตรวจสอบว่ารูปภาพในฐานข้อมูลเป็นค่าว่างหรือไม่
                if (getUser.img != null)
                {
                    // แสดงรูปภาพจากฐานข้อมูล
                    imgContent.ImageUrl = "data:image;base64," + Convert.ToBase64String(getUser.img);
                }
                else
                {
                    // แสดงรูปภาพเริ่มต้น
                    imgContent.ImageUrl = "~/Images/NewUser.png";
                }

                if (getUser.permission == "student")
                {
                    navApprove.Visible = true;
                    navAddReserch.Visible = true;
                    navAddNewUser.Visible = false;
                    navMangeUser.Visible = false;
                    navMangeUserBy1.Visible = false;
                }
                else if(getUser.permission == "teacher")
                {
                    navApprove.Visible = true;
                    navAddReserch.Visible = false;
                    navAddNewUser.Visible = false;
                    navMangeUser.Visible = false;
                    navMangeUserBy1.Visible = false;
                }
                else if (getUser.permission == "admin")
                {
                    navApprove.Visible = false;
                    navAddReserch.Visible = false;
                    navAddNewUser.Visible = true;
                    navMangeUser.Visible = true;
                    navMangeUserBy1.Visible = true;
                }
            }

            
        }
    }
}
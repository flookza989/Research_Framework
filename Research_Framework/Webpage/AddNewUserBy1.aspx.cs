using Newtonsoft.Json;
using Research_Framework.ApplicationDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Research_Framework.Webpage
{
    public partial class AddNewUserBy1 : System.Web.UI.Page
    {
        private static ResearchDBEntities _db;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userID"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }
            }
        }

        private static void ConnectDB()
        {
            if(_db == null)
            {
                _db = new ResearchDBEntities();
            }
        }

        [WebMethod]
        public static string GetFaculties()
        {
            ConnectDB();
            var faculties = _db.faculties.Select(c => new
                                            {
                                                id = c.faculty_id,
                                                name = c.faculty_name,
                                            })
                                         .ToList();

            string result = JsonConvert.SerializeObject(faculties);

            return result;
        }

        [WebMethod]
        public static string GetBranches(int facultyId)
        {
            ConnectDB();
            var branches = _db.branches
                              .Where(b => b.faculty_id == facultyId)
                              .Select(b => new
                              {
                                  id = b.branch_id,
                                  name = b.branch_name,
                              })
                              .ToList();
            string result = JsonConvert.SerializeObject(branches);
            return result;
        }

        [WebMethod]
        public static string GetAllBranches()
        {
            ConnectDB();
            var branches = _db.branches
                              .Select(b => new
                              {
                                  id = b.branch_id,
                                  name = b.branch_name,
                              })
                              .ToList();
            string result = JsonConvert.SerializeObject(branches);
            return result;
        }

        [WebMethod]
        public static string SaveUser(user userData)
        {
            try
            {
                ConnectDB();
                var newUser = new user
                {
                    username = userData.username,
                    password = userData.password,
                    name = userData.name,
                    lname = userData.lname,
                    branch_id = userData.branch_id,
                    permission = userData.permission
                };
                _db.users.Add(newUser);
                _db.SaveChanges();
                return "success";
            }
            catch (Exception ex)
            {
                // ในการใช้งานจริง ควรบันทึก log ของ exception
                return "error: " + ex.Message;
            }
        }


    }
}
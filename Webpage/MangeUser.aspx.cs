using Research_Framework.ApplicationDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Data.Entity.Migrations;

namespace Research_Framework.Webpage
{
    public partial class MangeUser : System.Web.UI.Page
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

                using (var _db = new ResearchDBEntities())
                {
                    List<user> userList = _db.users.ToList();
                    Gv_User.DataSource = userList.Select(c => new
                    {
                        c.user_id,
                        c.username,
                        c.password,
                        c.name,
                        c.lname,
                        c.permission,
                        c.branch.branch_name,
                        c.branch.faculty.faculty_name,
                    });
                    Gv_User.DataBind();
                }

                PopulateFacultyDropDownList();
            }
        }

        protected void Gv_User_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            e.Row.Cells[0].Visible = false;
        }

        protected void btn_save_Click(object sender, EventArgs e)
        {
            // Get the user id from the hidden field
            int userId = Convert.ToInt32(UserIdHiddenField.Value);

            string name = EditNameTextBox.Text.Trim();
            string lname = EditLastNameTextBox.Text.Trim();
            string username = EditUsernameTextBox.Text.Trim();
            string password = EditPasswordTextBox.Text.Trim();
            string branch = HiddenEditBranchDropDownList.Value.Trim();
            string faculty = EditFacultyDropDownList.Value.Trim();
            string permission = EditPermissionDropDownList.Value.Trim();

            //// Retrieve the user from the database
            using (var _db = new ResearchDBEntities()) // YourDbContext คือ DbContext ของแอพพลิเคชันของคุณ
            {
                var user = _db.users.FirstOrDefault(u => u.user_id == userId); // ต้องแก้ไขตามโครงสร้างของตาราง User ของคุณ

                if (user != null)
                {
                    // Update user properties with values from the edit form
                    user.name = name;
                    user.lname = lname;
                    user.username = username;
                    user.password = password;
                    user.branch_id = _db.branches.FirstOrDefault(c => c.branch_name == branch &&
                                                                      c.faculty.faculty_name == faculty).branch_id;
                    user.permission = permission;

                    _db.users.AddOrUpdate(user);
                    // Save changes to the database
                    _db.SaveChanges();
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "GetUpdatedUserData", "GetUpdatedUserData();", true);
            }
        }

        private void PopulateFacultyDropDownList()
        {
            List<string> faculties = GetFacultiesFromDataSource();

            EditFacultyDropDownList.DataSource = faculties;
            EditFacultyDropDownList.DataBind();
        }

        private List<string> GetFacultiesFromDataSource()
        {
            using (var _db = new ResearchDBEntities())
            {
                var facultiesList = _db.faculties.Select(c => c.faculty_name).ToList();
                return facultiesList;
            }
        }

        [WebMethod]
        public static void RemoveUser(string userId)
        {
            using (var _db = new ResearchDBEntities())
            {
                int id = Convert.ToInt32(userId);
                user user = _db.users.FirstOrDefault(c => c.user_id == id);
                _db.users.Remove(user); 
                _db.SaveChanges();
            }
        }

        [WebMethod]
        public static string GetUpdatedUserData()
        {
            using (var _db = new ResearchDBEntities())
            {
                List<user> userList = _db.users.ToList();
                var userData = userList.Select(c => new
                {
                    c.user_id,
                    c.username,
                    c.password,
                    c.name,
                    c.lname,
                    c.permission,
                    c.branch.branch_name,
                    c.branch.faculty.faculty_name,
                }).ToList();

                return JsonConvert.SerializeObject(userData);
            }
        }

        [WebMethod]
        public static string SearchUsers(string searchText)
        {
            var searchResults = SearchUsersFromDatabase(searchText);
            return JsonConvert.SerializeObject(searchResults);
        }

        private static object SearchUsersFromDatabase(string searchText)
        {
            using (var _db = new ResearchDBEntities())
            {
                var userData = _db.users.Where(c => c.username.Contains(searchText) ||
                                                    c.name.Contains(searchText) ||
                                                    c.lname.Contains(searchText) ||
                                                    c.branch.branch_name.Contains(searchText) ||
                                                    c.branch.faculty.faculty_name.Contains(searchText) ||
                                                    c.permission.Contains(searchText) ||
                                                    c.password.Contains(searchText))
                                        .Select(c => new
                                            {
                                                c.user_id,
                                                c.username,
                                                c.password,
                                                c.name,
                                                c.lname,
                                                c.permission,
                                                c.branch.branch_name,
                                                c.branch.faculty.faculty_name,
                                            })
                                        .ToList();

                return userData;
            }
        }

        [WebMethod]
        public static string GetUserDetails(string userId)
        {
            using (var _db = new ResearchDBEntities())
            {
                int id = Convert.ToInt32(userId);

                var users = _db.users.Select(c => new
                                             {
                                                 c.user_id,
                                                 c.name,
                                                 c.lname,
                                                 c.username,
                                                 c.password,
                                                 c.branch.branch_name,
                                                 c.branch.faculty.faculty_name,
                                                 c.permission
                                             })
                                     .FirstOrDefault(c => c.user_id == id);

                return JsonConvert.SerializeObject(users);
            }
        }

        [WebMethod]
        public static string GetBranchesByFaculty(string facultyName)
        {
            using (var _db = new ResearchDBEntities())
            {
                List<string> branchs = _db.branches.Where(c => c.faculty.faculty_name == facultyName).Select(c => c.branch_name).ToList();
                return JsonConvert.SerializeObject(branchs);
            }
        }
    }
}
using Research_Framework.ApplicationDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.DynamicData;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Research_Framework.Webpage
{
    public partial class AddReserch : System.Web.UI.Page
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

                if (Application["userNew"] != null)
                    Application["userNew"] = null;

                if (Application["researchNew"] != null)
                    Application["researchNew"] = null;

                if (Application["userID"] is null)
                    return;

                View_user username = (View_user)Application["userID"];

                using (var _db = new ResearchDBEntities())
                {
                    View_research_member userFirst = _db.View_research_member.FirstOrDefault(c => c.username == username.username);

                    if (userFirst == null)
                        return;

                    List<int> viewUser_list = _db.View_research_member.Where(c => c.research_id == userFirst.research_id)
                                                                      .Select(c => c.user_id)
                                                                      .ToList();

                    List<View_user> user_list = _db.View_user.Where(c => viewUser_list.Contains(c.user_id)).ToList();



                    if (user_list is null || user_list.Count == 0)
                        return;

                    getStudent(user_list);

                    var research = _db.View_research.Where(c => c.research_id == userFirst.research_id)
                                                    .FirstOrDefault();

                    if (research == null)
                        return;

                    Application["researchNew"] = research;

                    Tb_reserch.Text = research.research_name;

                    var teacher = _db.View_user.FirstOrDefault(c => c.user_id == research.teacher_id);

                    if (teacher == null)
                        return;

                    Tb_teacher.Text = teacher.name + " " + teacher.lname;
                    Tb_teacher.Enabled = false;
                }
            }
        }

        protected void Btn_add_Click(object sender, EventArgs e)
        {
            string username = Tb_student.Text.Trim();

            string script;

            if (string.IsNullOrEmpty(username))
            {
                script = "swal('พบข้อผิดพลาด', 'กรุณากรอกชื่อนักศึกษา', 'error');";
                ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert", script, true);
                return;
            }

            using(var _db = new ResearchDBEntities())
            {
                string[] usernameSpilt = username.Split(' ');
                if (usernameSpilt.Length != 2)
                {
                    script = "swal('พบข้อผิดพลาด', 'ชื่อนักศึกษาไม่ถูกต้อง', 'error');";
                    ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert", script, true);
                    return;
                }

                string name = usernameSpilt[0];
                string lname = usernameSpilt[1];

                View_user user = _db.View_user.FirstOrDefault(c => c.name == name && c.lname == lname);

                if(user == null)
                {
                    script = "swal('พบข้อผิดพลาด', 'ไม่พบชื่อนักศึกษาในระบบ', 'error');";
                    ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert", script, true);
                    return;
                }

                List<View_user> user_list;
                if (Application["userNew"] == null)
                    user_list = new List<View_user>();
                else
                    user_list = (List<View_user>)Application["userNew"];

                if(user_list.Count(c => c.user_id == user.user_id) > 0)
                {
                    script = "swal('พบข้อผิดพลาด', 'มีชื่อนักศึกษานี้อยู่แล้ว', 'error');";
                    ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert", script, true);
                    return;
                }

                user_list.Add(user);
                getStudent(user_list);

            }
        }

        protected void Btn_save_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Tb_teacher.Text.Trim()) || string.IsNullOrEmpty(Tb_reserch.Text.Trim()))
                return;

            if (Application["userNew"] == null)
                return;

            string teacher = Tb_teacher.Text.Trim();
            string researchName = Tb_reserch.Text.Trim();

            string[] teacherSpilt = teacher.Split(' ');
            if (teacherSpilt.Length != 2)
                return;

            string name = teacherSpilt[0];
            string lname = teacherSpilt[1];


            using (var _db = new ResearchDBEntities())
            {
                View_user user = _db.View_user.FirstOrDefault(c => c.name == name && c.lname == lname && c.permission == "teacher");

                if (user == null)
                    return;

                research researchNew;
                View_research researchNew_view;

                if (Application["researchNew"] == null)
                {
                    researchNew = new research()
                    {
                        research_name = researchName,
                        teacher_id = user.user_id,
                        approve = false
                    };
                    _db.researches.Add(researchNew);
                }
                else
                {
                    researchNew_view = (View_research)Application["researchNew"];

                    research researchOld = _db.researches.FirstOrDefault(c => c.research_id == researchNew_view.research_id);

                    if (researchOld != null)
                    {
                        researchOld.research_name = researchName;
                        //researchOld.teacher_id = user.user_id;
                        //researchOld.approve = researchNew_view.approve;
                    }
                }

                _db.SaveChanges();

                researchNew_view = _db.View_research.FirstOrDefault(c => c.research_name == researchName);

                if (researchNew_view == null)
                    return;

                Application["researchNew"] = researchNew_view;

                List<View_user> user_list = (List<View_user>)Application["userNew"];

                user_list.ForEach(c =>
                {
                    research_member member = _db.research_member.FirstOrDefault(x => x.user_id == c.user_id);
                    if (member == null)
                    {
                        var memberNew = new research_member()
                        {
                            research_id = researchNew_view.research_id,
                            user_id = c.user_id
                        };

                        _db.research_member.Add(memberNew);
                    }
                });

                var list_process = _db.processes.ToList();

                list_process.ForEach(c =>
                {
                    process_path newProcess_path = _db.process_path.FirstOrDefault(x => x.process_id == c.process_id && x.research_id == researchNew_view.research_id);

                    if (newProcess_path == null)
                    {
                        _db.process_path.Add(new process_path()
                        {
                            process_id = c.process_id,
                            research_id = researchNew_view.research_id,
                        });
                    }
                });

                _db.SaveChanges();

                Response.Redirect(Request.RawUrl);
            }
        }

        private void getStudent(List<View_user> user_list)
        {
            user_list = user_list.Select(c => new View_user
                                 {
                                     user_id = c.user_id,
                                     username = c.username,
                                     password = c.password,
                                     name = c.name,
                                     lname = c.lname,
                                     faculty_id = c.faculty_id,
                                     faculty_name = c.faculty_name,
                                     branch_id = c.branch_id,
                                     branch_name = c.branch_name,
                                     permission = c.permission,
                                 })
                                 .ToList();

            Application["userNew"] = user_list;


            var ss = (List<View_user>)Application["userNew"];

            DataTable dt = new DataTable();
            dt.Columns.Add("stdID", typeof(string));
            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("lname", typeof(string));
            dt.Columns.Add("faculty", typeof(string));
            dt.Columns.Add("branch", typeof(string));

            user_list.ForEach(c =>
            {
                dt.Rows.Add(c.username, c.name, c.lname, c.faculty_name, c.branch_name);
            });

            Dgv_std.DataSource = dt;
            Dgv_std.DataBind();
        }

        [WebMethod]
        public static List<string> GetEmployeeName(string empName, string permission)
        {
            using (var _db = new ResearchDBEntities())
            {
                // Filter suggestions based on empName and permission
                List<string> teacherSuggestions = _db.users.Where(c => c.permission == permission && (c.name + " " + c.lname).ToLower().Contains(empName.ToLower()))
                                                           .Select(c => c.name + " " + c.lname)
                                                           .ToList();

                return teacherSuggestions;
            }
        }
    }
}
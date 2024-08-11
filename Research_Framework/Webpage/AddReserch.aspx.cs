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
                if (Session["userID"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                if (Session["userNew"] != null)
                    Session["userNew"] = null;

                if (Session["researchNew"] != null)
                    Session["researchNew"] = null;

                if (Session["userID"] == null)
                    return;

                user username = (user)Session["userID"];

                using (var _db = new ResearchDBEntities())
                {
                    research_member userFirst = _db.research_member.FirstOrDefault(c => c.user.username == username.username);

                    if (userFirst == null)
                        return;

                    List<int> viewUser_list = _db.research_member.Where(c => c.research_id == userFirst.research_id)
                                                                      .Select(c => c.user_id)
                                                                      .ToList();

                    List<user> user_list = _db.users.Where(c => viewUser_list.Contains(c.user_id)).ToList();



                    if (user_list is null || user_list.Count == 0)
                        return;

                    getStudent(user_list);

                    var research = _db.researches.Where(c => c.research_id == userFirst.research_id)
                                                    .FirstOrDefault();

                    if (research == null)
                        return;

                    Session["researchNew"] = research;

                    Tb_reserch.Text = research.research_name;

                    var teacher = _db.users.FirstOrDefault(c => c.user_id == research.teacher_id);

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

                user user = _db.users.FirstOrDefault(c => c.name == name && c.lname == lname);

                if(user == null)
                {
                    script = "swal('พบข้อผิดพลาด', 'ไม่พบชื่อนักศึกษาในระบบ', 'error');";
                    ScriptManager.RegisterStartupScript(this, GetType(), "SweetAlert", script, true);
                    return;
                }

                List<user> user_list;
                if (Session["userNew"] == null)
                    user_list = new List<user>();
                else
                    user_list = (List<user>)Session["userNew"];

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

            if (Session["userNew"] == null)
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
                user user = _db.users.FirstOrDefault(c => c.name == name && c.lname == lname && c.permission == "teacher");

                if (user == null)
                    return;

                research researchNew;
                research researchNew_view;

                if (Session["researchNew"] == null)
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
                    researchNew_view = (research)Session["researchNew"];

                    research researchOld = _db.researches.FirstOrDefault(c => c.research_id == researchNew_view.research_id);

                    if (researchOld != null)
                    {
                        researchOld.research_name = researchName;
                        //researchOld.teacher_id = user.user_id;
                        //researchOld.approve = researchNew_view.approve;
                    }
                }

                _db.SaveChanges();

                using(var _db2 = new ResearchDBEntities())
                {

                    researchNew_view = _db2.researches.FirstOrDefault(c => c.research_name == researchName);

                    if (researchNew_view == null)
                        return;

                    Session["researchNew"] = researchNew_view;

                    List<user> user_list = (List<user>)Session["userNew"];

                    foreach (var item in user_list)
                    {
                        research_member member = _db2.research_member.FirstOrDefault(x => x.user_id == item.user_id);
                        if (member == null)
                        {
                            var memberNew = new research_member()
                            {
                                research_id = researchNew_view.research_id,
                                user_id = item.user_id
                            };

                            _db2.research_member.Add(memberNew);
                        }
                    }

                    var list_process = _db2.processes.ToList();

                    foreach (var item in list_process)
                    {
                        process_path newProcess_path = _db2.process_path.FirstOrDefault(x => x.process_id == item.process_id && x.research_id == researchNew_view.research_id);

                        if (newProcess_path == null)
                        {
                            process_path process_Path = new process_path()
                            {
                                process_id = item.process_id,
                                research_id = researchNew_view.research_id,
                                status = "ยังไม่อนุมัติ"
                            };

                            _db2.process_path.Add(process_Path);
                        }
                    }

                    _db2.SaveChanges();

                    Response.Redirect(Request.RawUrl);
                }
            }
        }

        private void getStudent(List<user> user_list)
        {
            user_list = user_list.Select(c => new user
                                 {
                                     user_id = c.user_id,
                                     username = c.username,
                                     password = c.password,
                                     name = c.name,
                                     lname = c.lname,
                                     branch_id = c.branch_id,
                                     permission = c.permission,
                                 })
                                 .ToList();

            Session["userNew"] = user_list;


            var ss = (List<user>)Session["userNew"];

            DataTable dt = new DataTable();
            dt.Columns.Add("stdID", typeof(string));
            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("lname", typeof(string));
            dt.Columns.Add("faculty", typeof(string));
            dt.Columns.Add("branch", typeof(string));

            using (var _db = new ResearchDBEntities())
            {
                user_list.ForEach(c =>
                {
                    user user = _db.users.FirstOrDefault(x => x.user_id == c.user_id);
                    dt.Rows.Add(user.username, user.name, user.lname, user.branch.faculty.faculty_name, user.branch.branch_name);
                });

                Dgv_std.DataSource = dt;
                Dgv_std.DataBind();
            }

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
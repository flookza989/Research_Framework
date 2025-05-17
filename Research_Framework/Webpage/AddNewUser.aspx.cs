using OfficeOpenXml;
using Research_Framework.ApplicationDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Research_Framework.Webpage
{
    public partial class AddnewUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                // ตรวจสอบสิทธิ์การเข้าถึง
                using (var db = new ResearchDBEntities())
                {
                    int userId = Convert.ToInt32(Session["UserID"]);
                    var user = db.users.Find(userId);
                    if (user == null || (user.user_type != "ADMIN" && user.user_type != "TEACHER"))
                    {
                        Response.Redirect("~/index.aspx");
                        return;
                    }
                }

                if (Session["NewSTD"] != null)
                    Session["NewSTD"] = null;

                div_Btn_save.Visible = false;
            }
        }

        protected void Btn_upload_Click(object sender, EventArgs e)
        {
            if (!FileUploadControl.HasFile)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                    "Swal.fire('แจ้งเตือน', 'กรุณาเลือกไฟล์ที่ต้องการอัพโหลด', 'warning');", true);
                return;
            }

            try
            {
                string fileExtension = Path.GetExtension(FileUploadControl.FileName).ToLower();
                if (fileExtension != ".xlsx" && fileExtension != ".xls")
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                        "Swal.fire('Error!', 'กรุณาอัพโหลดไฟล์ Excel เท่านั้น (.xlsx, .xls)', 'error');", true);
                    return;
                }

                using (var package = new ExcelPackage(FileUploadControl.FileContent))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                            "Swal.fire('Error!', 'ไม่พบข้อมูลในไฟล์ Excel', 'error');", true);
                        return;
                    }

                    if (ValidateExcelData(worksheet))
                    {
                        // แปลงข้อมูลเป็น DataTable สำหรับแสดงใน GridView
                        DataTable dt = new DataTable();
                        dt.Columns.AddRange(new DataColumn[]
                        {
                    new DataColumn("UserName"),
                    new DataColumn("Password"),
                    new DataColumn("Name"),
                    new DataColumn("SurName"),
                    new DataColumn("UserType"),
                    new DataColumn("Faculty"),
                    new DataColumn("Branch")
                        });

                        // อ่านข้อมูลจาก Excel ใส่ใน DataTable
                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            if (string.IsNullOrEmpty(worksheet.Cells[row, 1].Text))
                                continue;

                            dt.Rows.Add(
                                worksheet.Cells[row, 1].Text, // UserName
                                worksheet.Cells[row, 2].Text, // Password
                                worksheet.Cells[row, 3].Text, // Name
                                worksheet.Cells[row, 4].Text, // SurName
                                worksheet.Cells[row, 5].Text, // UserType
                                worksheet.Cells[row, 6].Text, // Faculty
                                worksheet.Cells[row, 7].Text  // Branch
                            );
                        }

                        Session["NewSTD"] = dt;
                        Gv_newSTD.DataSource = dt;
                        Gv_newSTD.DataBind();
                        div_Btn_save.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                    $"Swal.fire('Error!', 'เกิดข้อผิดพลาดในการอัพโหลดไฟล์: {ex.Message}', 'error');", true);
            }
        }

        protected void Btn_save_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    DataTable dt = Session["NewSTD"] as DataTable;
            //    if (dt == null)
            //    {
            //        ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
            //            "Swal.fire('Error!', 'ไม่พบข้อมูลที่จะบันทึก กรุณาอัพโหลดไฟล์ใหม่', 'error');", true);
            //        return;
            //    }

            //    using (var db = new ResearchDBEntities())
            //    {
            //        using (var transaction = db.Database.BeginTransaction())
            //        {
            //            try
            //            {
            //                var faculties = db.facultys.ToList();
            //                var branches = db.branchs.ToList();
            //                foreach (DataRow row in dt.Rows)
            //                {
            //                    // สร้าง user
            //                    var user = new user
            //                    {
            //                        username = row["UserName"].ToString(),
            //                        password = row["Password"].ToString(),
            //                        first_name = row["Name"].ToString(),
            //                        last_name = row["SurName"].ToString(),
            //                        user_type = row["UserType"].ToString(),
            //                        is_active = true,
            //                        created_date = DateTime.Now
            //                    };
            //                    var userDb = db.users.Add(user);
            //                    db.SaveChanges();
            //                    // หา branch
            //                    var faculty = faculties.FirstOrDefault(f =>
            //                        f.faculty_name == row["Faculty"].ToString());
            //                    var branch = branches.FirstOrDefault(b =>
            //                        b.branch_name == row["Branch"].ToString() &&
            //                        b.faculty_id == faculty.faculty_id);

            //                    // สร้าง student หรือ teacher
            //                    if (user.user_type == "STUDENT")
            //                    {
            //                        var student = new student
            //                        {
            //                            user_id = userDb.id,
            //                            branch_id = branch.branch_id,
            //                            faculty_id = faculty.faculty_id
            //                        };
            //                        db.students.Add(student);
            //                    }
            //                    else if (user.user_type == "TEACHER")
            //                    {
            //                        var teacher = new teacher
            //                        {
            //                            user_id = userDb.id,
            //                            branch_id = branch.branch_id,
            //                            faculty_id = faculty.faculty_id
            //                        };
            //                        db.teachers.Add(teacher);
            //                    }
            //                }

            //                db.SaveChanges();
            //                transaction.Commit();

            //                // Clear session และ reset UI
            //                Session["NewSTD"] = null;
            //                Gv_newSTD.DataSource = null;
            //                Gv_newSTD.DataBind();
            //                div_Btn_save.Visible = false;

            //                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
            //                    "Swal.fire('Success!', 'บันทึกข้อมูลสำเร็จ', 'success');", true);
            //            }
            //            catch (Exception ex)
            //            {
            //                transaction.Rollback();
            //                throw ex;
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
            //        $"Swal.fire('Error!', 'เกิดข้อผิดพลาดในการบันทึกข้อมูล: {ex.Message}', 'error');", true);
            //}
        }

        protected void Btn_Download_Click(object sender, EventArgs e)
        {
            try
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("NewUsers");

                    // ตั้งค่าหัวตาราง
                    worksheet.Cells[1, 1].Value = "รหัสผู้ใช้งาน*";
                    worksheet.Cells[1, 2].Value = "รหัสผ่าน*";
                    worksheet.Cells[1, 3].Value = "ชื่อ*";
                    worksheet.Cells[1, 4].Value = "นามสกุล*";
                    worksheet.Cells[1, 5].Value = "ประเภทผู้ใช้*";
                    worksheet.Cells[1, 6].Value = "คณะ*";
                    worksheet.Cells[1, 7].Value = "สาขา*";

                    // จัดรูปแบบหัวตาราง
                    using (var range = worksheet.Cells[1, 1, 1, 7])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        range.Style.Font.Size = 12;
                        range.AutoFilter = true;
                    }

                    // ปรับความกว้างของคอลัมน์
                    worksheet.Column(1).Width = 15; // รหัสผู้ใช้
                    worksheet.Column(2).Width = 15; // รหัสผ่าน
                    worksheet.Column(3).Width = 20; // ชื่อ
                    worksheet.Column(4).Width = 20; // นามสกุล
                    worksheet.Column(5).Width = 15; // ประเภทผู้ใช้
                    worksheet.Column(6).Width = 25; // คณะ
                    worksheet.Column(7).Width = 25; // สาขา

                    // สร้าง dropdown list สำหรับประเภทผู้ใช้
                    var userTypeValidation = worksheet.DataValidations.AddListValidation("E2:E1000");
                    userTypeValidation.Formula.Values.Add("STUDENT");
                    userTypeValidation.Formula.Values.Add("TEACHER");

                    using (var db = new ResearchDBEntities())
                    {
                        // ดึงข้อมูลคณะ
                        var faculties = db.facultys.Select(f => f.faculty_name).ToList();
                        var facultyValidation = worksheet.DataValidations.AddListValidation("F2:F1000");
                        foreach (var faculty in faculties)
                        {
                            facultyValidation.Formula.Values.Add(faculty);
                        }

                        // ดึงข้อมูลสาขา
                        var branches = db.branchs.Select(f => f.branch_name).ToList();
                        var branchesValidation = worksheet.DataValidations.AddListValidation("G2:G1000");
                        foreach (var branch in branches)
                        {
                            branchesValidation.Formula.Values.Add(branch);
                        }
                    }

                    // เพิ่มตัวอย่างข้อมูล
                    worksheet.Cells[2, 1].Value = "6411234567"; // ตัวอย่างรหัสนักศึกษา
                    worksheet.Cells[2, 2].Value = "Pass@word1"; // ตัวอย่างรหัสผ่าน
                    worksheet.Cells[2, 3].Value = "สมชาย";
                    worksheet.Cells[2, 4].Value = "ใจดี";
                    worksheet.Cells[2, 5].Value = "STUDENT";

                    // จัดรูปแบบแถวตัวอย่าง
                    using (var range = worksheet.Cells[2, 1, 2, 7])
                    {
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                    }

                    // เพิ่มคำอธิบาย
                    worksheet.Cells[4, 1].Value = "หมายเหตุ:";
                    worksheet.Cells[5, 1].Value = "1. ช่องที่มีเครื่องหมาย * จำเป็นต้องกรอกข้อมูล";
                    worksheet.Cells[6, 1].Value = "2. รหัสผ่านต้องมีความยาวอย่างน้อย 8 ตัวอักษร ประกอบด้วยตัวอักษรพิมพ์ใหญ่ พิมพ์เล็ก ตัวเลข และอักขระพิเศษ";
                    worksheet.Cells[7, 1].Value = "3. ประเภทผู้ใช้งานต้องเป็น STUDENT หรือ TEACHER เท่านั้น";
                    worksheet.Cells[8, 1].Value = "4. กรุณาเลือกคณะและสาขาจาก dropdown list";

                    // ป้องกันการแก้ไขโครงสร้าง
                    worksheet.Protection.IsProtected = true;
                    worksheet.Protection.AllowAutoFilter = true;
                    worksheet.Protection.AllowSort = true;

                    var dataRange = worksheet.Cells[2, 1, 1000, 7];
                    dataRange.Style.Locked = false;

                    // ส่งไฟล์
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=template_add_users.xlsx");
                    Response.BinaryWrite(package.GetAsByteArray());
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                    $"Swal.fire('Error!', 'เกิดข้อผิดพลาดในการสร้างเทมเพลต: {ex.Message}', 'error');", true);
            }
        }

        private bool ValidateExcelData(ExcelWorksheet worksheet)
        {
            List<string> errors = new List<string>();
            int rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                // ข้ามแถวว่าง
                if (string.IsNullOrEmpty(worksheet.Cells[row, 1].Text))
                    continue;

                // ตรวจสอบข้อมูลที่จำเป็น
                if (string.IsNullOrEmpty(worksheet.Cells[row, 1].Text) || // รหัสผู้ใช้
                    string.IsNullOrEmpty(worksheet.Cells[row, 2].Text) || // รหัสผ่าน
                    string.IsNullOrEmpty(worksheet.Cells[row, 3].Text) || // ชื่อ
                    string.IsNullOrEmpty(worksheet.Cells[row, 4].Text) || // นามสกุล
                    string.IsNullOrEmpty(worksheet.Cells[row, 5].Text) || // ประเภทผู้ใช้
                    string.IsNullOrEmpty(worksheet.Cells[row, 6].Text) || // คณะ
                    string.IsNullOrEmpty(worksheet.Cells[row, 7].Text))   // สาขา
                {
                    errors.Add($"แถวที่ {row}: กรุณากรอกข้อมูลที่จำเป็นให้ครบถ้วน");
                    continue;
                }

                // ตรวจสอบประเภทผู้ใช้
                string userType = worksheet.Cells[row, 5].Text.Trim().ToUpper();
                if (userType != "STUDENT" && userType != "TEACHER")
                {
                    errors.Add($"แถวที่ {row}: ประเภทผู้ใช้ต้องเป็น STUDENT หรือ TEACHER เท่านั้น");
                }

                // ตรวจสอบรหัสผ่าน
                string password = worksheet.Cells[row, 2].Text;
                if (!IsValidPassword(password))
                {
                    errors.Add($"แถวที่ {row}: รหัสผ่านไม่ถูกต้องตามเงื่อนไข");
                }

                // ตรวจสอบรหัสผู้ใช้ซ้ำ
                string username = worksheet.Cells[row, 1].Text;
                using (var db = new ResearchDBEntities())
                {
                    if (db.users.Any(u => u.username == username))
                    {
                        errors.Add($"แถวที่ {row}: รหัสผู้ใช้ {username} มีในระบบแล้ว");
                    }
                }

                // ตรวจสอบคณะและสาขา
                string faculty = worksheet.Cells[row, 6].Text;
                string branch = worksheet.Cells[row, 7].Text;
                using (var db = new ResearchDBEntities())
                {
                    var facultyExists = db.facultys.Any(f => f.faculty_name == faculty);
                    if (!facultyExists)
                    {
                        errors.Add($"แถวที่ {row}: ไม่พบคณะ '{faculty}' ในระบบ");
                    }
                    else
                    {
                        var facultyId = db.facultys.First(f => f.faculty_name == faculty).faculty_id;
                        var branchExists = db.branchs.Any(b => b.branch_name == branch && b.faculty_id == facultyId);
                        if (!branchExists)
                        {
                            errors.Add($"แถวที่ {row}: ไม่พบสาขา '{branch}' ในคณะ '{faculty}'");
                        }
                    }
                }
            }

            if (errors.Any())
            {
                string errorMessage = string.Join("<br/>", errors);
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                    $"Swal.fire('ข้อผิดพลาด!', '{errorMessage}', 'error');", true);
                return false;
            }

            return true;
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return false;

            var hasNumber = password.Any(char.IsDigit);
            var hasUpperChar = password.Any(char.IsUpper);
            var hasLowerChar = password.Any(char.IsLower);
            var hasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));
            var isLengthValid = password.Length >= 8;

            return hasNumber && hasUpperChar && hasLowerChar && hasSpecialChar && isLengthValid;
        }
    }
}
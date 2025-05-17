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
                if (Session["userID"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                if (Session["NewSTD"] != null)
                    Session["NewSTD"] = null;
            }


        }

        protected void Btn_upload_Click(object sender, EventArgs e)
        {
            if (FileUploadControl.HasFile && IsExcelFile(FileUploadControl.FileName))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (MemoryStream stream = new MemoryStream(FileUploadControl.FileBytes))
                using (ExcelPackage excelPackage = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[0];
                    int rowCount = 2;

                    List<InsertUser> users = new List<InsertUser>();
                    while (true)
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(worksheet.Cells[rowCount, 1].Value)))
                            break;

                        InsertUser insertUser = new InsertUser()
                        {
                            Name = Convert.ToString(worksheet.Cells[rowCount, 1].Value),
                            SurName = Convert.ToString(worksheet.Cells[rowCount, 2].Value),
                            UserName = Convert.ToString(worksheet.Cells[rowCount, 3].Value),
                            Password = Convert.ToString(worksheet.Cells[rowCount, 4].Value),
                            Faculty = Convert.ToString(worksheet.Cells[rowCount, 5].Value),
                            Branch = Convert.ToString(worksheet.Cells[rowCount, 6].Value),
                            Permission = Convert.ToString(worksheet.Cells[rowCount, 7].Value),
                        };
                        users.Add(insertUser);
                        rowCount++;
                    }

                    if (!ValidateFileExcel(users))
                        return;

                    AddUserToGridview(users);
                }
            }
        }

        private void AddUserToGridview(List<InsertUser> users)
        {
            Session["NewSTD"] = users;
            Gv_newSTD.DataSource = users;
            Gv_newSTD.DataBind();
            div_Btn_save.Visible = true;
        }

        private bool ValidateFileExcel(List<InsertUser> users)
        {
            List<string> userNameList = users.Select(c => c.UserName).ToList();

            if (!ValidateUsername(userNameList))
                return false;

            List<string> facultyList = users.Select(c => c.Faculty).ToList();
            if (!ValidateFaculty(facultyList))
                return false;

            List<string> BranchList = users.Select(c => c.Branch).ToList();
            if (!ValidateBranch(BranchList))
                return false;

            if (!ValidateFacultyAndBranch(users))
                return false;


            return true;

        }

        private bool ValidateUsername(List<string> userNameList)
        {
            using (var _db = new ResearchDBEntities())
            {
                List<user> userContainsList = _db.users.Where(c => userNameList.Contains(c.username)).ToList();

                if (userContainsList.Count > 0)
                {
                    Console.WriteLine("รหัสนักศึกษาซ้ำ!!!");
                    return false;
                }

                return true;
            }
        }

        private bool ValidateFaculty(List<string> facultyList)
        {
            using (var _db = new ResearchDBEntities())
            {
                facultyList = facultyList.Distinct().ToList();

                foreach (var item in facultyList)
                {
                    faculty facultyCheck = _db.faculties.FirstOrDefault(c => c.faculty_name == item);

                    if (facultyCheck == null)
                    {
                        Console.WriteLine("ไม่มีคณะ!!!");
                        return false;
                    }
                }

                return true;
            }
        }

        private bool ValidateBranch(List<string> BranchList)
        {
            using (var _db = new ResearchDBEntities())
            {
                BranchList = BranchList.Distinct().ToList();

                foreach (var item in BranchList)
                {
                    branch branchCheck = _db.branches.FirstOrDefault(c => c.branch_name == item);

                    if (branchCheck == null)
                    {
                        Console.WriteLine("ไม่มีคณะ!!!");
                        return false;
                    }
                }

                return true;
            }
        }

        private bool ValidateFacultyAndBranch(List<InsertUser> users)
        {
            using (var _db = new ResearchDBEntities())
            {
                foreach (var item in users)
                {
                    branch branchAndFacultyCheck = _db.branches.FirstOrDefault(c => c.branch_name == item.Branch && c.faculty.faculty_name == item.Faculty);

                    if (branchAndFacultyCheck == null)
                    {
                        Console.WriteLine("คณะและสาขาไม่ตรงกัน!!!");
                        return false;
                    }
                }

                return true;
            }
        }

        protected void Btn_Download_Click(object sender, EventArgs e)
        {
            using (var _db = new ResearchDBEntities())
            {
                master_file master_File = _db.master_file.FirstOrDefault(c => c.fileName == "MasterUser");
                byte[] pdfBytes = master_File.fileData;
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", $"attachment;filename={master_File.fileName}.xlsx");
                Response.Buffer = true;
                Response.BinaryWrite(pdfBytes);
                Response.End();
            }
        }

        private bool IsExcelFile(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            return !string.IsNullOrEmpty(extension) && extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase);
        }

        protected void Btn_save_Click(object sender, EventArgs e)
        {
            if (Session["NewSTD"] == null)
                return;

            if (Session["NewSTD"] is List<InsertUser> newStd)
            {
                using (var _db = new ResearchDBEntities())
                {
                    try
                    {
                        foreach (var item in newStd)
                        {
                            _db.users.Add(new user()
                            {
                                name = item.Name,
                                lname = item.SurName,
                                username = item.UserName,
                                password = item.Password,
                                branch_id = _db.branches.FirstOrDefault(c => c.branch_name == item.Branch && c.faculty.faculty_name == item.Faculty).branch_id,
                                permission = "student"
                            });
                        }

                        _db.SaveChanges();
                    }
                    catch (DbEntityValidationException ex)
                    {
                        // Iterate through the validation errors for each entity
                        foreach (var validationErrors in ex.EntityValidationErrors)
                        {
                            // Iterate through the validation errors for each property of the entity
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                // Log or display the validation error
                                string errorMessage = $"Entity: {validationErrors.Entry.Entity.GetType().Name}, Property: {validationError.PropertyName}, Error: {validationError.ErrorMessage}";
                                // Here, you can log or display the errorMessage as needed
                                // For example, you can write it to a log file, display it in a message box, etc.
                                // Logging or displaying these errors will help in debugging and resolving validation issues.
                            }
                        }
                    }

                    Response.Redirect(Request.RawUrl);
                }
            }
        }
    }

    class InsertUser
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Faculty { get; set; }
        public string Branch { get; set; }
        public string Permission {  get; set; }
    }
}
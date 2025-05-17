using Research_Framework.ApplicationDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Research_Framework.Webpage
{
    public partial class Contact : System.Web.UI.Page
    {
        // พาธสำหรับเก็บไฟล์ที่อัพโหลด
        private string uploadFolderPath = "~/Uploads/ContactFiles/";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // โหลดข้อมูลไฟล์ที่มีอยู่
                LoadFiles();
                
                // ตรวจสอบสิทธิ์การเข้าถึง
                CheckUserPermission();
            }
        }

        /// <summary>
        /// ตรวจสอบสิทธิ์การเข้าถึงของผู้ใช้งาน
        /// </summary>
        private void CheckUserPermission()
        {
            try
            {
                if (Session["UserID"] != null && Session["UserType"] != null)
                {
                    int userId = Convert.ToInt32(Session["UserID"]);
                    string userType = Session["UserType"].ToString();

                    // แสดงแผงควบคุมสำหรับ Admin เท่านั้น
                    AdminPanel.Visible = (userType == "ADMIN");

                    // ซ่อนคอลัมน์ลบสำหรับผู้ใช้ที่ไม่ใช่ Admin
                    if (FilesGridView.Columns.Count > 0)
                    {
                        FilesGridView.Columns[FilesGridView.Columns.Count - 1].Visible = (userType == "ADMIN");
                    }
                }
                else
                {
                    // ถ้าไม่มีข้อมูลการเข้าสู่ระบบให้เปลี่ยนเส้นทางไปหน้า Login
                    Response.Redirect("~/Webpage/Login.aspx");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("เกิดข้อผิดพลาด: " + ex.Message);
            }
        }

        /// <summary>
        /// โหลดข้อมูลไฟล์ที่มีอยู่ในโฟลเดอร์
        /// </summary>
        private void LoadFiles()
        {
            try
            {
                // สร้าง DataTable สำหรับเก็บข้อมูลไฟล์
                DataTable dt = new DataTable();
                dt.Columns.Add("FileId", typeof(int));
                dt.Columns.Add("FileName", typeof(string));
                dt.Columns.Add("FileType", typeof(string));
                dt.Columns.Add("UploadDate", typeof(DateTime));
                dt.Columns.Add("FileSize", typeof(string));
                dt.Columns.Add("FilePath", typeof(string));

                // ตรวจสอบว่าโฟลเดอร์มีอยู่หรือไม่
                string physicalPath = Server.MapPath(uploadFolderPath);
                if (!Directory.Exists(physicalPath))
                {
                    Directory.CreateDirectory(physicalPath);
                }

                // อ่านไฟล์ทั้งหมดในโฟลเดอร์
                DirectoryInfo dir = new DirectoryInfo(physicalPath);
                FileInfo[] files = dir.GetFiles();

                // เพิ่มข้อมูลไฟล์ลงใน DataTable
                int fileId = 1;
                foreach (FileInfo file in files)
                {
                    DataRow row = dt.NewRow();
                    row["FileId"] = fileId++;
                    
                    // หากมีเครื่องหมาย _ ในชื่อไฟล์ให้แยกส่วนชื่อจริงออกมา
                    string fileName = file.Name;
                    string displayName = fileName;
                    
                    // ตรวจสอบว่ามีเครื่องหมาย _ ตามด้วยวันที่เวลาหรือไม่
                    if (fileName.Contains("_"))
                    {
                        try
                        {
                            // ชื่อไฟล์อาจจะอยู่ในรูปแบบ "ชื่อที่แสดง_yyyyMMddHHmmss.ext"
                            string[] parts = fileName.Split('_');
                            if (parts.Length > 1)
                            {
                                // ส่วนแรกคือชื่อที่แสดง
                                displayName = parts[0];
                                
                                // พยายามแปลงส่วนที่เหลือเป็นวันที่เวลา
                                string dateTimePart = parts[1].Split('.')[0]; // ตัดนามสกุลออก
                                
                                // ใช้วันที่ปัจจุบันหรือวันที่สร้างไฟล์แทนการพยายามแปลงวันที่จากชื่อไฟล์
                                // เพื่อป้องกันการแสดงวันที่ผิดพลาด
                                row["UploadDate"] = DateTime.Now.Date;
                            }
                            else
                            {
                                row["UploadDate"] = file.CreationTime;
                            }
                        }
                        catch
                        {
                            // หากเกิดข้อผิดพลาดให้ใช้ชื่อไฟล์เดิมและวันที่สร้างไฟล์
                            row["UploadDate"] = file.CreationTime;
                        }
                    }
                    else
                    {
                    // ไม่มีเครื่องหมาย _ ก็ใช้วันที่ปัจจุบัน
                    row["UploadDate"] = DateTime.Now.Date;
                    }

                    row["FileName"] = displayName;
                    row["FileType"] = file.Extension.ToUpper().Replace(".", "");
                    row["FileSize"] = FormatFileSize(file.Length);
                    row["FilePath"] = file.Name;
                    
                    dt.Rows.Add(row);
                }

                // จัดเรียงข้อมูลตามวันที่อัพโหลดล่าสุด
                DataView dv = dt.DefaultView;
                dv.Sort = "UploadDate DESC";
                
                // ผูกข้อมูลกับ GridView
                FilesGridView.DataSource = dv;
                FilesGridView.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage("เกิดข้อผิดพลาดในการโหลดไฟล์: " + ex.Message);
            }
        }

        /// <summary>
        /// แปลงขนาดไฟล์ให้อยู่ในรูปแบบที่อ่านง่าย
        /// </summary>
        private string FormatFileSize(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            double fileSize = bytes;

            while (fileSize >= 1024 && counter < suffixes.Length - 1)
            {
                fileSize /= 1024;
                counter++;
            }

            return string.Format("{0:0.##} {1}", fileSize, suffixes[counter]);
        }

        /// <summary>
        /// แสดงข้อความแจ้งเตือน
        /// </summary>
        private void ShowMessage(string message, bool isSuccess = false)
        {
            StatusLabel.Text = message;
            StatusLabel.CssClass = isSuccess ? "alert alert-success mt-3" : "alert alert-danger mt-3";
            StatusLabel.CssClass = StatusLabel.CssClass.Replace("d-none", "").Trim();
        }

        /// <summary>
        /// อัพโหลดไฟล์
        /// </summary>
        protected void BtnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                // ตรวจสอบว่ามีการเลือกไฟล์หรือไม่
                if (FileUploadControl.HasFile)
                {
                    // ตรวจสอบขนาดไฟล์ (10MB = 10485760 bytes)
                    if (FileUploadControl.PostedFile.ContentLength < 10485760)
                    {
                        // ตรวจสอบนามสกุลไฟล์
                        string fileExtension = Path.GetExtension(FileUploadControl.FileName).ToLower();
                        string[] allowedExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".zip" };

                        if (allowedExtensions.Contains(fileExtension))
                        {
                            // รับชื่อไฟล์จาก TextBox หรือใช้ชื่อไฟล์เดิมถ้าไม่ได้ระบุ
                            string fileName = !string.IsNullOrWhiteSpace(TxtFileName.Text) 
                                ? TxtFileName.Text 
                                : Path.GetFileNameWithoutExtension(FileUploadControl.FileName);

                            // เพิ่มวันที่เวลาเพื่อป้องกันชื่อไฟล์ซ้ำ
                            // กำหนด timestamp เป็นรหัสแบบสุ่มแทนวันที่เพื่อป้องกันการสับสนของวันที่
                            string timestamp = Guid.NewGuid().ToString().Substring(0, 8);
                            string savedFileName = $"{fileName}_{timestamp}{fileExtension}";
                            
                            // ตรวจสอบว่าโฟลเดอร์มีอยู่หรือไม่
                            string physicalPath = Server.MapPath(uploadFolderPath);
                            if (!Directory.Exists(physicalPath))
                            {
                                Directory.CreateDirectory(physicalPath);
                            }

                            // บันทึกไฟล์
                            string savedFilePath = Path.Combine(physicalPath, savedFileName);
                            FileUploadControl.SaveAs(savedFilePath);

                            // แสดงข้อความสำเร็จ
                            ShowMessage($"อัพโหลดไฟล์ {fileName}{fileExtension} สำเร็จ", true);

                            // ล้าง TextBox
                            TxtFileName.Text = string.Empty;

                            // โหลดข้อมูลไฟล์ใหม่
                            LoadFiles();
                        }
                        else
                        {
                            ShowMessage("ประเภทไฟล์ไม่ได้รับอนุญาต โปรดอัพโหลดไฟล์ PDF, Word, Excel, PowerPoint หรือ ZIP เท่านั้น");
                        }
                    }
                    else
                    {
                        ShowMessage("ไฟล์มีขนาดใหญ่เกินไป ขนาดสูงสุดที่อนุญาตคือ 10MB");
                    }
                }
                else
                {
                    ShowMessage("โปรดเลือกไฟล์ที่ต้องการอัพโหลด");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("เกิดข้อผิดพลาดในการอัพโหลดไฟล์: " + ex.Message);
            }
        }

        /// <summary>
        /// จัดการคำสั่งใน GridView (ดาวน์โหลด/ลบไฟล์)
        /// </summary>
        protected void FilesGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                // รับชื่อไฟล์จาก CommandArgument
                string fileName = e.CommandArgument.ToString();
                string physicalPath = Server.MapPath(uploadFolderPath + fileName);

                // ตรวจสอบคำสั่ง
                if (e.CommandName == "DownloadFile")
                {
                    // ตรวจสอบว่าไฟล์มีอยู่จริง
                    if (File.Exists(physicalPath))
                    {
                        // กำหนด content type ตามนามสกุลไฟล์
                        string contentType;
                        string fileExtension = Path.GetExtension(fileName).ToLower();

                        switch (fileExtension)
                        {
                            case ".pdf":
                                contentType = "application/pdf";
                                break;
                            case ".doc":
                            case ".docx":
                                contentType = "application/msword";
                                break;
                            case ".xls":
                            case ".xlsx":
                                contentType = "application/vnd.ms-excel";
                                break;
                            case ".ppt":
                            case ".pptx":
                                contentType = "application/vnd.ms-powerpoint";
                                break;
                            case ".zip":
                                contentType = "application/zip";
                                break;
                            default:
                                contentType = "application/octet-stream";
                                break;
                        }

                        // ส่งไฟล์ไปยังเบราว์เซอร์
                        Response.Clear();
                        Response.ContentType = contentType;
                        Response.AppendHeader("Content-Disposition", $"attachment; filename={Path.GetFileName(fileName)}");
                        Response.TransmitFile(physicalPath);
                        Response.Flush();
                        Response.End();
                    }
                    else
                    {
                        ShowMessage("ไม่พบไฟล์ที่ต้องการดาวน์โหลด");
                    }
                }
                else if (e.CommandName == "DeleteFile")
                {
                    // ตรวจสอบว่าผู้ใช้มีสิทธิ์ลบไฟล์ (ADMIN เท่านั้น)
                    if (Session["UserType"] != null && Session["UserType"].ToString() == "ADMIN")
                    {
                        // ตรวจสอบว่าไฟล์มีอยู่จริง
                        if (File.Exists(physicalPath))
                        {
                            // ลบไฟล์
                            File.Delete(physicalPath);
                            
                            // แสดงข้อความสำเร็จ
                            ShowMessage("ลบไฟล์สำเร็จ", true);

                            // โหลดข้อมูลไฟล์ใหม่
                            LoadFiles();
                        }
                        else
                        {
                            ShowMessage("ไม่พบไฟล์ที่ต้องการลบ");
                        }
                    }
                    else
                    {
                        ShowMessage("คุณไม่มีสิทธิ์ลบไฟล์");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("เกิดข้อผิดพลาด: " + ex.Message);
            }
        }

        /// <summary>
        /// จัดการการแสดงผลของแต่ละแถวใน GridView
        /// </summary>
        protected void FilesGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    // ตรวจสอบสิทธิ์การลบไฟล์ (ADMIN เท่านั้น)
                    if (Session["UserType"] != null && Session["UserType"].ToString() != "ADMIN")
                    {
                        // ซ่อนปุ่มลบสำหรับผู้ใช้ที่ไม่ใช่ ADMIN
                        LinkButton deleteButton = (LinkButton)e.Row.FindControl("DeleteButton");
                        if (deleteButton != null)
                        {
                            deleteButton.Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FilesGridView_RowDataBound: {ex.Message}");
            }
        }
    }
}
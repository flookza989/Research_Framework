<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Research_Framework.Webpage.Login" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Login</title>

    <!-- CSS -->
    <link rel="stylesheet" href="../Content/bootstrap-sweetalert/sweetalert.min.css">
    <link rel="stylesheet" href="../Content/bootstrap/css/bootstrap.min.css">
    <link rel="stylesheet" href="../Content/font-awesome/css/all.min.css">
    <link rel="stylesheet" href="../Content/css/Site.css">
</head>
<body class="bgLogin">
    <form id="form1" runat="server">
        <div class="containerLogin">
            <div class="contentHead">
                <label>ระบบจัดการวิทยานิพนธ์</label>
            </div>
            <div class="contentBody">
                <div class="text-start">
                    <label class="labelContent">ผู้ใช้งาน</label>
                    <asp:TextBox ID="Tb_user" runat="server"  class="form-control textboxContent" placeholder="รหัสนักศึกษา" required="true"></asp:TextBox>
                </div>
                <div class="mt-2 text-start">
                    <label class="labelContent">รหัสผ่าน</label>
                    <asp:TextBox ID="Tb_pass" runat="server"  class="form-control textboxContent" TextMode="Password" placeholder="รหัสผ่าน" required="true"></asp:TextBox>
                </div>
                <asp:Button ID="Btn_login" runat="server" class="btn buttonNormal mt-2 w-100" Text="เข้าสู่ระบบ" OnClick="Btn_login_Click" />
            </div>
        </div>

        <!-- JavaScript -->
        <script src="../Content/bootstrap-sweetalert/sweetalert.min.js"></script>
        <script src="../Content/bootstrap/js/bootstrap.bundle.min.js"></script>
    </form>
</body>
</html>

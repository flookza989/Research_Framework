<%@ Page Title="" Language="C#" MasterPageFile="~/Webpage/Layout.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="AddReserch.aspx.cs" Inherits="Research_Framework.Webpage.AddReserch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
    </asp:ScriptManager>

    <%-- หัวข้อหลัก --%>
    <div class="content-header">
        <div class="d-flex justify-content-between align-items-center">
            <h2 class="mb-0">
                <i class="fas fa-book-open me-2"></i>
                <asp:Literal ID="LtPageTitle" runat="server">เพิ่มวิทยานิพนธ์</asp:Literal>
            </h2>
            <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="0">
                <ProgressTemplate>
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">กำลังโหลด...</span>
                    </div>
                </ProgressTemplate>
            </asp:UpdateProgress>
        </div>
    </div>

    <div class="row">
        <%-- ข้อมูลวิทยานิพนธ์ --%>
        <div class="col-12">
            <div class="card shadow-sm mb-4">
                <div class="card-header d-flex justify-content-between align-items-center">
                    <h5 class="mb-0">ข้อมูลวิทยานิพนธ์</h5>
                    <asp:Label ID="LbResearchStatus" runat="server" CssClass="badge bg-primary"></asp:Label>
                </div>
                <div class="card-body">
                    <div class="row g-3">
                        <div class="col-md-12">
                            <label class="form-label">ชื่อวิทยานิพนธ์ <span class="text-danger">*</span></label>
                            <asp:TextBox ID="Tb_reserch" runat="server" CssClass="form-control" placeholder="กรุณากรอกชื่อวิทยานิพนธ์"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RfvResearchName" runat="server"
                                ControlToValidate="Tb_reserch"
                                ErrorMessage="กรุณากรอกชื่อวิทยานิพนธ์"
                                CssClass="text-danger"
                                Display="Dynamic"
                                ValidationGroup="SaveResearch">
                            </asp:RequiredFieldValidator>
                        </div>
                        <div class="col-md-12">
                            <label class="form-label">รายละเอียด</label>
                            <asp:TextBox ID="Tb_description" runat="server" CssClass="form-control"
                                TextMode="MultiLine" Rows="3" placeholder="กรุณากรอกรายละเอียด (ถ้ามี)">
                            </asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <%-- อาจารย์ที่ปรึกษา --%>
        <div class="col-md-4">
            <div class="card shadow-sm mb-4">
                <div class="card-header bg-white">
                    <h5 class="card-title mb-0">อาจารย์ที่ปรึกษา</h5>
                </div>
                <div class="card-body text-center">
                    <asp:HiddenField ID="HdnAdvisorId" runat="server" />
                    <asp:Image ID="ImgAdvisor" runat="server" CssClass="rounded-circle mb-3" Width="120" Height="120" />
                    <h5 class="mb-1">
                        <span id="LtAdvisorName" runat="server">เลือกอาจารย์ที่ปรึกษา</span>
                    </h5>
                    <p class="text-muted small mb-3">
                        <span id="LtAdvisorDepartment" runat="server"></span>
                    </p>
                    <button id="btnSelectTeacher" runat="server" type="button" class="btn btn-primary" onclick="showTeacherModal()">
                        <i class="fas fa-user-plus me-2"></i>เลือกอาจารย์ที่ปรึกษา
                    </button>
                </div>
            </div>
        </div>

        <%-- สมาชิกกลุ่ม --%>
        <div class="col-md-8">
            <div class="card shadow-sm mb-4">
                <div class="card-header bg-white d-flex justify-content-between align-items-center">
                    <h5 class="card-title mb-0">สมาชิกกลุ่ม</h5>
                    <button type="button" class="btn btn-primary btn-sm" 
                        onclick="showMemberModal()" 
                        runat="server" 
                        id="BtnAddMember">
                        <i class="fas fa-user-plus me-2"></i>เพิ่มสมาชิก
                    </button>
                </div>
                <div class="card-body">
                    <asp:GridView ID="Dgv_std" runat="server" CssClass="table table-hover" 
                        AutoGenerateColumns="false" OnRowCommand="Dgv_std_RowCommand" EnableEventValidation="false"> 
                        <Columns>
                            <asp:TemplateField ItemStyle-Width="80px">
                                <ItemTemplate>
                                    <asp:Image ID="ImgStudent" runat="server" CssClass="rounded-circle"
                                        ImageUrl='<%# GetStudentImage(Eval("user_id")) %>' Width="50" Height="50" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="stdID" HeaderText="รหัสนักศึกษา" />
                            <asp:BoundField DataField="name" HeaderText="ชื่อ-นามสกุล" />
                            <asp:BoundField DataField="faculty" HeaderText="คณะ" />
                            <asp:BoundField DataField="branch" HeaderText="สาขา" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <div class="btn-group">
                                        <asp:LinkButton ID="BtnRemove" runat="server" 
                                            CommandName="DeleteMember" 
                                            CommandArgument='<%# Eval("id") %>'
                                            CssClass="btn btn-danger btn-sm"
                                            Visible='<%# IsCurrentUserLeader() && !Convert.ToBoolean(Eval("is_leader")) %>'>
                                            <i class="fas fa-user-minus"></i> ลบ
                                        </asp:LinkButton>

                                        <asp:LinkButton ID="BtnTransfer" runat="server" 
                                            CommandName="TransferLeader" 
                                            CommandArgument='<%# Eval("id") %>'
                                            CssClass="btn btn-warning btn-sm ms-2"
                                            Visible='<%# IsCurrentUserLeader() && !Convert.ToBoolean(Eval("is_leader")) %>'>
                                            <i class="fas fa-crown"></i> โอนสิทธิ์หัวหน้ากลุ่ม
                                        </asp:LinkButton>

                                        <asp:Label ID="LblLeader" runat="server" 
                                            CssClass="badge bg-primary ms-2"
                                            Visible='<%# Convert.ToBoolean(Eval("is_leader")) %>'>
                                            <i class="fas fa-crown"></i> หัวหน้ากลุ่ม
                                        </asp:Label>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>

        <%-- ปุ่มดำเนินการ --%>
        <div class="col-12">
            <div class="d-flex justify-content-between">
                <asp:LinkButton ID="BtnSave" runat="server" CssClass="btn btn-primary" OnClick="BtnSave_Click"
                    ValidationGroup="SaveResearch">
                    <i class="fas fa-save me-2"></i>บันทึก
                </asp:LinkButton>
            </div>
        </div>
    </div>

    <%-- Modal เลือกอาจารย์ที่ปรึกษา --%>
    <div class="modal fade" id="teacherModal" tabindex="-1">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">เลือกอาจารย์ที่ปรึกษา</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <div class="input-group">
                            <span class="input-group-text">
                                <i class="fas fa-search"></i>
                            </span>
                            <input type="text" id="teacherSearch" class="form-control"
                                placeholder="ค้นหาจากชื่อหรือสาขา...">
                        </div>
                        <p id="teacherCount" class="text-muted mb-3 mt-2"></p>
                    </div>
                    <div id="teacherList" class="row g-3">
                        <!-- รายการอาจารย์จะถูกเพิ่มที่นี่ด้วย JavaScript -->
                    </div>
                </div>
            </div>
        </div>
    </div>

    <%-- Modal เพิ่มสมาชิก --%>
    <div class="modal fade" id="memberModal" tabindex="-1">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">เพิ่มสมาชิก</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <div class="input-group">
                            <span class="input-group-text">
                                <i class="fas fa-search"></i>
                            </span>
                            <input type="text" id="studentSearch" class="form-control"
                                placeholder="ค้นหาจากรหัสนักศึกษา ชื่อ หรือสาขา...">
                        </div>
                    </div>
                    <div id="studentList" class="row g-3">
                        <!-- รายการนักศึกษาจะถูกเพิ่มที่นี่ด้วย JavaScript -->
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal บังคับสร้างงานวิจัย -->
    <div class="modal fade" id="modalForceResearch" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">เริ่มต้นงานวิจัย</h5>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-12 mb-3">
                            <label class="form-label">ชื่องานวิจัย <span class="text-danger">*</span></label>
                            <input type="text" id="txtResearchName" class="form-control" 
                                   placeholder="กรอกชื่องานวิจัย" required>
                        </div>
                        <div class="col-12 mb-3">
                            <label class="form-label">อาจารย์ที่ปรึกษา <span class="text-danger">*</span></label>
                            <select id="ddlAdvisor" class="form-select" required>
                                <option value="">เลือกอาจารย์ที่ปรึกษา</option>
                            </select>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary" onclick="createInitialResearch()">
                        เริ่มต้นงานวิจัย
                    </button>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        let teacherModal, memberModal;

        $(document).ready(function () {
            teacherModal = new bootstrap.Modal(document.getElementById('teacherModal'));
            memberModal = new bootstrap.Modal(document.getElementById('memberModal'));

            $('#teacherSearch').on('input', _.debounce(function () {
                const keyword = $(this).val();
                if (keyword.length >= 2 || keyword.length === 0) {
                    searchTeachers(keyword);
                }
            }, 300));

            $('#studentSearch').on('input', _.debounce(function () {
                const keyword = $(this).val();
                if (keyword.length >= 2 || keyword.length === 0) {
                    searchStudents(keyword);
                }
            }, 300));

            checkExistingResearch();
        });

        function showMemberModal() {
            $('#studentSearch').val('');
            showLoading('#studentList');
            searchStudents('');
            memberModal.show();
        }

        function searchTeachers(keyword) {
            showLoading('#teacherList');
            $.ajax({
                type: 'POST',
                url: 'AddReserch.aspx/SearchTeachers',
                data: JSON.stringify({ searchText: keyword }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    renderTeacherList(response.d.teachers);
                    $('#teacherCount').text(`พบอาจารย์ทั้งหมด ${response.d.totalCount} คน`);
                },
                error: function () {
                    showError('#teacherList', 'เกิดข้อผิดพลาดในการค้นหา');
                }
            });
        }

        function showTeacherModal() {
            $('#teacherSearch').val('');
            $('#teacherCount').text('');
            showLoading('#teacherList');
            searchTeachers('');
            teacherModal.show();
        }

        function searchStudents(keyword) {
            showLoading('#studentList');
            $.ajax({
                type: 'POST',
                url: 'AddReserch.aspx/SearchStudents',
                data: JSON.stringify({ searchText: keyword }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    renderStudentList(response.d);
                },
                error: function () {
                    showError('#studentList', 'เกิดข้อผิดพลาดในการค้นหา');
                }
            });
        }

        function renderTeacherList(teachers) {
            if (teachers.length === 0) {
                $('#teacherList').html(getEmptyTemplate('ไม่พบข้อมูลอาจารย์'));
                return;
            }

            const html = teachers.map(teacher => `
                <div class="col-md-6">
                    <div class="card member-card h-100" onclick="selectTeacher(${teacher.id})">
                        <div class="card-body d-flex align-items-center">
                            <img src="${teacher.imageUrl}" 
                                class="rounded-circle me-3" width="64" height="64" 
                                alt="${teacher.name}"
                                onerror="this.src='../Images/default-profile.png'">
                            <div>
                                <h6 class="card-title mb-1">${teacher.name}</h6>
                                <p class="card-text small text-muted mb-0">${teacher.department}</p>
                                <div class="text-primary mt-2">คลิกเพื่อเลือก</div>
                            </div>
                        </div>
                    </div>
                </div>
            `).join('');

            $('#teacherList').html(html);
        }

        function renderStudentList(students) {
            if (students.length === 0) {
                $('#studentList').html(getEmptyTemplate('ไม่พบข้อมูลนักศึกษา'));
                return;
            }

            const html = students.map(student => `
                <div class="col-md-6">
                    <div class="card member-card h-100" onclick="addStudent(${student.id})">
                        <div class="card-body d-flex align-items-center">
                            <img src="${student.imageUrl}" 
                                class="rounded-circle me-3" width="64" height="64" 
                                alt="${student.name}"
                                onerror="this.src='../Images/default-profile.png'">
                            <div>
                                <h6 class="card-title mb-1">${student.name}</h6>
                                <p class="card-text small text-muted mb-1">${student.studentId}</p>
                                <p class="card-text small text-muted mb-0">${student.department}</p>
                                <div class="text-primary mt-2">คลิกเพื่อเลือก</div>
                            </div>
                        </div>
                    </div>
                </div>
            `).join('');

            $('#studentList').html(html);
        }

        function selectTeacher(teacherId) {
            showLoadingOverlay();
            $.ajax({
                type: 'POST',
                url: 'AddReserch.aspx/SelectTeacher',
                data: JSON.stringify({ teacherId: teacherId }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    hideLoadingOverlay();
                    if (response.d.success) {
                        updateAdvisorDisplay(response.d);
                        teacherModal.hide();

                        Swal.fire({
                            icon: 'success',
                            title: 'เลือกอาจารย์ที่ปรึกษาสำเร็จ',
                            showConfirmButton: false,
                            timer: 1500
                        });
                    } else {
                        Swal.fire('ข้อผิดพลาด', response.d.message, 'error');
                    }
                },
                error: function () {
                    hideLoadingOverlay();
                    Swal.fire('ข้อผิดพลาด', 'เกิดข้อผิดพลาดในการเลือกอาจารย์ที่ปรึกษา', 'error');
                }
            });
        }

        function addStudent(studentId) {
            showLoadingOverlay();
            $.ajax({
                type: 'POST',
                url: 'AddReserch.aspx/AddStudent',
                data: JSON.stringify({ studentId: studentId }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    hideLoadingOverlay();
                    if (response.d.success) {
                        memberModal.hide();
                        Swal.fire({
                            icon: 'success',
                            title: 'เพิ่มสมาชิกสำเร็จ',
                            text: 'เพิ่มสมาชิกในกลุ่มเรียบร้อยแล้ว',
                            showConfirmButton: false,
                            timer: 1500
                        }).then(() => {
                            window.location.reload();
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'ไม่สามารถเพิ่มสมาชิกได้',
                            text: response.d.message
                        });
                    }
                },
                error: function () {
                    hideLoadingOverlay();
                    Swal.fire({
                        icon: 'error',
                        title: 'เกิดข้อผิดพลาด',
                        text: 'ไม่สามารถเพิ่มสมาชิกได้ กรุณาลองใหม่อีกครั้ง'
                    });
                }
            });
        }


        function updateAdvisorDisplay(data) {
            $('#<%= HdnAdvisorId.ClientID %>').val(data.id);
            $('#<%= ImgAdvisor.ClientID %>').attr('src', data.imageUrl || '../Images/NewUser.png');
            $('#<%= LtAdvisorName.ClientID %>').text(data.name);
            $('#<%= LtAdvisorDepartment.ClientID %>').text(data.department);
        }

        function loadTeachers() {
            $.ajax({
                type: 'POST',
                url: 'AddReserch.aspx/LoadAdvisors',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    var ddlAdvisor = $('#ddlAdvisor');
                    ddlAdvisor.empty();
                    ddlAdvisor.append('<option value="">เลือกอาจารย์ที่ปรึกษา</option>');

                    response.d.forEach(function (teacher) {
                        ddlAdvisor.append(
                            `<option value="${teacher.id}">${teacher.name} - ${teacher.department}</option>`
                        );
                    });
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'เกิดข้อผิดพลาด',
                        text: 'ไม่สามารถโหลดรายชื่ออาจารย์ที่ปรึกษา'
                    });
                }
            });
        }

        function createInitialResearch() {
            var researchName = $('#txtResearchName').val().trim();
            var advisorId = $('#ddlAdvisor').val();

            if (!researchName) {
                Swal.fire({
                    icon: 'warning',
                    title: 'แจ้งเตือน',
                    text: 'กรุณากรอกชื่องานวิจัย'
                });
                return;
            }

            if (!advisorId) {
                Swal.fire({
                    icon: 'warning',
                    title: 'แจ้งเตือน',
                    text: 'กรุณาเลือกอาจารย์ที่ปรึกษา'
                });
                return;
            }

            $.ajax({
                type: 'POST',
                url: 'AddReserch.aspx/CreateInitialResearch',
                data: JSON.stringify({
                    researchName: researchName,
                    advisorId: parseInt(advisorId)
                }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.d.success) {
                        Swal.fire({
                            icon: 'success',
                            title: 'สร้างงานวิจัยสำเร็จ',
                            text: 'คุณสามารถเริ่มทำงานวิจัยได้แล้ว',
                            confirmButtonText: 'ตกลง'
                        }).then(() => {
                            $('#modalForceResearch').modal('hide');
                            location.reload();
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'เกิดข้อผิดพลาด',
                            text: response.d.message
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'เกิดข้อผิดพลาด',
                        text: 'ไม่สามารถสร้างงานวิจัยได้'
                    });
                }
            });
        }

        function showLoading(selector) {
            $(selector).html(`
                <div class="col-12 text-center py-5">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">กำลังโหลด...</span>
                    </div>
                </div>
            `);
        }

        function showError(selector, message) {
            $(selector).html(`
                <div class="col-12 text-center py-5">
                    <div class="text-danger">
                        <i class="fas fa-exclamation-circle fa-3x mb-3"></i>
                        <p>${message}</p>
                    </div>
                </div>
            `);
        }

        function getEmptyTemplate(message) {
            return `
                <div class="col-12">
                    <div class="empty-state">
                        <i class="fas fa-search fa-3x mb-3"></i>
                        <p>${message}</p>
                    </div>
                </div>
            `;
        }

        function showLoadingOverlay() {
            if (!document.querySelector('.loading-overlay')) {
                const overlay = document.createElement('div');
                overlay.className = 'loading-overlay';
                overlay.innerHTML = `
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">กำลังโหลด...</span>
                    </div>
                `;
                document.body.appendChild(overlay);
            }
        }

        function hideLoadingOverlay() {
            const overlay = document.querySelector('.loading-overlay');
            if (overlay) {
                overlay.remove();
            }
        }

        function checkExistingResearch() {
            const hasResearch = $('#<%= Tb_reserch.ClientID %>').val() !== '';
            if (hasResearch) {
                $('.member-card').off('click');
                $('#teacherSearch').prop('disabled', true);
                $('#studentSearch').prop('disabled', true);
            }
        }
    </script>
</asp:Content>
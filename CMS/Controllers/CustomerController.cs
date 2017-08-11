using CMS.Bussiness;
using CMS.Data;
using CMS.Helper;
using CMS.ViewModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBackendPlus.Controllers;
using static CMS.Common.Common;

namespace CMS.Controllers
{
    public class CustomerController : BaseAuthedController
    {
        // GET: User
        public ActionResult Index()
        {
            UserViewModel model = new UserViewModel();
            var managerList = new UserBussiness().GetManagerUser();
            managerList.Add(new SelectListItem { Text = "Tất cả", Value = "0" });
            model.ManagerList = managerList.OrderBy(x => x.Value).ToList();
            model.ManagerId = int.Parse(Session["SS-USERID"].ToString()) == 1 ? 0 : int.Parse(Session["SS-USERID"].ToString());
            model.Totalpage = 2;
            //get status payment
            model.PaymentStatus = getPaymentStatus();
            model.StatusId = 0;
            return View(model);
        }

        #region Json
        public JsonResult GetHistoryPayment(int UserId, int Page)
        {
            try
            {
                var itemList = (from a in new PaymentBussiness().GetPaymentHistoryByUserId(UserId, Page)
                                select new PaymentHistoryModel
                                {
                                    Id = a.Id,
                                    PaymentMethod = new PaymentBussiness().GetPaymentMethodById(a.PaymentMethodId),
                                    DateString = a.CreatedDate.ToString("dd/MM/yyyy"),
                                    Amount = string.Format("{0:n0}", a.Amount),
                                    Notes = a.Notes
                                }).ToList();
                var content = RenderPartialViewToString("~/Views/Customer/PaymentDetail.cshtml", itemList);
                return Json(new
                {
                    Content = content,
                    UserId = UserId
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    Content = ""
                }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult LoadMoreHistory(int UserId, int Page)
        {
            try
            {
                var itemList = getHistoryPayment(UserId, Page);
                return Json(itemList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult CustomerDetail(int id)
        {
            try
            {
                CustomerDetail cusDetail = new CustomerDetail();
                cusDetail = getCustomerDetail(id);
                var content = RenderPartialViewToString("~/Views/Customer/DetailInformation.cshtml", cusDetail);
                return Json(new
                {
                    Content = content
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Content = ""
                }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public JsonResult LoadData(string search, string pageIndex, int managerId, int statusId)
        {
            try
            {
                int totalpage = 0;
                UserViewModel model = new UserViewModel();
                model.UserList = getCustomerList(ref totalpage, int.Parse(pageIndex), 200, search, managerId, statusId);
                var content = RenderPartialViewToString("~/Views/Customer/CustomerDetail.cshtml", model.UserList);
                model.Totalpage = totalpage;
                var totalPayment = new PaymentBussiness().GetAllCashPaymentByAdmin(managerId);
                return Json(new
                {
                    TotalPage = model.Totalpage,
                    TotalPayment = totalPayment,
                    Content = content
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    TotalPage = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region ActionResult
        public ActionResult Edit(int id)
        {
            UserModel model = new UserModel();
            try
            {

                //get user by userId
                var user = new UserBussiness().GetUserById(id);
                if (user.ManagerBy == null || int.Parse(Session["SS-USERID"].ToString()) == 1 || user.ManagerBy == int.Parse(Session["SS-USERID"].ToString()))
                {
                    model.Id = user.Id;
                    model.UserName = user.UserName;
                    model.FullName = user.FullName;
                    model.Notes = user.Notes;
                    model.IsMember = user.IsMember ?? false;
                    model.IsRestore = user.IsDeleted ?? false;
                    model.ManagerBy = user.ManagerBy + "";
                    model.RoleUsers = new RoleBussiness().GetListByUserId(id);

                    //get dropdownlist and list role
                    var allRoles = new RoleBussiness().GetRolesByAdmin(int.Parse(Session["SS-USERID"].ToString()));
                    model.ManagerList = new UserBussiness().GetManagerUser();
                    if (allRoles != null)
                    {
                        List<RoleModel> lstRoles = new List<RoleModel>();
                        foreach (var role in allRoles)
                        {
                            RoleModel roleView = new RoleModel
                            {
                                Id = role.Id,
                                Name = role.Name
                            };
                            if (model.RoleUsers.FirstOrDefault(r => r.RoleId == roleView.Id) != null)
                            {
                                roleView.IsChecked = true;
                            }
                            lstRoles.Add(roleView);
                        }
                        model.ListRoles = lstRoles;
                    }
                    return View(model);
                }
                else
                {
                    TempData["Error"] = "Bạn không quản lý tài khoản này. vui lòng thử lại.";
                    return RedirectToAction("Index", "Customer");
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Bạn không quản lý tài khoản này. vui lòng thử lại.";
                return RedirectToAction("Index", "Customer");
            }
        }

        [HttpPost]
        public ActionResult Edit(UserModel model, string[] selectRoles)
        {
            try
            {
                //check staff is mananger customer
                var user = new UserBussiness().GetUserById(model.Id);
                if (user.ManagerBy == null || int.Parse(Session["SS-USERID"].ToString()) == 1 || user.ManagerBy == int.Parse(Session["SS-USERID"].ToString()))
                {
                    //update ismember and delete
                    new UserBussiness().Update(model);

                    //delte all role in role_users
                    new RoleBussiness().DeleteRoleUserByUserId(model.Id);

                    //insert role to role_user
                    List<Role_User> lstRoleUser = new List<Role_User>();
                    if (selectRoles != null)
                    {
                        lstRoleUser.AddRange(selectRoles.Select(roleId => new Role_User
                        {
                            UserId = model.Id,
                            RoleId = Convert.ToInt32(roleId)
                        }));
                        // Insert User to Roles
                        try
                        {
                            foreach (var item in lstRoleUser)
                            {
                                new RoleUserBussiness().Insert(item);
                            }
                        }
                        catch (Exception ex)
                        {
                            TempData["Error"] = ex;
                            return RedirectToAction("Index", "Customer");
                        }
                    }

                    TempData["Success"] = Messages_Contants.SUCCESS_UPDATE;
                }
                else
                {
                    TempData["Error"] = "Bạn không quản lý tài khoản này. vui lòng thử lại.";
                    return RedirectToAction("Index", "Customer");
                }
            }
            catch (Exception)
            {
                TempData["Error"] = Messages_Contants.ERROR_COMMON;
                return RedirectToAction("Index", "Customer");
            }

            return RedirectToAction("Index", "Customer");
        }

        public ActionResult ResertPassword(string id)
        {
            try
            {
                new UserBussiness().ResetPassword(int.Parse(id));
                TempData["Success"] = Messages_Contants.SUCCESS_RESETPASSWORD;
            }
            catch (Exception)
            {
                TempData["Error"] = Messages_Contants.ERROR_COMMON;
            }
            return RedirectToAction("Index", "Customer");
        }

        public ActionResult RemoveUser(string id)
        {
            try
            {
                int userId = int.Parse(id);
                if (userId != 1)
                {
                    new UserBussiness().Delete(userId);
                    TempData["Success"] = Messages_Contants.SUCCESS_DELETE;
                }
                else TempData["Error"] = "Bạn không thể xóa người dùng này.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex;
            }

            return RedirectToAction("Index", "Customer");
        }

        public ActionResult DetailInformation()
        {
            return View();
        }
        #endregion

        #region ExportExcel

        public ActionResult ExportExcel(int userId, int page)
        {
            string fileName = string.Format("User_{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            string filePath = Path.Combine(Request.PhysicalApplicationPath, "File\\ExportImport", fileName);
            var folder = Request.PhysicalApplicationPath + "File\\ExportImport";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var customerDetail = getCustomerDetail(userId);
            List<PaymentHistoryModel> paymentHistory = new List<PaymentHistoryModel>();
            for (int i = 0; i < page; i++)
            {
                var rs = getHistoryPayment(userId, i);
                paymentHistory.AddRange(rs);
            }
            ExportToExcel(filePath, customerDetail, paymentHistory);

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "text/xls", fileName);
        }

        public ActionResult ExportExcelAllCustomer(string listCustomerId)
        {
            string fileName = string.Format("Customer_{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            string filePath = Path.Combine(Request.PhysicalApplicationPath, "File\\ExportImport", fileName);
            var folder = Request.PhysicalApplicationPath + "File\\ExportImport";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            int userId = Convert.ToInt32(Session["SS-USERID"]);
            var listCustomer = getAllCustomer(listCustomerId);
            ExportToExcelAllCustomers(filePath, listCustomer);

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "text/xls", fileName);
        }
        #endregion

        #region Private funtion
        private List<SelectListItem> getPaymentStatus()
        {
            List<SelectListItem> status = new List<SelectListItem>();
            status.Add(new SelectListItem { Text = "Tất cả", Value = "0" });
            status.Add(new SelectListItem { Text = "Đang hoạt động", Value = "1" });
            status.Add(new SelectListItem { Text = "Sắp hết hạn", Value = "2" });
            status.Add(new SelectListItem { Text = "Đã hết hạn", Value = "3" });
            status.Add(new SelectListItem { Text = "Chưa có gói cước", Value = "4" });
            return status;
        }
        private string getNameRole(List<Role> role, List<Role_User> roleUser, int userId)
        {
            var rs = (from r in role
                      join ru in roleUser on r.Id equals ru.RoleId
                      where ru.UserId == userId
                      select r.Name).ToArray();
            return String.Join(", ", rs);
        }

        //get all khách hàng
        private List<UserModel> getCustomerList(ref int pageTotal, int pageIndex, int pageSize, string search, int managerId, int statusId)
        {
            double total = 0;
            var allAdmin = new UserBussiness().GetAdminUser();
            var allUser = new UserBussiness().GetCustomerUser(ref total, ref pageTotal, managerId, statusId, pageIndex, pageSize, search);
            var allRoles = new RoleBussiness().GetRoles();
            var allRolesUser = new RoleUserBussiness().GetAllRoleUser();
            return (from a in allUser
                    select new UserModel
                    {
                        Id = a.Id,
                        FullName = a.FullName,
                        UserName = a.UserName,
                        Phone = a.Phone,
                        Email = a.Email,
                        IsDelete = a.IsDelete,
                        IsMember = a.IsMember,
                        ManagerBy = a.ManagerId != 0 ? allAdmin.Where(x => x.Id == a.ManagerId).Select(x => x.FullName).FirstOrDefault() : "",
                        RoleName = getNameRole(allRoles, allRolesUser, a.Id),
                        IsOnline = checkCustomerOnline(a.Id),
                        EndTimePayment = getPaymentStatus(a.Id)
                    }).OrderBy(x => x.IsOnline ? false : true).ThenBy(x => x.EndTimePayment).ToList();
        }

        private DateTime getPaymentStatus(int userId)
        {
            return new PaymentBussiness().GetEndTimeByUserId(userId);
        }

        private Boolean checkCustomerOnline(int userId)
        {
            // kiểm tra xem tài khoản có đăng nhập ở thiết bị khác hay không
            var currentApp = (List<LoginInfomation>)System.Web.HttpContext.Current.Application["LoginInfomation"];
            var tokenLogin = currentApp.FirstOrDefault(x => x.UserId == userId);

            if (tokenLogin != null)
            {
                if (!string.IsNullOrEmpty(tokenLogin.PrivateKey))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        private CustomerDetail getCustomerDetail(int id)
        {
            // get all admin
            var allAdmin = new UserBussiness().GetAdminUser();

            var cusDetail = new UserBussiness().GetUserById(id);
            var rs = new CustomerDetail();
            rs.UserId = cusDetail.Id;
            rs.UserName = cusDetail.UserName;
            rs.FullName = cusDetail.FullName;
            rs.LastLogin = cusDetail.LastActivityDate ?? DateTime.Now;
            rs.CreateDate = cusDetail.CreatedOn ?? DateTime.Now;
            rs.ManagerBy = cusDetail.ManagerBy != null ? allAdmin.Where(x => x.Id == cusDetail.ManagerBy).Select(x => x.FullName).FirstOrDefault() : "";
            rs.Notes = cusDetail.Notes;
            //get paymen by Id
            rs.Amount = new PaymentBussiness().GetCashPaymentByUserId(cusDetail.Id);
            rs.TimeEnd = new PaymentBussiness().GetTimePaymentByUserId(cusDetail.Id);
            //get payment by Id 
            var payment = new PaymentBussiness().GetPaymentByUserId(cusDetail.Id);
            rs.CashPayment = payment.FirstOrDefault(x => x.PaymentMethodId == 1) != null ? payment.FirstOrDefault(x => x.PaymentMethodId == 1).AmoutPayment : "0";
            rs.CardPayment = payment.FirstOrDefault(x => x.PaymentMethodId == 2) != null ? payment.FirstOrDefault(x => x.PaymentMethodId == 2).AmoutPayment : "0";
            return rs;
        }

        private List<UserModel> getAllCustomer(string listCustomer)
        {
            var allAdmin = new UserBussiness().GetAdminUser();
            var listCus = new UserBussiness().GetCustomerByListUserId(listCustomer);
            var allRoles = new RoleBussiness().GetRoles();
            var allRolesUser = new RoleUserBussiness().GetAllRoleUser();
            var rs = (from a in listCus
                      select new UserModel
                      {
                          Id = a.Id,
                          FullName = a.FullName,
                          UserName = a.UserName,
                          Phone = a.Phone,
                          Email = a.Email,
                          IsDelete = a.IsDelete,
                          IsMember = a.IsMember,
                          ManagerBy = a.ManagerBy != null ? allAdmin.Where(x => x.Id == a.ManagerId).Select(x => x.FullName).FirstOrDefault() : "",
                          RoleName = getNameRole(allRoles, allRolesUser, a.Id),
                          EndTimePayment = getPaymentStatus(a.Id)
                      }).OrderBy(x => x.EndTimePayment).ToList();
            return rs;
        }

        private List<PaymentHistoryModel> getHistoryPayment(int userId, int page)
        {
            return (from a in new PaymentBussiness().GetPaymentHistoryByUserId(userId, page)
                    select new PaymentHistoryModel
                    {
                        Id = a.Id,
                        PaymentMethod = new PaymentBussiness().GetPaymentMethodById(a.PaymentMethodId),
                        DateString = a.CreatedDate.ToString("dd/MM/yyyy"),
                        Amount = string.Format("{0:n0}", a.Amount),
                        Notes = a.Notes
                    }).ToList();
        }

        public virtual void ExportToExcel(string filePath, CustomerDetail cusDetail, List<PaymentHistoryModel> paymentHistory)
        {
            var newFile = new FileInfo(filePath);

            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(newFile))
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Chi tiết khách hàng");
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;

                //Create Headers customer Detail
                var headerCustomer = new string[]
                    {
                        "Họ và tên",
                        "Số ĐT",
                        "Nhân viên theo dõi",
                        "Đăng nhập gần nhất",
                        "Hết hạn",
                        "Số dư",
                        "Tiền mặt (T "+DateTime.Now.AddMonths(-1).ToString("MM/yyyy")+")",
                        "Tiền CK (T "+DateTime.Now.AddMonths(-1).ToString("MM/yyyy")+")"
                    };

                var Detail = new string[]
                {
                    cusDetail.FullName,
                    cusDetail.UserName,
                    cusDetail.ManagerBy,
                    cusDetail.LastLogin.ToString("dd/MM/yyyy"),
                    cusDetail.TimeEnd,
                    cusDetail.Amount,
                    cusDetail.CashPayment,
                    cusDetail.CardPayment
                };
                for (var i = 0; i < 4; i++)
                {
                    worksheet.Cells[i + 1, 1].Value = headerCustomer[i];
                    worksheet.Cells[i + 1, 2].Value = Detail[i];
                    worksheet.Cells[i + 1, 1].Style.Font.Bold = true;

                    worksheet.Cells[i + 1, 6].Value = headerCustomer[i + 4];
                    worksheet.Cells[i + 1, 7].Value = Detail[i + 4];
                    worksheet.Cells[i + 1, 6].Style.Font.Bold = true;
                }

                //create header and full history payment
                var historyPayment = new string[]
                    {
                        "STT",
                        "Ngày giao dịch",
                        "Loại giao dịch",
                        "Số tiền giao dịch",
                        "Ghi chú"
                    };
                for (var i = 0; i < historyPayment.Length; i++)
                {
                    worksheet.Cells[6, i + 1].Value = historyPayment[i];
                    worksheet.Cells[6, i + 1].Style.Font.Bold = true;
                }
                var row = 7;
                var dem = 0;
                foreach (var item in paymentHistory)
                {
                    dem++;
                    int col = 1;

                    worksheet.Cells[row, col].Value = dem;
                    col++;

                    worksheet.Cells[row, col].Value = item.DateString;
                    col++;

                    worksheet.Cells[row, col].Value = item.PaymentMethod;
                    col++;

                    worksheet.Cells[row, col].Value = item.Amount;
                    col++;

                    worksheet.Cells[row, col].Value = item.Notes;
                    col++;
                    //next row
                    row++;
                }


                var nameexcel = "Chi tiết khách hàng" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                xlPackage.Workbook.Properties.Title = string.Format("{0}", nameexcel);
                xlPackage.Workbook.Properties.Author = "Admin-IT";
                xlPackage.Workbook.Properties.Subject = string.Format("{0} User", "");
                xlPackage.Workbook.Properties.Category = "User";

                xlPackage.Workbook.Properties.Company = "OZO";
                xlPackage.Save();
            }
        }

        public virtual void ExportToExcelAllCustomers(string filePath, List<UserModel> listCustomer)
        {
            var newFile = new FileInfo(filePath);

            // ok, we can run the real code of the sample now
            using (var xlPackage = new ExcelPackage(newFile))
            {
                // uncomment this line if you want the XML written out to the outputDir
                //xlPackage.DebugMode = true; 

                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Danh sách khách hàng");
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;

                //create header and full history payment
                var CustomerTitle = new string[]
                    {
                        "STT",
                        "Tên đăng nhập",
                        "Tên khách hàng",
                        "Trạng thái",
                        "Hạn sử dụng",
                        "Quản lý bởi",
                        "Nhóm quyền"
                    };
                for (var i = 0; i < CustomerTitle.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = CustomerTitle[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }
                var row = 2;
                var dem = 0;
                foreach (var item in listCustomer)
                {
                    dem++;
                    int col = 1;

                    worksheet.Cells[row, col].Value = dem;
                    col++;

                    worksheet.Cells[row, col].Value = item.UserName;
                    col++;

                    worksheet.Cells[row, col].Value = item.FullName;
                    col++;

                    worksheet.Cells[row, col].Value = item.IsDelete == true ? "TK bị khóa" : item.IsMember == true ? "Kích hoạt" : "Chưa được duyệt";
                    col++;

                    worksheet.Cells[row, col].Value = item.EndTimePayment.Date == DateTime.MinValue.Date ? "Chưa có gói cước" : item.EndTimePayment.ToString("dd/MM/yyyy");
                    col++;

                    worksheet.Cells[row, col].Value = item.ManagerBy;
                    col++;

                    worksheet.Cells[row, col].Value = item.RoleName;
                    col++;
                    //next row
                    row++;
                }


                var nameexcel = "Danh sách khách hàng" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                xlPackage.Workbook.Properties.Title = string.Format("{0}", nameexcel);
                xlPackage.Workbook.Properties.Author = "Admin-IT";
                xlPackage.Workbook.Properties.Subject = string.Format("{0} User", "");
                xlPackage.Workbook.Properties.Category = "User";

                xlPackage.Workbook.Properties.Company = "OZO";
                xlPackage.Save();
            }
        }
        #endregion
    }
}
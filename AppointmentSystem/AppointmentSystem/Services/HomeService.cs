using AppointmentSystem.Models.DBModels;
using AppointmentSystem.Models.ViewModels;
using AppointmentSystem.Models.ViewModels.AppointmentModels;
using AppointmentSystem.Models.ViewModels.BaseInfoModels;
using AppointmentSystem.Models.ViewModels.HomeModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Linq;
using static AppointmentSystem.Models.ViewModels.SelectListModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppointmentSystem.Services
{
    public class HomeService
    {
        private readonly EkasContext _db;
        private readonly WebFunctions _functions;

        public HomeService(EkasContext context)
        {
            _db = context;
            _functions = new WebFunctions(context);
        }

        public bool UserLogin(string account, string password)
        {
            var a = _db.Accounts.Where(x => x.Account1 == account && x.Password == password).FirstOrDefault();

            if (a == null)
                return false;
            else
                return true;
        }

        public Models.ViewModels.HomeModels.IndexVM GetIndexVMData()
        {
            Models.ViewModels.HomeModels.IndexVM indexVM = new Models.ViewModels.HomeModels.IndexVM();

            //取得療程標籤列表
            var LableList = _db.Lables.Where(x => x.Status == "Y" && x.Type == "Treatment").OrderBy(x => x.CreateDate).ToList();

            foreach (var item in LableList)
            {
                var tl = _db.Treatmentlables.AsEnumerable().Where(x => x.Status == "Y" && x.LabelId == item.Id).ToList();
                LableListWithTreatment lableList = new LableListWithTreatment();

                lableList.LableId = item.Id;
                lableList.LableName = item.LableName;
                lableList.TreatmentIdList = "";

                foreach (var treatment in tl)
                {
                    if (lableList.TreatmentIdList != "")
                        lableList.TreatmentIdList += ",";

                    lableList.TreatmentIdList += treatment.TreatmentId;
                }

                indexVM.LableList.Add(lableList);
            }

            //取得療程列表
            var TreatmentList = _db.Treatments.Where(x => x.Status == "Y").OrderBy(x => x.Sort).ToList();

            foreach (var item in TreatmentList)
            {
                TreatmentCheckboxList treatmentCheckboxList = new TreatmentCheckboxList();

                treatmentCheckboxList.TreatmentId = item.Id;
                treatmentCheckboxList.TreatmentName = item.TreatmentName;
                treatmentCheckboxList.IsChecked = "N";

                indexVM.TreatmentList.Add(treatmentCheckboxList);
            }

            return indexVM;
        }

        public IndexAppointmentVM GetIndexAppointmentVMData(string currentYear, string currentMonth, string currentDay)
        {
            IndexAppointmentVM indexAppointmentVM = new IndexAppointmentVM();

            //取得醫師
            var DoctorItems = _db.Doctors.Where(x => x.Status == "Y").OrderBy(x => x.Sort);

            foreach (var item in DoctorItems)
            {
                indexAppointmentVM.DoctorDatas.Add(new DoctorDataVM()
                {
                    DoctorId = item.Id,
                    DoctorName = item.DoctorName,
                    DepartmentTitle = item.DepartmentTitle
                });
            }

            //取得門診時刻表
            var OutpatientItems = _db.Doctoroutpatients.AsEnumerable().Where(x => x.Year == currentYear && x.Month == currentMonth && x.Day == currentDay).Select(x => x.BeginTime).Distinct().OrderBy(x => x);

            foreach (var item in OutpatientItems)
            {
                indexAppointmentVM.OutpatientTimeDatas.Add(new OutpatientTimeData()
                {
                    BeginTime = item
                });
            }

            return indexAppointmentVM;
        }

        public List<AppointmentData> GetAppointmentData(string currentYear, string currentMonth, string currentDay)
        {
            List<AppointmentData> result = new List<AppointmentData>();
            string date = currentYear + "-" + int.Parse(currentMonth).ToString("D2") + "-" + int.Parse(currentDay).ToString("D2");

            var items = _db.Appointments.AsEnumerable().Where(x => x.Status == "Y" && x.Date == date).ToList();

            foreach (var item in items)
            {
                List<TreatmentDataVM> treatmentDataVMs = new List<TreatmentDataVM>();

                var customer = _db.Customers.FirstOrDefault(x => x.Id == item.CustomerId);
                var ats = _db.Appointmenttreatments.Where(x => x.AppointmentId == item.Id && x.Type == "A").ToList();

                foreach (var at in ats)
                {
                    var treatment = _db.Treatments.FirstOrDefault(x => x.Id == at.TreatmentId);
                    TreatmentDataVM td = new TreatmentDataVM();

                    if (treatment != null)
                    {
                        var file = _db.Systemfiles.FirstOrDefault(x => x.Id == treatment.ImageFileId);

                        td.TreatmentId = treatment.Id;
                        td.TreatmentName = treatment.TreatmentName;
                        td.Introduction = treatment.Introduction;
                        td.Time = treatment.Time;
                        td.Image = "data:image/" + file.FileExtension.Replace(".", "") + "; base64," + _functions.ConvertJpgToBase64(file.Path);
                        td.ImageFile = new FileData()
                        {
                            FileID = file.Id,
                            FileName = file.FileName,
                            FileExtension = file.FileExtension,
                            FileSize = file.FileSize,
                            Path = file.Path,
                        };

                        treatmentDataVMs.Add(td);
                    }

                }

                result.Add(new AppointmentData()
                {
                    AppointmentId = item.Id,
                    DoctorId = item.DoctorId,
                    Date = date,
                    BookingBeginTime = item.BookingBeginTime,
                    BookingEndTime = item.BookingEndTime,
                    TimeUnitCount = (TimeSpan.Parse(item.BookingEndTime) - TimeSpan.Parse(item.BookingBeginTime)).TotalMinutes / double.Parse(_functions.GetSystemParameter("MinutesPerUnit")),
                    TreatmentData = treatmentDataVMs,
                    customerData = new CustomerData()
                    {
                        Id = customer.Id,
                        LineId = customer.LineId,
                        LineDiaplayName = customer.LineDisplayName,
                        LinePictureUrl = customer.LinePictureUrl,
                        Name = customer.Name,
                        CellPhone = customer.CellPhone,
                        Email = customer.Email,
                        Birthday = customer.Birthday,
                        Gender = customer.Gender,
                        NationalIdNumber = customer.NationalIdNumber
                    }
                });

                //indexVM.DoctorDatas.Add(new DoctorDataVM()
                //{
                //    DoctorId = item.Id,
                //    DoctorName = item.DoctorName,
                //    DepartmentTitle = item.DepartmentTitle
                //});
            }

            return result;
        }

        public AppointmentData GetAppointmentDetail(string appointmentId)
        {
            //AppointmentData result = new AppointmentData();

            var item = _db.Appointments.FirstOrDefault(x => x.Id == appointmentId);

            //取得實際施作療程列表
            var TreatmentList = _db.Appointmenttreatments.Where(x => x.Status == "Y" && x.AppointmentId == appointmentId && x.Type == "T").ToList();
            List<TreatmentCheckboxList> ActualTreatmentData = new List<TreatmentCheckboxList>();

            foreach (var tl in TreatmentList)
            {
                var td = _db.Treatments.FirstOrDefault(x => x.Id == tl.TreatmentId);

                TreatmentCheckboxList treatmentCheckboxList = new TreatmentCheckboxList();

                treatmentCheckboxList.TreatmentId = tl.TreatmentId;
                treatmentCheckboxList.TreatmentName = td.TreatmentName;

                ActualTreatmentData.Add(treatmentCheckboxList);
            }

            List<TreatmentDataVM> treatmentDataVMs = new List<TreatmentDataVM>();

            var customer = _db.Customers.FirstOrDefault(x => x.Id == item.CustomerId);
            var ats = _db.Appointmenttreatments.Where(x => x.AppointmentId == item.Id && x.Type == "R").ToList();//實際施作項目

            foreach (var at in ats)
            {
                var treatment = _db.Treatments.FirstOrDefault(x => x.Id == at.TreatmentId);
                TreatmentDataVM td = new TreatmentDataVM();

                if (treatment != null)
                {
                    var file = _db.Systemfiles.FirstOrDefault(x => x.Id == treatment.ImageFileId);

                    td.TreatmentId = treatment.Id;
                    td.TreatmentName = treatment.TreatmentName;
                    td.Introduction = treatment.Introduction;
                    td.Time = treatment.Time;
                    td.Image = "data:image/" + file.FileExtension.Replace(".", "") + "; base64," + _functions.ConvertJpgToBase64(file.Path);
                    td.ImageFile = new FileData()
                    {
                        FileID = file.Id,
                        FileName = file.FileName,
                        FileExtension = file.FileExtension,
                        FileSize = file.FileSize,
                        Path = file.Path,
                    };

                    treatmentDataVMs.Add(td);
                }

            }

            DateTime currentDate = DateTime.Now;
            int age = currentDate.Year - DateTime.Parse(customer.Birthday).Year;

            if (currentDate < DateTime.Parse(customer.Birthday).AddYears(age))
                age--;
            DateTime today = DateTime.Now.AddDays(-1);

            int missed = _db.Appointments.AsEnumerable().Where(x => x.CustomerId == customer.Id && x.Status == "Y" && x.CheckIn == "N" && DateTime.Parse(x.Date) < today).Count();

            return new AppointmentData()
            {
                AppointmentId = item.Id,
                DoctorId = item.DoctorId,
                BookingBeginTime = item.BookingBeginTime,
                BookingEndTime = item.BookingEndTime,
                TimeUnitCount = (TimeSpan.Parse(item.BookingEndTime) - TimeSpan.Parse(item.BookingBeginTime)).TotalMinutes / double.Parse(_functions.GetSystemParameter("MinutesPerUnit")),
                TreatmentData = treatmentDataVMs,
                ActualTreatmentData= ActualTreatmentData,
                CheckIn = item.CheckIn,
                CheckInTime = item.CheckInTime,
                customerData = new CustomerData()
                {
                    Id = customer.Id,
                    LineId = customer.LineId,
                    LineDiaplayName = customer.LineDisplayName,
                    LinePictureUrl = customer.LinePictureUrl,
                    Name = customer.Name,
                    CellPhone = customer.CellPhone,
                    Email = customer.Email,
                    Birthday = customer.Birthday,
                    Age = age,
                    missed = missed,
                    Gender = customer.Gender,
                    NationalIdNumber = customer.NationalIdNumber
                }
            };
        }

        public void AppointmentCheckIn(string appointmentId, string account, Appointment value)
        {
            _db.Appointments.FirstOrDefault(x => x.Id == appointmentId).Modifier = account;
            _db.Appointments.FirstOrDefault(x => x.Id == appointmentId).ModifyDate = DateTime.Now;
            _db.Appointments.FirstOrDefault(x => x.Id == appointmentId).CheckIn = value.CheckIn;

            _db.SaveChanges();

        }

        public void AppointmentCancel(string appointmentId, string account, Appointment value)
        {
            _db.Appointments.FirstOrDefault(x => x.Id == appointmentId).Modifier = account;
            _db.Appointments.FirstOrDefault(x => x.Id == appointmentId).ModifyDate = DateTime.Now;
            _db.Appointments.FirstOrDefault(x => x.Id == appointmentId).Status = value.Status;

            _db.SaveChanges();

        }

        public void RemoveActualAppointmentTreatment(string? AppointmentId)
        {
            _db.Appointmenttreatments.RemoveRange(_db.Appointmenttreatments.Where(x => x.AppointmentId == AppointmentId && x.Type == "T"));

            _db.SaveChanges();
        }

        public void CreateAppointmenttreatment(Appointmenttreatment value)
        {
            _db.Appointmenttreatments.Add(value);
            _db.SaveChanges();
        }



        public User GetUserDate(string account)
        {
            var userid = _db.Accounts.Where(x => x.Account1 == account).FirstOrDefault().UserId;
            var user = _db.Users.Where(x => x.Id == userid).FirstOrDefault();

            return user;
        }

        //public string GetAdminName()
        //{
        //    var user = _db.SysParameters.Where(x => x.Name == "AdminName").FirstOrDefault();

        //    if (user == null)
        //        return "";
        //    else
        //        return user.Value;
        //}

        //public Boolean CheckAdmin(string password)
        //{
        //    var user = _db.SysParameters.Where(x => x.Name == "AdminPassword" && x.Value == password).FirstOrDefault();

        //    if (user == null)
        //        return false;
        //    else
        //        return true;
        //}

        //public void UpdateAdminPassword(string password)
        //{
        //    _db.SysParameters.FirstOrDefault(x => x.Name == "AdminPassword").Value = password;

        //    _db.SaveChanges();
        //}

        //public void UpdatePassword(string userID, string password)
        //{
        //    _db.Users.FirstOrDefault(x => x.UserId == userID).Modifier = userID;
        //    _db.Users.FirstOrDefault(x => x.UserId == userID).ModiDate = DateTime.Now;

        //    _db.Users.FirstOrDefault(x => x.UserId == userID).Password = password;

        //    _db.SaveChanges();
        //}
    }
}

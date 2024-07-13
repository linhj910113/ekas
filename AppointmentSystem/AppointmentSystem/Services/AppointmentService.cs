using AppointmentSystem.Models.DBModels;
using AppointmentSystem.Models.ViewModels;
using AppointmentSystem.Models.ViewModels.AppointmentModels;
using AppointmentSystem.Models.ViewModels.BaseInfoModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppointmentSystem.Services
{
    public class AppointmentService
    {
        private readonly EkasContext _db;
        private readonly WebFunctions _functions;

        public AppointmentService(EkasContext context)
        {
            _db = context;
            _functions = new WebFunctions(context);
        }

        public bool CheckCustomerExist(string LineId)
        {
            var c = _db.Customers.Where(x => x.LineId == LineId).FirstOrDefault();

            if (c == null)
                return false;
            else
                return true;
        }

        public bool CheckCustomerId(string CustomerId)
        {
            if (_db.Customers.Where(x => x.Id == CustomerId).Count() == 0)
                return false;
            else
                return true;
        }

        public bool CheckAppointmentId(string AppointmentId)
        {
            if (_db.Appointments.Where(x => x.Id == AppointmentId).Count() == 0)
                return false;
            else
                return true;
        }

        public Customer GetCustomerDateById(string CustomerId)
        {
            var customer = _db.Customers.FirstOrDefault(x => x.Id == CustomerId);

            return customer;
        }

        public Customer GetCustomerDateByLineId(string LineId)
        {
            var customer = _db.Customers.FirstOrDefault(x => x.LineId == LineId);

            return customer;
        }

        public int GetCustomerAppointmentCount(string CustomerId)
        {
            return _db.Appointments.Where(x => x.CustomerId == CustomerId && x.Status == "Y").Count();
        }

        public int CheckFirstAppointmentWithoutSelf(string CustomerId, string AppointmentId, string date)
        {
            var appointment = _db.Appointments.AsEnumerable().Where(x => x.CustomerId == CustomerId && x.Status == "Y" && DateTime.Parse(x.Date) < DateTime.Parse(date)).ToList();

            if (AppointmentId is not null && AppointmentId != "")
                appointment = appointment.AsEnumerable().Where(x => x.Id != AppointmentId).ToList();

            return appointment.Count();
        }

        public IndexVM GetCustomerAppointmentData(string CustomerId)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");

            IndexVM result = new IndexVM();
            var appointments = _db.Appointments.AsEnumerable().Where(x => x.CustomerId == CustomerId && x.Status == "Y" && DateTime.Parse(x.Date) >= DateTime.Parse(date) && x.CheckIn == "N").OrderBy(x => x.Date).ThenBy(x => x.BookingBeginTime).ToList();

            foreach (var appointment in appointments)
            {
                AppointmentData appointmentData = new AppointmentData();

                appointmentData.AppointmentId = appointment.Id;
                appointmentData.Date = appointment.Date;
                appointmentData.DoctorName = _db.Doctors.FirstOrDefault(x => x.Id == appointment.DoctorId).DoctorName!;
                appointmentData.BookingBeginTime = appointment.BookingBeginTime;

                var ats = _db.Appointmenttreatments.Where(x => x.AppointmentId == appointment.Id && x.Type == "A").ToList();

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

                        appointmentData.TreatmentData.Add(td);
                    }

                }

                result.AppointmentData.Add(appointmentData);
            }

            return result;
        }

        public EditVM GetAppointmentDataById(string AppointmentId)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");

            EditVM result = new EditVM();
            var appointment = _db.Appointments.AsEnumerable().FirstOrDefault(x => x.Id == AppointmentId);

            //確認是否為第一次預約(Count=0為第一次)
            int AppointmentCount = CheckFirstAppointmentWithoutSelf(appointment.CustomerId, AppointmentId, date);
            //取得新客填寫資料時間
            int FillinTime = int.Parse(_functions.GetSystemParameter("NewCustomerFillInInformationTime"));

            result.AppointmentId = appointment.Id;
            result.Date = appointment.Date;
            result.DoctorId = appointment.DoctorId;
            result.BookingBeginTime = appointment.BookingBeginTime;
            result.TreatmentList = GetTreatmentList();

            var ats = _db.Appointmenttreatments.Where(x => x.AppointmentId == appointment.Id && x.Type == "A").ToList();

            foreach (var at in ats)
            {
                var treatment = _db.Treatments.FirstOrDefault(x => x.Id == at.TreatmentId);

                if (treatment != null)
                {
                    result.SelectedTreatment.Add(treatment.Id);
                }
            }

            result.DoctorList = GetDoctorList(result.SelectedTreatment.ToArray());

            //設定時間清單
            int Year = DateTime.Parse(appointment.Date).Year;
            int Month = DateTime.Parse(appointment.Date).Month;
            int Day = DateTime.Parse(appointment.Date).Day;
            int MaxTreatmentTime = GetTreatmentListMaxTime(result.SelectedTreatment.ToArray());

            List<OutpatientTimeData> outpatientTimeData = new List<OutpatientTimeData>();
            var OutpatientTimes = _db.Doctoroutpatients.AsEnumerable().Where(
                x => x.Status != "N" && 
                x.DoctorId == appointment.DoctorId && 
                int.Parse(x.Year) == Year && 
                int.Parse(x.Month) == Month && 
                int.Parse(x.Day) == Day
            ).OrderBy(x => TimeSpan.Parse(x.BeginTime)).ToList();

            foreach (var times in OutpatientTimes)
            {
                //判斷該時段是否可使用
                string enabled = "Y";
                bool flag = false;
                string BookingBeginTime = times.BeginTime;
                string BookingEndTime = TimeSpan.Parse(times.BeginTime).Add(new TimeSpan(0, MaxTreatmentTime, 0)).ToString();

                if (AppointmentCount == 0)
                {
                    BookingBeginTime = DateTime.ParseExact(TimeSpan.Parse(BookingBeginTime).Add(new TimeSpan(0, FillinTime, 0)).ToString(), "HH:mm:ss", null).ToString("HH:mm");
                    BookingEndTime = DateTime.ParseExact(TimeSpan.Parse(BookingEndTime).Add(new TimeSpan(0, FillinTime, 0)).ToString(), "HH:mm:ss", null).ToString("HH:mm");
                }

                var dt = _db.Doctoroutpatients.AsEnumerable().Where(
                    x => x.Status == "Y" &&
                    x.AppointmentId == "" &&
                    x.DoctorId == appointment.DoctorId &&
                    int.Parse(x.Year) == Year &&
                    int.Parse(x.Month) == Month &&
                    int.Parse(x.Day) == Day &&
                    TimeSpan.Parse(x.BeginTime) >= TimeSpan.Parse(BookingBeginTime) &&
                    TimeSpan.Parse(x.EndTime) <= TimeSpan.Parse(BookingEndTime)
                ).OrderBy(x => TimeSpan.Parse(x.BeginTime)).ToList();
                double sum = 0;

                foreach (var d in dt)
                {
                    sum += (DateTime.ParseExact(d.EndTime, "HH:mm", null) - DateTime.ParseExact(d.BeginTime, "HH:mm", null)).TotalMinutes;
                }

                if (sum < MaxTreatmentTime)
                    flag = true;

                if (flag)
                    enabled = "N";

                outpatientTimeData.Add(new OutpatientTimeData()
                {
                    BeginTime = DateTime.ParseExact(times.BeginTime, "HH:mm", null).ToString("HH:mm"),
                    EndTime = DateTime.ParseExact(times.EndTime, "HH:mm", null).ToString("HH:mm"),
                    Enabled = enabled
                });

            }

            result.Outpatients = outpatientTimeData;


            return result;
        }

        public CustomerData GetAppointmentCustomerData(string AppointmentId)
        {
            var item = _db.Appointments.FirstOrDefault(x => x.Id == AppointmentId);

            if (item != null)
            {
                var customer = _db.Customers.FirstOrDefault(x => x.Id == item.CustomerId);

                return new CustomerData()
                {
                    Id = customer.Id,
                    LineId = customer.LineId,
                    LineDiaplayName = customer.LineDisplayName,
                    LinePictureUrl = customer.LinePictureUrl,
                    CellPhone = customer.CellPhone,
                    NationalIdNumber = customer.NationalIdNumber,
                    Gender = customer.Gender,
                    Birthday = customer.Birthday,
                    Email = customer.Email
                };
            }
            else
            {
                return null;
            }
        }

        public SuccessPageVM GetAppointmentDataByIdForSuccessPage(string AppointmentId)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");

            SuccessPageVM result = new SuccessPageVM();
            var appointment = _db.Appointments.AsEnumerable().FirstOrDefault(x => x.Id == AppointmentId);

            result.Date = appointment.Date;
            result.BookingBeginTime = appointment.BookingBeginTime;
            result.NotifyMessage = _functions.GetSystemParameter("AppointmentSuccessNotice");

            var ats = _db.Appointmenttreatments.Where(x => x.AppointmentId == appointment.Id && x.Type == "A").ToList();

            foreach (var at in ats)
            {
                var treatment = _db.Treatments.FirstOrDefault(x => x.Id == at.TreatmentId);
                TreatmentDataVM treatmentDataVM = new TreatmentDataVM();

                if (treatment != null)
                {
                    var file = _db.Systemfiles.FirstOrDefault(x => x.Id == treatment.ImageFileId);

                    treatmentDataVM.TreatmentId = treatment.Id;
                    treatmentDataVM.TreatmentName = treatment.TreatmentName;
                    treatmentDataVM.Time = treatment.Time;
                    treatmentDataVM.Introduction = treatment.Introduction;
                    treatmentDataVM.Image = "data:image/" + file.FileExtension.Replace(".", "") + "; base64," + _functions.ConvertJpgToBase64(file.Path);
                    treatmentDataVM.ImageFile = new FileData()
                    {
                        FileID = file.Id,
                        FileName = file.FileName,
                        FileExtension = file.FileExtension,
                        FileSize = file.FileSize,
                        Path = file.Path,
                    };

                    result.SelectedTreatment.Add(treatmentDataVM);
                }
            }

            var doc = _db.Doctors.FirstOrDefault(x => x.Id == appointment.DoctorId);
            if (doc != null)
            {
                var file = _db.Systemfiles.FirstOrDefault(x => x.Id == doc.ImageFileId);

                result.SelectedDoctor = new DoctorDataVM()
                {
                    DoctorId = doc.Id,
                    DoctorName = doc.DoctorName,
                    DepartmentTitle = doc.DepartmentTitle,
                    Introduction = doc.Introduction,
                    Image = "data:image/" + file.FileExtension.Replace(".", "") + "; base64," + _functions.ConvertJpgToBase64(file.Path),
                    ImageFile = new FileData()
                    {
                        FileID = file.Id,
                        FileName = file.FileName,
                        FileExtension = file.FileExtension,
                        FileSize = file.FileSize,
                        Path = file.Path,
                    }
                };
            }

            return result;
        }

        public void CreateCustomer(Customer value)
        {
            _db.Customers.Add(value);
            _db.SaveChanges();
        }

        public void CreateCustomerToken(Customertoken value)
        {
            _db.Customertokens.Add(value);
            _db.SaveChanges();
        }

        public void CreateAppointment(Appointment value)
        {
            _db.Appointments.Add(value);
            _db.SaveChanges();
        }

        public void CreateAppointmenttreatment(Appointmenttreatment value)
        {
            _db.Appointmenttreatments.Add(value);
            _db.SaveChanges();
        }

        public void CreateVerificationcode(Verificationcode value)
        {
            _db.Verificationcodes.Add(value);
            _db.SaveChanges();
        }

        public void UpdateAppointment(Appointment value, string AppointmentId)
        {
            _db.Appointments.FirstOrDefault(x => x.Id == AppointmentId).Modifier = value.Modifier;
            _db.Appointments.FirstOrDefault(x => x.Id == AppointmentId).ModifyDate = value.ModifyDate;

            _db.Appointments.FirstOrDefault(x => x.Id == AppointmentId).DoctorId = value.DoctorId;
            _db.Appointments.FirstOrDefault(x => x.Id == AppointmentId).Date = value.Date;
            _db.Appointments.FirstOrDefault(x => x.Id == AppointmentId).BookingBeginTime = value.BookingBeginTime;
            _db.Appointments.FirstOrDefault(x => x.Id == AppointmentId).BookingEndTime = value.BookingEndTime;

            _db.SaveChanges();
        }

        public void UpdateCustomerLineInformation(Customer value, string LineId)
        {
            _db.Customers.FirstOrDefault(x => x.LineId == LineId).Modifier = value.Modifier;
            _db.Customers.FirstOrDefault(x => x.LineId == LineId).ModifyDate = value.ModifyDate;

            _db.Customers.FirstOrDefault(x => x.LineId == LineId).LineDisplayName = value.LineDisplayName;
            _db.Customers.FirstOrDefault(x => x.LineId == LineId).LinePictureUrl = value.LinePictureUrl;

            _db.SaveChanges();
        }

        public string UpdateCustomer(string customerId, Customer value)
        {
            try
            {
                _db.Customers.FirstOrDefault(x => x.Id == customerId).Modifier = value.Modifier;
                _db.Customers.FirstOrDefault(x => x.Id == customerId).ModifyDate = value.ModifyDate;

                _db.Customers.FirstOrDefault(x => x.Id == customerId).Name = value.Name;
                _db.Customers.FirstOrDefault(x => x.Id == customerId).NationalIdNumber = value.NationalIdNumber;
                _db.Customers.FirstOrDefault(x => x.Id == customerId).CellPhone = value.CellPhone;
                _db.Customers.FirstOrDefault(x => x.Id == customerId).Gender = value.Gender;
                _db.Customers.FirstOrDefault(x => x.Id == customerId).Birthday = value.Birthday;
                _db.Customers.FirstOrDefault(x => x.Id == customerId).Email = value.Email;

                _db.SaveChanges();

                return "success";
            }
            catch
            {
                return "failed";
            }

        }

        public List<TreatmentDataVM> GetTreatmentList()
        {
            var items = _db.Treatments.Where(x => x.Status == "Y" && x.Hide == "N").ToList();
            List<TreatmentDataVM> list = new List<TreatmentDataVM>();

            foreach (var item in items)
            {
                var file = _db.Systemfiles.FirstOrDefault(x => x.Id == item.ImageFileId);

                TreatmentDataVM t = new TreatmentDataVM()
                {
                    TreatmentId = item.Id,
                    TreatmentName = item.TreatmentName,
                    Introduction = item.Introduction,
                    Time = item.Time,
                    Image = "data:image/" + file.FileExtension.Replace(".", "") + "; base64," + _functions.ConvertJpgToBase64(file.Path),
                    ImageFile = new FileData()
                    {
                        FileID = file.Id,
                        FileName = file.FileName,
                        FileExtension = file.FileExtension,
                        FileSize = file.FileSize,
                        Path = file.Path
                    }
                };

                list.Add(t);
            }

            return list;
        }

        public int GetTreatmentListMaxTime(string[] treatments)
        {
            int MaxTime = 0;

            foreach (var treatment in treatments)
            {
                var item = _db.Treatments.FirstOrDefault(x => x.Id == treatment);

                if (item != null)
                {
                    if (item.Time > MaxTime)
                        MaxTime = item.Time;
                }
            }

            return MaxTime;
        }

        public List<DoctorDataVM> GetDoctorList(string[] treatments)
        {
            var items = _db.Doctors.Where(x => x.Status == "Y").ToList();
            List<DoctorDataVM> list = new List<DoctorDataVM>();

            foreach (var item in items)
            {
                bool flag = false;

                foreach (var t in treatments)
                {
                    if (_db.Doctortreatments.FirstOrDefault(x => x.DoctorId == item.Id && x.TreatmentId == t) == null)
                    {
                        flag = true;
                        break;
                    }
                }

                if (flag)
                    continue;

                var file = _db.Systemfiles.FirstOrDefault(x => x.Id == item.ImageFileId);

                DoctorDataVM d = new DoctorDataVM()
                {
                    DoctorId = item.Id,
                    DoctorName = item.DoctorName,
                    Introduction = item.Introduction,
                    DepartmentTitle = item.DepartmentTitle,
                    Image = "data:image/" + file.FileExtension.Replace(".", "") + "; base64," + _functions.ConvertJpgToBase64(file.Path),
                    ImageFile = new FileData()
                    {
                        FileID = file.Id,
                        FileName = file.FileName,
                        FileExtension = file.FileExtension,
                        FileSize = file.FileSize,
                        Path = file.Path
                    }
                };

                list.Add(d);
            }

            return list;
        }

        public List<FillinDateTimeVM> GetFillInDateTimeData(string[] treatments, string doctor, string CustomerId, string appointmentId)
        {
            List<FillinDateTimeVM> result = new List<FillinDateTimeVM>();
            int CurrentYear = DateTime.Now.Year;
            int CurrentMonth = DateTime.Now.Month;
            int CurrentDay = DateTime.Now.Day;
            int MaxTreatmentTime = GetTreatmentListMaxTime(treatments);
            string cd = CurrentYear + "-" + CurrentMonth.ToString("D2") + "-" + CurrentDay.ToString("D2");

            //確認是否為第一次預約(Count=0為第一次)
            int AppointmentCount = CheckFirstAppointmentWithoutSelf(CustomerId, appointmentId, cd);
            //取得新客填寫資料時間
            int FillinTime = int.Parse(_functions.GetSystemParameter("NewCustomerFillInInformationTime"));

            var OutaetientDates = _db.Doctoroutpatients.AsEnumerable().Where(x => x.DoctorId == doctor && int.Parse(x.Year) >= CurrentYear && int.Parse(x.Month) >= CurrentMonth && int.Parse(x.Day) >= CurrentDay).Select(x => new { x.Year, x.Month, x.Day }).Distinct().OrderBy(x => int.Parse(x.Year)).ThenBy(x => int.Parse(x.Month)).ThenBy(x => int.Parse(x.Day)).ToList();
            //var doctoroutaetients = _db.Doctoroutpatients.AsEnumerable().Where(x => x.DoctorId == doctor && int.Parse(x.Year) >= CurrentYear && int.Parse(x.Month) >= CurrentMonth && int.Parse(x.Day) >= CurrentDay).OrderBy(x => int.Parse(x.Year)).ThenBy(x => int.Parse(x.Month)).ThenBy(x => int.Parse(x.Day)).ThenBy(x => TimeSpan.Parse(x.BeginTime)).ToList();

            foreach (var date in OutaetientDates)
            {
                FillinDateTimeVM item = new FillinDateTimeVM();
                List<OutpatientTimeData> outpatientTimeData = new List<OutpatientTimeData>();
                item.Date = date.Year + "-" + int.Parse(date.Month).ToString("D2") + "-" + int.Parse(date.Day).ToString("D2");

                var OutpatientTimes = _db.Doctoroutpatients.AsEnumerable().Where(x => x.Status != "N" && x.DoctorId == doctor && x.Year == date.Year && x.Month == date.Month && x.Day == date.Day).OrderBy(x => TimeSpan.Parse(x.BeginTime)).ToList();

                foreach (var times in OutpatientTimes)
                {
                    //判斷該時段是否可使用
                    string enabled = "Y";
                    bool flag = false;
                    string BookingBeginTime = times.BeginTime;
                    string BookingEndTime = TimeSpan.Parse(times.BeginTime).Add(new TimeSpan(0, MaxTreatmentTime, 0)).ToString();

                    if (AppointmentCount == 0)
                    {
                        BookingBeginTime = DateTime.ParseExact(TimeSpan.Parse(BookingBeginTime).Add(new TimeSpan(0, FillinTime, 0)).ToString(), "HH:mm:ss", null).ToString("HH:mm");
                        BookingEndTime = DateTime.ParseExact(TimeSpan.Parse(BookingEndTime).Add(new TimeSpan(0, FillinTime, 0)).ToString(), "HH:mm:ss", null).ToString("HH:mm");
                    }

                    var dt = _db.Doctoroutpatients.AsEnumerable().Where(x =>
                        x.Status == "Y" &&
                        x.AppointmentId == "" &&
                        x.DoctorId == doctor &&
                        x.Year == date.Year &&
                        x.Month == date.Month &&
                        x.Day == date.Day &&
                        TimeSpan.Parse(x.BeginTime) >= TimeSpan.Parse(BookingBeginTime) &&
                        TimeSpan.Parse(x.EndTime) <= TimeSpan.Parse(BookingEndTime)
                    ).OrderBy(x => TimeSpan.Parse(x.BeginTime)).ToList();
                    double sum = 0;

                    foreach (var d in dt)
                    {
                        sum += (DateTime.ParseExact(d.EndTime, "HH:mm", null) - DateTime.ParseExact(d.BeginTime, "HH:mm", null)).TotalMinutes;
                    }

                    if (sum < MaxTreatmentTime)
                        flag = true;

                    if (flag)
                        enabled = "N";

                    outpatientTimeData.Add(new OutpatientTimeData()
                    {
                        BeginTime = DateTime.ParseExact(times.BeginTime, "HH:mm", null).ToString("HH:mm"),
                        EndTime = DateTime.ParseExact(times.EndTime, "HH:mm", null).ToString("HH:mm"),
                        Enabled = enabled
                    });

                }

                item.Outpatients = outpatientTimeData;
                result.Add(item);
            }

            return result;
        }

        public string CheckVerificationCode(string code)
        {
            DateTime now = DateTime.Now;

            if (_db.Verificationcodes.Where(x => x.SouceTable == "Appointment" && x.HashCode == code && x.ExpireTime >= now).Count() == 0)
                return "";
            else
                return _db.Verificationcodes.FirstOrDefault(x => x.SouceTable == "Appointment" && x.HashCode == code).ForeignKey;
        }

        public void SetAppointmentToOutpatient(string AppointmentId, string UserId, Appointment value)
        {
            //確認是否為第一次預約(Count=0為第一次)
            int AppointmentCount = GetCustomerAppointmentCount(UserId);
            //取得新客填寫資料時間
            int FillinTime = int.Parse(_functions.GetSystemParameter("NewCustomerFillInInformationTime"));

            //設定預約有效
            _db.Appointments.FirstOrDefault(x => x.Id == AppointmentId).Modifier = value.Modifier;
            _db.Appointments.FirstOrDefault(x => x.Id == AppointmentId).ModifyDate = value.ModifyDate;
            _db.Appointments.FirstOrDefault(x => x.Id == AppointmentId).Status = value.Status;
            _db.SaveChanges();

            //將門診時段綁定預約資料
            var AppointmentData = _db.Appointments.FirstOrDefault(x => x.Id == AppointmentId);

            if (AppointmentData != null)
            {
                DateTime dateTime = DateTime.ParseExact(AppointmentData.Date, "yyyy-MM-dd", null);

                int year = dateTime.Year;
                int month = dateTime.Month;
                int day = dateTime.Day;
                string begintime = AppointmentData.BookingBeginTime!;
                string endtime = AppointmentData.BookingEndTime!;

                if (AppointmentCount == 0)
                {
                    begintime = DateTime.ParseExact(TimeSpan.Parse(begintime).Add(new TimeSpan(0, FillinTime, 0)).ToString(), "HH:mm:ss", null).ToString("HH:mm");
                    endtime = DateTime.ParseExact(TimeSpan.Parse(endtime).Add(new TimeSpan(0, FillinTime, 0)).ToString(), "HH:mm:ss", null).ToString("HH:mm");
                }

                _db.Doctoroutpatients.AsEnumerable().Where(x =>
                    x.Year == year.ToString() &&
                    x.Month == month.ToString() &&
                    x.Day == day.ToString() &&
                    DateTime.ParseExact(x.BeginTime, "HH:mm", null) >= DateTime.ParseExact(begintime, "HH:mm", null) &&
                    DateTime.ParseExact(x.EndTime, "HH:mm", null) <= DateTime.ParseExact(endtime, "HH:mm", null)
                ).ToList().ForEach(x =>
                {
                    x.AppointmentId = AppointmentId;
                    x.ModifyDate = DateTime.Now; x.Modifier = UserId;
                });
                _db.SaveChanges();
            }
        }

        public void UpdateAppointmentToOutpatient(string AppointmentId, string UserId, Appointment value)
        {
            //先將原本門診資料刪除
            _db.Doctoroutpatients.AsEnumerable().Where(x => x.AppointmentId == AppointmentId).ToList().ForEach(x =>
                {
                    x.AppointmentId = "";
                    x.ModifyDate = DateTime.Now;
                    x.Modifier = UserId;
                });
            _db.SaveChanges();

            //將門診時段綁定預約資料
            var AppointmentData = _db.Appointments.FirstOrDefault(x => x.Id == AppointmentId);

            //確認是否為第一次預約(Count=0為第一次)
            int AppointmentCount = CheckFirstAppointmentWithoutSelf(UserId, AppointmentId, AppointmentData.Date);
            //取得新客填寫資料時間
            int FillinTime = int.Parse(_functions.GetSystemParameter("NewCustomerFillInInformationTime"));

            if (AppointmentData != null)
            {
                DateTime dateTime = DateTime.ParseExact(AppointmentData.Date, "yyyy-MM-dd", null);

                int year = dateTime.Year;
                int month = dateTime.Month;
                int day = dateTime.Day;
                string begintime = AppointmentData.BookingBeginTime!;
                string endtime = AppointmentData.BookingEndTime!;

                if (AppointmentCount == 0)
                {
                    begintime = DateTime.ParseExact(TimeSpan.Parse(begintime).Add(new TimeSpan(0, FillinTime, 0)).ToString(), "HH:mm:ss", null).ToString("HH:mm");
                    endtime = DateTime.ParseExact(TimeSpan.Parse(endtime).Add(new TimeSpan(0, FillinTime, 0)).ToString(), "HH:mm:ss", null).ToString("HH:mm");
                }

                _db.Doctoroutpatients.AsEnumerable().Where(x =>
                    x.Year == year.ToString() &&
                    x.Month == month.ToString() &&
                    x.Day == day.ToString() &&
                    DateTime.ParseExact(x.BeginTime, "HH:mm", null) >= DateTime.ParseExact(begintime, "HH:mm", null) &&
                    DateTime.ParseExact(x.EndTime, "HH:mm", null) <= DateTime.ParseExact(endtime, "HH:mm", null)
                ).ToList().ForEach(x =>
                {
                    x.AppointmentId = AppointmentId;
                    x.ModifyDate = DateTime.Now; x.Modifier = UserId;
                });
                _db.SaveChanges();
            }
        }

        public void CancelAppointmentAndOutpatient(string AppointmentId, string UserId, Appointment value)
        {
            //設定預約狀態
            _db.Appointments.FirstOrDefault(x => x.Id == AppointmentId).Modifier = value.Modifier;
            _db.Appointments.FirstOrDefault(x => x.Id == AppointmentId).ModifyDate = value.ModifyDate;
            _db.Appointments.FirstOrDefault(x => x.Id == AppointmentId).Status = value.Status;
            _db.SaveChanges();

            //將門診時段綁定預約資料
            var AppointmentData = _db.Appointments.FirstOrDefault(x => x.Id == AppointmentId);

            if (AppointmentData != null)
            {
                _db.Doctoroutpatients.AsEnumerable().Where(x => x.AppointmentId == AppointmentData.Id).ToList().ForEach(x =>
                {
                    x.ModifyDate = DateTime.Now;
                    x.Modifier = UserId;

                    x.AppointmentId = "";
                });

                //DateTime dateTime = DateTime.ParseExact(AppointmentData.Date, "yyyy-MM-dd", null);

                //int year = dateTime.Year;
                //int month = dateTime.Month;
                //int day = dateTime.Day;
                //string begintime = AppointmentData.BookingBeginTime!;
                //string endtime = AppointmentData.BookingEndTime!;

                //_db.Doctoroutpatients.AsEnumerable().Where(x =>
                //    x.Year == year.ToString() &&
                //    x.Month == month.ToString() &&
                //    x.Day == day.ToString() &&
                //    DateTime.ParseExact(x.BeginTime, "HH:mm", null) >= DateTime.ParseExact(begintime, "HH:mm", null) &&
                //    DateTime.ParseExact(x.EndTime, "HH:mm", null) <= DateTime.ParseExact(endtime, "HH:mm", null)
                //).ToList().ForEach(x =>
                //{
                //    x.AppointmentId = AppointmentId;
                //    x.ModifyDate = DateTime.Now; x.Modifier = UserId;
                //});
                _db.SaveChanges();
            }
        }

        public void RemoveAppointmenttreatment(string? AppointmentId)
        {
            _db.Appointmenttreatments.RemoveRange(_db.Appointmenttreatments.Where(x => x.AppointmentId == AppointmentId && x.Type == "A"));

            _db.SaveChanges();
        }

        public Systemfile GetFileData(string fileId)
        {
            return _db.Systemfiles.FirstOrDefault(x => x.Id == fileId);
        }

        //public async Task<bool> SendLineMessageAsync(string to, string message)
        //{
        //    string MessagingApiChannelAccessToken = _functions.GetSystemParameter("MessagingApiChannelAccessToken");

        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", MessagingApiChannelAccessToken);

        //        var payload = new
        //        {
        //            to = to,
        //            messages = new[]
        //            {
        //            new { type = "text", text = message }
        //        }
        //        };

        //        var json = JsonConvert.SerializeObject(payload);
        //        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //        var response = await client.PostAsync("https://api.line.me/v2/bot/message/push", content);

        //        return response.IsSuccessStatusCode;
        //    }
        //}

    }
}

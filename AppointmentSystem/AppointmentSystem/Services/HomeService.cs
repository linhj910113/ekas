using AppointmentSystem.Models.DBModels;
using AppointmentSystem.Models.ViewModels;
using AppointmentSystem.Models.ViewModels.AppointmentModels;
using AppointmentSystem.Models.ViewModels.BaseInfoModels;
using AppointmentSystem.Models.ViewModels.HomeModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
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
            var LabelList = _db.Labels.Where(x => x.Status == "Y" && x.Type == "Treatment").OrderBy(x => x.CreateDate).ToList();

            foreach (var item in LabelList)
            {
                var tl = _db.Treatmentlabels.AsEnumerable().Where(x => x.Status == "Y" && x.LabelId == item.Id).ToList();
                LabelListWithTreatment labelList = new LabelListWithTreatment();

                labelList.LabelId = item.Id;
                labelList.LabelName = item.LabelName;
                labelList.TreatmentIdList = "";

                foreach (var treatment in tl)
                {
                    if (labelList.TreatmentIdList != "")
                        labelList.TreatmentIdList += ",";

                    labelList.TreatmentIdList += treatment.TreatmentId;
                }

                indexVM.LabelList.Add(labelList);
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
                    DepartmentTitle = item.DepartmentTitle,
                    ColorHEX = item.ColorHex
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

                var customer = _db.Customerdata.FirstOrDefault(x => x.Id == item.CustomerId);
                var ats = _db.Appointmenttreatments.Where(x => x.AppointmentId == item.Id && x.Type == "A").ToList();
                var doctor = _db.Doctors.FirstOrDefault(x => x.Id == item.DoctorId);

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
                    Type = item.Type,
                    Date = date,
                    BookingBeginTime = item.BookingBeginTime,
                    BookingEndTime = item.BookingEndTime,
                    CheckIn = item.CheckIn,
                    CheckInTime = item.CheckInTime,
                    TimeUnitCount = (TimeSpan.Parse(item.BookingEndTime) - TimeSpan.Parse(item.BookingBeginTime)).TotalMinutes / double.Parse(_functions.GetSystemParameter("MinutesPerUnit")),
                    TreatmentData = treatmentDataVMs,
                    DoctorData = new DoctorDataVM()
                    {
                        DoctorId = doctor.Id,
                        DoctorName = doctor.DoctorName,
                        DepartmentTitle = doctor.DepartmentTitle,
                        ColorHEX = doctor.ColorHex,
                        Introduction = doctor.Introduction
                    },
                    customerData = new CustomerDataClass()
                    {
                        Id = customer.Id,
                        //LineId = customer.LineId,
                        //DisplayName = customer.DisplayName,
                        //LinePictureUrl = customer.LinePictureUrl,
                        MedicalRecordNumber = customer.MedicalRecordNumber,
                        Name = customer.Name,
                        Cellphone = customer.Cellphone,
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

            var customer = _db.Customerdata.FirstOrDefault(x => x.Id == item.CustomerId);
            //var ats = _db.Appointmenttreatments.Where(x => x.AppointmentId == item.Id && x.Type == "T").ToList();//實際施作項目

            foreach (var at in TreatmentList)
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
            //int age = currentDate.Year - DateTime.Parse(customer.Birthday).Year;

            //if (currentDate < DateTime.Parse(customer.Birthday).AddYears(age))
            //    age--;

            string age_string = "-";

            if (!string.IsNullOrEmpty(customer.Birthday))
            {
                int age = currentDate.Year - DateTime.Parse(customer.Birthday).Year;
                if (currentDate < DateTime.Parse(customer.Birthday).AddYears(age))
                    age--;

                age_string = age.ToString();
            }

            DateTime today = DateTime.Now.AddDays(-1);
            int missed = _db.Appointments.AsEnumerable().Where(x => x.CustomerId == customer.Id && x.Status == "Y" && x.CheckIn == "N" && DateTime.Parse(x.Date) < today).Count();
            var doctor = _db.Doctors.FirstOrDefault(x => x.Id == item.DoctorId);

            return new AppointmentData()
            {
                AppointmentId = item.Id,
                BookingBeginTime = item.BookingBeginTime,
                BookingEndTime = item.BookingEndTime,
                TimeUnitCount = (TimeSpan.Parse(item.BookingEndTime) - TimeSpan.Parse(item.BookingBeginTime)).TotalMinutes / double.Parse(_functions.GetSystemParameter("MinutesPerUnit")),
                TreatmentData = treatmentDataVMs,
                ActualTreatmentData = ActualTreatmentData,
                CheckIn = item.CheckIn,
                CheckInTime = item.CheckInTime,
                DoctorData = new DoctorDataVM()
                {
                    DoctorId = doctor.Id,
                    DoctorName = doctor.DoctorName,
                    DepartmentTitle = doctor.DepartmentTitle,
                    ColorHEX = doctor.ColorHex,
                    Introduction = doctor.Introduction
                },
                customerData = new CustomerDataClass()
                {
                    Id = customer.Id,
                    //LineId = customer.LineId,
                    //DisplayName = customer.DisplayName,
                    //LinePictureUrl = customer.LinePictureUrl,
                    MedicalRecordNumber = customer.MedicalRecordNumber,
                    Name = customer.Name,
                    Cellphone = customer.Cellphone,
                    Email = customer.Email,
                    Birthday = customer.Birthday,
                    Age = age_string,
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
            _db.Appointments.FirstOrDefault(x => x.Id == appointmentId).CheckInTime = value.CheckInTime;

            _db.SaveChanges();

        }

        public void AppointmentCancel(string appointmentId, string account, Appointment value)
        {
            //設定預約狀態
            _db.Appointments.FirstOrDefault(x => x.Id == appointmentId).Modifier = value.Modifier;
            _db.Appointments.FirstOrDefault(x => x.Id == appointmentId).ModifyDate = value.ModifyDate;
            _db.Appointments.FirstOrDefault(x => x.Id == appointmentId).Status = value.Status;
            _db.SaveChanges();

            //將門診時段綁定預約資料
            var AppointmentData = _db.Appointments.FirstOrDefault(x => x.Id == appointmentId);

            if (AppointmentData != null)
            {
                _db.Outpatientappointments.RemoveRange(_db.Outpatientappointments.Where(x => x.AppointmentId == appointmentId));

                //_db.Doctoroutpatients.AsEnumerable().Where(x => x.AppointmentId == AppointmentData.Id).ToList().ForEach(x =>
                //{
                //    x.ModifyDate = DateTime.Now;
                //    x.Modifier = UserId;

                //    x.AppointmentId = "";
                //});

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
            var OutpatientTimes = _db.Doctoroutpatients.AsEnumerable().Where(x => x.Status != "N" && x.DoctorId == appointment.DoctorId && int.Parse(x.Year) == Year && int.Parse(x.Month) == Month && int.Parse(x.Day) == Day).OrderBy(x => TimeSpan.Parse(x.BeginTime)).ToList();

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

                var doctorOutpatients = _db.Doctoroutpatients
                .Where(x => x.Status == "Y" &&
                            x.DoctorId == appointment.DoctorId &&
                            x.Year == Year.ToString() &&
                            x.Month == Month.ToString() &&
                            x.Day == Day.ToString())
                .ToList();

                var outpatientIdsWithAppointments = _db.Outpatientappointments
                    .Select(y => y.OutpatientId)
                    .ToHashSet();

                var dt = doctorOutpatients
                    .Where(x => !outpatientIdsWithAppointments.Contains(x.Id) &&
                                TimeSpan.Parse(x.BeginTime) >= TimeSpan.Parse(BookingBeginTime) &&
                                TimeSpan.Parse(x.EndTime) <= TimeSpan.Parse(BookingEndTime))
                    .OrderBy(x => TimeSpan.Parse(x.BeginTime))
                    .ToList();


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
                        MaxTime = (int)item.Time;
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

        public void RemoveAppointmenttreatment(string? AppointmentId)
        {
            _db.Appointmenttreatments.RemoveRange(_db.Appointmenttreatments.Where(x => x.AppointmentId == AppointmentId && x.Type == "A"));

            _db.SaveChanges();
        }

        public int CheckFirstAppointmentWithoutSelf(string CustomerId, string AppointmentId, string date)
        {
            var appointment = _db.Appointments.AsEnumerable().Where(x => x.CustomerId == CustomerId && x.Status == "Y" && DateTime.Parse(x.Date) < DateTime.Parse(date)).ToList();

            if (AppointmentId is not null && AppointmentId != "")
                appointment = appointment.AsEnumerable().Where(x => x.Id != AppointmentId).ToList();

            return appointment.Count();
        }

        public void UpdateAppointmentToOutpatient(string AppointmentId, string UserId, Appointment value)
        {
            //先將原本門診資料刪除
            _db.Outpatientappointments.RemoveRange(_db.Outpatientappointments.Where(x => x.AppointmentId == AppointmentId));

            //_db.Doctoroutpatients.AsEnumerable().Where().ToList().ForEach(x =>
            //{
            //    x.AppointmentId = "";
            //    x.ModifyDate = DateTime.Now;
            //    x.Modifier = UserId;
            //});
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

                var ops = _db.Doctoroutpatients.AsEnumerable().Where(x =>
                    x.Year == year.ToString() &&
                    x.Month == month.ToString() &&
                    x.Day == day.ToString() &&
                    DateTime.ParseExact(x.BeginTime, "HH:mm", null) >= DateTime.ParseExact(begintime, "HH:mm", null) &&
                    DateTime.ParseExact(x.EndTime, "HH:mm", null) <= DateTime.ParseExact(endtime, "HH:mm", null)
                ).ToList();
                //.ForEach(x =>
                //{
                //    x.AppointmentId = AppointmentId;
                //    x.ModifyDate = DateTime.Now; 
                //    x.Modifier = UserId;
                //});

                foreach (var op in ops)
                {
                    _db.Outpatientappointments.Add(new Outpatientappointment()
                    {
                        CreateDate = DateTime.Now,
                        Creator = UserId,
                        ModifyDate = DateTime.Now,
                        Modifier = UserId,
                        Status = "Y",

                        AppointmentId = AppointmentId,
                        Type = "Appointment",
                        OutpatientId = op.Id,
                    });
                }


                _db.SaveChanges();
            }
        }

        public string EditCustomerInfo(string AppointmentId, string customerMedicalRecordNumber, string customerNationalIdNumber, string customerGender, string customerName, string customerCellphone, string customerBirth, string customerEmail)
        {
            var appointment = _db.Appointments.FirstOrDefault(x => x.Id == AppointmentId);

            if (appointment != null)
            {
                string customerId = appointment.CustomerId;

                _db.Customerdata.FirstOrDefault(x => x.Id == customerId).MedicalRecordNumber = customerMedicalRecordNumber;
                _db.Customerdata.FirstOrDefault(x => x.Id == customerId).Name = customerName;
                _db.Customerdata.FirstOrDefault(x => x.Id == customerId).NationalIdNumber = customerNationalIdNumber;
                _db.Customerdata.FirstOrDefault(x => x.Id == customerId).Gender = customerGender;
                _db.Customerdata.FirstOrDefault(x => x.Id == customerId).Cellphone = customerCellphone;
                _db.Customerdata.FirstOrDefault(x => x.Id == customerId).Birthday = customerBirth;
                _db.Customerdata.FirstOrDefault(x => x.Id == customerId).Email = customerEmail;

                _db.SaveChanges();
            }

            //var ddo = _db.Doctordayoffs.FirstOrDefault(x => x.Index == Index);

            ////修改門診表狀態
            //DateTime d = DateTime.Parse(ddo.Date);
            //int year = d.Year; int month = d.Month; int day = d.Day;

            //var OutpatientTimes = _db.Doctoroutpatients.AsEnumerable().Where(
            //    x => x.Status != "N" &&
            //    x.DoctorId == ddo.DoctorId &&
            //    int.Parse(x.Year) == year &&
            //    int.Parse(x.Month) == month &&
            //    int.Parse(x.Day) == day && x.AppointmentId == ""
            //).OrderBy(x => TimeSpan.Parse(x.BeginTime));

            //foreach (var times in OutpatientTimes)
            //{
            //    if (TimeSpan.Parse(times.BeginTime) < TimeSpan.Parse(ddo.EndTime) && TimeSpan.Parse(times.EndTime) > TimeSpan.Parse(ddo.BeginTime))
            //        times.Status = "Y";
            //}
            //_db.SaveChanges();

            ////設定假單狀態為N(保留原假單)
            //_db.Doctordayoffs.FirstOrDefault(x => x.Index == Index).Status = "N";
            //_db.SaveChanges();

            return "success";
        }

        public User GetUserDate(string account)
        {
            var userid = _db.Accounts.Where(x => x.Account1 == account).FirstOrDefault().UserId;
            var user = _db.Users.Where(x => x.Id == userid).FirstOrDefault();

            return user;
        }

        public int GetCustomerAppointmentCount(string CustomerId)
        {
            return _db.Appointments.Where(x => x.CustomerId == CustomerId && x.Status == "Y").Count();
        }

        public List<CustomerDataClass> getCustomerForIndexSearch(string searchCustomerName, string searchCustomerPhone, string searchCustomerBirth)
        {
            var items = _db.Customerdata.Where(x => x.Status == "Y").ToList();

            if (!string.IsNullOrEmpty(searchCustomerName))
                items = items.Where(x => x.Name.Contains(searchCustomerName)).ToList();
            if (!string.IsNullOrEmpty(searchCustomerPhone))
                items = items.Where(x => x.Cellphone.Contains(searchCustomerPhone)).ToList();
            if (!string.IsNullOrEmpty(searchCustomerBirth))
                items = items.Where(x => x.Birthday == searchCustomerBirth).ToList();

            List<CustomerDataClass> result = new List<CustomerDataClass>();

            foreach (var item in items)
            {
                DateTime currentDate = DateTime.Now;
                string age_string = "-";

                if (!string.IsNullOrEmpty(item.Birthday))
                {
                    int age = currentDate.Year - DateTime.Parse(item.Birthday).Year;
                    if (currentDate < DateTime.Parse(item.Birthday).AddYears(age))
                        age--;

                    age_string = age.ToString();
                }

                DateTime today = DateTime.Now.AddDays(-1);

                int missed = _db.Appointments.AsEnumerable().Where(x => x.CustomerId == item.Id && x.Status == "Y" && x.CheckIn == "N" && DateTime.Parse(x.Date) < today).Count();

                result.Add(new CustomerDataClass()
                {
                    Id = item.Id,
                    MedicalRecordNumber = item.MedicalRecordNumber,
                    Name = item.Name,
                    Cellphone = item.Cellphone,
                    Email = item.Email,
                    Birthday = item.Birthday,
                    Age = age_string,
                    missed = missed,
                    Gender = item.Gender,
                    NationalIdNumber = item.NationalIdNumber,
                    appointmentData = getCustomerAppointment(item.Id)
                });
            }

            return result;
        }

        public CustomerDataClass GetAppointmentCustomerData(string AppointmentId)
        {
            var item = _db.Appointments.FirstOrDefault(x => x.Id == AppointmentId);

            if (item != null)
            {
                CustomerDataClass result = new CustomerDataClass();

                var customer = _db.Customerdata.FirstOrDefault(x => x.Id == item.CustomerId);
                var lineaccount = _db.Customerlineaccounts.FirstOrDefault(x => x.Id == item.CustomerId);

                result.Id = customer.Id;
                result.Cellphone = customer.Cellphone;
                result.NationalIdNumber = customer.NationalIdNumber;
                result.Gender = customer.Gender;
                result.Birthday = customer.Birthday;
                result.Email = customer.Email;

                if (lineaccount != null)
                {
                    result.LineId = lineaccount.LineId;
                    result.DisplayName = lineaccount.DisplayName;
                    result.LinePictureUrl = lineaccount.LinePictureUrl;
                }

                return result;
            }
            else
            {
                return null;
            }
        }

        //public CustomerData GetCustomerData(string CustomerId)
        //{
        //    var customer = _db.Customerdata.FirstOrDefault(x => x.Id == CustomerId);

        //    if (customer != null)
        //    {
        //        return new CustomerData()
        //        {
        //            Id = customer.Id,
        //            LineId = customer.LineId,
        //            DisplayName = customer.DisplayName,
        //            LinePictureUrl = customer.LinePictureUrl,
        //            CellPhone = customer.CellPhone,
        //            NationalIdNumber = customer.NationalIdNumber,
        //            Gender = customer.Gender,
        //            Birthday = customer.Birthday,
        //            Email = customer.Email
        //        };
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public List<AppointmentData> getCustomerAppointment(string id)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            var appointments = _db.Appointments.AsEnumerable().Where(x => x.Status == "Y" && x.CustomerId == id && DateTime.Parse(x.Date) >= DateTime.Parse(date) && x.CheckIn == "N").OrderBy(x => x.Date).ThenBy(x => x.BookingBeginTime).ToList();

            List<AppointmentData> result = new List<AppointmentData>();

            foreach (var appointment in appointments)
            {
                AppointmentData appointmentData = new AppointmentData();
                var doctor = _db.Doctors.FirstOrDefault(x => x.Id == appointment.DoctorId);
                var doctorimagefile = _db.Systemfiles.FirstOrDefault(x => x.Id == doctor.ImageFileId);

                appointmentData.AppointmentId = appointment.Id;
                appointmentData.Date = appointment.Date;
                appointmentData.DoctorData = new DoctorDataVM()
                {
                    DoctorId = doctor.Id,
                    DoctorName = doctor.DoctorName,
                    DepartmentTitle = doctor.DepartmentTitle,
                    ColorHEX = doctor.ColorHex,
                    Introduction = doctor.Introduction,
                    Image = "data:image/" + doctorimagefile.FileExtension.Replace(".", "") + "; base64," + _functions.ConvertJpgToBase64(doctorimagefile.Path)
                };
                appointmentData.BookingBeginTime = appointment.BookingBeginTime;
                appointmentData.BookingEndTime = appointment.BookingEndTime;
                appointmentData.CheckIn = appointment.CheckIn;
                appointmentData.CheckInTime = appointment.CheckInTime;

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

                result.Add(appointmentData);
            }

            return result;
        }

        public List<FillinDateTimeVM> GetFillInDateTimeData(string[] treatments, string doctor, string CustomerId, string appointmentId)
        {
            List<FillinDateTimeVM> result = new List<FillinDateTimeVM>();
            int CurrentYear = DateTime.Now.Year;
            int CurrentMonth = DateTime.Now.Month;
            int CurrentDay = DateTime.Now.Day;
            int MaxTreatmentTime = GetTreatmentListMaxTime(treatments);
            string cd = CurrentYear + "-" + CurrentMonth.ToString("D2") + "-" + CurrentDay.ToString("D2");
            var CurrentDate = new DateTime(CurrentYear, CurrentMonth, CurrentDay);

            //確認是否為第一次預約(Count=0為第一次)
            int AppointmentCount = CheckFirstAppointmentWithoutSelf(CustomerId, appointmentId, cd);
            //取得新客填寫資料時間
            int FillinTime = int.Parse(_functions.GetSystemParameter("NewCustomerFillInInformationTime"));


            var OutaetientDates = _db.Doctoroutpatients.AsEnumerable()
                .Where(x => x.DoctorId == doctor &&
                            new DateTime(int.Parse(x.Year), int.Parse(x.Month), int.Parse(x.Day)) > CurrentDate)
                .Select(x => new
                {
                    x.Year,
                    x.Month,
                    x.Day
                })
                .Distinct()
                .OrderBy(x => new DateTime(int.Parse(x.Year), int.Parse(x.Month), int.Parse(x.Day)))
                .ToList();
            //var OutaetientDates = _db.Doctoroutpatients.AsEnumerable().Where(x => x.DoctorId == doctor && int.Parse(x.Year) >= CurrentYear && int.Parse(x.Month) >= CurrentMonth && int.Parse(x.Day) >= CurrentDay).Select(x => new { x.Year, x.Month, x.Day }).Distinct().OrderBy(x => int.Parse(x.Year)).ThenBy(x => int.Parse(x.Month)).ThenBy(x => int.Parse(x.Day)).ToList();
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

                    if (times.Status != "Y")
                        enabled = "N";
                    //bool flag = false;
                    //string BookingBeginTime = times.BeginTime;
                    //string BookingEndTime = TimeSpan.Parse(times.BeginTime).Add(new TimeSpan(0, MaxTreatmentTime, 0)).ToString();

                    //if (AppointmentCount == 0)
                    //{
                    //    BookingBeginTime = DateTime.ParseExact(TimeSpan.Parse(BookingBeginTime).Add(new TimeSpan(0, FillinTime, 0)).ToString(), "HH:mm:ss", null).ToString("HH:mm");
                    //    BookingEndTime = DateTime.ParseExact(TimeSpan.Parse(BookingEndTime).Add(new TimeSpan(0, FillinTime, 0)).ToString(), "HH:mm:ss", null).ToString("HH:mm");
                    //}

                    //var dt = _db.Doctoroutpatients.Where(x =>
                    //    x.Status == "Y" &&
                    //    !_db.Outpatientappointments.Any(y => y.OutpatientId == x.Id) &&
                    //    x.DoctorId == doctor &&
                    //    x.Year == date.Year &&
                    //    x.Month == date.Month &&
                    //    x.Day == date.Day &&
                    //    string.Compare(x.BeginTime, BookingBeginTime) >= 0 &&
                    //    string.Compare(x.EndTime, BookingEndTime) <= 0
                    //).OrderBy(x => x.BeginTime).ToList();
                    //double sum = 0;

                    //foreach (var d in dt)
                    //{
                    //    sum += (DateTime.ParseExact(d.EndTime, "HH:mm", null) - DateTime.ParseExact(d.BeginTime, "HH:mm", null)).TotalMinutes;
                    //}

                    //if (sum < MaxTreatmentTime)
                    //    flag = true;

                    //if (flag)
                    //    enabled = "N";

                    //if (BookingBeginTime.Contains(":15") || BookingBeginTime.Contains(":45"))
                    //    continue;

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

        public bool CheckAppointmentId(string AppointmentId)
        {
            if (_db.Appointments.Where(x => x.Id == AppointmentId).Count() == 0)
                return false;
            else
                return true;
        }

        public void CreateAppointment(Appointment value)
        {
            _db.Appointments.Add(value);
            _db.SaveChanges();

            //確認是否為第一次預約(Count=0為第一次)
            int AppointmentCount = GetCustomerAppointmentCount(value.CustomerId);
            //取得新客填寫資料時間
            int FillinTime = int.Parse(_functions.GetSystemParameter("NewCustomerFillInInformationTime"));

            //將門診時段綁定預約資料
            var AppointmentData = _db.Appointments.FirstOrDefault(x => x.Id == value.Id);

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

                var ops = _db.Doctoroutpatients.AsEnumerable().Where(x =>
                    x.Year == year.ToString() &&
                    x.Month == month.ToString() &&
                    x.Day == day.ToString() &&
                    DateTime.ParseExact(x.BeginTime, "HH:mm", null) >= DateTime.ParseExact(begintime, "HH:mm", null) &&
                    DateTime.ParseExact(x.EndTime, "HH:mm", null) <= DateTime.ParseExact(endtime, "HH:mm", null)
                ).ToList();

                foreach (var op in ops)
                {
                    _db.Outpatientappointments.Add(new Outpatientappointment()
                    {
                        CreateDate = DateTime.Now,
                        Creator = value.Creator,
                        ModifyDate = DateTime.Now,
                        Modifier = value.Modifier,
                        Status = "Y",

                        AppointmentId = value.Id,
                        Type = "Appointment",
                        OutpatientId = op.Id,
                    });
                }

                _db.SaveChanges();
            }
        }

        public string GetUserRoleName(string userId)
        {
            var user = _db.Users.FirstOrDefault(x => x.Id == userId);

            if (user != null)
            {
                var role = _db.Roles.FirstOrDefault(x => x.Id == user.RoleId);

                if (role != null)
                    return role.RoleName;
                else
                    return null;
            }
            else
                return null;

        }

        public bool CheckCustomerId(string CustomerId)
        {
            if (_db.Customerdata.Where(x => x.Id == CustomerId).Count() == 0)
                return false;
            else
                return true;
        }

        public void CreateCustomer(Customerdatum value)
        {
            _db.Customerdata.Add(value);
            _db.SaveChanges();
        }

        public string getCustomerMedicalRecordNumber(string birthday)
        {
            DateTime date = DateTime.Parse(birthday);
            string formattedDate = date.ToString("MMdd");

            var maxSequenceQuery = _db.Customerdata.AsEnumerable().Where(x => x.MedicalRecordNumber.StartsWith(formattedDate)).Select(x => new { SequenceNumber = int.Parse(x.MedicalRecordNumber.Substring(4, 3)) }).OrderByDescending(x => x.SequenceNumber).FirstOrDefault(); ;
            int newSequenceNumber = (maxSequenceQuery != null ? maxSequenceQuery.SequenceNumber : 0) + 1;

            string newMedicalRecordNumber = formattedDate + newSequenceNumber.ToString("D3");

            return newMedicalRecordNumber;
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

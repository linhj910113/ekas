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

        public bool CheckCustomerLineAccountExist(string LineId)
        {
            var c = _db.Customerlineaccounts.Where(x => x.LineId == LineId).FirstOrDefault();

            if (c == null)
                return false;
            else
                return true;
        }

        public bool CheckCustomerExistByCellphone(string nationalIdNumber, string cellphone)
        {
            var c = _db.Customerdata.Where(x => x.Cellphone == cellphone && x.NationalIdNumber == nationalIdNumber).FirstOrDefault();

            if (c == null)
                return false;
            else
                return true;
        }

        public bool CheckCustomerId(string CustomerId)
        {
            if (_db.Customerdata.Where(x => x.Id == CustomerId).Count() == 0)
                return false;
            else
            {
                if (_db.Customerlineaccounts.Where(x => x.Id == CustomerId).Count() == 0)
                    return false;
                else
                    return true;
            }

        }

        public bool CheckAppointmentId(string AppointmentId)
        {
            if (_db.Appointments.Where(x => x.Id == AppointmentId).Count() == 0)
                return false;
            else
                return true;
        }

        public CustomerDataClass GetCustomerDateById(string CustomerId)
        {
            CustomerDataClass result = new CustomerDataClass();
            var customer = _db.Customerdata.FirstOrDefault(x => x.Id == CustomerId);
            var lineaccount = _db.Customerlineaccounts.FirstOrDefault(x => x.Id == customer.Id);

            result.Name = customer.Name;
            result.NationalIdNumber = customer.NationalIdNumber;
            result.MedicalRecordNumber = customer.MedicalRecordNumber;
            result.Gender = customer.Gender;
            result.Birthday = customer.Birthday;
            result.Cellphone = customer.Cellphone;
            result.Email = customer.Email;

            if (lineaccount != null)
            {
                result.Id = lineaccount.Id;
                result.LineId = lineaccount.LineId;
                result.LinePictureUrl = lineaccount.LinePictureUrl;
                result.DisplayName = lineaccount.DisplayName;
            }

            return result;
        }

        public CustomerDataClass GetCustomerDateByNationalIdNumber(string NationalIdNumber)
        {
            CustomerDataClass result = new CustomerDataClass();
            var customer = _db.Customerdata.FirstOrDefault(x => x.NationalIdNumber == NationalIdNumber);
            if (customer != null)
            {
                var lineaccount = _db.Customerlineaccounts.FirstOrDefault(x => x.Id == customer.Id);

                result.Name = customer.Name;
                result.NationalIdNumber = customer.NationalIdNumber;
                result.MedicalRecordNumber = customer.MedicalRecordNumber;
                result.Gender = customer.Gender;
                result.Birthday = customer.Birthday;
                result.Cellphone = customer.Cellphone;
                result.Email = customer.Email;

                if (lineaccount != null)
                {
                    result.Id = lineaccount.Id;
                    result.LineId = lineaccount.LineId;
                    result.LinePictureUrl = lineaccount.LinePictureUrl;
                    result.DisplayName = lineaccount.DisplayName;
                }

                return result;
            }
            else
            {
                return null;
            }

        }

        public CustomerDataClass GetCustomerDateByLineId(string LineId)
        {
            CustomerDataClass result = new CustomerDataClass();
            var lineaccount = _db.Customerlineaccounts.FirstOrDefault(x => x.LineId == LineId);
            var customer = _db.Customerdata.FirstOrDefault(x => x.Id == lineaccount.Id);

            result.Id = lineaccount.Id;
            result.LineId = lineaccount.LineId;
            result.LinePictureUrl = lineaccount.LinePictureUrl;
            result.DisplayName = lineaccount.DisplayName;

            if (customer != null)
            {
                result.Name = customer.Name;
                result.NationalIdNumber = customer.NationalIdNumber;
                result.MedicalRecordNumber = customer.MedicalRecordNumber;
                result.Gender = customer.Gender;
                result.Birthday = customer.Birthday;
                result.Cellphone = customer.Cellphone;
                result.Email = customer.Email;
            }

            return result;
        }

        public CustomerDataClass GetCustomerDateByCellphone(string nationalIdNumber, string cellphone)
        {
            CustomerDataClass result = new CustomerDataClass();
            var customer = _db.Customerdata.FirstOrDefault(x => x.Cellphone == cellphone && x.NationalIdNumber == nationalIdNumber);
            var lineaccount = _db.Customerlineaccounts.FirstOrDefault(x => x.Id == customer.Id);

            result.Id = customer.Id;
            result.Name = customer.Name;
            result.NationalIdNumber = customer.NationalIdNumber;
            result.MedicalRecordNumber = customer.MedicalRecordNumber;
            result.Gender = customer.Gender;
            result.Birthday = customer.Birthday;
            result.Cellphone = customer.Cellphone;
            result.Email = customer.Email;

            if (lineaccount != null)
            {
                result.LineId = lineaccount.LineId;
                result.LinePictureUrl = lineaccount.LinePictureUrl;
                result.DisplayName = lineaccount.DisplayName;
            }

            return result;
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

                if (BookingBeginTime.Contains(":15") || BookingBeginTime.Contains(":45"))
                    continue;

                if (AppointmentCount == 0)
                {
                    BookingBeginTime = DateTime.ParseExact(TimeSpan.Parse(BookingBeginTime).Add(new TimeSpan(0, FillinTime, 0)).ToString(), "HH:mm:ss", null).ToString("HH:mm");
                    BookingEndTime = DateTime.ParseExact(TimeSpan.Parse(BookingEndTime).Add(new TimeSpan(0, FillinTime, 0)).ToString(), "HH:mm:ss", null).ToString("HH:mm");
                }

                //var dt = _db.Doctoroutpatients.AsEnumerable().Where(
                //    x => x.Status == "Y" &&
                //    (_db.Outpatientappointments.Where(y => y.OutpatientId == x.Id).ToList().Count() == 0) &&
                //    x.DoctorId == appointment.DoctorId &&
                //    int.Parse(x.Year) == Year &&
                //    int.Parse(x.Month) == Month &&
                //    int.Parse(x.Day) == Day &&
                //    TimeSpan.Parse(x.BeginTime) >= TimeSpan.Parse(BookingBeginTime) &&
                //    TimeSpan.Parse(x.EndTime) <= TimeSpan.Parse(BookingEndTime)
                //).OrderBy(x => TimeSpan.Parse(x.BeginTime)).ToList();

                var dt = _db.Doctoroutpatients
                .Where(x => x.Status == "Y" &&
                            !_db.Outpatientappointments.Any(y => y.OutpatientId == x.Id) &&
                            x.DoctorId == appointment.DoctorId &&
                            x.Year == Year.ToString() &&
                            x.Month == Month.ToString() &&
                            x.Day == Day.ToString() &&
                            string.Compare(x.BeginTime, BookingBeginTime) >= 0 &&
                            string.Compare(x.EndTime, BookingEndTime) <= 0)
                .OrderBy(x => x.BeginTime)
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

        public CustomerDataClass GetAppointmentCustomerData(string AppointmentId)
        {
            var item = _db.Appointments.FirstOrDefault(x => x.Id == AppointmentId);

            if (item != null)
            {
                var customer = _db.Customerdata.FirstOrDefault(x => x.Id == item.CustomerId);

                return new CustomerDataClass()
                {
                    Id = customer.Id,
                    //LineId = customer.LineId,
                    //DisplayName = customer.DisplayName,
                    //LinePictureUrl = customer.LinePictureUrl,
                    Cellphone = customer.Cellphone,
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
            result.CustomerId = appointment.CustomerId;

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

        public void CreateCustomer(Customerdatum value)
        {
            _db.Customerdata.Add(value);
            _db.SaveChanges();
        }

        public void CreateCustomerLineAccount(Customerlineaccount value)
        {
            _db.Customerlineaccounts.Add(value);
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

        public void UpdateCustomerLineInformation(Customerlineaccount value, string LineId)
        {
            _db.Customerlineaccounts.FirstOrDefault(x => x.LineId == LineId).Modifier = value.Modifier;
            _db.Customerlineaccounts.FirstOrDefault(x => x.LineId == LineId).ModifyDate = value.ModifyDate;

            _db.Customerlineaccounts.FirstOrDefault(x => x.LineId == LineId).LinePictureUrl = value.LinePictureUrl;

            _db.SaveChanges();
        }

        //public void BindCustomerLineId(Customerlineaccount value, string customerId)
        //{

        //    //_db.Customers.FirstOrDefault(x => x.Id == customerId).Modifier = value.Modifier;
        //    //_db.Customers.FirstOrDefault(x => x.Id == customerId).ModifyDate = value.ModifyDate;

        //    //_db.Customers.FirstOrDefault(x => x.Id == customerId).LineId = value.LineId;
        //    //_db.Customers.FirstOrDefault(x => x.Id == customerId).LinePictureUrl = value.LinePictureUrl;

        //    _db.SaveChanges();
        //}

        public string UpdateCustomerIdToDatabase(string customerId, Customerdatum value)
        {
            try
            {
                var oldCustomer = _db.Customerdata.FirstOrDefault(x => x.NationalIdNumber == value.NationalIdNumber);

                if (_db.Customerlineaccounts.FirstOrDefault(x => x.Id == customerId) != null)
                {
                    _db.Customerlineaccounts.FirstOrDefault(x => x.Id == customerId).NationalIdNumber = oldCustomer.NationalIdNumber;
                    _db.Customerlineaccounts.FirstOrDefault(x => x.Id == customerId).Cellphone = oldCustomer.Cellphone;
                }                

                if (oldCustomer.Id != customerId)
                {
                    //更新相關資料表的顧客ID
                    _db.Appointments.Where(x => x.Creator == oldCustomer.Id).ToList().ForEach(x => { x.Creator = customerId; });
                    _db.Appointments.Where(x => x.Modifier == oldCustomer.Id).ToList().ForEach(x => { x.Modifier = customerId; });
                    _db.Appointments.Where(x => x.CustomerId == oldCustomer.Id).ToList().ForEach(x => { x.CustomerId = customerId; });

                    _db.Appointmenttreatments.Where(x => x.Creator == oldCustomer.Id).ToList().ForEach(x => { x.Creator = customerId; });
                    _db.Appointmenttreatments.Where(x => x.Modifier == oldCustomer.Id).ToList().ForEach(x => { x.Modifier = customerId; });

                    _db.Customertokens.Where(x => x.CustomerId == oldCustomer.Id).ToList().ForEach(x => { x.CustomerId = customerId; });

                    _db.Outpatientappointments.Where(x => x.Creator == oldCustomer.Id).ToList().ForEach(x => { x.Creator = customerId; });
                    _db.Outpatientappointments.Where(x => x.Modifier == oldCustomer.Id).ToList().ForEach(x => { x.Modifier = customerId; });

                    _db.Systemlogs.Where(x => x.Creator == oldCustomer.Id).ToList().ForEach(x => { x.Creator = customerId; });
                    _db.Systemlogs.Where(x => x.UserAccount == oldCustomer.Id).ToList().ForEach(x => { x.UserAccount = customerId; });


                    //將相關資料表的ID修改完成後才更新customer資料表的ID及相關資料
                    Customerdatum newItem = new Customerdatum()
                    {
                        CreateDate = DateTime.Now,
                        Creator = customerId,
                        ModifyDate = DateTime.Now,
                        Modifier = customerId,
                        Status = "Y",

                        Id = customerId,
                        Name = oldCustomer.Name,
                        MedicalRecordNumber = oldCustomer.MedicalRecordNumber,
                        NationalIdNumber = oldCustomer.NationalIdNumber,
                        Cellphone = oldCustomer.Cellphone,
                        Birthday = oldCustomer.Birthday,
                        Email = oldCustomer.Email,
                        Gender = oldCustomer.Gender,
                        Memo = oldCustomer.Memo
                    };

                    _db.Customerdata.Add(newItem);
                    _db.Customerdata.Remove(oldCustomer);

                    _db.SaveChanges();
                }
                else
                {
                    _db.Customerdata.FirstOrDefault(x => x.Id == customerId).Modifier = value.Modifier;
                    _db.Customerdata.FirstOrDefault(x => x.Id == customerId).ModifyDate = value.ModifyDate;

                    _db.Customerdata.FirstOrDefault(x => x.Id == customerId).MedicalRecordNumber = value.MedicalRecordNumber;
                    _db.Customerdata.FirstOrDefault(x => x.Id == customerId).Name = value.Name;
                    _db.Customerdata.FirstOrDefault(x => x.Id == customerId).NationalIdNumber = value.NationalIdNumber;                    
                    _db.Customerdata.FirstOrDefault(x => x.Id == customerId).Cellphone = value.Cellphone;
                    _db.Customerdata.FirstOrDefault(x => x.Id == customerId).Gender = value.Gender;
                    _db.Customerdata.FirstOrDefault(x => x.Id == customerId).Birthday = value.Birthday;
                    _db.Customerdata.FirstOrDefault(x => x.Id == customerId).Email = value.Email;

                    _db.SaveChanges();
                }

                

                //_db.Customerdata.FirstOrDefault(x => x.Id == oldCustomerId).Modifier = value.Modifier;
                //_db.Customerdata.FirstOrDefault(x => x.Id == oldCustomerId).ModifyDate = value.ModifyDate;

                //_db.Customerdata.FirstOrDefault(x => x.Id == oldCustomerId).MedicalRecordNumber = value.MedicalRecordNumber;
                //_db.Customerdata.FirstOrDefault(x => x.Id == oldCustomerId).Name = value.Name;
                //_db.Customerdata.FirstOrDefault(x => x.Id == oldCustomerId).NationalIdNumber = value.NationalIdNumber;
                //_db.Customerdata.FirstOrDefault(x => x.Id == oldCustomerId).Cellphone = value.Cellphone;
                //_db.Customerdata.FirstOrDefault(x => x.Id == oldCustomerId).Gender = value.Gender;
                //_db.Customerdata.FirstOrDefault(x => x.Id == oldCustomerId).Birthday = value.Birthday;
                //_db.Customerdata.FirstOrDefault(x => x.Id == oldCustomerId).Email = value.Email;
                //_db.Customerdata.FirstOrDefault(x => x.Id == oldCustomerId).Id = customerId;

                //var cd = _db.Customerdata.FirstOrDefault(x => x.NationalIdNumber == value.NationalIdNumber);
                //string id = "";

                //if (cd != null)
                //{
                //    id = cd.Id;
                //    _db.Appointments.Where(x => x.CustomerId == customerId).ToList().ForEach(x => x.CustomerId = id);
                //    _db.Customerdata.FirstOrDefault(x => x.Id == id).Id = customerId;
                //}
                //else
                //    id = customerId;

                //_db.Customers.FirstOrDefault(x => x.Id == customerId).Modifier = value.Modifier;
                //_db.Customers.FirstOrDefault(x => x.Id == customerId).ModifyDate = value.ModifyDate;

                //_db.Customers.FirstOrDefault(x => x.Id == customerId).MedicalRecordNumber = value.MedicalRecordNumber;
                //_db.Customers.FirstOrDefault(x => x.Id == customerId).Name = value.Name;
                //_db.Customers.FirstOrDefault(x => x.Id == customerId).NationalIdNumber = value.NationalIdNumber;
                //_db.Customers.FirstOrDefault(x => x.Id == customerId).CellPhone = value.CellPhone;
                //_db.Customers.FirstOrDefault(x => x.Id == customerId).Gender = value.Gender;
                //_db.Customers.FirstOrDefault(x => x.Id == customerId).Birthday = value.Birthday;
                //_db.Customers.FirstOrDefault(x => x.Id == customerId).Email = value.Email;


                return customerId;
            }
            catch
            {
                return "failed";
            }

        }

        //public string UpdateCustomer(string customerId, Customer value)
        //{
        //    try
        //    {
        //        var cd = _db.Customerdata.FirstOrDefault(x => x.NationalIdNumber == value.NationalIdNumber);
        //        string id = "";

        //        if (cd != null)
        //        {
        //            id = cd.Id;
        //            _db.Appointments.Where(x => x.CustomerId == customerId).ToList().ForEach(x => x.CustomerId = id);
        //            _db.Customerdata.FirstOrDefault(x => x.Id == id).Id = customerId;
        //        }
        //        else
        //            id = customerId;

        //        _db.Customers.FirstOrDefault(x => x.Id == customerId).Modifier = value.Modifier;
        //        _db.Customers.FirstOrDefault(x => x.Id == customerId).ModifyDate = value.ModifyDate;

        //        _db.Customers.FirstOrDefault(x => x.Id == customerId).MedicalRecordNumber = value.MedicalRecordNumber;
        //        _db.Customers.FirstOrDefault(x => x.Id == customerId).Name = value.Name;
        //        _db.Customers.FirstOrDefault(x => x.Id == customerId).NationalIdNumber = value.NationalIdNumber;
        //        _db.Customers.FirstOrDefault(x => x.Id == customerId).CellPhone = value.CellPhone;
        //        _db.Customers.FirstOrDefault(x => x.Id == customerId).Gender = value.Gender;
        //        _db.Customers.FirstOrDefault(x => x.Id == customerId).Birthday = value.Birthday;
        //        _db.Customers.FirstOrDefault(x => x.Id == customerId).Email = value.Email;

        //        _db.SaveChanges();

        //        return id;
        //    }
        //    catch
        //    {
        //        return "failed";
        //    }

        //}

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
                    bool flag = false;
                    string BookingBeginTime = times.BeginTime;
                    string BookingEndTime = TimeSpan.Parse(times.BeginTime).Add(new TimeSpan(0, MaxTreatmentTime, 0)).ToString();

                    if (AppointmentCount == 0)
                    {
                        BookingBeginTime = DateTime.ParseExact(TimeSpan.Parse(BookingBeginTime).Add(new TimeSpan(0, FillinTime, 0)).ToString(), "HH:mm:ss", null).ToString("HH:mm");
                        BookingEndTime = DateTime.ParseExact(TimeSpan.Parse(BookingEndTime).Add(new TimeSpan(0, FillinTime, 0)).ToString(), "HH:mm:ss", null).ToString("HH:mm");
                    }

                    var dt = _db.Doctoroutpatients.Where(x =>
                        x.Status == "Y" &&
                        !_db.Outpatientappointments.Any(y => y.OutpatientId == x.Id) &&
                        x.DoctorId == doctor &&
                        x.Year == date.Year &&
                        x.Month == date.Month &&
                        x.Day == date.Day &&
                        string.Compare(x.BeginTime, BookingBeginTime) >= 0 &&
                        string.Compare(x.EndTime, BookingEndTime) <= 0
                    ).OrderBy(x => x.BeginTime).ToList();
                    double sum = 0;

                    foreach (var d in dt)
                    {
                        sum += (DateTime.ParseExact(d.EndTime, "HH:mm", null) - DateTime.ParseExact(d.BeginTime, "HH:mm", null)).TotalMinutes;
                    }

                    if (sum < MaxTreatmentTime)
                        flag = true;

                    if (flag)
                        enabled = "N";

                    if (BookingBeginTime.Contains(":15") || BookingBeginTime.Contains(":45"))
                        continue;

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

        public Verificationcode CheckVerificationCode(string code)
        {
            DateTime now = DateTime.Now;

            if (_db.Verificationcodes.Where(x => x.SouceTable == "Appointment" && x.HashCode == code && x.ExpireTime >= now).Count() == 0)
                return null;
            else
                return _db.Verificationcodes.FirstOrDefault(x => x.SouceTable == "Appointment" && x.HashCode == code);
        }

        public void SetAppointmentToOutpatient(string AppointmentId, string UserId)
        {
            //確認是否為第一次預約(Count=0為第一次)
            int AppointmentCount = GetCustomerAppointmentCount(UserId);
            //取得新客填寫資料時間
            int FillinTime = int.Parse(_functions.GetSystemParameter("NewCustomerFillInInformationTime"));

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
                _db.Outpatientappointments.RemoveRange(_db.Outpatientappointments.Where(x => x.AppointmentId == AppointmentId));
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

        public void RemoveAppointmenttreatment(string? AppointmentId)
        {
            _db.Appointmenttreatments.RemoveRange(_db.Appointmenttreatments.Where(x => x.AppointmentId == AppointmentId && x.Type == "A"));

            _db.SaveChanges();
        }

        public void CheckActiveVerificationCode(string cellphone, string hashCode)
        {
            DateTime now = DateTime.Now;

            //用hashCode查詢是否有生效中驗證碼
            var item = _db.Verificationcodes.FirstOrDefault(x => x.Status == "Y" && x.SouceTable == "Customer" && x.HashCode == hashCode && x.ExpireTime >= now);
            if (item != null)
            {
                item.Status = "N";
                _db.SaveChanges();
            }

            //用手機號碼查詢是否有生效中驗證碼
            item = _db.Verificationcodes.FirstOrDefault(x => x.Status == "Y" && x.SouceTable == "Customer" && x.ForeignKey == cellphone && x.ExpireTime >= now);
            if (item != null)
            {
                item.Status = "N";
                _db.SaveChanges();
            }
        }

        public string VerifyLoginInfo(string cellphone, string hashCode, string otp)
        {
            DateTime now = DateTime.Now;
            var item = _db.Verificationcodes.FirstOrDefault(x => x.Status == "Y" && x.SouceTable == "Customer" && x.HashCode == hashCode);

            if (item is null)
                return "驗證碼無效或已過期，請重新登入。";
            else if (item.ForeignKey == cellphone && item.Otp == otp)
                return "OK";
            else if (item.ForeignKey != cellphone)
                return "手機號碼與驗證碼紀錄不符合，請重新登入。";
            else if (item.Otp != otp)
                return "驗證碼有誤，請重新登入。";
            else
                return "其他錯誤，請重新登入或洽詢櫃台人員。";
        }

        public string checkCellphone(string nationalIdNumber, string cellphone, string userId)
        {
            var item = _db.Customerdata.Where(x => x.NationalIdNumber != nationalIdNumber && x.Cellphone == cellphone && x.Id != userId);

            if (item.Count() > 0)
                return "此電話號碼已註冊，請電話洽詢診所";
            else
                return "";
        }

        public string checkNationalIdNumber(string nationalIdNumber, string userId)
        {
            var item = _db.Customerlineaccounts.Where(x => x.NationalIdNumber == nationalIdNumber && x.Id != userId);

            if (item.Count() > 0)
                return "身分證字號已重複，請確認!!";
            else
                return "";
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

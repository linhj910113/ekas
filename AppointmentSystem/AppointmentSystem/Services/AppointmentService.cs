using AppointmentSystem.Models.DBModels;
using AppointmentSystem.Models.ViewModels;
using AppointmentSystem.Models.ViewModels.AppointmentModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;

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

        public Customer GetCustomerDateByLineId(string LineId)
        {
            var customer = _db.Customers.FirstOrDefault(x => x.LineId == LineId);

            return customer;
        }

        public int GetCustomerAppointmentCount(string CustomerId)
        {
            return _db.Appointments.Where(x => x.CustomerId == CustomerId && x.Status == "Y").Count();
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
            //string[] treatmentList = treatments.Split(',');
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

        public List<FillinDateTimeVM> GetFillInDateTimeData(string[] treatments, string doctor)
        {
            List<FillinDateTimeVM> result = new List<FillinDateTimeVM>();
            int CurrentYear = DateTime.Now.Year;
            int CurrentMonth = DateTime.Now.Month;
            int CurrentDay = DateTime.Now.Day;

            var OutaetientDates = _db.Doctoroutpatients.AsEnumerable().Where(x => x.DoctorId == doctor && int.Parse(x.Year) >= CurrentYear && int.Parse(x.Month) >= CurrentMonth && int.Parse(x.Day) >= CurrentDay).Select(x => new { x.Year, x.Month, x.Day }).Distinct().OrderBy(x => int.Parse(x.Year)).ThenBy(x => int.Parse(x.Month)).ThenBy(x => int.Parse(x.Day)).ToList();
            //var doctoroutaetients = _db.Doctoroutpatients.AsEnumerable().Where(x => x.DoctorId == doctor && int.Parse(x.Year) >= CurrentYear && int.Parse(x.Month) >= CurrentMonth && int.Parse(x.Day) >= CurrentDay).OrderBy(x => int.Parse(x.Year)).ThenBy(x => int.Parse(x.Month)).ThenBy(x => int.Parse(x.Day)).ThenBy(x => TimeSpan.Parse(x.BeginTime)).ToList();

            foreach (var date in OutaetientDates)
            {
                FillinDateTimeVM item = new FillinDateTimeVM();
                List<OutpatientTimeData> outpatientTimeData = new List<OutpatientTimeData>();
                item.Date = date.Year + "-" + int.Parse(date.Month).ToString("D2") + "-" + int.Parse(date.Day).ToString("D2");

                var OutpatientTimes = _db.Doctoroutpatients.AsEnumerable().Where(x => x.DoctorId == doctor && x.Year == date.Year && x.Month == date.Month && x.Day == date.Day).OrderBy(x => TimeSpan.Parse(x.BeginTime));

                foreach (var times in OutpatientTimes)
                {
                    string enabled = "N";
                    if (times.AppointmentId == "")
                        enabled = "Y";

                    outpatientTimeData.Add(new OutpatientTimeData()
                    {
                        BeginTime = DateTime.ParseExact(times.BeginTime, "HH:mm:ss", null).ToString("HH:mm"),
                        EndTime = DateTime.ParseExact(times.EndTime, "HH:mm:ss", null).ToString("HH:mm"),
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
            if (_db.Verificationcodes.Where(x => x.SouceTable == "Appointment" && x.HashCode == code).Count() == 0)
                return "";
            else
                return _db.Verificationcodes.FirstOrDefault(x => x.SouceTable == "Appointment" && x.HashCode == code).ForeignKey;
        }

        public void SetAppointmentToOutpatient(string AppointmentId, string UserId, Appointment value)
        {
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

                _db.Doctoroutpatients.AsEnumerable().Where(x => x.Year == year.ToString() && x.Month == month.ToString() && x.Day == day.ToString() && DateTime.ParseExact(x.BeginTime, "HH:mm", null) >= DateTime.ParseExact(begintime, "HH:mm", null) && DateTime.ParseExact(x.EndTime, "HH:mm", null) <= DateTime.ParseExact(endtime, "HH:mm", null)).ToList().ForEach(x => x.AppointmentId = AppointmentId);
                _db.SaveChanges();
            }
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

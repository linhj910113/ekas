using AppointmentSystem.Models.DBModels;
using AppointmentSystem.Models.ViewModels;
using AppointmentSystem.Models.ViewModels.AppointmentModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        public Customer GetCustomerDateByLineId(string LineId)
        {
            var customer = _db.Customers.FirstOrDefault(x => x.LineId == LineId);

            return customer;
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

        public void UpdateCustomer(Customer value, string LineId)
        {
            _db.Customers.FirstOrDefault(x => x.LineId == LineId).Modifier = value.Modifier;
            _db.Customers.FirstOrDefault(x => x.LineId == LineId).ModifyDate = value.ModifyDate;

            _db.Customers.FirstOrDefault(x => x.LineId == LineId).LineDisplayName = value.LineDisplayName;
            _db.Customers.FirstOrDefault(x => x.LineId == LineId).LinePictureUrl = value.LinePictureUrl;

            _db.SaveChanges();
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








        public async Task<bool> SendLineMessageAsync(string to, string message)
        {
            string MessagingApiChannelAccessToken = _functions.GetSystemParameter("MessagingApiChannelAccessToken");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", MessagingApiChannelAccessToken);

                var payload = new
                {
                    to = to,
                    messages = new[]
                    {
                    new { type = "text", text = message }
                }
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://api.line.me/v2/bot/message/push", content);

                return response.IsSuccessStatusCode;
            }
        }

    }
}

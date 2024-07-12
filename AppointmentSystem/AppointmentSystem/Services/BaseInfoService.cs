using AppointmentSystem.Models.DBModels;
using AppointmentSystem.Models.ViewModels.BaseInfoModels;
using AppointmentSystem.Models.ViewModels.SystemModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using static AppointmentSystem.Models.ViewModels.SelectListModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppointmentSystem.Services
{
    public class BaseInfoService
    {
        private readonly EkasContext _db;
        private readonly WebFunctions _functions;

        public BaseInfoService(EkasContext db)
        {
            _db = db;
            _functions = new WebFunctions(db);
        }

        #region -- 醫師基本資料 -- (Doctor)

        public bool CheckDoctorId(string DoctorId)
        {
            if (_db.Doctors.Where(x => x.Id == DoctorId).Count() == 0)
                return false;
            else
                return true;
        }

        public List<DoctorIndexVM> GetDoctorListForIndex()
        {
            var items = _db.Doctors.OrderBy(x => x.Sort).ThenBy(x => x.CreateDate).ToList();
            List<DoctorIndexVM> datas = new List<DoctorIndexVM>();

            foreach (var item in items)
            {
                datas.Add(new DoctorIndexVM
                {
                    DoctorId = item.Id,
                    DoctorName = item.DoctorName,
                    Sort = item.Sort
                });
            }

            return datas;
        }

        public DoctorEditVM GetDoctorById(string id)
        {
            var item = _db.Doctors.Where(x => x.Id == id).FirstOrDefault();
            DoctorEditVM doctor = new DoctorEditVM();

            doctor.DoctorId = id;
            doctor.DoctorName = item.DoctorName;
            doctor.DoctorNameEnglish = item.DoctorNameEnglish;
            doctor.Introduction = item.Introduction;
            doctor.DepartmentTitle = item.DepartmentTitle;
            doctor.DoctorImage.FileID = item.ImageFileId;
            doctor.Memo = item.Memo;
            doctor.Sort = item.Sort;
            doctor.Status = item.Status;

            var filedata = _db.Systemfiles.Where(x => x.Id == item.ImageFileId).FirstOrDefault();

            if (filedata != null)
            {
                doctor.DoctorImage.FileID = filedata.Id;
                doctor.DoctorImage.FileName = filedata.FileName;
                doctor.DoctorImage.FileExtension = filedata.FileExtension;
                doctor.DoctorImage.Path = filedata.Path;
                doctor.DoctorImage.FileSize = filedata.FileSize;
            }

            return doctor;
        }

        public string GetDoctorImageIdByDoctorId(string id)
        {
            var item = _db.Doctors.FirstOrDefault(x => x.Id == id);

            if (item != null)
                return item.ImageFileId;
            else
                return "";
        }

        public int GetDoctorCount()
        {
            return _db.Doctors.Count();
        }

        public void CreateDoctor(Doctor value)
        {
            _db.Doctors.Add(value);
            _db.SaveChanges();
        }

        public void UpdateDoctorName(string id, string account, Doctor value)
        {
            _db.Doctors.FirstOrDefault(x => x.Id == id).Modifier = account;
            _db.Doctors.FirstOrDefault(x => x.Id == id).ModifyDate = DateTime.Now;

            _db.Doctors.FirstOrDefault(x => x.Id == id).DoctorName = value.DoctorName;

            _db.SaveChanges();
        }

        public void UpdateDoctor(string id, string account, Doctor value)
        {
            _db.Doctors.FirstOrDefault(x => x.Id == id).Modifier = account;
            _db.Doctors.FirstOrDefault(x => x.Id == id).ModifyDate = DateTime.Now;

            _db.Doctors.FirstOrDefault(x => x.Id == id).DoctorNameEnglish = value.DoctorNameEnglish;
            _db.Doctors.FirstOrDefault(x => x.Id == id).Introduction = value.Introduction;
            _db.Doctors.FirstOrDefault(x => x.Id == id).DepartmentTitle = value.DepartmentTitle;
            _db.Doctors.FirstOrDefault(x => x.Id == id).Sort = value.Sort;
            _db.Doctors.FirstOrDefault(x => x.Id == id).Memo = value.Memo;
            _db.Doctors.FirstOrDefault(x => x.Id == id).ImageFileId = value.ImageFileId;

            _db.SaveChanges();
        }

        public void RemoveDoctorTreatment(string? DoctorId)
        {
            _db.Doctortreatments.RemoveRange(_db.Doctortreatments.Where(x => x.DoctorId == DoctorId));

            _db.SaveChanges();
        }

        public void CreateDoctorTreatment(Doctortreatment value)
        {
            _db.Doctortreatments.Add(value);
            _db.SaveChanges();
        }

        public List<LableCheckboxList> GetLableListForCheckbox(string TreatmentId)
        {
            var items = _db.Lables.Where(x => x.Status == "Y" && x.Type == "Treatment").OrderBy(x => x.CreateDate).ToList();

            List<LableCheckboxList> lables = new List<LableCheckboxList>();

            foreach (var item in items)
            {
                LableCheckboxList lableCheckboxList = new LableCheckboxList();
                var dt = _db.Treatmentlables.FirstOrDefault(x => x.TreatmentId == TreatmentId && x.LabelId == item.Id);

                lableCheckboxList.LableId = item.Id;
                lableCheckboxList.LableName = item.LableName;

                if (dt != null)
                    lableCheckboxList.IsChecked = "Y";
                else
                    lableCheckboxList.IsChecked = "N";

                lables.Add(lableCheckboxList);
            }

            return lables;
        }

        #endregion

        #region -- 療程基本資料 -- (Treatment)

        public bool CheckTreatmentId(string TreatmentId)
        {
            if (_db.Treatments.Where(x => x.Id == TreatmentId).Count() == 0)
                return false;
            else
                return true;
        }

        public List<TreatmentIndexVM> GetTreatmentListForIndex()
        {
            var items = _db.Treatments.OrderBy(x => x.Sort).ThenBy(x => x.CreateDate).ToList();
            List<TreatmentIndexVM> datas = new List<TreatmentIndexVM>();

            foreach (var item in items)
            {
                datas.Add(new TreatmentIndexVM
                {
                    TreatmentId = item.Id,
                    TreatmentName = item.TreatmentName,
                    Hide = item.Hide,
                    Sort = item.Sort,
                });
            }

            return datas;
        }

        public TreatmentEditVM GetTreatmentById(string id)
        {
            var item = _db.Treatments.Where(x => x.Id == id).FirstOrDefault();
            TreatmentEditVM treatment = new TreatmentEditVM();

            treatment.TreatmentId = id;
            treatment.TreatmentName = item.TreatmentName;
            treatment.Introduction = item.Introduction;
            treatment.TreatmentImage.FileID = item.ImageFileId;
            treatment.Time = item.Time;
            treatment.AlertMessage = item.AlertMessage;
            treatment.Memo = item.Memo;
            treatment.Sort = item.Sort;
            treatment.Status = item.Status;

            var filedata = _db.Systemfiles.Where(x => x.Id == item.ImageFileId).FirstOrDefault();

            if (filedata != null)
            {
                treatment.TreatmentImage.FileID = filedata.Id;
                treatment.TreatmentImage.FileName = filedata.FileName;
                treatment.TreatmentImage.FileExtension = filedata.FileExtension;
                treatment.TreatmentImage.Path = filedata.Path;
                treatment.TreatmentImage.FileSize = filedata.FileSize;
            }

            return treatment;
        }

        public string GetTreatmentImageIdByTreatmentId(string id)
        {
            var item = _db.Treatments.FirstOrDefault(x => x.Id == id);

            if (item != null)
                return item.ImageFileId;
            else
                return "";
        }

        public List<TreatmentCheckboxList> GetTreatmentListForCheckbox(string DoctorId)
        {
            var items = _db.Treatments.Where(x => x.Status == "Y").OrderBy(x => x.Sort).ToList();

            List<TreatmentCheckboxList> treatments = new List<TreatmentCheckboxList>();

            foreach (var item in items)
            {
                TreatmentCheckboxList treatmentCheckboxList = new TreatmentCheckboxList();
                var dt = _db.Doctortreatments.FirstOrDefault(x => x.DoctorId == DoctorId && x.TreatmentId == item.Id);

                treatmentCheckboxList.TreatmentId = item.Id;
                treatmentCheckboxList.TreatmentName = item.TreatmentName;

                if (dt != null)
                    treatmentCheckboxList.IsChecked = "Y";
                else
                    treatmentCheckboxList.IsChecked = "N";

                treatments.Add(treatmentCheckboxList);
            }

            return treatments;
        }

        public int GetTreatmentCount()
        {
            return _db.Treatments.Count();
        }

        public void CreateTreatment(Treatment value)
        {
            _db.Treatments.Add(value);
            _db.SaveChanges();
        }

        public void UpdateTreatmentName(string id, string account, Treatment value)
        {
            _db.Treatments.FirstOrDefault(x => x.Id == id).Modifier = account;
            _db.Treatments.FirstOrDefault(x => x.Id == id).ModifyDate = DateTime.Now;

            _db.Treatments.FirstOrDefault(x => x.Id == id).TreatmentName = value.TreatmentName;

            _db.SaveChanges();
        }

        public void UpdateTreatment(string id, string account, Treatment value)
        {
            _db.Treatments.FirstOrDefault(x => x.Id == id).Modifier = account;
            _db.Treatments.FirstOrDefault(x => x.Id == id).ModifyDate = DateTime.Now;

            _db.Treatments.FirstOrDefault(x => x.Id == id).Introduction = value.Introduction;
            _db.Treatments.FirstOrDefault(x => x.Id == id).AlertMessage = value.AlertMessage;
            _db.Treatments.FirstOrDefault(x => x.Id == id).Time = value.Time;
            _db.Treatments.FirstOrDefault(x => x.Id == id).Sort = value.Sort;
            _db.Treatments.FirstOrDefault(x => x.Id == id).Memo = value.Memo;
            _db.Treatments.FirstOrDefault(x => x.Id == id).ImageFileId = value.ImageFileId;

            _db.SaveChanges();
        }

        public bool CheckLableId(string LableId)
        {
            if (_db.Lables.Where(x => x.Id == LableId).Count() == 0)
                return false;
            else
                return true;
        }

        public void CreateLable(Lable value)
        {
            _db.Lables.Add(value);
            _db.SaveChanges();
        }

        public void RemoveTreatmentLable(string? TreatmentId)
        {
            _db.Treatmentlables.RemoveRange(_db.Treatmentlables.Where(x => x.TreatmentId == TreatmentId));

            _db.SaveChanges();
        }

        public void CreateTreatmentLable(Treatmentlable value)
        {
            _db.Treatmentlables.Add(value);
            _db.SaveChanges();
        }

        public void setTreatmentHideValue(string treatmentId, Treatment value)
        {
            _db.Treatments.FirstOrDefault(x => x.Id == treatmentId).Modifier = value.Modifier;
            _db.Treatments.FirstOrDefault(x => x.Id == treatmentId).ModifyDate = value.ModifyDate;
            _db.Treatments.FirstOrDefault(x => x.Id == treatmentId).Hide = value.Hide;

            _db.SaveChanges();
        }

        #endregion

        #region -- 醫師班表資料 -- (DoctorShift)

        public bool CheckArrangeDoctorShiftId(string ArrangeDoctorShiftId)
        {
            if (_db.Arrangedoctorshifts.Where(x => x.Id == ArrangeDoctorShiftId).Count() == 0)
                return false;
            else
                return true;
        }

        public bool CheckArrangeMonthShiftId(string ArrangeMonthShiftId)
        {
            if (_db.Arrangemonthshifts.Where(x => x.Id == ArrangeMonthShiftId).Count() == 0)
                return false;
            else
                return true;
        }

        public bool CheckOutpatientIdId(string OutpatientId)
        {
            if (_db.Doctoroutpatients.Where(x => x.Id == OutpatientId).Count() == 0)
                return false;
            else
                return true;
        }

        public SetDoctorShiftVM GetDoctorShift(string year, string month)
        {
            //取得班表類別
            var shifttypes = _db.Shifttypes.Where(x => x.Status == "Y").OrderBy(x => x.Sort).ToList();
            List<ShiftTypeData> shiftTypeDatas = new List<ShiftTypeData>();

            foreach (var type in shifttypes)
            {
                shiftTypeDatas.Add(new ShiftTypeData
                {
                    ShiftId = type.Id,
                    ShiftName = type.Name,
                    BeginTime = type.BeginTime,
                    EndTime = type.EndTime
                });
            }

            //取得醫師列表
            var doctors = _db.Doctors.Where(x => x.Status == "Y").OrderBy(x => x.Sort).ToList();
            List<DoctorCheckboxList> doctorDatas = new List<DoctorCheckboxList>();

            foreach (var doctor in doctors)
            {
                doctorDatas.Add(new DoctorCheckboxList
                {
                    DoctorId = doctor.Id,
                    DoctorName = doctor.DoctorName,
                });
            }

            //取得當月醫師預排班表
            var arrange = _db.Arrangedoctorshifts.Where(x => x.Status == "Y" && x.Year == year && x.Month == month).ToList();
            List<ArrangeDoctorShiftData> arrangeDoctorShiftDatas = new List<ArrangeDoctorShiftData>();

            foreach (var d in arrange)
            {
                var doctor = _db.Doctors.FirstOrDefault(x => x.Id == d.DoctorId);

                arrangeDoctorShiftDatas.Add(new ArrangeDoctorShiftData
                {
                    Id = d.Id,
                    DoctorId = d.DoctorId,
                    DoctorName = doctor.DoctorName,
                    Year = d.Year,
                    Month = d.Month,
                    Day = d.Day,
                    ShiftTypeId = d.ShiftTypeId
                });
            }

            //確認班表是否已鎖定
            string Locked = "Y";
            var monthshift = _db.Arrangemonthshifts.Where(x => x.Status == "Y" && x.Year == year && x.Month == month);

            if (monthshift.Count() == 0)
                Locked = "N";

            return new SetDoctorShiftVM()
            {
                shiftTypeData = shiftTypeDatas,
                doctorData = doctorDatas,
                arrangeDoctorShiftData = arrangeDoctorShiftDatas,
                Locked = Locked
            };

        }

        public List<ShiftTypeData> GetShiftTypeData()
        {
            List<ShiftTypeData> shiftTypeDatas = new List<ShiftTypeData>();
            var items = _db.Shifttypes.Where(x => x.Status == "Y");

            foreach (var item in items)
            {
                shiftTypeDatas.Add(new ShiftTypeData()
                {
                    ShiftId = item.Id,
                    ShiftName = item.Name,
                    BeginTime = item.BeginTime,
                    EndTime = item.EndTime
                });
            }

            return shiftTypeDatas;
        }

        public void RemoveArrangeDoctorShift(string year, string month)
        {
            _db.Arrangedoctorshifts.RemoveRange(_db.Arrangedoctorshifts.Where(x => x.Year == year && x.Month == month));

            _db.SaveChanges();
        }

        public void SaveArrangeDoctorShift(Arrangedoctorshift data)
        {
            _db.Arrangedoctorshifts.Add(data);
            _db.SaveChanges();
        }

        //儲存ArrangeMonthShift(鎖定該月份班表)
        public void SaveArrangeMonthShift(Arrangemonthshift data)
        {
            _db.Arrangemonthshifts.Add(data);
            _db.SaveChanges();
        }

        public void SaveDoctorOutpatient(Doctoroutpatient data)
        {
            _db.Doctoroutpatients.Add(data);
            _db.SaveChanges();
        }




        #endregion

        #region -- 醫師請假 -- (DoctorDayOff)

        public List<DoctorCheckboxList> GetDoctorListForDropdown()
        {
            var items = _db.Doctors.Where(x => x.Status == "Y").OrderBy(x => x.Sort);

            List<DoctorCheckboxList> doctors = new List<DoctorCheckboxList>();

            foreach (var item in items)
            {
                doctors.Add(new DoctorCheckboxList
                {
                    DoctorId = item.Id,
                    DoctorName = item.DoctorName
                });
            }

            return doctors;
        }

        public List<DoctorDayOffIndexVM> GetDoctorDayOffForIndex()
        {
            var items = _db.Doctordayoffs.Where(x => x.Status == "Y").OrderByDescending(x => x.Date).ToList();

            List<DoctorDayOffIndexVM> ddo = new List<DoctorDayOffIndexVM>();

            foreach (var item in items)
            {
                var doctor = _db.Doctors.FirstOrDefault(x => x.Id == item.DoctorId);
                var type = _db.Systemselectlists.FirstOrDefault(x => x.GroupName == "DoctorDayOffSelectList" && x.SelectValue == item.Type);

                ddo.Add(new DoctorDayOffIndexVM
                {
                    DayOffIndex = item.Index,
                    DoctorName = doctor.DoctorName,
                    Type = type.SelectName,
                    Date = item.Date,
                    BeginTime = item.BeginTime,
                    EndTime = item.EndTime
                });
            }

            return ddo;
        }

        public void CreateDoctorDayOff(Doctordayoff value)
        {
            //建立假單資料
            _db.Doctordayoffs.Add(value);
            _db.SaveChanges();

            //修改門診表狀態
            DateTime d = DateTime.Parse(value.Date);
            int year = d.Year; int month = d.Month; int day = d.Day;

            var OutpatientTimes = _db.Doctoroutpatients.AsEnumerable().Where(
                x => x.Status != "N" &&
                x.DoctorId == value.DoctorId &&
                int.Parse(x.Year) == year &&
                int.Parse(x.Month) == month &&
                int.Parse(x.Day) == day && x.AppointmentId == ""
            ).OrderBy(x => TimeSpan.Parse(x.BeginTime)).ToList();

            foreach (var times in OutpatientTimes)
            {
                if (TimeSpan.Parse(times.BeginTime) < TimeSpan.Parse(value.EndTime) && TimeSpan.Parse(times.EndTime) > TimeSpan.Parse(value.BeginTime))
                    times.Status = value.Type;
            }

            _db.SaveChanges();
        }

        public string CheckDayOffData(string doctorId, string date, string beginTime, string endTime)
        {
            //確認該月份是否已排班
            DateTime d = DateTime.Parse(date);
            int year = d.Year; int month = d.Month; int day = d.Day;

            var monthshift = _db.Arrangemonthshifts.AsEnumerable().Where(x => x.Status == "Y" && x.Year == year.ToString() && x.Month == month.ToString() && x.Locked == "Y").ToList();
            if (monthshift.Count() == 0)
                return "該月尚無班表，請先設定班表並鎖定。";

            //確認該時端是否已有預約
            var OutpatientTimes = _db.Doctoroutpatients.AsEnumerable().Where(
                x => x.Status != "N" &&
                x.DoctorId == doctorId &&
                int.Parse(x.Year) == year &&
                int.Parse(x.Month) == month &&
                int.Parse(x.Day) == day &&
                x.AppointmentId != ""
            ).OrderBy(x => TimeSpan.Parse(x.BeginTime)).ToList();

            foreach (var times in OutpatientTimes)
            {
                if (TimeSpan.Parse(times.BeginTime) < TimeSpan.Parse(endTime) && TimeSpan.Parse(times.EndTime) > TimeSpan.Parse(beginTime))
                    return "請假時間內已有顧客預約，請先調整預約再進行請假。";
            }

            return "";
        }

        public string DeleteDoctorDayOff(long Index)
        {
            var ddo = _db.Doctordayoffs.FirstOrDefault(x => x.Index == Index);

            //修改門診表狀態
            DateTime d = DateTime.Parse(ddo.Date);
            int year = d.Year; int month = d.Month; int day = d.Day;

            var OutpatientTimes = _db.Doctoroutpatients.AsEnumerable().Where(
                x => x.Status != "N" &&
                x.DoctorId == ddo.DoctorId &&
                int.Parse(x.Year) == year &&
                int.Parse(x.Month) == month &&
                int.Parse(x.Day) == day && x.AppointmentId == ""
            ).OrderBy(x => TimeSpan.Parse(x.BeginTime));

            foreach (var times in OutpatientTimes)
            {
                if (TimeSpan.Parse(times.BeginTime) < TimeSpan.Parse(ddo.EndTime) && TimeSpan.Parse(times.EndTime) > TimeSpan.Parse(ddo.BeginTime))
                    times.Status = "Y";
            }
            _db.SaveChanges();

            //設定假單狀態為N(保留原假單)
            _db.Doctordayoffs.FirstOrDefault(x => x.Index == Index).Status = "N";
            _db.SaveChanges();

            return "success";
        }





        #endregion

    }
}

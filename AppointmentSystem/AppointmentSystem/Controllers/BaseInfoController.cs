using AppointmentSystem.Models.DBModels;
using AppointmentSystem.Models.ViewModels.AppointmentModels;
using AppointmentSystem.Models.ViewModels.BaseInfoModels;
using AppointmentSystem.Models.ViewModels.SystemModels;
using AppointmentSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System.Numerics;
using static AppointmentSystem.Models.ViewModels.SelectListModels;

namespace AppointmentSystem.Controllers
{
    public class BaseInfoController : Controller
    {
        private readonly BaseInfoService _baseinfoService;
        private readonly WebFunctions _functions;

        public BaseInfoController(EkasContext context)
        {
            _baseinfoService = new BaseInfoService(context);
            _functions = new WebFunctions(context);
        }

        #region -- 醫師基本資料 -- (Doctor)

        [Authorize]
        public IActionResult DoctorIndex()
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                List<DoctorIndexVM> doctors = _baseinfoService.GetDoctorListForIndex();
                ViewBag.Count = doctors.Count;

                return View(doctors);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult DoctorCreate([FromForm] DoctorEditVM value)
        {
            var user = HttpContext.User.Claims.ToList();
            string DoctorId = _functions.GetGuid();

            while (_baseinfoService.CheckDoctorId(DoctorId))
                DoctorId = _functions.GetGuid();

            string FileId = "";

            if (value.DoctorImage.File != null)
                FileId = _functions.SaveFile(value.DoctorImage.File, user.FirstOrDefault(u => u.Type == "Account").Value);
            else
                FileId = _baseinfoService.GetDoctorImageIdByDoctorId(value.DoctorId);

            if (String.IsNullOrEmpty(value.DoctorNameEnglish))
                value.DoctorNameEnglish = "";
            if (String.IsNullOrEmpty(value.Memo))
                value.Memo = "";

            Doctor item = new Doctor()
            {
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = "Y",

                Id = DoctorId,
                DoctorName = value.DoctorName,
                DoctorNameEnglish = value.DoctorNameEnglish,
                Introduction = value.Introduction,
                DepartmentTitle = value.DepartmentTitle,
                ColorHex = value.ColorHEX,
                Sort = _baseinfoService.GetDoctorCount() + 1,
                Memo = value.Memo,
                ImageFileId = FileId
            };
            _baseinfoService.CreateDoctor(item);

            //儲存醫師負責的療程
            //先刪除相關資料再新增
            string[] treatments = value.SelectedTreatment.Split(',');
            _baseinfoService.RemoveDoctorTreatment(value.DoctorId);

            foreach (var treatmentId in treatments)
            {
                Doctortreatment dt = new Doctortreatment()
                {
                    Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                    Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    Status = "Y",

                    DoctorId = DoctorId,
                    TreatmentId = treatmentId
                };

                _baseinfoService.CreateDoctorTreatment(dt);
            }

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Add doctor id='" + DoctorId + "'."
            });

            return new JsonResult(DoctorId);
        }

        //[HttpGet]
        //[Authorize]
        //public IActionResult DoctorNameUpdate(string name, string id)
        //{
        //    string DoctorId = id;

        //    var user = HttpContext.User.Claims.ToList();

        //    Doctor item = new Doctor()
        //    {
        //        Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
        //        ModifyDate = DateTime.Now,

        //        DoctorName = name
        //    };

        //    _baseinfoService.UpdateDoctorName(id, user.FirstOrDefault(u => u.Type == "Account").Value, item);

        //    _functions.SaveSystemLog(new Systemlog
        //    {
        //        CreateDate = DateTime.Now,
        //        Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
        //        UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
        //        Description = "Update doctor id='" + DoctorId + "'."
        //    });

        //    return new JsonResult(DoctorId);
        //}

        [HttpPost]
        [Authorize]
        public IActionResult DoctorUpdate([FromForm] DoctorEditVM value)
        {
            var user = HttpContext.User.Claims.ToList();
            string FileId = "";

            if (value.DoctorImage.File != null)
                FileId = _functions.SaveFile(value.DoctorImage.File, user.FirstOrDefault(u => u.Type == "Account").Value);
            else
                FileId = _baseinfoService.GetDoctorImageIdByDoctorId(value.DoctorId);

            if (String.IsNullOrEmpty(value.DoctorNameEnglish))
                value.DoctorNameEnglish = "";
            if (String.IsNullOrEmpty(value.Memo))
                value.Memo = "";

            Doctor item = new Doctor()
            {
                Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                ModifyDate = DateTime.Now,

                DoctorName = value.DoctorName,
                DoctorNameEnglish = value.DoctorNameEnglish,
                Introduction = value.Introduction,
                DepartmentTitle = value.DepartmentTitle,
                ColorHex = value.ColorHEX,
                Sort = value.Sort,
                Memo = value.Memo,
                ImageFileId = FileId
            };
            _baseinfoService.UpdateDoctor(value.DoctorId, user.FirstOrDefault(u => u.Type == "Account").Value, item);

            //儲存醫師負責的療程
            //先刪除相關資料再新增
            string[] treatments = value.SelectedTreatment.Split(',');
            _baseinfoService.RemoveDoctorTreatment(value.DoctorId);

            foreach (var treatmentId in treatments)
            {
                Doctortreatment dt = new Doctortreatment()
                {
                    Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                    Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    Status = "Y",

                    DoctorId = value.DoctorId,
                    TreatmentId = treatmentId
                };

                _baseinfoService.CreateDoctorTreatment(dt);
            }

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Update treatment id='" + value.DoctorId + "'."
            });

            return new JsonResult("OK");
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetDoctorDetailData(string id)
        {
            DoctorEditVM doctor = _baseinfoService.GetDoctorById(id);
            List<TreatmentCheckboxList> TreatmentCheckboxList = _baseinfoService.GetDoctorTreatmentListForCheckbox(id);

            doctor.TreatmentList = TreatmentCheckboxList;

            return new JsonResult(doctor);
        }

        #endregion

        #region -- 療程基本資料 -- (Treatment)

        [Authorize]
        public IActionResult TreatmentIndex()
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                List<TreatmentIndexVM> treatments = _baseinfoService.GetTreatmentListForIndex();
                ViewBag.Count = treatments.Count;

                return View(treatments);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult TreatmentCreate([FromForm] TreatmentEditVM value)
        {
            var user = HttpContext.User.Claims.ToList();
            string TreatmentId = _functions.GetGuid();

            while (_baseinfoService.CheckTreatmentId(TreatmentId))
                TreatmentId = _functions.GetGuid();

            string FileId = "";

            if (value.TreatmentImage.File != null)
                FileId = _functions.SaveFile(value.TreatmentImage.File, user.FirstOrDefault(u => u.Type == "Account").Value);
            else
                FileId = _baseinfoService.GetTreatmentImageIdByTreatmentId(value.TreatmentId);

            if (String.IsNullOrEmpty(value.Memo))
                value.Memo = "";

            Treatment item = new Treatment()
            {
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = "Y",

                Id = TreatmentId,
                TreatmentName = value.TreatmentName,
                Introduction = value.Introduction,
                ImageFileId = FileId,
                Time = value.Time,
                AlertMessage = value.AlertMessage,
                Hide = "Y",
                Sort = _baseinfoService.GetTreatmentCount() + 1,
                Memo = ""
            };
            _baseinfoService.CreateTreatment(item);

            //儲存療程的標籤
            //先刪除相關資料再新增
            if (value.SelectedLabel != null)
            {
                string[] labels = value.SelectedLabel.Split(',');
                _baseinfoService.RemoveTreatmentLabel(value.TreatmentId);

                foreach (var LabelId in labels)
                {
                    Treatmentlabel tl = new Treatmentlabel()
                    {
                        Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                        Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Status = "Y",

                        TreatmentId = TreatmentId,
                        LabelId = LabelId,
                    };

                    _baseinfoService.CreateTreatmentLabel(tl);
                }
            }

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Add treatment id='" + TreatmentId + "'."
            });

            return new JsonResult(TreatmentId);
        }

        //[HttpGet]
        //[Authorize]
        //public IActionResult TreatmentNameUpdate(string name, string id)
        //{
        //    string TreatmentId = id;

        //    var user = HttpContext.User.Claims.ToList();

        //    Treatment item = new Treatment()
        //    {
        //        Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
        //        ModifyDate = DateTime.Now,

        //        TreatmentName = name
        //    };

        //    _baseinfoService.UpdateTreatmentName(id, user.FirstOrDefault(u => u.Type == "Account").Value, item);

        //    _functions.SaveSystemLog(new Systemlog
        //    {
        //        CreateDate = DateTime.Now,
        //        Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
        //        UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
        //        Description = "Update treatment id='" + TreatmentId + "'."
        //    });

        //    return new JsonResult(TreatmentId);
        //}

        [HttpPost]
        [Authorize]
        public IActionResult TreatmentUpdate([FromForm] TreatmentEditVM value)
        {
            var user = HttpContext.User.Claims.ToList();
            string FileId = "";

            if (value.TreatmentImage.File != null)
                FileId = _functions.SaveFile(value.TreatmentImage.File, user.FirstOrDefault(u => u.Type == "Account").Value);
            else
                FileId = _baseinfoService.GetTreatmentImageIdByTreatmentId(value.TreatmentId);

            if (String.IsNullOrEmpty(value.Memo))
                value.Memo = "";

            Treatment item = new Treatment()
            {
                Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                ModifyDate = DateTime.Now,

                TreatmentName = value.TreatmentName,
                Introduction = value.Introduction,
                AlertMessage = value.AlertMessage,
                Time = value.Time,
                Sort = value.Sort,
                Memo = value.Memo,
                ImageFileId = FileId
            };

            _baseinfoService.UpdateTreatment(value.TreatmentId, user.FirstOrDefault(u => u.Type == "Account").Value, item);

            //儲存療程的標籤
            //先刪除相關資料再新增
            if (value.SelectedLabel != null)
            {
                string[] labels = value.SelectedLabel.Split(',');
                _baseinfoService.RemoveTreatmentLabel(value.TreatmentId);

                foreach (var LabelId in labels)
                {
                    Treatmentlabel tl = new Treatmentlabel()
                    {
                        Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                        Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Status = "Y",

                        TreatmentId = value.TreatmentId,
                        LabelId = LabelId,
                    };

                    _baseinfoService.CreateTreatmentLabel(tl);
                }
            }

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Update treatment id='" + value.TreatmentId + "'."
            });

            return new JsonResult("OK");
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetTreatmentDetailData(string id)
        {
            TreatmentEditVM treatment = _baseinfoService.GetTreatmentById(id);
            List<LabelCheckboxList> LabelCheckboxList = _baseinfoService.GetTreatmentLabelListForCheckbox(id);
            List<SelectListItem> TimeSelectList = _functions.CreateTimeUnitSelectList(true);

            treatment.LabelList = LabelCheckboxList;
            treatment.TimeSelectList = TimeSelectList;

            return new JsonResult(treatment);
        }

        [HttpGet]
        [Authorize]
        public IActionResult TreatmentLabelNameCreate(string name)
        {
            string LabelId = _functions.GetGuid();

            while (_baseinfoService.CheckLabelId(LabelId))
                LabelId = _functions.GetGuid();

            var user = HttpContext.User.Claims.ToList();

            Label item = new Label()
            {
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = "Y",

                Id = LabelId,
                Type = "Treatment",
                LabelName = name
            };
            _baseinfoService.CreateLabel(item);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Add label id='" + LabelId + "'."
            });

            return new JsonResult(LabelId);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetTreatmentLabelList(string id)
        {
            List<LabelCheckboxList> LabelCheckboxList = _baseinfoService.GetTreatmentLabelListForCheckbox(id);

            return new JsonResult(LabelCheckboxList);
        }

        [HttpPost]
        [Authorize]
        public IActionResult setTreatmentHideValue(string treatmentId, string hide)
        {
            var user = HttpContext.User.Claims.ToList();

            Treatment item = new Treatment()
            {
                Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                ModifyDate = DateTime.Now,

                Hide = hide
            };
            _baseinfoService.setTreatmentHideValue(treatmentId, item);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Set treatment hide='" + hide + "', id='" + treatmentId + "'."
            });

            return new JsonResult(treatmentId);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetTreatmentCheckboxList()
        {
            List<TreatmentCheckboxList> TreatmentCheckboxList = _baseinfoService.GetTreatmentListForCheckbox();

            return new JsonResult(TreatmentCheckboxList);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetTreatmentCreateVM()
        {
            TreatmentCreateVM createVM = new TreatmentCreateVM();
            createVM.LabelList = _baseinfoService.GetLabelListForCheckbox();
            createVM.TimeSelectList = _functions.CreateTimeUnitSelectList(true);

            return new JsonResult(createVM);
        }



        

        #endregion

        #region -- 醫師班表資料 -- (DoctorShift)

        [Authorize]
        public IActionResult SetDoctorShift()
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                //List<DoctorIndexVM> doctors = _baseinfoService.GetDoctorListForIndex();
                //ViewBag.Count = doctors.Count;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetDoctorShift(string year, string month)
        {
            SetDoctorShiftVM item = _baseinfoService.GetDoctorShift(year, month);

            return new JsonResult(item);
        }

        [HttpPost]
        [Authorize]
        public IActionResult RemoveArrangeDoctorShift(string year, string month)
        {
            _baseinfoService.RemoveArrangeDoctorShift(year, month);

            return new JsonResult("OK");
        }

        [HttpPost]
        [Authorize]
        public IActionResult SaveArrangeDoctorShift([FromForm] PostArrangeShiftData value)
        {
            var user = HttpContext.User.Claims.ToList();
            string[] doctors = value.DoctorIdList.Split(',');

            foreach (string doctor in doctors)
            {
                string ArrangeDoctorShiftId = _functions.GetGuid();

                while (_baseinfoService.CheckArrangeDoctorShiftId(ArrangeDoctorShiftId))
                    ArrangeDoctorShiftId = _functions.GetGuid();

                Arrangedoctorshift item = new Arrangedoctorshift()
                {
                    Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                    Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    Status = "Y",

                    Id = ArrangeDoctorShiftId,
                    DoctorId = doctor,
                    Year = value.Year,
                    Month = value.Month,
                    Day = value.Day,
                    ShiftTypeId = value.ShiftTypeId
                };
                _baseinfoService.SaveArrangeDoctorShift(item);

                _functions.SaveSystemLog(new Systemlog
                {
                    CreateDate = DateTime.Now,
                    Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                    UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                    Description = "Add doctor shift id='" + ArrangeDoctorShiftId + "'."
                });
            }



            return new JsonResult("OK");


            //string FileId = "";

            //if (value.DoctorImage.File != null)
            //    FileId = _functions.SaveFile(value.DoctorImage.File, user.FirstOrDefault(u => u.Type == "Account").Value);
            //else
            //    FileId = _baseinfoService.GetDoctorImageIdByDoctorId(value.DoctorId);

            //if (String.IsNullOrEmpty(value.DoctorNameEnglish))
            //    value.DoctorNameEnglish = "";
            //if (String.IsNullOrEmpty(value.Memo))
            //    value.Memo = "";

            //Doctor item = new Doctor()
            //{
            //    Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
            //    ModifyDate = DateTime.Now,

            //    DoctorNameEnglish = value.DoctorNameEnglish,
            //    Introduction = value.Introduction,
            //    DepartmentTitle = value.DepartmentTitle,
            //    Sort = value.Sort,
            //    Memo = value.Memo,
            //    ImageFileId = FileId
            //};
            //_baseinfoService.UpdateDoctor(value.DoctorId, user.FirstOrDefault(u => u.Type == "Account").Value, item);

            ////儲存醫師負責的療程
            ////先刪除相關資料再新增
            //string[] treatments = value.SelectedTreatment.Split(',');
            //_baseinfoService.RemoveDoctorTreatment(value.DoctorId);

            //foreach (var treatmentId in treatments)
            //{
            //    Doctortreatment dt = new Doctortreatment()
            //    {
            //        Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
            //        Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
            //        CreateDate = DateTime.Now,
            //        ModifyDate = DateTime.Now,
            //        Status = "Y",

            //        DoctorId = value.DoctorId,
            //        TreatmentId = treatmentId
            //    };

            //    _baseinfoService.CreateDoctorTreatment(dt);
            //}

            //_functions.SaveSystemLog(new Systemlog
            //{
            //    CreateDate = DateTime.Now,
            //    Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
            //    UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
            //    Description = "Update treatment id='" + value.DoctorId + "'."
            //});
        }

        [HttpPost]
        [Authorize]
        public IActionResult LockShift(string year, string month)
        {
            var user = HttpContext.User.Claims.ToList();

            //儲存ArrangeMonthShift(鎖定該月份班表)
            string ArrangeMonthShiftId = _functions.GetGuid();
            while (_baseinfoService.CheckArrangeMonthShiftId(ArrangeMonthShiftId))
                ArrangeMonthShiftId = _functions.GetGuid();

            Arrangemonthshift item = new Arrangemonthshift()
            {
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = "Y",

                Id = ArrangeMonthShiftId,
                Year = year,
                Month = month,
                Locked = "Y"
            };
            _baseinfoService.SaveArrangeMonthShift(item);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Year=" + year + ", month=" + month + " shift is locked. id='" + ArrangeMonthShiftId + "'."
            });


            //產生醫師門診表
            List<ShiftTypeData> shifts = _baseinfoService.GetShiftTypeData();
            SetDoctorShiftVM ds = _baseinfoService.GetDoctorShift(year, month);
            TimeSpan MinutesPerUnit = new TimeSpan(0, int.Parse(_functions.GetSystemParameter("MinutesPerUnit")), 0);

            foreach (var d in ds.arrangeDoctorShiftData)
            {
                string doctor = d.DoctorId!;
                string day = d.Day!;
                TimeSpan shiftbegin = TimeSpan.Parse(shifts.FirstOrDefault(x => x.ShiftId == d.ShiftTypeId).BeginTime)!;
                TimeSpan shiftend = TimeSpan.Parse(shifts.FirstOrDefault(x => x.ShiftId == d.ShiftTypeId).EndTime)!;

                while (true)
                {
                    if (shiftbegin.Add(MinutesPerUnit) > shiftend)
                        break;

                    string OutpatientId = _functions.GetGuid();

                    while (_baseinfoService.CheckOutpatientIdId(OutpatientId))
                        OutpatientId = _functions.GetGuid();

                    Doctoroutpatient data = new Doctoroutpatient()
                    {
                        Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                        Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                        CreateDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                        Status = "Y",

                        Id = OutpatientId,
                        DoctorId = doctor,
                        ArrangeId = d.Id!,
                        Year = year,
                        Month = month,
                        Day = day,
                        BeginTime = shiftbegin.ToString(@"hh\:mm"),
                        EndTime = shiftbegin.Add(MinutesPerUnit).ToString(@"hh\:mm"),

                    };
                    _baseinfoService.SaveDoctorOutpatient(data);

                    shiftbegin = shiftbegin.Add(MinutesPerUnit);
                }

                _functions.SaveSystemLog(new Systemlog
                {
                    CreateDate = DateTime.Now,
                    Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                    UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                    Description = "Year=" + year + ", month=" + month + ", day=" + day + " , doctorId=" + doctor + " outpatient is created."
                });

            }

            return new JsonResult("OK");
        }



        #endregion

        #region -- 醫師請假 -- (DoctorDayOff)

        [Authorize]
        public IActionResult DoctorDayOffIndex()
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                List<DoctorDayOffIndexVM> ddo = _baseinfoService.GetDoctorDayOffForIndex();
                ViewBag.Count = ddo.Count;

                return View(ddo);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [Authorize]
        public IActionResult DoctorDayOffCreate()
        {
            //判斷cookie是員工還是顧客
            var user = HttpContext.User.Claims.ToList();

            if (user.FirstOrDefault(u => u.Type == "LoginType").Value == "EkUser")
            {
                List<SelectListItem> doctorList = CreateDoctorSelectList();
                List<SelectListItem> beginTimeList = CreateTimeSelectList();
                List<SelectListItem> endTimeList = CreateTimeSelectList();

                DoctorDayOffCreateVM model = new DoctorDayOffCreateVM() //上面的 Model
                {

                    TypeList = _functions.CreateSelectList("DoctorDayOffSelectList", true),
                    BeginTimeList = beginTimeList,
                    EndTimeList = endTimeList,
                    DoctorList = doctorList
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Appointment");
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult DoctorDayOffCreate(DoctorDayOffCreateVM value)
        {
            var user = HttpContext.User.Claims.ToList();

            Doctordayoff item = new Doctordayoff()
            {
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                Modifier = user.FirstOrDefault(u => u.Type == "Account").Value,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = "Y",

                DoctorId = value.DoctorId!,
                Type = value.Type!,
                Date = value.Date!,
                BeginTime = value.BeginTime!,
                EndTime = value.EndTime!
            };
            _baseinfoService.CreateDoctorDayOff(item);

            _functions.SaveSystemLog(new Systemlog
            {
                CreateDate = DateTime.Now,
                Creator = user.FirstOrDefault(u => u.Type == "Account").Value,
                UserAccount = user.FirstOrDefault(u => u.Type == "Account").Value,
                Description = "Add doctor day off doctor id='" + value.DoctorId + "'."
            });

            return RedirectToAction("DoctorDayOffIndex");
        }

        [HttpPost]
        [Authorize]
        public IActionResult CheckDayOffData(string doctorId, string date, string beginTime, string endTime)
        {
            string result = _baseinfoService.CheckDayOffData(doctorId, date, beginTime, endTime);

            return new JsonResult(result);
        }

        [HttpPost]
        [Authorize]
        public IActionResult DeleteDoctorDayOff(long Index)
        {
            string result = _baseinfoService.DeleteDoctorDayOff(Index);

            return new JsonResult(result);
        }







        #endregion

        [Authorize]
        private List<SelectListItem> CreateTimeSelectList()
        {
            List<SelectListItem> SelectItemList = new List<SelectListItem>();

            SelectItemList.Add(new SelectListItem()
            {
                Text = "請選擇...",
                Value = "",
                Selected = true
            });

            DateTime startTime = DateTime.Parse("06:00");
            DateTime endTime = DateTime.Parse("23:00");
            TimeSpan interval = new TimeSpan(0, int.Parse(_functions.GetSystemParameter("MinutesPerUnit")), 0);

            while (startTime <= endTime)
            {
                SelectItemList.Add(new SelectListItem()
                {
                    Text = startTime.ToString("HH:mm"),
                    Value = startTime.ToString("HH:mm"),
                    Selected = false
                });

                startTime = startTime.Add(interval);
            }

            return SelectItemList;
        }

        [Authorize]
        private List<SelectListItem> CreateDoctorSelectList()
        {
            List<DoctorCheckboxList> DoctorList = _baseinfoService.GetDoctorListForDropdown();
            List<SelectListItem> SelectItemList = new List<SelectListItem>();

            SelectItemList.Add(new SelectListItem()
            {
                Text = "請選擇...",
                Value = "",
                Selected = true
            });

            foreach (var item in DoctorList)
            {
                SelectItemList.Add(new SelectListItem()
                {
                    Text = item.DoctorName,
                    Value = item.DoctorId,
                    Selected = false
                });
            }

            return SelectItemList;
        }
    }
}

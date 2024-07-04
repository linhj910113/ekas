using AppointmentSystem.Models.DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace AppointmentSystem.Services
{
    public class WebFunctions
    {
        private readonly EkasContext _db;
        private string FilePath = @"Attachment\";

        public WebFunctions(EkasContext context)
        {
            _db = context;
        }

        public string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public string GetVerificationCode()
        {
            int codeLength = 32;
            string code = "";

            while (true)
            {
                string letters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                Random random = new Random();

                for (int i = 0; i < codeLength; i++)
                    code += letters[random.Next(0, letters.Length)];

                if (_db.Verificationcodes.Where(x => x.HashCode == code).Count() == 0)
                    break;
            }

            return code;
        }

        //public long GetMaxLogIndex()
        //{
        //    if (_db.Systemlogs.Count() > 0)
        //        return _db.Systemlogs.Max(x => x.Index);
        //    else
        //        return 0;
        //}

        public void SaveSystemLog(Systemlog log)
        {
            _db.Systemlogs.Add(log);
            _db.SaveChanges();
        }

        public string SHA256Hash(string value)
        {
            var hash = SHA256.Create();
            var byteArray = hash.ComputeHash(Encoding.UTF8.GetBytes(value));

            return Convert.ToHexString(byteArray).ToLower();
        }

        public bool CheckFileID(string FileID)
        {
            if (_db.Systemfiles.Where(x => x.Id == FileID).Count() == 0)
                return false;
            else
                return true;
        }

        public string SaveFile(IFormFile? File, string UserAccount)
        {
            if (!Directory.Exists(FilePath))
                Directory.CreateDirectory(FilePath);

            //檔案上傳至資料夾
            string FileID = Guid.NewGuid().ToString();

            while (CheckFileID(FileID))
                FileID = Guid.NewGuid().ToString();

            string path = FilePath + FileID + Path.GetExtension(File.FileName).ToLowerInvariant(); ;

            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                File.CopyTo(stream);
            }

            //儲存至資料庫
            _db.Systemfiles.Add(new Systemfile()
            {
                Creator = UserAccount,
                Modifier = UserAccount,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,

                Id = FileID,
                FileName = Path.GetFileNameWithoutExtension(File.FileName),
                FileExtension = Path.GetExtension(File.FileName).ToLowerInvariant(),
                FileSize = File.Length,
                Path = path,
                Status = "Y"
            });
            _db.SaveChanges();

            SaveSystemLog(new Systemlog()
            {
                CreateDate = DateTime.Now,
                Creator = UserAccount,
                UserAccount = UserAccount,
                Description = "Upload File id='" + FileID + "'."
            });

            return FileID;
        }

        public string GetSystemParameter(string name)
        {
            var item = _db.Systemparameters.FirstOrDefault(x => x.Name == name);

            if (item == null)
                return "";
            else
                return item.Value;
        }

        public string ConvertJpgToBase64(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("文件未找到", filePath);
            }

            byte[] imageBytes = File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(imageBytes);

            return base64String;
        }

        public async Task<bool> SendLineMessageAsync(string to, string message)
        {
            string MessagingApiChannelAccessToken = GetSystemParameter("MessagingApiChannelAccessToken");

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

        #region -- 前端功能列使用 -- 

        public List<Module> GetAllModules()
        {
            var items = _db.Modules.Where(x => x.Status == "Y").OrderBy(x => x.Sort);

            return items.ToList();
        }

        public List<Module> GetMoudlesByUserId(string userId)
        {
            var items = (from up in _db.Userpermissions.Where(x => x.Status == "Y" && x.UserId == userId && x.IsAllow == "Y")
                         join f in _db.Functions on up.FunctionId equals f.Id
                         join m in _db.Modules on f.ModuleId equals m.Id
                         orderby m.Sort
                         select m).Distinct();

            return items.ToList();
        }

        public List<Function> GetALLFunctionByModule(string moduleId)
        {
            var items = _db.Functions.Where(x => x.Status == "Y" && x.ModuleId == moduleId).OrderBy(x => x.Sort);

            return items.ToList();
        }

        public List<Function> GetFunctionByModule(string moduleId, string userId)
        {
            var items = from up in _db.Userpermissions.Where(x => x.Status == "Y" && x.UserId == userId && x.IsAllow == "Y")
                        join f in _db.Functions on up.FunctionId equals f.Id
                        where f.ModuleId == moduleId && f.Status == "Y"
                        orderby f.Sort
                        select f;

            return items.ToList();
        }

        #endregion

        public List<SelectListItem> CreateSelectList(string groupName, bool withBlank)
        {
            var data = _db.Systemselectlists.Where(x => x.GroupName == groupName);
            List<SelectListItem> SelectItemList = new List<SelectListItem>();

            if (withBlank)
            {
                SelectItemList.Add(new SelectListItem()
                {
                    Text = "請選擇...",
                    Value = "",
                    Selected = true
                });
            }

            foreach (var d in data)
            {
                SelectItemList.Add(new SelectListItem()
                {
                    Text = d.SelectName,
                    Value = d.SelectValue
                });
            }

            return SelectItemList;
        }

        public List<SelectListItem> CreateTimeUnitSelectList(bool withBlank)
        {
            var data = _db.Systemparameters.FirstOrDefault(x => x.Name == "MinutesPerUnit");
            List<SelectListItem> SelectItemList = new List<SelectListItem>();
            //產生多少筆數的時間供選擇(預設8筆，時間跨度15分鐘)
            int num = 8;
            int time = 15;

            if (data != null)
                time = int.Parse(data.Value);

            if (withBlank)
            {
                SelectItemList.Add(new SelectListItem()
                {
                    Text = "請選擇...",
                    Value = "",
                    Selected = true
                });
            }

            for (int i = 1; i <= num; i++)
            {
                SelectItemList.Add(new SelectListItem()
                {
                    Text = (time * i).ToString(),
                    Value = (time * i).ToString()
                });
            }

            return SelectItemList;
        }
    }
}

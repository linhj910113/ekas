using AppointmentSystem.Models.DBModels;

namespace AppointmentSystem.Services
{
    public class HomeService
    {
        private readonly EkasContext _db;

        public HomeService(EkasContext context)
        {
            _db = context;
        }

        public bool UserLogin(string account, string password)
        {
            var a = _db.Accounts.Where(x => x.Account1 == account && x.Password == password).FirstOrDefault();

            if (a == null)
                return false;
            else
                return true;
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

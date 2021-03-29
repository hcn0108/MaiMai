using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaiMai.Models.ViewModel
{
    public class MemberViewModel
    {
        public HttpPostedFileBase upphoto { get; set; }
        public int UserID { get; set; }
        public string userAccount { get; set; }
        public string userPassWord { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public string phoneNumber { get; set; }

        public string ProImg { get; set; }

        public string connectionID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> birthday { get; set; }
        public string identityNumber { get; set; }
        public string profileImg { get; set; }

        public Nullable<int> userLevel { get; set; } 
        private string str;
        public string userLevelString {
        get
            {
                return str; 
            }
        set 
            {
                if (Convert.ToInt32(value) == 1) str = "管理員";
                else if (Convert.ToInt32(value) == 2) str = "優質會員";
                else  str = "一般會員";
            }
    }
        public Nullable<double> totalStarRate { get; set; }
        public string selfDescription { get; set; }
        public string email { get; set; }
    }
}
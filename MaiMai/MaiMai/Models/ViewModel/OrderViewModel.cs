using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaiMai.Models.ViewModel
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public Nullable<int> buyerUserID { get; set; }

        private string str;
        public Nullable<int> orderStatus { get; set; }
        public string orderStatusString
        {
            get
            {
                return str;
            }
            set
            {
                if (Convert.ToInt32(value) == 0) str = "待結帳";
                else if (Convert.ToInt32(value) == 1) str = "處理中";
                else str = "已完成";
            }
        }

public Nullable<System.DateTime> createdTime { get; set; }
        
        public Nullable<int> SellerID { get; set; }
        public int price { get; set; }

  
    }
}
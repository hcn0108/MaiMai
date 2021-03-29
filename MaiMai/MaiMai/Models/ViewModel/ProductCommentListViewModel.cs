using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaiMai.Models.ViewModel
{
    public class ProductCommentListViewModel
    {
        public int ProductPostID { get; set; }
        public string productName { get; set; }
        public string productDescription { get; set; }
        public string productImg { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> inStoreQTY { get; set; }
        public Nullable<int> price { get; set; }
        public Nullable<int> TagID { get; set; }
        public Nullable<System.DateTime> createdTime { get; set; }
        public Nullable<int> RequiredPostID { get; set; }

        public string county { get; set; }
        public string district { get; set; }

        public Nullable<int> status { get; set; }
        public HttpPostedFileBase upphoto { get; set; }
        public string userAccount { get; set; }
    }
}
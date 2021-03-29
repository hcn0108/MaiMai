using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaiMai.Models.ViewModel
{
    public class CommentViewModel
    {

        public int CommentID { get; set; }
        public Nullable<int> OrderID { get; set; }
        public Nullable<double> starRate { get; set; }
        public string commentDescription { get; set; }
        public int UserID { get; set; }
        public Nullable<int> CommentorUserID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string profileImg { get; set; }
        public virtual Member Member { get; set; }
    }
}
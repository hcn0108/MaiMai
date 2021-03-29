using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaiMai.Models.ViewModel
{
    public class upLoadPhotoViewModel
    {

        public string img { get; set; }
       public HttpPostedFileBase upphoto { get; set; }

    }
}
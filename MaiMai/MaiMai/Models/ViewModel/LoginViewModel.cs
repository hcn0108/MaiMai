using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MaiMai.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        public string userAccount { get; set; }
        [Required]
        public string userPassWord { get; set; }
        public int UserID { get; set; }
        public string RememberMe { get; set; }
    }
}
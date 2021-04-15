using MaiMai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MaiMai.Controllers
{
    public class googleMapController : Controller
    {

        maimaiEntities db = new maimaiEntities();
        // GET: googleMap
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult getmarket() 
        {
            var mapList = db.RequiredPost.Select(m => new
            {
                county=m.county,
                district=m.district,
                address= m.address,
                requiredPostName=m.postName,
                requiredPostID=m.RequiredPostID,
                postImg=m.postImg,
                estimatePrice=m.estimatePrice,
                posterName= m.Member.userAccount,
                userAvrta=m.Member.profileImg
            }).ToList();
            return Json(mapList, JsonRequestBehavior.AllowGet);
        }
        
    }
}
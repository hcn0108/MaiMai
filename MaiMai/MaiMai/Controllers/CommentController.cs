using MaiMai.Models;
using MaiMai.Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MaiMai.Controllers
{
    public class CommentController : Controller
    {
        // GET: Comment
        public ActionResult Comment()
        {
            return View();
        }

      
        maimaiEntities db = new maimaiEntities();
        public ActionResult ViewRate(int UserID)
        {


            var table = db.Comment.Where(m => m.CommentorUserID == UserID).Select(m => new CommentViewModel()
            {
                CommentID = m.CommentID,
                OrderID = m.OrderdetalID,
                starRate = m.starRate,
                commentDescription = m.commentDescription,
                UserID = m.UserID,
                CommentorUserID = m.CommentorUserID,
                profileImg = m.Member.profileImg,
                firstName = m.Member.firstName,
                lastName = m.Member.lastName
                //Member=m.Member.firstName,
            }).ToList();
            return Content(JsonConvert.SerializeObject(table), "application/json");

            //return Json(table, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TotalStarRate(int UserID)
        {
            var a = db.Comment.Where(m => m.CommentorUserID == UserID).GroupBy(i => i.CommentorUserID).Select(g => new {

                RateCount = g.Count(),
                Average = g.Average(i => i.starRate),
                RateMax = g.Max(i => i.starRate)
            });


            return Content(JsonConvert.SerializeObject(a), "application/json");

            //return Json(table, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Comment_WithoutLogin()
        {
            return View();
        }
    }
}
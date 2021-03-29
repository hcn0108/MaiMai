using MaiMai.Models;
using MaiMai.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MaiMai.Controllers
{
    public class PersonalMarketController : Controller
    {
        // GET: PersonalMarket
        public ActionResult Personal_Index()
        {
            return View();
        }
         maimaiEntities db = new maimaiEntities();
        public ActionResult GetMember( int UserID)
        {
            Member member = db.Member.FirstOrDefault(m => m.UserID == UserID);
            MemberViewModel mb = new MemberViewModel()
            {
                userAccount = member.userAccount,
                userPassWord = member.userPassWord,
                city = member.city,
                address = member.address,
                phoneNumber = member.phoneNumber,
                firstName = member.firstName,
                lastName = member.lastName,
                birthday = member.birthday,
                identityNumber = member.identityNumber,
                profileImg = member.profileImg,
                userLevel = member.userLevel,
                totalStarRate = member.totalStarRate,
                selfDescription = member.selfDescription,
                email = member.email
            };
        
            
             return Json(mb, JsonRequestBehavior.AllowGet);
            }

        public ActionResult checkAllComment(int UserID)
        {

             
            var table = db.ProductPost.Where(m => m.UserID == UserID&&m.status==1).Select(m => new ProductCommentListViewModel()
            {
                ProductPostID = m.ProductPostID,
                productName = m.productName,
                productDescription = m.productDescription,
                productImg = m.productImg,
                UserID = m.UserID,
                inStoreQTY = m.inStoreQTY,
                price = m.price,
                TagID = m.TagID,
                createdTime = m.createdTime,
                RequiredPostID = m.RequiredPostID,
                userAccount = m.Member.userAccount
            }).ToList();


            return Json(table, JsonRequestBehavior.AllowGet);
        }

        maimaiRepository<ProductPost> ProductPostRepository = new maimaiRepository<ProductPost>();


        public string deletepost(int UserID)
        {
            
           ProductPost abc=ProductPostRepository.GetbyID(UserID);

            abc.status = 0;

            ProductPostRepository.Update(abc);
            return "成功";

        }

       
        public ActionResult Personal_Index_WithoutLogin()
        {
            return View();
        }
    }
    }

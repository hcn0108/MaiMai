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

             
            var table = db.ProductPost.Where(m => m.UserID == UserID&&m.status==1&&m.inStoreQTY>0).Select(m => new ProductCommentListViewModel()
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
        maimaiRepository<ProductPost> productPostRepository = new maimaiRepository<ProductPost>();
        public string commemtProductPost(ProductCommentListViewModel ps)
        {
            try
            {
                ProductPost product = new ProductPost()
                {
                    //ProductPostID = ps.ProductPostID,
                    productName = ps.productName,
                    productDescription = ps.productDescription,
                     status= 1,
                    inStoreQTY = ps.inStoreQTY,
                    price = ps.price,
                    TagID = ps.TagID,
                    RequiredPostID = ps.RequiredPostID,

                    createdTime = DateTime.Now,
                    county = ps.county,
                    district = ps.district,
                    UserID = Convert.ToInt32(Request.Cookies["LoginAccount"].Value)

                };
                if (ps.upphoto == null)
                {
                    product.productImg = "無圖示.jpg";
                }
                else
                {
                    

                    product.productImg = ps.upphoto.FileName;
                    string filename = ps.upphoto.FileName;
                    ps.upphoto.SaveAs(Server.MapPath("../Content/ProductPostImg/") + filename);
                    string filePath = $"../Content/ProductPostImg/{filename}";

                }
                //HttpPostedFileBase photo = new HttpPostedFileBase(upphoto);

                productPostRepository.Create(product);

                return "留言成功";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

    }
}

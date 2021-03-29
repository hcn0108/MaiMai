using DocumentFormat.OpenXml.Bibliography;
using MaiMai.Models;
using MaiMai.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MaiMai.Controllers
{
    public class RequiredPostController : Controller
    {
        // GET: RequiredPost


        maimaiEntities db = new maimaiEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult requiredPostIndex()
        {

            return View();
        }

        public ActionResult requiredPostIndexWithLogin()
        {

            return View();
        }

        public JsonResult allrequiredPostWithLogin(int loginID)
        {

            var table = db.RequiredPost.Where(m => m.UserID == loginID && m.isPast == false).OrderByDescending(e => e.postTime).Select(m => new RequiredPostViewModel_C()
            {
                RequiredPostID = m.RequiredPostID,
                postTime = m.postTime,
                postDescription = m.postDescription,
                postName = m.postName,
                postImg = m.postImg,
                UserID = m.UserID,
                requiredQTY = m.requiredQTY,
                estimatePrice = m.estimatePrice,
                TagID = m.TagID,
                OrderID = m.OrderID,
                userAccount = m.Member.userAccount,
                county = m.county,
                district = m.district

            }).ToList();


            return Json(table, JsonRequestBehavior.AllowGet);
        }
        public JsonResult allrequiredPost()
        {

            var table = db.RequiredPost.Where(m => m.isPast == false).OrderByDescending(e => e.postTime).Select(m => new RequiredPostViewModel_C()
            {
                RequiredPostID = m.RequiredPostID,
                postTime = m.postTime,
                postDescription = m.postDescription,
                postName = m.postName,
                postImg = m.postImg,
                UserID = m.UserID,
                requiredQTY = m.requiredQTY,
                estimatePrice = m.estimatePrice,
                TagID = m.TagID,
                OrderID = m.OrderID,
                userAccount = m.Member.userAccount,
                county = m.county,
                district = m.district
            }).ToList();


            return Json(table, JsonRequestBehavior.AllowGet);
        }// allpostend

        public ActionResult allpoccessPost()
        {

            var table = db.RequiredPost.Where(m => m.OrderID != null && m.isPast == false).Select(m => new RequiredPostViewModel_C()
            {
                RequiredPostID = m.RequiredPostID,
                postTime = m.postTime,
                postDescription = m.postDescription,
                postName = m.postName,
                postImg = m.postImg,
                UserID = m.UserID,
                requiredQTY = m.requiredQTY,
                estimatePrice = m.estimatePrice,
                TagID = m.TagID,
                OrderID = m.OrderID,
                userAccount = m.Member.userAccount,
                county = m.county,
                district = m.district
            }).ToList();


            return Json(table, JsonRequestBehavior.AllowGet);

        }

        public ActionResult allPastPost(int loginID)
        {

            var table = db.RequiredPost.Where(m => m.UserID == loginID && m.isPast == true).Select(m => new RequiredPostViewModel_C()
            {
                RequiredPostID = m.RequiredPostID,
                postTime = m.postTime,
                postDescription = m.postDescription,
                postName = m.postName,
                postImg = m.postImg,
                UserID = m.UserID,
                requiredQTY = m.requiredQTY,
                estimatePrice = m.estimatePrice,
                TagID = m.TagID,
                OrderID = m.OrderID,
                userAccount = m.Member.userAccount,
                county = m.county,
                district = m.district
            }).ToList();


            return Json(table, JsonRequestBehavior.AllowGet);

        }

        public string uploadPhoto(upLoadPhotoViewModel data)
        {
            if (data.upphoto == null)
            {
                return "../Content/resource_nico/images/無圖示.jpg";
            }
            //HttpPostedFileBase photo = new HttpPostedFileBase(upphoto);
            string filename = data.upphoto.FileName;
            data.upphoto.SaveAs(Server.MapPath("../Content/ProductPostImg/") + filename);
            string filePath = $"../Content/ProductPostImg/{filename}";

            return filePath;
        }


        public ActionResult getAllTag()
        {

            var table = db.Tag.Select(m => new TagViewModel()
            {

                TagID = m.TagID,
                tagName = m.tagName

            }).ToList();


            return Json(table, JsonRequestBehavior.AllowGet);
        }

        maimaiRepository<ProductPost> productPostRepository = new maimaiRepository<ProductPost>();
        public string commemtProductPost(ProductCommentListViewModel ps)
        {
            ProductPost product = new ProductPost()
            {
                ProductPostID = ps.ProductPostID,
                productName = ps.productName,
                productDescription = ps.productDescription,
                status=ps.status,
                inStoreQTY = ps.inStoreQTY,
                price = ps.price,
                TagID = ps.TagID,
                RequiredPostID = ps.RequiredPostID,
                productImg = ps.upphoto.FileName,
                createdTime = DateTime.Now,
                UserID = Convert.ToInt32(Request.Cookies["LoginAccount"].Value)

            };
            if (ps.upphoto == null)
            {
                product.productImg = "無圖示.jpg";
            }
            else {
                product.status = 1;
            
            product.productImg = ps.upphoto.FileName;
            string filename = ps.upphoto.FileName;
            ps.upphoto.SaveAs(Server.MapPath("../Content/ProductPostImg/") + filename);
            string filePath = $"../Content/ProductPostImg/{filename}";

            }
            //HttpPostedFileBase photo = new HttpPostedFileBase(upphoto);

            productPostRepository.Create(product);

            return "留言成功";
        }

        public ActionResult checkAllComment(string data)
        {

            var RequiredPostID = Convert.ToInt32(data);
            var table = db.ProductPost.Where(m => m.RequiredPostID == RequiredPostID).Select(m => new 
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
                userAccount = m.Member.userAccount,
                useraccountID=m.Member.UserID,
                county = m.county,
                district = m.district

            }).ToList();


            return Json(table, JsonRequestBehavior.AllowGet);
        }
        maimaiRepository<RequiredPost> requiredPostRepository = new maimaiRepository<RequiredPost>();
        public string sendRequiredPost(RequiredPostViewModel_C rp)
        {
            RequiredPost post = new RequiredPost()
            {

                postDescription = rp.postDescription,
                postName = rp.postName,
                postImg = rp.upphoto.FileName,
                requiredQTY = rp.requiredQTY,
                TagID = rp.TagID,
                estimatePrice = rp.estimatePrice,
                OrderID = rp.OrderID,
                county = rp.county,
                district = rp.district,
                isPast = false

            };
            post.postTime = DateTime.Now;
            post.UserID = Convert.ToInt32(Request.Cookies["LoginAccount"].Value);

            requiredPostRepository.Create(post);

            if (rp.upphoto == null)
            {
                rp.postImg = "無圖示.jpg";
            }
            else
            {

                string filename = rp.upphoto.FileName;
                rp.upphoto.SaveAs(Server.MapPath("../Content/resource_nico/images/徵求台POST/") + filename);
                string filePath = $"../Content/resource_nico/images/徵求台POST/{filename}";

            }
            return "發文成功";
        }

        public string checkCommentSPan(int i)
        {

            var table = db.ProductPost.Where(m => m.RequiredPostID == i);
            int count = table.Count();


            return count.ToString();
        }
        [HttpGet]
        public ActionResult getRequireDetail(string odID) {

            var id = Convert.ToInt32(odID);
            var rPost = db.RequiredPost.
            Select(m => new
            {
                RequiredPostID=m.RequiredPostID,
                postDescription = m.postDescription,
                postName = m.postName,
                postImg = m.postImg,
                requiredQTY = m.requiredQTY,
                TagName = m.Tag.tagName,
                TagID= m.TagID,
                estimatePrice = m.estimatePrice,
                OrderID = m.OrderID,
                county = m.county,
                district = m.district,
                isPast = false

            }).FirstOrDefault(m => m.RequiredPostID == id);
            return Json(rPost, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]

        public string sendRequireDetail( RequiredPostViewModel_C rp) {

            try
            {
                RequiredPost post = requiredPostRepository.GetbyID(rp.RequiredPostID);


                    post.postDescription = rp.postDescription;
                    post.postName = rp.postName;
                
                    post.requiredQTY = rp.requiredQTY;
                    post.TagID = rp.TagID;
                    post.estimatePrice = rp.estimatePrice;
                    post.OrderID = rp.OrderID;
                    post.county = rp.county;
                    post.district = rp.district;
                    post.isPast = false;
                   

         
                post.postTime = DateTime.Now;
                post.UserID = Convert.ToInt32(Request.Cookies["LoginAccount"].Value);

                requiredPostRepository.Update(post);

                if (rp.upphoto != null) { 
               
                    string filename = rp.upphoto.FileName;
                    rp.upphoto.SaveAs(Server.MapPath("../Content/resource_nico/images/徵求台POST/") + filename);
                    string filePath = $"../Content/resource_nico/images/徵求台POST/{filename}";

                }

                return "修改成功";
            }
            catch (Exception e) {

                return "修改失敗";
            }
        
        }

        public string deleteRequireDetail(string data) {

            try
            {
                var id = Convert.ToInt32(data);
                var reqiredPost = requiredPostRepository.GetbyID(id);
                reqiredPost.isPast = true;
                requiredPostRepository.Update(reqiredPost);
                return "已變成過去貼文";
            }
            catch (Exception e) {

                return e.Message;
            }
        
        }

        // required singal page


        public ActionResult requiredSingalPage() {



            return View();
        }

        public ActionResult getsingalPost(string requiredPostID) {

            try
            {
                var id = Convert.ToInt32(requiredPostID);
                var rPost = requiredPostRepository.GetbyID(id);
                RequiredPostViewModel_C rp = new RequiredPostViewModel_C()
                {
                    RequiredPostID=rPost.RequiredPostID,
                    postDescription=rPost.postDescription,
                    postTime = rPost.postTime,
                    postName = rPost.postName,
                    postImg = rPost.postImg,
                    UserID = rPost.UserID,
                    requiredQTY = rPost.requiredQTY,
                    estimatePrice = rPost.estimatePrice,
                    tagName = rPost.Tag.tagName,
                    county = rPost.county,
                    district = rPost.district,
                    userAccount = rPost.Member.userAccount,
                    userAvrta=rPost.Member.profileImg

                };



                return Json(rp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) {

                Response.StatusCode = 404;
                return Content(e.Message);
                
            }

           
        }


    }//class end
}//namespace end
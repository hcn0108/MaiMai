using MaiMai.Models;
using MaiMai.Models.MaimaiIndexViewModel;
using MaiMai.Models.ViewModel;
using System.Linq;
using System.Web.Mvc;

namespace MaiMai.Controllers
{
    public class MaimaiIndexController : Controller
    {
        maimaiEntities db = new maimaiEntities();
        // GET: MaimaiIndex     
        public ActionResult TagResult()
        {
            var tagList = db.Tag.Select(m => new MaimaiIndexViewModel()
            {
                TagID = m.TagID,
                tagName = m.tagName,
                TagImg = m.TagImg,

            }).ToList();
            return Json(tagList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProductPost() 
        {
            var PostList = db.ProductPost.Select(m => new MaimaiIndexViewModel()
            {
               productImg = m.productImg,
               price = m.price,
               productDescription = m.productDescription
            }).ToList();
            return Json(PostList,JsonRequestBehavior.AllowGet);
        }

        //布告欄輪播
        public ActionResult RequiredPost()
        {
            var PostList = db.RequiredPost.Select(m => new RequiredPostViewModel_C()
            {
                postImg = m.postImg,
                postDescription = m.postDescription
            }).ToList();
            return Json(PostList, JsonRequestBehavior.AllowGet);
        }

        //匿名類別 左邊的是新的變數名稱 例如 : (新)inStoreQTY = m.inStoreQTY,        
        public ActionResult tagsearch123(int TagID)         
        {
            var PostList = db.ProductPost.Where(m => m.TagID == TagID && m.inStoreQTY > 0).Select(m => new
            {
                PostID = m.ProductPostID,
                price = m.price,
                img = m.productImg,
                name = m.productName,
            }).ToList();            
            return Json(PostList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Bells(int? TagID =  5)
        {
            var PostList = db.ProductPost.Where(m => m.UserID == TagID && m.inStoreQTY > 0).Select(m => new
            {
                price = m.price,
                productImg = m.productImg,
                productName = m.productName,
            }).ToList();
            return Json(PostList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MaimaiIndex()
        {
            return View();
        }

        public ActionResult tagsearch() 
        {
            return View();
        }
    }
}
using MaiMai.Models;
using MaiMai.Models.MaimaiIndexViewModel;
using MaiMai.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MaiMai.Controllers
{
    public class NewMaimaiIndexController : Controller
    {
        maimaiRepository<Cart> CartRepository = new maimaiRepository<Cart>();
        maimaiEntities db = new maimaiEntities();
        // GET: NewMaimaiIndex

        public ActionResult searchall(string text)
        {
            var SearchAll =
                db.ProductPost.Where(m => m.productName.Contains(text) || m.productDescription.Contains(text)).Select(m => new
                {
                    
                    name = m.productName,
                    prd = m.productDescription
                }).ToList();
            return Json(SearchAll, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }
        //首頁
        public ActionResult MaimaiIndexNew()
        {
            return View();
        }
        //首頁
        //五星廣告大版
        public ActionResult StarFive()
        {
            var StarFiveList = db.Member.Where(m => m.totalStarRate == 5.0).Select(m => new
            {
                img = m.ProImg,
                userAccount = m.userAccount,
                UserID=m.UserID,
            }).ToList();
            return Json(StarFiveList, JsonRequestBehavior.AllowGet);
        }
        //五星廣告大版
        //增加輪播圖片
        public ActionResult addCarousel()
        {
            var addCarouselList = db.ProductPost.Where(m=>m.status==1).OrderByDescending(m => m.ProductPostID).Select(m => new MaimaiIndexViewModel()
            {
                ProductPostID = m.ProductPostID,
                productImg = m.productImg,
                price = m.price,
                productName = m.productName,
                productDescription = m.productDescription
            }).ToList();
            return Json(addCarouselList, JsonRequestBehavior.AllowGet);
        }
        //增加輪播圖片
        //增加徵求台
        public ActionResult NewaddCarousel()
        {
            var addCarouselList = db.RequiredPost.OrderByDescending(m => m.RequiredPostID).Select(m => new RequiredPostViewModel_C()
            {
                RequiredPostID = m.RequiredPostID,
                postTime = m.postTime,
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
            return Json(addCarouselList, JsonRequestBehavior.AllowGet);
        }
        //增加徵求台
        //商品頁面總覽
        public ActionResult ProuductpostAll()
        {
            return View();
        }
        public ActionResult ProuductpostAll123(int TagID)
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


        //商品頁面
        public ActionResult ProdutPost()
        {

            return View();
        }
        public ActionResult ProdutPostDetail(int PostID)
        {
            var ProdutPostDetailList = db.ProductPost.Where(m => m.ProductPostID == PostID).Select(m => new
            {
                img = m.productImg,
                ProductName = m.productName,
                price = m.price,
                Description = m.productDescription,
                QTY = m.inStoreQTY,
                UserID = m.UserID,
                userAccount = m.Member.userAccount,

                ProductPostID = m.ProductPostID,
            }).ToList();
            return Json(ProdutPostDetailList, JsonRequestBehavior.AllowGet);
        }
        //商品頁面
        //送去購物車
        public ActionResult goCar()
        {
            return View();
        }

        public string goCar1(Cart ProductPostIDlll)
        {
            if (Request.Cookies["LoginID"] == null)
            {
                return ("請先登入");
            }
            //使用者
            var UserID = Convert.ToInt32(Request.Cookies["LoginID"].Value);
            //商品( 確認商品的ID)
            var pdpost = db.ProductPost.FirstOrDefault(m => m.ProductPostID == ProductPostIDlll.ProductPostID);
            //購物車            
            //Cart cart = new Cart();
            //( 1.有購物車號碼 2.使用者ID符合 3.未結單)
            var carnumver = db.Cart.FirstOrDefault(m => m.CartNumber != null && m.UserID == UserID && m.Status == false);
            var cardb = db.Cart.FirstOrDefault(m => m.ProductPostID == ProductPostIDlll.ProductPostID && m.UserID == UserID && m.Status == false);

            if (carnumver != null)
            {
                if (cardb != null)
                {
                    pdpost.inStoreQTY = pdpost.inStoreQTY - ProductPostIDlll.QTY;
                    db.SaveChanges();
                    ProductPostIDlll.CartNumber = cardb.CartNumber;
                    ProductPostIDlll.ProductPostID = cardb.ProductPostID;
                    cardb.QTY = ProductPostIDlll.QTY + cardb.QTY;
                    ProductPostIDlll.UserID = UserID;
                    ProductPostIDlll.Status = cardb.Status;
                    db.SaveChanges();
                    return "商品更新成功";
                }
                else
                {
                    ProductPostIDlll.CartNumber = carnumver.CartNumber;
                    ProductPostIDlll.UserID = UserID;
                    ProductPostIDlll.Status = false;
                    CartRepository.Create(ProductPostIDlll);
                    pdpost.inStoreQTY = pdpost.inStoreQTY - ProductPostIDlll.QTY;
                    db.SaveChanges();
                    return "增加一筆新商品";
                }
            }
            else
            {
                ProductPostIDlll.CartNumber = (new Random().Next(0, int.MaxValue) + (db.Cart.FirstOrDefault().UserID.ToString()));
                ProductPostIDlll.Status = false;
                ProductPostIDlll.UserID = UserID;
                CartRepository.Create(ProductPostIDlll);
                pdpost.inStoreQTY = pdpost.inStoreQTY - ProductPostIDlll.QTY;
                db.SaveChanges();
                return "新增成功";
            }
        }
        //送去購物車
        public ActionResult opay()
        {
            return View();
        }

        public ActionResult opay1(int orderID)
        {
            

            var is4 = "HashKey=5294y06JbISpM5x9&ChoosePayment=Credit&ClientBackURL=https://developers.opay.tw/AioMock/MerchantClientBackUrl&CreditInstallment=&EncryptType=1&InstallmentAmount=&ItemName=MacBook 30元X2#iPhone6s 40元X1&MerchantID=2000132&MerchantTradeDate=2021/03/26 16:09:37&MerchantTradeNo=DX202103261609371970&PaymentType=aio&Redeem=&ReturnURL=https://developers.opay.tw/AioMock/MerchantReturnUrl&StoreID=&TotalAmount=5&TradeDesc=建立信用卡測試訂單&HashIV=v77hoKGq4kWxNNIS";
            is4 = Server.UrlEncode(is4).ToLower();//正確
            var bytes = System.Text.Encoding.Default.GetBytes(is4);
            byte[] hash = System.Security.Cryptography.SHA256Managed.Create().ComputeHash(bytes);
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }
            return Json(builder.ToString(), JsonRequestBehavior.AllowGet);
        }

        //Michael add>>>>

        public ActionResult getAutocompleteID(string ProductPostName)
        {
            var productName = db.ProductPost.FirstOrDefault(m => m.productName == ProductPostName).ProductPostID;

            return Json(productName, JsonRequestBehavior.AllowGet);
        }
        //<<<<<
    }

}



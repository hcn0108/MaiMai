using MaiMai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MaiMai.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart

        maimaiEntities db = new maimaiEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult cartIndex()
        {
            return View();
        }

        public ActionResult getCart_P()
        {
            if (Request.Cookies["LoginID"] == null)
            {
                return Content("fail");
            }

            var UserID = Convert.ToInt32(Request.Cookies["LoginID"].Value);
            var products = db.Cart.Where(c => c.UserID == UserID && c.Status == false).Join(db.ProductPost, x => x.ProductPostID, y => y.ProductPostID, (x, y) => new
            {
                x.CartID,
                x.CartNumber,
                x.UserID,
                x.QTY,
                y.ProductPostID,
                y.productName,
                y.productImg,
                y.price
            }).GroupBy(g => new { g.CartID,g.QTY, g.CartNumber, g.UserID }).Select(s => new  ///所有從Cart取得的資料須groupby否則會當作同一筆
            {
                CartID = s.Key.CartID,
                CartNumber = s.Key.CartNumber,
                UserID = s.Key.UserID,
                QTY = s.Key.QTY,
                ProductPostID = s.Select(i => i.ProductPostID),
                productName = s.Select(i => i.productName),
                productImg = s.Select(i => i.productImg),
                price = s.Select(i => i.price),
            }).ToList();          

            return Json(products, JsonRequestBehavior.AllowGet);
        }

        maimaiRepository<Cart> dbCart = new maimaiRepository<Cart>();
       
        public ActionResult delProduct(int? productID)
        {
            if (productID ==null)
            {
                return Content("false");
            }
            var delQTY = db.Cart.Find(productID).QTY;
            var delProductPostId = db.Cart.Find(productID).ProductPostID;

            db.ProductPost.Find(delProductPostId).inStoreQTY = db.ProductPost.Find(delProductPostId).inStoreQTY + delQTY;
            db.SaveChanges();

            dbCart.Delete((int)productID);

            return Content("true");
        }

        public ActionResult cartToOrder(string[] CartID)
        {
            if (Request.Cookies["LoginID"] == null)
            {
                return Content("fail");
            }

            var UserID = Convert.ToInt32(Request.Cookies["LoginID"].Value);
            List<Cart> cart = new List<Cart>();

            IQueryable<Cart> oneProduct = db.Cart;
            IQueryable<Order>orded = db.Order;
            IQueryable<ProductPost> product = db.ProductPost;
                Order ord = new Order();

            foreach (string i in CartID.ToArray())
            {
                var oneProduct1 = oneProduct.ToList().FirstOrDefault(m=>m.CartID==Convert.ToInt32(i) && m.Status == false);   
                /////取出CartID存入Order
                if(db.Order.FirstOrDefault(m=>m.CartNumber == oneProduct1.CartNumber) == null) { 
                ord.buyerUserID = UserID;
                ord.CartNumber = oneProduct1.CartNumber;
                ord.orderStatus = 0;
                ord.createdTime = DateTime.Now;
                db.Order.Add(ord);
                db.SaveChanges();
                }
                ///抓取存入的OrderID存入OrderDetail
                var orded1 = orded.FirstOrDefault(o => o.CartNumber == oneProduct1.CartNumber);                
                //var product1 = product.FirstOrDefault(p=>p.ProductPostID == oneProduct1.ProductPostID);
                OrderDetail ordt = new OrderDetail();
                ordt.OrderID = orded1.OrderId;
                ordt.ProductPostID = oneProduct1.ProductPostID;
                ordt.QTY = oneProduct1.QTY;
                ordt.oneProductTotalPrice = oneProduct1.QTY * oneProduct1.ProductPost.price;
                ordt.SellerID = oneProduct1.ProductPost.UserID;
                ordt.buyerStatus = 0;
                ordt.sellerStatus = 0;
                db.OrderDetail.Add(ordt);
                db.SaveChanges();

                oneProduct1.Status = true;
                db.SaveChanges();
            }
                var ordTotalPrice = db.OrderDetail.Where(o => o.OrderID == ord.OrderId).Select(s=>s.oneProductTotalPrice).Sum();
                ord.OrderTotalPrice = ordTotalPrice;
                db.SaveChanges();

            return Content("success");
        }
    }
}
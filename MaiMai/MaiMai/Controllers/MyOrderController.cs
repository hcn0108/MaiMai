                     
//using AllPay.Payment.Integration;
using MaiMai.Models;
using MaiMai.Models.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace MaiMai.Controllers
{

    public class MyOrderController : Controller
    {
        // GET: backstage
        public ActionResult Index()
        {
            return View();
        }
        maimaiRepository<OrderDetail> odtail = new maimaiRepository<OrderDetail>();
        maimaiRepository<Member> mb = new maimaiRepository<Member>();
        maimaiRepository<Order> odRepository = new maimaiRepository<Order>();
        maimaiEntities db = new maimaiEntities();
        public ActionResult myOrder()
        {
            return View();
        }

        public ActionResult getMemberList_P()
        {

            var memList = db.Member.Select(m => new MemberViewModel()
            {
                UserID = m.UserID,
                userAccount = m.userAccount,
                userPassWord = m.userPassWord,
                city = m.city,
                address = m.address,
                phoneNumber = m.phoneNumber,
                firstName = m.firstName,
                lastName = m.lastName,
                Name = m.lastName + m.firstName,
                birthday = m.birthday,
                identityNumber = m.identityNumber,
                profileImg = m.profileImg,
                userLevel = m.userLevel,
                totalStarRate = m.totalStarRate,
                selfDescription = m.selfDescription,
                email = m.email,
                userLevelString = m.userLevel.ToString()
            });

            return Json(memList, JsonRequestBehavior.AllowGet);

        }

        public ActionResult getAdminList_P(int userLevel)
        {

            var memList = db.Member.Where(m => m.userLevel == userLevel).Select(m => new MemberViewModel()
            {
                UserID = m.UserID,
                userAccount = m.userAccount,
                userPassWord = m.userPassWord,
                city = m.city,
                address = m.address,
                phoneNumber = m.phoneNumber,
                firstName = m.firstName,
                lastName = m.lastName,
                Name = m.lastName + m.firstName,
                birthday = m.birthday,
                identityNumber = m.identityNumber,
                profileImg = m.profileImg,
                userLevel = m.userLevel,
                totalStarRate = m.totalStarRate,
                selfDescription = m.selfDescription,
                email = m.email,
                userLevelString = m.userLevel.ToString()
            }).ToList();

            return Json(memList, JsonRequestBehavior.AllowGet);

        }

        public ActionResult getMember_P(int? id)
        {
            if (id == null)
            {
                return Content("錯誤");
            }
            var mem = db.Member.Where(m => m.UserID == id).Select(m => new MemberViewModel()
            {
                UserID = m.UserID,
                userAccount = m.userAccount,
                userPassWord = m.userPassWord,
                city = m.city,
                address = m.address,
                phoneNumber = m.phoneNumber,
                Name = m.lastName + m.firstName,
                birthday = m.birthday,
                identityNumber = m.identityNumber,
                profileImg = m.profileImg,
                userLevel = m.userLevel,
                totalStarRate = m.totalStarRate,
                selfDescription = m.selfDescription,
                email = m.email,
                firstName = m.firstName,
                lastName = m.lastName,
                userLevelString = m.userLevel.ToString()
            }).ToList();

            return Json(mem, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult editUserLevel_P(Member m)
        {
            mb.Update(m);
            return Json(m, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getOrderList_P(int? status)  //key group by
        {

            var id = Convert.ToInt32(Request.Cookies["LoginID"].Value); 
            if (status != null)
            {
                if (status >= 2)
                {
                    var ordercmplist = db.Order.Where(m => (m.orderStatus >= 2) && (m.buyerUserID == id)).Join(db.OrderDetail, x => x.OrderId, y => y.OrderID, (x, y) => new
                    {
                        x.OrderId,
                        x.orderStatus,
                        x.createdTime,
                        x.buyerUserID,
                        x.Member.firstName,
                        //y.SellerID,
                        y.oneProductTotalPrice,

                    }).GroupBy(g => new { g.OrderId, g.orderStatus, g.createdTime, g.buyerUserID, g.firstName }).Select(s => new
                    {
                        OrderId = s.Key.OrderId,
                        orderStatus = s.Key.orderStatus,
                        createdTime = s.Key.createdTime,
                        buyerUserID = s.Key.buyerUserID,
                        buyerName = s.Key.firstName,
                        //SellerID =s.Select(i => i.SellerID),
                        price = s.Select(i => i.oneProductTotalPrice).Sum(),
                    }) ;
                    return Json(ordercmplist, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var ordercmplist = db.Order.Where(m => (m.orderStatus < 2)&&(m.buyerUserID==id)).Join(db.OrderDetail, x => x.OrderId, y => y.OrderID, 
                        /*db.Order, x => x.OrderId -----db.OrderDetail,y=>y.OrderID兩張表單串聯*/
                        (x, y) => new{
                        x.OrderId,
                        x.orderStatus,
                        x.createdTime,
                        x.buyerUserID,
                        x.Member.firstName,
                        //y.SellerID,
                        y.oneProductTotalPrice,

                    }).GroupBy(g => new { g.OrderId, g.orderStatus, g.createdTime, g.buyerUserID, g.firstName }).Select(s => new
                    {
                        OrderId = s.Key.OrderId,
                        orderStatus = s.Key.orderStatus,
                        createdTime = s.Key.createdTime,
                        buyerUserID = s.Key.buyerUserID,
                        buyerName = s.Key.firstName,
                        //SellerID =s.Select(i => i.SellerID),
                        price = s.Select(i => i.oneProductTotalPrice).Sum()
                    });

                    return Json(ordercmplist, JsonRequestBehavior.AllowGet);
                }
            }

            var orderlist = db.Order.Join(db.OrderDetail, x => x.OrderId, y => y.OrderID, (x, y) => new
            {
                x.OrderId,
                x.orderStatus,
                x.createdTime,
                x.buyerUserID,
                x.Member.firstName,
                y.oneProductTotalPrice,

            }).GroupBy(g => new { g.OrderId, g.orderStatus, g.createdTime, g.buyerUserID, g.firstName }).Select(s => new
            {
                OrderId = s.Key.OrderId,
                orderStatus = s.Key.orderStatus,
                createdTime = s.Key.createdTime,
                buyerUserID = s.Key.buyerUserID,
                buyerName = s.Key.firstName,
                //SellerID =s.Select(i => i.SellerID),
                price = s.Select(i => i.oneProductTotalPrice).Sum()
            });

            return Json(orderlist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult delOrder_P(int OrderId)
        {
            odRepository.Delete(OrderId);
            return Content("刪除成功");
        }

        public ActionResult getOrderDetail_P(int OrderId)
        {
            var orderDetail = db.OrderDetail.Where(o => o.OrderID == OrderId).Select(s => new
            {
                ProductPostID = s.ProductPostID,
                productName = s.ProductPost.productName,
                oneProductTotalPrice = s.QTY * s.ProductPost.price,
                QTY = s.QTY,
                createdTime = s.Order.createdTime,
                SellerName = s.ProductPost.Member.firstName
            }).ToList();

            return Json(orderDetail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult getProduct_P(int? ProductPostID, int? QTY)
        {
            if (ProductPostID == null || QTY == null)
            {
                return Content("格式錯誤");
            }
            var product = db.ProductPost.Where(p => p.ProductPostID == ProductPostID).Select(s => new
            {
                ProductPostID = s.ProductPostID,
                productName = s.productName,
                productDescription = s.productDescription,
                productImg = s.productImg,
                UserName = s.Member.firstName,
                QTY = QTY,
                price = QTY * s.price,
                TagID = s.TagID,
                Tag = s.Tag.tagName,
                createdTime = s.createdTime

            }).ToList();

            return Json(product, JsonRequestBehavior.AllowGet);
        }


        public ActionResult report()
        {
            return View();
        }

        public ActionResult getReport(int ?postID)
        {
            var reportDetail = db.ReportDetail.Select(t => new
            {
                reason = t.reason
            }).ToList();

            return Json(reportDetail, JsonRequestBehavior.AllowGet);
        }


        maimaiRepository<Report> rtdb = new maimaiRepository<Report>();

        public ActionResult saveReport(string reportTXT, int reportTAG)
        {

            Report r = new Report
            {
                reportorID = 13,//Convert.ToInt32(Request.Cookies["LoginID"].Value),
                repotedUserID = 5,
                reportStatus = 0,
                createdTime = DateTime.Now,
                ReportDetailID = reportTAG,
                reportDescription = reportTXT, //[Product(0)OrRequire(1)] 
                ProductOrRequire = 1,
                ProductOrRequireID = 4
            };
            rtdb.Create(r);

        

            return Content("新增成功");
        }


        public ActionResult commentForm()
        {
            return View();
        }

        public ActionResult getComment(int OrderDetailID)
            //userID買家-評論者  commentorUserID賣家-被評論者
        {
            var loginID= Convert.ToInt32(Request.Cookies["LoginID"].Value);
            var SellerID = db.OrderDetail.Find(OrderDetailID).SellerID;
            var buyerID = db.OrderDetail.Find(OrderDetailID).Order.buyerUserID;
            var sellerImg= db.Member.Find(SellerID).profileImg;
            var buyerImg=db.Member.Find(buyerID).profileImg;

            var commentedPSNid=(loginID == buyerID) ? SellerID : buyerID;
            var commentDetail = db.Comment.Where(x =>x.CommentorUserID== commentedPSNid).Select(x=>new { starRate=x.starRate}); 
            //選擇多筆訂單紀錄



            if (commentDetail.Count() == 0)
            {
            var starTotal = 0;
            var CNT = 0;

                var result =new
                {
                    starTotal ,
                    sellerImg ,
                    buyerImg,
                    SellerID,
                    buyerID,
                    CNT,
                };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var starTotal = 0;  //不可移到外面
                var CNT = 0;
                foreach (var item in commentDetail)
            {
                    starTotal += (int)item.starRate;
                    CNT += 1;   
            }
                var result = new
                {
                    starTotal = starTotal,
                    sellerImg,
                    buyerImg,
                    SellerID,
                    buyerID,
                    CNT,
                };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }


        maimaiRepository<Comment> cmdb = new maimaiRepository<Comment>();
        public ActionResult saveComment(int orderDetailID, string starRate, string description)
        {
            var orderID = db.OrderDetail.Find(orderDetailID).OrderID;
            var star = Convert.ToInt32(starRate);
            var UserID = Convert.ToInt32(Request.Cookies["LoginID"].Value);
            var od = odtail.GetbyID(orderDetailID);

            //var orderDetailID=
            var loginID = Convert.ToInt32(Request.Cookies["LoginID"].Value);
            var SellerID = db.OrderDetail.FirstOrDefault(s=>s.OrderID==orderID).SellerID;
            var buyerID = db.Order.Find(orderID).buyerUserID;
            var commentedPSNid = (loginID == buyerID) ? SellerID : buyerID;

            Comment cmt = new Comment() {
                starRate= star,
                commentDescription=description,
                UserID=UserID,
                OrderdetalID = orderDetailID,
                CommentorUserID= commentedPSNid,
            };

            if (od.SellerID == UserID)
            {
                od.sellerStatus = 3;
                
            }
            else {
                od.buyerStatus = 3;
            }

            if (od.sellerStatus == 3 && od.buyerStatus == 3) {

                Order o = odRepository.GetbyID((int)od.OrderID);
                o.orderStatus = 2;

                odRepository.Update(o);
            }

            odtail.Update(od);
            cmdb.Create(cmt);
            return Content("成功");
        }


        public ActionResult checkOut()
        {
            return View();
        }

        public ActionResult creditCardcheckOut(int OrderId)
        {//orderID=30; USerID=18

            var tradeDescription = "";

            var obj = db.OrderDetail.Where(x => x.OrderID == OrderId && x.Order.orderStatus == 0).Select(s => new
            {
                ItemName = s.ProductPost.productName,
                totalAmount = s.oneProductTotalPrice,
            });
            foreach (var item in obj)
            {
                tradeDescription += item.ItemName + "," + item.totalAmount + ";";
            }



            var Item = db.OrderDetail.FirstOrDefault(x => x.OrderID == OrderId && x.Order.orderStatus == 0);
            if (Item == null)
            {
                return Content("此商品不存在");
            }
            var ItemName = Item.ProductPost.productName;
            var MerchantTradeNo = Item.OrderID + DateTime.Now.ToString("mmss");
            var totalAmount = Item.Order.OrderTotalPrice;
            var MerchantTradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            //harsh key--5294y06JbISpM5x9 Hash IV--v77hoKGq4kWxNNIS
            var is4 = "HashKey=5294y06JbISpM5x9&ChoosePayment=Credit&ClientBackURL=https://localhost:44340/&CreditInstallment=&EncryptType=1&InstallmentAmount=&ItemName=" + tradeDescription + "&MerchantID=2000132&MerchantTradeDate=" + MerchantTradeDate + "&MerchantTradeNo=" + MerchantTradeNo + "&PaymentType=aio&Redeem=&ReturnURL=https://localhost:44340/&StoreID=&TotalAmount=" + totalAmount + "&TradeDesc=" + tradeDescription + "&HashIV=v77hoKGq4kWxNNIS";

            is4 = Server.UrlEncode(is4).ToLower();//正確
            var bytes = System.Text.Encoding.Default.GetBytes(is4);
            byte[] hash = System.Security.Cryptography.SHA256Managed.Create().ComputeHash(bytes);
            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }


            var CheckMacValue = builder.ToString();
            var orderlist = db.Order.Where(x => x.OrderId == OrderId && x.orderStatus == 0).Select(s => new
            {
                MerchantTradeNo = MerchantTradeNo,
                ItemName = ItemName,
                totalAmount ,
                tradeDescription = tradeDescription,
                MerchantTradeDate,
                CheckMacValue,
                is4,
            }).ToList();

            return Json(orderlist, JsonRequestBehavior.AllowGet);
        }


    }
}
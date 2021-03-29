                     
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

        maimaiRepository<Member> mb = new maimaiRepository<Member>();
        maimaiRepository<Order> od = new maimaiRepository<Order>();
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
            od.Delete(OrderId);
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
            var SellerID = db.OrderDetail.Find(OrderDetailID).SellerID;
            var img = db.Member.Find(SellerID).profileImg;
            //PK才可以用find,  OrderDetailID=23, OrderID=3, SellerID=7
            //OrderDetailID=25, OrderID=, SellerID=21   no__comment

            var commentDetail = db.Comment.Where(x => x.CommentorUserID == SellerID);
            if (commentDetail.Count() == 0)
            {
                var CNT = 0;
                var result =new
                {
                    starTotal = 0,
                    img = img,
                    CNT,
                };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            else
            {
             var starTotal = 0;
             var CNT = 0;

            foreach(var item in commentDetail)
            {
                    starTotal += (int)item.starRate;
                    CNT += 1;   
            }
                var result = new
                {
                    starTotal = starTotal,
                    img = img,
                    CNT,
                };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }


        maimaiRepository<Comment> cmdb = new maimaiRepository<Comment>();
        public ActionResult saveComment(string OrderID, string starRate, string description)
        {
            var orderID = Convert.ToInt32(OrderID);
            var star = Convert.ToInt32(starRate);
            var UserID = Convert.ToInt32(Request.Cookies["LoginID"].Value);
            //var  OrderDetailID=db.OrderDetail.FirstOrDefault(t => t.OrderID == orderID).OrderDetailID;
            var CommentorUserID=db.OrderDetail.Find(orderID).SellerID;

            Comment cmt = new Comment() {
                starRate= star,
                commentDescription=description,
                UserID=UserID,
                OrderdetalID = orderID,
                CommentorUserID= CommentorUserID,
            };
            cmdb.Create(cmt);
            return Content("成功");
        }


        public ActionResult checkOut()
        {

            //DateTime dt = DateTime.Now.ToString("G", "");

            Console.WriteLine(DateTime.Now.ToLocalTime());

            return View();
        }

        public ActionResult creditCardcheckOut(int OrderId)
        {//orderID=30; USerID=18

            //var 



            //var MerchantTradeDate = DateTime.Now;
            var orderlist = db.Order.Join(db.OrderDetail, x => x.OrderId, y => y.OrderID, (x, y) => new
            {
                x.OrderId,
                x.orderStatus,
                x.CartNumber,
                y.QTY,
                y.oneProductTotalPrice,
                y.ProductPost.productName,
            }).GroupBy(g => new { g.OrderId, g.orderStatus, g.CartNumber, g.QTY, g.oneProductTotalPrice,g.productName }).Select(s => new
            {
                OrderId = s.Key.OrderId,
                orderStatus = s.Key.orderStatus,
                MerchantTradeNo = s.Key.CartNumber,
                buyerUserID = s.Key.QTY,
                ItemName= s.Key.productName,
                TotalAmounot = s.Key.oneProductTotalPrice,
                //MerchantTradeDate,
            }).ToList();

            

            return Json(orderlist, JsonRequestBehavior.AllowGet);
        }

        //    List<string> enErrors = null;

        //    try
        //    {
        //        using (AllInEscrow oPayment = new AllInEscrow())
        //        {
        //            /* 服務參數 */
        //            oPayment.ServiceMethod = HttpMethod.HttpPOST;
        //            oPayment.ServiceURL = "https://payment-stage.opay.tw/Cashier/AioCheckOut/V5";
        //            oPayment.HashKey = "5294y06JbISpM5x9";
        //            oPayment.HashIV = "v77hoKGq4kWxNNIS";
        //            oPayment.MerchantID = "2000132";
        //            /* 基本參數 */
        //            oPayment.Send.ReturnURL = "https://localhost:44340/NewMaimaiIndex/MaimaiIndexNew";
        //            oPayment.Send.ClientBackURL = "https://localhost:44340/NewMaimaiIndex/MaimaiIndexNew";

        //            oPayment.Send.MerchantTradeNo = "12345678901234567890";
        //            oPayment.Send.MerchantTradeDate = DateTime.Parse("20210105");
        //            oPayment.Send.TotalAmount = Decimal.Parse("40");
        //            oPayment.Send.TradeDesc = "SONY遊戲機台";
        //            oPayment.Send.Currency = "TW";
        //            oPayment.Send.EncodeChartset = "Encode Chartset";
        //            oPayment.Send.UseAllpayAddress = "https://payment-stage.opay.tw/Cashier/AioCheckOut/V5";
        //            oPayment.Send.CreditInstallment = Int32.Parse("4311-9522-2222-2222");
        //            oPayment.Send.InstallmentAmount = Decimal.Parse("Installment Amount");
        //            oPayment.Send.Redeem = false;
        //            oPayment.Send.ShippingDate = "<20210514>";
        //            oPayment.Send.ConsiderHour = Int32.Parse("48");
        //            oPayment.Send.Remark = "易碎品，請輕放";
        //            // 加入選購商品資料。
        //            oPayment.Send.Items.Add(new Item() { Name = "馬力歐賽車", Price = Decimal.Parse("500"), Currency = "Currency", Quantity = Int32.Parse("20"), URL = "Product Detail URL" });
        //            oPayment.Send.Items.Add(new Item() { Name = "薩爾達傳說", Price = Decimal.Parse("900"), Currency = "Currency", Quantity = Int32.Parse("20"), URL = "Product Detail URL" });
        //            oPayment.Send.Items.Add(new Item() { Name = "Product Name", Price = Decimal.Parse("Unit Price"), Currency = "Currency", Quantity = Int32.Parse("Quantity"), URL = "Product Detail URL" });

        //            enErrors.AddRange(oPayment.CheckOut());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // 例外錯誤處理。
        //        enErrors.Add(ex.Message);
        //    }
        //    finally
        //    {
        //        // 顯示錯誤訊息。
        //        //if (enErrors.Count() > 0)
        //        //    ScriptManager.RegisterStartupScript(this, typeof(Page), "_MESSAGE", String.Format("alert(\"{0}\");", String.Join("\\r\\n", enErrors)), true);
        //    }


    }
    }
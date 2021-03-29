﻿using MaiMai.Models;
using MaiMai.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MaiMai.Controllers
{
    public class OrderManagerController : Controller
    {
        // GET: OrderMenager
        maimaiEntities db = new maimaiEntities();
     
        public ActionResult OrderIndex() {
            return View();
        }

        public ActionResult getNonPayOrderList(string userid)
        {

            var id = Convert.ToInt32(userid);

            var table = db.Order.Where(m => m.buyerUserID == id && m.orderStatus == 0).Join(db.OrderDetail, o => o.OrderId, od => od.OrderID
            , (o, od) => new
            {
                o.OrderId,
                o.orderStatus,
                o.createdTime,
                o.buyerUserID,
                od.oneProductTotalPrice,

            }).GroupBy(g => new { g.OrderId, g.orderStatus, g.createdTime, g.buyerUserID }).Select(s => new OrderViewModel()
            {
                OrderId = s.Key.OrderId,
                orderStatus = s.Key.orderStatus,
                orderStatusString = s.Key.orderStatus.ToString(),
                createdTime = s.Key.createdTime,
                price = (int)s.Select(i => i.oneProductTotalPrice).Sum()
            }).ToList(); ;


            return Json(table, JsonRequestBehavior.AllowGet);

        }

        public ActionResult getPastOrderList(string userid)
        {

            var id = Convert.ToInt32(userid);

            var table = db.Order.Where(m => m.buyerUserID == id && m.orderStatus == 2).Join(db.OrderDetail, o => o.OrderId, od => od.OrderID
            , (o, od) => new
            {
                o.OrderId,
                o.orderStatus,
                o.createdTime,
                o.buyerUserID,
                od.oneProductTotalPrice,

            }).GroupBy(g => new { g.OrderId, g.orderStatus, g.createdTime, g.buyerUserID }).Select(s => new 
            {
                buyusrId = s.Key.buyerUserID,
                OrderId = s.Key.OrderId,
                orderStatus = s.Key.orderStatus,
                orderStatusString = s.Key.orderStatus.ToString(),
                createdTime = s.Key.createdTime,
                price = (int)s.Select(i => i.oneProductTotalPrice).Sum()
            }).ToList(); ;


            return Json(table, JsonRequestBehavior.AllowGet);

        }

        public ActionResult getProcessOrderList(string userid)
        {

            var id = Convert.ToInt32(userid);

            var table = db.Order.Where(m => m.buyerUserID == id && m.orderStatus == 1).Join(db.OrderDetail, o => o.OrderId, od => od.OrderID
            , (o, od) => new
            {
                OrderId=o.OrderId,
                orderStatus=o.orderStatus,
                createdTime=o.createdTime,
                oneProductTotalPrice=od.oneProductTotalPrice,
                oderdeail=od.OrderDetailID,
                productImg=od.ProductPost.productImg,
                ProductPostName =  od.ProductPost.productName,
                ProductPostID= od.ProductPostID,
                buyerName= od.Member.userAccount,
                buyusrId=od.Member.UserID,
                buyerStatus= od.buyerStatus,
                sellerStatus=od.sellerStatus

            }).ToList();


            return Json(table, JsonRequestBehavior.AllowGet);

        }

        public ActionResult showOrderRecipt(string orderid, string userid)
        {
            var oid = Convert.ToInt32(orderid);
            var mid = Convert.ToInt32(userid);

            var table = db.Order.Where(m => m.buyerUserID == mid && m.OrderId == oid).Join(db.OrderDetail, o => o.OrderId, od => od.OrderID
               , (o, od) => new
               {
                   createdTime = o.createdTime,
                   productName =od.ProductPost.productName,
                   productImg= od.ProductPost.productImg,
                   QTY= od.QTY,
                   OrderDetailID= od.OrderDetailID,
                   oneProductTotalPrice= od.oneProductTotalPrice,
                   userAccount=od.Member.userAccount
               }).ToList();

            return Json(table, JsonRequestBehavior.AllowGet);
        }

        maimaiRepository<OrderDetail> OrderDetailRepository = new maimaiRepository<OrderDetail>();
        maimaiRepository<Order> order = new maimaiRepository<Order>();

        maimaiRepository<RequiredPost> requiredPostRepository = new maimaiRepository<RequiredPost>();

        public string checkout(string orderid)
        {

            var oid = Convert.ToInt32(orderid);
            var orderReciept = order.GetbyID(oid);
            var requirePost = db.RequiredPost.FirstOrDefault(m => m.OrderID == oid);
            if (requirePost != null) {
                requirePost.isPast = true;
                requiredPostRepository.Update(requirePost);
            }
          

            if (orderReciept == null)
            {
                Response.StatusCode = 404;

                return "錯誤";
            }
                
                orderReciept.orderStatus = 1;
                 changeStatus(oid);
                order.Update(orderReciept);

                return "繳費成功";
        }


        public void changeStatus( int orderid) {

            var tableD = db.OrderDetail.Where(m => m.OrderID == orderid).ToList();


            foreach (OrderDetail item in tableD)
            {
               
                item.buyerStatus = 1;

                db.SaveChanges();
                //OrderDetailRepository.Update(item);
            }

        }

        public string endOrder(string oderdeail)
        {
           
            var oid = Convert.ToInt32(oderdeail);
            var orderDetail = OrderDetailRepository.GetbyID(oid);

            if (orderDetail == null)
            {
                Response.StatusCode = 404;

                return "結單失敗";
            }

            orderDetail.buyerStatus = 2;

            OrderDetailRepository.Update(orderDetail);

            return "結單成功 ! 別忘了下評論喔";
        }


        // ========================= 以下為賣價訂單管理=============================



        public ActionResult salesOrderIndex()
        {
            return View();
        }

        public ActionResult getComfirmOrderList( string userid) {
            
            var id = Convert.ToInt32(userid);

            var table = db.OrderDetail.Where(m => m.SellerID == id && m.buyerStatus == 1 && m.sellerStatus ==0 ).Select(od => new {
                orderDetailId= od.OrderDetailID,
                orderID= od.OrderID,
                createdTime = od.Order.createdTime,
                productName = od.ProductPost.productName,
                productImg = od.ProductPost.productImg,
                QTY = od.QTY,
                OrderDetailID = od.OrderDetailID,
                oneProductTotalPrice = od.oneProductTotalPrice,
                buyerUserAccount = od.Order.buyerUserID

            }); 


            return Json(table, JsonRequestBehavior.AllowGet);
        }

        public string  confirmOrder(string oderdeail) {

            var oid = Convert.ToInt32(oderdeail);
            var orderDetail = OrderDetailRepository.GetbyID(oid);

            orderDetail.sellerStatus = 1;
            try
            {
                OrderDetailRepository.Update(orderDetail);
                return "確認訂單成功";
            }
            catch( Exception e) {
                Response.StatusCode = 404;
                return "確認失敗";

            }
            
         
        }

        public ActionResult getSalesProcessOrderList(string userid) {
            var id = Convert.ToInt32(userid);

            var table = db.OrderDetail.Where(m => m.SellerID == id && m.sellerStatus == 1 && m.buyerStatus == 1).Select(od => new {
                orderDetailId = od.OrderDetailID,
                orderID = od.OrderID,
                createdTime = od.Order.createdTime,
                productName = od.ProductPost.productName,
                productImg = od.ProductPost.productImg,
                QTY = od.QTY,
                OrderDetailID = od.OrderDetailID,
                oneProductTotalPrice = od.oneProductTotalPrice,
                buyerUserAccount = od.Order.buyerUserID

            });


            return Json(table, JsonRequestBehavior.AllowGet);


        }
        public string sentOrder(string oderdeail)
        {

            var oid = Convert.ToInt32(oderdeail);
            var orderDetail = OrderDetailRepository.GetbyID(oid);

            orderDetail.sellerStatus = 2;
            try
            {
                OrderDetailRepository.Update(orderDetail);
                return "出貨成功";
            }
            catch (Exception e)
            {
                Response.StatusCode = 404;
                return "出貨失敗";

            }


        }

        public ActionResult getSalesProcessedOrderList(string userid)
        {
            var id = Convert.ToInt32(userid);

            var table = db.OrderDetail.Where(m => m.SellerID == id && m.sellerStatus == 2 && m.buyerStatus==1).Select(od => new {
                orderDetailId = od.OrderDetailID,
                orderID = od.OrderID,
                createdTime = od.Order.createdTime,
                productName = od.ProductPost.productName,
                productImg = od.ProductPost.productImg,
                QTY = od.QTY,
                OrderDetailID = od.OrderDetailID,
                oneProductTotalPrice = od.oneProductTotalPrice,
                buyerUserAccount = od.Order.buyerUserID

            });


            return Json(table, JsonRequestBehavior.AllowGet);


        }

        public ActionResult getSalesPastOrderList(string userid)
        {
            var id = Convert.ToInt32(userid);

            var table = db.OrderDetail.Where(m => m.SellerID == id && m.sellerStatus == 2 && m.buyerStatus == 2).Select(od => new {
                orderDetailId = od.OrderDetailID,  
                orderID = od.OrderID,
                createdTime = od.Order.createdTime,
                productName = od.ProductPost.productName,
                productImg = od.ProductPost.productImg,
                QTY = od.QTY,
                OrderDetailID = od.OrderDetailID,
                oneProductTotalPrice = od.oneProductTotalPrice,
                buyerUserAccount = od.Order.Member.userAccount

            }).ToList();


            return Json(table, JsonRequestBehavior.AllowGet);


        }

    } //end of class
}//end of namespace
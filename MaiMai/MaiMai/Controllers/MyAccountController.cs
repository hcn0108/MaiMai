using MaiMai.Models;
using MaiMai.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MaiMai.Controllers
{
    public class MyAccountController : Controller
    {
        // GET: MyAccount
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult myAccountIndex()
        {
            return View();
        }

        maimaiEntities db = new maimaiEntities();

        public ActionResult getAccount_P()
        {
            var userName = Request.Cookies["LoginName"].Value.ToString();
            if (userName == null)
                return Content("尚未登入");
            var getAccount = db.Member.Where(m => m.userAccount == userName).Select(s => new MemberViewModel()
            {
                ProImg = s.ProImg,
                connectionID = s.connectionID,
                UserID = s.UserID,
                userAccount = s.userAccount,
                userPassWord = s.userPassWord,
                city = s.city,
                address = s.address,
                phoneNumber = s.phoneNumber,
                firstName = s.firstName,
                lastName = s.lastName,
                birthday = s.birthday,
                identityNumber = s.identityNumber,
                profileImg = s.profileImg,
                userLevel = s.userLevel,
                totalStarRate = s.totalStarRate,
                selfDescription = s.selfDescription,
                email = s.email
            }).ToList();

            return Json(getAccount, JsonRequestBehavior.AllowGet);
        }
        maimaiRepository<Member> mb = new maimaiRepository<Member>();

        [HttpPost]
        public ActionResult editMember_P(Member mem)
        {
            mb.Update(mem);

            return Content("修改成功");
        }

        public ActionResult MyAccount()
        {
            return View();
        }
        public ActionResult MyPassword()
        {
            return View();
        }

        public String Editpassword(LoginViewModel password)
        {
            Member mb1 = db.Member.FirstOrDefault(m => m.userAccount == password.userAccount && m.userPassWord == password.userPassWord);
            if (mb1 == null)
            {
                return "帳號不存在或密碼錯誤!";
            }

           

            return "登入成功";
        }
        public string Doingmember(MemberViewModel mber)
        {
          
            Member memmgg = new Member()
            {
                UserID = mber.UserID,

                userAccount = mber.userAccount,

                city = mber.city,
                address = mber.address,
                phoneNumber = mber.phoneNumber,
                //profileImg = mber.upphoto.FileName,
                
                firstName = mber.firstName,
                lastName = mber.lastName,
                selfDescription = mber.selfDescription,
                email = mber.email,

                identityNumber = mber.identityNumber,
                userLevel = mber.userLevel,
                totalStarRate = mber.totalStarRate,
                ProImg = mber.ProImg,
                connectionID = mber.connectionID,
                userPassWord = mber.userPassWord,
                birthday = mber.birthday,


                 
                
            };
            if (mber.upphoto == null)
            {
                var mbb = db.Member.FirstOrDefault(a => a.UserID == mber.UserID);
                memmgg.profileImg = mbb.profileImg;
            }
            else
            {
                

                memmgg.profileImg = mber.upphoto.FileName;
                string filename = mber.upphoto.FileName;
                mber.upphoto.SaveAs(Server.MapPath("../Content/ProductPostImg/member/") + filename);
                string filePath = $"../Content/ProductPostImg/member/{filename}";

            }
            //HttpPostedFileBase photo = new HttpPostedFileBase(upphoto);




            mb.Update(memmgg);
            return "註冊成功";



        }



    }
}
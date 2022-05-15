using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Serivces;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class ItemController : Controller
    {
        private readonly CartService cartService = new CartService();
        private readonly ItemService itemService = new ItemService();
        // GET: Item
        public ActionResult Index(int Page=1)
        {
            ItemViewModel Data = new ItemViewModel();
            Data.Paging = new ForPaging(Page);
            Data.IdList = itemService.GetIdList(Data.Paging);
            Data.ItemBlock = new List<ItemDetailViewModel>();
            foreach(var Id in Data.IdList)
            {
                ItemDetailViewModel newBlock = new ItemDetailViewModel();
                newBlock.Data = itemService.GetDataById(Id);
                string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : null;
                newBlock.InCart = cartService.CheckInCart(Cart, Id);
                Data.ItemBlock.Add(newBlock);
            }
            return View(Data);
        }

        #region 商品頁面
        public ActionResult Item(int Id)
        {
            ItemDetailViewModel ViewData = new ItemDetailViewModel();
            ViewData.Data = itemService.GetDataById(Id);
            string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : null;
            ViewData.InCart = cartService.CheckInCart(Cart, Id);
            return View(ViewData);
        }
        #endregion

        #region 商品列表區塊
        public ActionResult ItemBlock(int Id)
        {
            ItemDetailViewModel ViewData = new ItemDetailViewModel();
            ViewData.Data = itemService.GetDataById(Id);
            string Cart = (HttpContext.Session["Cart"] != null) ? HttpContext.Session["Cart"].ToString() : null;
            ViewData.InCart = cartService.CheckInCart(Cart, Id);
            return PartialView(ViewData);
        }
        #endregion

        #region 新增商品
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public ActionResult Add(ItemCreateViewModel Data)
        {
            if (Data.ItemImage != null)
            {
                string filename = Path.GetFileName(Data.ItemImage.FileName);
                string Url = Path.Combine(Server.MapPath("~/Upload/"), filename);
                Data.ItemImage.SaveAs(Url);
                Data.newData.Image = filename;
                itemService.Insert(Data.newData);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("ItemImage", "請選擇上傳檔案");
                return View(Data);
            }
        }
        #endregion

        #region 刪除商品
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int Id)
        {
            itemService.Delete(Id);
            return RedirectToAction("Index");
        }
        #endregion
    }
}
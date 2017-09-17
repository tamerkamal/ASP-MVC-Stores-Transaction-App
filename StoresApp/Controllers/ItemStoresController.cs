using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StoresApp.Models;

namespace StoresApp.Controllers
{
    public class ItemStoresController : Controller
    {
        private StoresDbEntities2 db = new StoresDbEntities2();

        // GET: ItemStores
        public ActionResult Index()
        {
            var itemStores = db.ItemStores.Include(i => i.Item).Include(i => i.Store);
            return View(itemStores.ToList());
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

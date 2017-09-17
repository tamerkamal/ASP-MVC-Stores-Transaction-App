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
    public class OutTransactionsController : Controller
    {
        private StoresDbEntities2 db = new StoresDbEntities2();

        // GET: OutTransactions
        public ActionResult Index()
        {
            var transactions = db.Transactions.Where(t => t.Type == "Export").Include(t => t.Item).Include(t => t.ItemStore).Include(t => t.Store);


            return View(transactions.ToList());
        }

        // GET: OutTransactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: OutTransactions/Create
        public ActionResult Create()
        {
            ViewBag.ItemId = new SelectList(db.Items, "Id", "Name");

            ViewBag.StoreId = new SelectList(db.Stores, "Id", "Name");
            return View();
        }

        // POST: OutTransactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ItemId,StoreId,ItemStoreId,Date,TQ")] Transaction transaction)
        {
          

            if (ModelState.IsValid )
            {

                

                transaction.Type = "Export";

                if (transaction.Date > DateTime.Now)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, " Sorry,You can't submit a transaction in the future !!");
                }

                db.Transactions.Add(transaction);
                var query =

                    from c in db.ItemStores
                    where c.ItemId == transaction.ItemId && c.StoreId == transaction.StoreId
                    select c;

                foreach (var IS in query)
                {  if(IS.ISQ>=transaction.TQ)
                    IS.ISQ -= transaction.TQ;
                   else
                    {

                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Sorry, This Item quantity is not available in the selected Store !! ");

                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ItemId = new SelectList(db.Items, "Id", "Name", transaction.ItemId);

            ViewBag.StoreId = new SelectList(db.Stores, "Id", "Name", transaction.StoreId);
            return View(transaction);
        }

        // GET: OutTransactions/Edit/5
        public ActionResult Edit(int? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);

           
            var x = transaction.TQ; //


            TempData["ID"] = x; //
            

            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.ItemId = new SelectList(db.Items, "Id", "Name", transaction.ItemId);
        
            ViewBag.StoreId = new SelectList(db.Stores, "Id", "Name", transaction.StoreId);
            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ItemId,StoreId,ItemStoreId,Date,TQ")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                transaction.Type = "Export";
                db.Entry(transaction).State = EntityState.Modified;

                int id = Convert.ToInt32(TempData["ID"]);//
                var query =

                from c in db.ItemStores
                where c.ItemId == transaction.ItemId && c.StoreId == transaction.StoreId
                select c;

                foreach (var IS in query)
                { if(IS.ISQ>(transaction.TQ-id))
                    IS.ISQ = IS.ISQ - transaction.TQ +id;

                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Sorry, This Item quantity not available in the selected Store !! ");
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ItemId = new SelectList(db.Items, "Id", "Name", transaction.ItemId);
         
            ViewBag.StoreId = new SelectList(db.Stores, "Id", "Name", transaction.StoreId);
            return View(transaction);
        }

        // GET: OutTransactions/Delete/5
        public ActionResult Delete(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: OutTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            transaction.Type = "Export";
           var query =

             from c in db.ItemStores
             where c.ItemId == transaction.ItemId && c.StoreId == transaction.StoreId
             select c;

            foreach (var IS in query)
            {  
                IS.ISQ += transaction.TQ;
            }

            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StoresApp.Models;
using System.Data.SqlClient;

namespace StoresApp.Controllers
{
    public class TransactionsController : Controller
    {
        private StoresDbEntities2 db = new StoresDbEntities2();              
        // GET: Transactions
        public ActionResult Index()
        {

            var transactions = db.Transactions.Where(t => t.Type == "Import").Include(t => t.Item).Include(t => t.ItemStore).Include(t => t.Store);
          

            return View(transactions.ToList());

        }

        // GET: Transactions/Details/5
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

        // GET: Transactions/Create
        public ActionResult Create()
        {
            
                
           ViewBag.ItemId = new SelectList(db.Items, "Id", "Name");
      
            ViewBag.StoreId = new SelectList(db.Stores, "Id", "Name");

            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ItemId,StoreId,ItemStoreId,Date,TQ")] Transaction _transaction)
        {
            if (ModelState.IsValid)
            {
                _transaction.Type = "Import";
                             

                db.Transactions.Add(_transaction);

         
                var query =

                    from c in db.ItemStores
                    where c.ItemId == _transaction.ItemId && c.StoreId == _transaction.StoreId
                    select c;

                foreach (var IS in query)
                {
                    IS.ISQ += _transaction.TQ;

                }
                
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.ItemId = new SelectList(db.Items, "Id", "Name", _transaction.ItemId);
         
            ViewBag.StoreId = new SelectList(db.Stores, "Id", "Name", _transaction.StoreId);
            return View(_transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
              Transaction _transaction = db.Transactions.Find(id);
            
           var   x = _transaction.TQ; //
            

            TempData["ID"] = x; //

        


            if (_transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.ItemId = new SelectList(db.Items, "Id", "Name", _transaction.ItemId);
          
            ViewBag.StoreId = new SelectList(db.Stores, "Id", "Name", _transaction.StoreId);



            return View(_transaction);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ItemId,StoreId,ItemStoreId,Date,TQ")] Transaction _transaction)
        {
            if (ModelState.IsValid )
            {
             
                _transaction.Type = "Import";
               db.Entry(_transaction).State = EntityState.Modified;
                int id = Convert.ToInt32(TempData["ID"]); //
                var query =

                  from c in db.ItemStores
                  where c.ItemId == _transaction.ItemId && c.StoreId == _transaction.StoreId
                  select c;

                foreach (var IS in query)
                {
                    if ((IS.ISQ +_transaction.TQ-id<0))
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, " Sorry, Editing This transaction with this new Quantity will cause a shortage in the Stock !!");

                    IS.ISQ += _transaction.TQ -id; //
                   
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ItemId = new SelectList(db.Items, "Id", "Name", _transaction.ItemId);
          
            ViewBag.StoreId = new SelectList(db.Stores, "Id", "Name", _transaction.StoreId);
            return View(_transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);

           // y = transaction.TQ;

            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            transaction.Type = "Import";
           var query =

              from c in db.ItemStores
              where c.ItemId == transaction.ItemId && c.StoreId == transaction.StoreId
              select c;

            foreach (var IS in query)
            {
                if ((IS.ISQ -transaction.TQ<0))
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, " Sorry, Deleting This transaction will cause a shortage in the Stock !!");
                IS.ISQ -= transaction.TQ;
              
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

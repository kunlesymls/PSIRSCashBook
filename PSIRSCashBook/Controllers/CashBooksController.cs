using Microsoft.AspNet.Identity;
using PSIRSCashBook.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PSIRSCashBook.Controllers
{
    public class CashBooksController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: CashBooks
        public ActionResult Index()
        {
            ViewBag.PsirsCodeId = new SelectList(_db.PsirsCodes, "PsirsCodeId", "CodeName");
            return View();
        }

        public async Task<ActionResult> GetIndex(int? PsirsCodeId, string StartDate, string EndDate)
        {
            // dc.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
            var cashBooks = new List<CashBook>();

            if (PsirsCodeId != null)
            {
                cashBooks = await _db.CashBooks.Include(i => i.PsirsCode).Include(i => i.Item)
                    .AsNoTracking().Where(x => x.PsirsCodeId.Equals((int)PsirsCodeId)).ToListAsync();
            }
            else
            {
                cashBooks = await _db.CashBooks.Include(i => i.PsirsCode).Include(i => i.Item)
                    .AsNoTracking().OrderBy(i => i.PsirsCodeId).ToListAsync();
            }
            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                var dateCashBook = new List<CashBook>();
                var myStartDate = Convert.ToDateTime(StartDate);
                var myEndDate = Convert.ToDateTime(EndDate);
                DateTime[] myDates = GetDatesBetween(myStartDate, myEndDate).ToArray();
                foreach (var date in myDates)
                {
                    dateCashBook.AddRange(cashBooks.Where(x => x.TransactionDate.ToString("d").Equals(date.ToString("d")))
                        .ToList());
                }
                cashBooks = dateCashBook;

            }
            else if (!string.IsNullOrEmpty(StartDate))
            {
                var myStartDate = Convert.ToDateTime(StartDate);
                cashBooks = cashBooks.Where(x => x.TransactionDate.ToString("d").Equals(myStartDate.ToString("d")))
                                        .ToList();
            }




            var data = cashBooks.Select(cashBook => new DisplayCashBookVm()
            {
                ItemName = cashBook.Item.ItemName,
                CodeName = cashBook.PsirsCode.CodeName,
                Payee = cashBook.Payee,
                TransactionDate = cashBook.TransactionDate.ToString("d"),
                Amount = cashBook.Amount,
                PvCode = cashBook.PvCode
            }).ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItemList(string codeId)
        {
            int psirsCodeId = Convert.ToInt32(codeId);
            var item = _db.Items.Include(i => i.PsirsCode).AsNoTracking()
                            .Where(x => x.PsirsCode.PsirsCodeId.Equals(psirsCodeId))
                            .Select(s => new { s.ItemName, s.ItemId }).ToList();
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(item);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public DateTime[] GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            var allDates = new List<DateTime>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date);
            return allDates.ToArray();
        }

        public async Task<PartialViewResult> Save(int id)
        {
            var cashBook = await _db.CashBooks.FindAsync(id);
            ViewBag.ItemId = new SelectList(_db.Items, "ItemId", "ItemName");
            ViewBag.PsirsCodeId = new SelectList(_db.PsirsCodes, "PsirsCodeId", "CodeName");
            return PartialView(cashBook);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(CashBook model)
        {
            bool status = false;
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                if (model.CashBookId > 0)
                {
                    var cashBook = await _db.CashBooks.FindAsync(model.ItemId);
                    if (cashBook != null)
                    {
                        cashBook.StaffId = User.Identity.GetUserId();
                        cashBook.PsirsCodeId = model.PsirsCodeId;
                        cashBook.ItemId = model.ItemId;
                        cashBook.Payee = model.Payee;
                        cashBook.Purpose = model.Purpose;
                        cashBook.PvCode = model.PvCode;
                        cashBook.Amount = model.Amount;
                        _db.Entry(cashBook).State = EntityState.Modified;
                        await _db.SaveChangesAsync();
                        message = $"{model.Payee} Updated Successfully...";
                        return new JsonResult { Data = new { status = true, message = message } };
                    }
                }
                else
                {
                    model.StaffId = User.Identity.GetUserId();
                    model.TransactionDate = DateTime.Now;
                    _db.CashBooks.Add(model);
                    await _db.SaveChangesAsync();
                    message = $"{model.Payee} Added Successfully.";
                    return new JsonResult { Data = new { status = true, message = message } };
                }
            }
            return new JsonResult { Data = new { status = status, message = message } };
            //return View(subject);
        }

        // GET: CashBooks/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashBook cashBook = await _db.CashBooks.FindAsync(id);
            if (cashBook == null)
            {
                return HttpNotFound();
            }
            return View(cashBook);
        }

        // GET: CashBooks/Create
        public ActionResult Create()
        {
            ViewBag.ItemId = new SelectList(_db.Items, "ItemId", "ItemName");
            ViewBag.PsirsCodeId = new SelectList(_db.PsirsCodes, "PsirsCodeId", "CodeName");
            return View();
        }

        // POST: CashBooks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CashBook cashBook)
        {
            if (ModelState.IsValid)
            {
                cashBook.TransactionDate = DateTime.Now;
                _db.CashBooks.Add(cashBook);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ItemId = new SelectList(_db.Items, "ItemId", "ItemName", cashBook.ItemId);
            ViewBag.PsirsCodeId = new SelectList(_db.PsirsCodes, "PsirsCodeId", "CodeName", cashBook.PsirsCodeId);
            return View(cashBook);
        }

        // GET: CashBooks/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashBook cashBook = await _db.CashBooks.FindAsync(id);
            if (cashBook == null)
            {
                return HttpNotFound();
            }
            ViewBag.ItemId = new SelectList(_db.Items, "ItemId", "ItemName", cashBook.ItemId);
            ViewBag.PsirsCodeId = new SelectList(_db.PsirsCodes, "PsirsCodeId", "CodeName", cashBook.PsirsCodeId);
            return View(cashBook);
        }

        // POST: CashBooks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CashBook cashBook)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(cashBook).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ItemId = new SelectList(_db.Items, "ItemId", "ItemName", cashBook.ItemId);
            ViewBag.PsirsCodeId = new SelectList(_db.PsirsCodes, "PsirsCodeId", "CodeName", cashBook.PsirsCodeId);
            return View(cashBook);
        }

        // GET: CashBooks/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashBook cashBook = await _db.CashBooks.FindAsync(id);
            if (cashBook == null)
            {
                return HttpNotFound();
            }
            return View(cashBook);
        }

        // POST: CashBooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CashBook cashBook = await _db.CashBooks.FindAsync(id);
            if (cashBook != null) _db.CashBooks.Remove(cashBook);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

using PSIRSCashBook.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PSIRSCashBook.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Items
        public async Task<ActionResult> Index()
        {
            var items = _db.Items.Include(i => i.PsirsCode);
            return View(await items.ToListAsync());
        }
        public async Task<ActionResult> GetIndex()
        {
            // dc.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
            var data = await _db.Items.Include(i => i.PsirsCode).AsNoTracking().Select(s => new
            {
                s.PsirsCode.CodeName,
                s.ItemName,
                s.ItemId
            }).ToListAsync();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        public async Task<PartialViewResult> Save(int id)
        {
            var item = await _db.Items.FindAsync(id);
            ViewBag.PsirsCodeId = new SelectList(_db.PsirsCodes, "PsirsCodeId", "CodeName");
            return PartialView(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(Item model)
        {
            bool status = false;
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                if (model.ItemId > 0)
                {
                    var item = await _db.Items.FindAsync(model.ItemId);
                    if (item != null)
                    {
                        item.ItemName = model.ItemName;
                        item.PsirsCodeId = model.PsirsCodeId;
                        _db.Entry(item).State = EntityState.Modified;
                        await _db.SaveChangesAsync();
                        message = $"{model.ItemName} Updated Successfully...";
                        return new JsonResult { Data = new { status = true, message = message } };
                    }
                }
                else
                {
                    _db.Items.Add(model);
                    await _db.SaveChangesAsync();
                    message = $"{model.ItemName} Added Successfully.";
                    return new JsonResult { Data = new { status = true, message = message } };
                }
            }
            return new JsonResult { Data = new { status = status, message = message } };
            //return View(subject);
        }

        // GET: Items/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = await _db.Items.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // GET: Items/Create
        public ActionResult Create()
        {
            ViewBag.PsirsCodeId = new SelectList(_db.PsirsCodes, "PsirsCodeId", "CodeName");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Item item)
        {
            if (ModelState.IsValid)
            {
                _db.Items.Add(item);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.PsirsCodeId = new SelectList(_db.PsirsCodes, "PsirsCodeId", "CodeName", item.PsirsCodeId);
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = await _db.Items.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.PsirsCodeId = new SelectList(_db.PsirsCodes, "PsirsCodeId", "CodeName", item.PsirsCodeId);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Item item)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(item).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.PsirsCodeId = new SelectList(_db.PsirsCodes, "PsirsCodeId", "CodeName", item.PsirsCodeId);
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = await _db.Items.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Item item = await _db.Items.FindAsync(id);
            _db.Items.Remove(item);
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

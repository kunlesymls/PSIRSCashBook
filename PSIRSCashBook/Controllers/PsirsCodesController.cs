using PSIRSCashBook.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PSIRSCashBook.Controllers
{
    public class PsirsCodesController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: PsirsCodes
        public async Task<ActionResult> Index()
        {
            return View(await _db.PsirsCodes.ToListAsync());
        }

        public async Task<ActionResult> GetIndex()
        {
            // dc.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
            var data = await _db.PsirsCodes.AsNoTracking().Select(s => new
            {
                s.PsirsCodeId,
                s.CodeName
            }).ToListAsync();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        public async Task<PartialViewResult> Save(int id)
        {
            var code = await _db.PsirsCodes.FindAsync(id);
            return PartialView(code);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(PsirsCode model)
        {
            bool status = false;
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                if (model.PsirsCodeId > 0)
                {
                    var code = await _db.PsirsCodes.FindAsync(model.PsirsCodeId);
                    if (code != null)
                    {
                        code.CodeName = model.CodeName;

                        _db.Entry(code).State = EntityState.Modified;
                        await _db.SaveChangesAsync();
                        message = $"{model.CodeName} Updated Successfully...";
                        return new JsonResult { Data = new { status = true, message = message } };
                    }
                }
                else
                {
                    _db.PsirsCodes.Add(model);
                    await _db.SaveChangesAsync();
                    message = $"{model.CodeName} Added Successfully.";
                    return new JsonResult { Data = new { status = true, message = message } };
                }
            }
            return new JsonResult { Data = new { status = status, message = message } };
            //return View(subject);
        }


        // GET: PsirsCodes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PsirsCode psirsCode = await _db.PsirsCodes.FindAsync(id);
            if (psirsCode == null)
            {
                return HttpNotFound();
            }
            return View(psirsCode);
        }

        // GET: PsirsCodes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PsirsCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PsirsCode psirsCode)
        {
            if (ModelState.IsValid)
            {
                _db.PsirsCodes.Add(psirsCode);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(psirsCode);
        }

        // GET: PsirsCodes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PsirsCode psirsCode = await _db.PsirsCodes.FindAsync(id);
            if (psirsCode == null)
            {
                return HttpNotFound();
            }
            return View(psirsCode);
        }

        // POST: PsirsCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PsirsCode psirsCode)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(psirsCode).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(psirsCode);
        }

        // GET: PsirsCodes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PsirsCode psirsCode = await _db.PsirsCodes.FindAsync(id);
            if (psirsCode == null)
            {
                return HttpNotFound();
            }
            return View(psirsCode);
        }

        // POST: PsirsCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PsirsCode psirsCode = await _db.PsirsCodes.FindAsync(id);
            _db.PsirsCodes.Remove(psirsCode);
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

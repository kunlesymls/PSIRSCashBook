using PSIRSCashBook.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PSIRSCashBook.Controllers
{
    public class StaffsController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Staffs
        public ActionResult Index()
        {
            return View();
        }


        public async Task<ActionResult> GetIndex(string gender)
        {
            #region Server Side filtering

            //Get parameter for sorting from grid table
            // get Start (paging start index) and length (page size for paging)
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Get Sort columns values when we click on Header Name of column
            //getting column name
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            //Soring direction(either desending or ascending)
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            string search = Request.Form.GetValues("search[value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int totalRecords = 0;

            var studentIndex = new List<Staff>();

            if (!string.IsNullOrEmpty(search))
            {
                var v = await _db.Staffs.AsNoTracking()
                                        .Where(x => x.StaffId.ToUpper().Equals(search.ToUpper().Trim())
                                        || x.FirstName.ToUpper().Equals(search.ToUpper().Trim())
                                        || x.LastName.ToUpper().Equals(search.ToUpper().Trim())
                                        || x.MiddleName.ToUpper().Equals(search.ToUpper().Trim()))
                                        .ToListAsync();
                // Mapping the student to the correct ViewModel for json display
                studentIndex.AddRange(v);
            }
            else
            {
                var v = await _db.Staffs.AsNoTracking().ToListAsync();
                // Mapping the student to the correct ViewModel for json display
                studentIndex.AddRange(v);
            }

            totalRecords = studentIndex.Count();
            var data = studentIndex.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw, recordsFiltered = totalRecords, recordsTotal = totalRecords, data = data },
                JsonRequestBehavior.AllowGet);

            #endregion Server Side filtering
        }


        public async Task<PartialViewResult> PartialDetails(string id)
        {
            if (id == null)
            {
                id = "";
            }

            var staff = await _db.Staffs.FindAsync(id);

            return PartialView(staff);
        }

        // GET: Staffs/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = await _db.Staffs.FindAsync(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            return View(staff);
        }

        // GET: Staffs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Staffs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "StaffId,Designation,MaritalStatus,IsActiveStaff,ActiveStatus,FirstName,MiddleName,LastName,Gender,Email,PhoneNumber,Religion,TownOfBirth,StateOfOrigin,Nationality,CountryOfBirth,DateOfBirth,Age,Passport")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                _db.Staffs.Add(staff);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(staff);
        }

        // GET: Staffs/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = await _db.Staffs.FindAsync(id);
            if (staff == null)
            {
                return HttpNotFound();
            }
            var mystates = from State s in Enum.GetValues(typeof(State))
                           select new { ID = s, Name = s.ToString() };
            var religion = from Religion s in Enum.GetValues(typeof(Religion))
                           select new { ID = s, Name = s.ToString() };
            var mygender = from Gender s in Enum.GetValues(typeof(Gender))
                           select new { ID = s, Name = s.ToString() };
            var maritalStatus = from Maritalstatus s in Enum.GetValues(typeof(Maritalstatus))
                                select new { ID = s, Name = s.ToString() };

            ViewBag.MaritalStatus = new SelectList(maritalStatus, "Name", "Name");
            ViewBag.Religion = new SelectList(religion, "Name", "Name");
            ViewBag.StateOfOrigin = new SelectList(mystates, "Name", "Name");
            ViewBag.Gender = new SelectList(mygender, "Name", "Name");
            return View(staff);
        }

        // POST: Staffs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Staff staff)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(staff).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var mystates = from State s in Enum.GetValues(typeof(State))
                           select new { ID = s, Name = s.ToString() };
            var religion = from Religion s in Enum.GetValues(typeof(Religion))
                           select new { ID = s, Name = s.ToString() };
            var mygender = from Gender s in Enum.GetValues(typeof(Gender))
                           select new { ID = s, Name = s.ToString() };
            var maritalStatus = from Maritalstatus s in Enum.GetValues(typeof(Maritalstatus))
                                select new { ID = s, Name = s.ToString() };

            ViewBag.MaritalStatus = new SelectList(maritalStatus, "Name", "Name");
            ViewBag.Religion = new SelectList(religion, "Name", "Name");
            ViewBag.StateOfOrigin = new SelectList(mystates, "Name", "Name");
            ViewBag.Gender = new SelectList(mygender, "Name", "Name");
            return View(staff);
        }

        // GET: Staffs/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Staff staff = await _db.Staffs.FindAsync(id);
            if (staff == null)
            {
                return HttpNotFound();
            }

            return View(staff);
        }

        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Staff staff = await _db.Staffs.FindAsync(id);
            _db.Staffs.Remove(staff);
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

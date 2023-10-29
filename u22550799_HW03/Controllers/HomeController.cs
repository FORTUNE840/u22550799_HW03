using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Data.Entity;
using u22550799_HW03.Models;
using System.Net;
using PagedList;

namespace u22550799_HW03.Controllers
{
    public class HomeController : Controller
    {
        private readonly LibraryEntities db = new LibraryEntities();
        public async Task<ActionResult> CombinedIndex(int? bookPages, int?studentPages)
        {
            int pageSize = 5;

            var students = await db.students.ToListAsync();

            var books = await db.books.ToListAsync();

            IPagedList<students> studentsPage = students.ToPagedList(studentPages ?? 1, pageSize);
            IPagedList<books> booksPages = books.ToPagedList(bookPages ?? 1, pageSize);

            var viewModel = new firstCombinedViewModel
            {
                students = studentsPage,
                books = booksPages
            };

            return View(viewModel);
        }

        public async Task<ActionResult> CombinedIndex2(int? authorPages, int?typePages, int? borrowPages )
        {
            int pageSize = 5;

            var authors = await db.authors.ToListAsync();

            var types = await db.types.ToListAsync();

            var borrows = await db.borrows.ToListAsync();

            IPagedList<authors> authorsPages = authors.ToPagedList(authorPages ?? 1, pageSize);
            IPagedList<types> typesPages = types.ToPagedList(typePages ?? 1, pageSize);
            IPagedList<borrows> borrowsPages = borrows.ToPagedList(borrowPages ?? 1, pageSize);
            var viewModel = new secondCombinedViewModel
            {
                Authors = authorsPages,
                Types = typesPages,
                Borrows = borrowsPages
            };
            return View(viewModel);
        }

        public ActionResult ReportView()
        {
            return View();
        }

        // GET: Students
        public async Task<ActionResult> studentsIndex()
        {
            return View(await db.students.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<ActionResult> studentsDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            students Student = await db.students.FindAsync(id);
            if (Student == null)
            {
                return HttpNotFound();
            }
            return View(Student);
        }

        // GET: Students/Create
        public ActionResult studentsCreate()
        {
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> studentsCreate([Bind(Include = "studentId,name,surname,birthdate,gender,class,point")] students Student)
        {
            if (ModelState.IsValid)
            {
                db.students.Add(Student);
                await db.SaveChangesAsync();
                return RedirectToAction("studentsIndex");
            }

            return View(Student);
        }

        // GET: Books
        public async Task<ActionResult> booksIndex()
        {
            var bookEntity = db.books
                .Include(a => a.authors)
                .Include(t => t.types);
            return View(await bookEntity.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<ActionResult> booksDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            books bookEntity = await db.books.FindAsync(id);
            if (bookEntity == null)
            {
                return HttpNotFound();
            }
            return View(bookEntity);
        }

        // GET: Books/Create
        public ActionResult booksCreate()
        {
            ViewBag.authorName = new SelectList(db.authors, "name", "surname", "birthdate", "gender", "class", "point");
            ViewBag.typeName = new SelectList(db.types, "name");
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> booksCreate([Bind(Include = "bookId,name,pagecount,point,authorId,typeId")] books bookEntity)
        {
            if (ModelState.IsValid)
            {
                db.books.Add(bookEntity);
                await db.SaveChangesAsync();
                return RedirectToAction("booksIndex");
            }

            ViewBag.authorName = new SelectList(db.authors, "name", "surname", "birthdate", "gender", "class", bookEntity.name);
            ViewBag.typeName = new SelectList(db.types, "name");
            return View(bookEntity);
        }

        // GET: Borrows
        public async Task<ActionResult> borrowsIndex()
        {
            var borrowEntity = db.borrows
                .Include(s => s.students)
                .Include(b => b.books);
            return View(await borrowEntity.ToListAsync());
        }

        // GET: Borrows/Details/5
        public async Task<ActionResult> borrowsDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            borrows borrowEntity = await db.borrows.FindAsync(id);
            if (borrowEntity == null)
            {
                return HttpNotFound();
            }
            return View(borrowEntity);
        }

        // GET: Borrows/Create
        public ActionResult borrowsCreate()
        {
            ViewBag.studentName = new SelectList(db.students, "name", "surname", "birthdate", "gender", "class", "point");
            ViewBag.bookName = new SelectList(db.books, "name", "pagecount", "point");
            return View();
        }

        // POST: Borrows/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> borrowsCreate([Bind(Include = "borrowId,studentId,bookId,takenDate,broughtDate")] borrows borrowEntity)
        {
            if (ModelState.IsValid)
            {
                db.borrows.Add(borrowEntity);
                await db.SaveChangesAsync();
                return RedirectToAction("borrowsIndex");
            }

            ViewBag.studentName = new SelectList(db.students, "name", "surname", "birthdate", "gender", "class", "point");
            ViewBag.bookName = new SelectList(db.books, "name", "pagecount", "point");
            return View(borrowEntity);
        }

        // GET: Borrows/Edit/5
        public async Task<ActionResult> borrowsEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            borrows borrowEntity = await db.borrows.FindAsync(id);
            if (borrowEntity == null)
            {
                return HttpNotFound();
            }
            ViewBag.studentName = new SelectList(db.students, "name", "surname", "birthdate", "gender", "class", "point");
            ViewBag.bookName = new SelectList(db.books, "name", "pagecount", "point");
            return View(borrowEntity);
        }

        // POST: Borrows/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> borrowsEdit([Bind(Include = "borrowId,studentId,bookId,takenDate,broughtDate")] borrows borrowEntity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(borrowEntity).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("borrowsIndex");
            }
            ViewBag.studentName = new SelectList(db.students, "name", "surname", "birthdate", "gender", "class", "point");
            ViewBag.bookName = new SelectList(db.books, "name", "pagecount", "point");
            return View(borrowEntity);
        }

        // GET: Borrows/Delete/5
        public async Task<ActionResult> borrowsDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            borrows borrowsEntity = await db.borrows.FindAsync(id);
            if (borrowsEntity == null)
            {
                return HttpNotFound();
            }
            return View(borrowsEntity);
        }

        // POST: Borrows/Delete/5
        [HttpPost, ActionName("borrowsDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> borrowsDeleteConfirmed(int id)
        {
            borrows borrowsEntity = await db.borrows.FindAsync(id);
            db.borrows.Remove(borrowsEntity);
            await db.SaveChangesAsync();
            return RedirectToAction("borrowsIndex");
        }

        // GET: Types
        public async Task<ActionResult> typesIndex()
        {
            return View(await db.types.ToListAsync());
        }

        // GET: Types/Details/5
        public async Task<ActionResult> typesDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            types typeEntity = await db.types.FindAsync(id);
            if (typeEntity == null)
            {
                return HttpNotFound();
            }
            return View(typeEntity);
        }

        // GET: Types/Create
        public ActionResult typesCreate()
        {
            return View();
        }

        // POST: Types/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> typesCreate([Bind(Include = "typeId,name")] types typeEntity)
        {
            if (ModelState.IsValid)
            {
                db.types.Add(typeEntity);
                await db.SaveChangesAsync();
                return RedirectToAction("typesIndex");
            }

            return View(typeEntity);
        }

        // GET: Types/Edit/5
        public async Task<ActionResult> typesEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            types typeEntity = await db.types.FindAsync(id);
            if (typeEntity == null)
            {
                return HttpNotFound();
            }
            return View(typeEntity);
        }

        // POST: Types/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> typesEdit([Bind(Include = "typeId,name")] types typeEntity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(typeEntity).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("typesIndex");
            }
            return View(typeEntity);
        }

        // GET: Types/Delete/5
        public async Task<ActionResult> typesDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            types typeEntity = await db.types.FindAsync(id);
            if (typeEntity == null)
            {
                return HttpNotFound();
            }
            return View(typeEntity);
        }

        // POST: Types/Delete/5
        [HttpPost, ActionName("typesDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> typesDeleteConfirmed(int id)
        {
            types typeEntity = await db.types.FindAsync(id);
            db.types.Remove(typeEntity);
            await db.SaveChangesAsync();
            return RedirectToAction("typesIndex");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OnlineBookstore.Models;

namespace OnlineBookstore.Views
{
    
    public class BooksController : Controller
    {
		private BookstoreDBEntities1 db = new BookstoreDBEntities1();
        private Order order = new Order();
        private OrderItem orderitem = new OrderItem();
        private Customer customer = new Customer();
        private Author author = new Author();
       
        Phone phone = new Phone();
        Address address = new Address();
        Email email = new Email();

        // GET: Books
        public ActionResult Index(string searchString)
        {
            //var books = db.Books.Include(b => b.Supplier);
            //string searchString = id;

            var books = from b in db.Books
                         select b;

            if (!String.IsNullOrEmpty(searchString)) 
            { 
                books = books.Where(s => s.Title.Contains(searchString)); 
            } 
 
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ViewBag.Name = user.Name;

                ViewBag.displayMenu = "No";

                if (isAdminUser())
                {
                    ViewBag.displayMenu = "Yes";
                }
                return View(books.ToList());
            }
            else
            {
                ViewBag.Name = "Not Logged IN";
            }
            return View(books.ToList());
        }

        // GET: Books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        [Authorize(Roles = "Admin")]
        // GET: Books/Create
        public ActionResult Create()
        {
            ViewBag.ID = new SelectList(db.Suppliers, "ID", "Name");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ISBN,PublicationDate,UserReview,Title,Price,ID")] Book book)
        {

            if (ModelState.IsValid)
            {

                //Add Author
                author.Fname = "Anna";
                author.Lname = "Wol";
                author.DateOfBirth = "06/01/2019";
                author.Gender = "Female";

                address.Street = "1234 Main";
                address.City = "Katy";
                address.State = "TX";
                address.PostalCode = "77450";

                author.Addresses.Add(address);

                //Add Phone
                phone.PhoneNumber = "713.123.1234";
                phone.Type = "mobile";

                author.Phones.Add(phone);

                email.EmailAddress = "author@hotmail.com";
                email.Type = "personal";

                author.Emails.Add(email);

                //author.Books.Add(book);

                db.Authors.Add(author);
                db.SaveChanges();

                book.UserReview = "Love this book! Great ending!";

                db.Books.Add(book);
                db.SaveChanges();


                return RedirectToAction("Index");
            }

            ViewBag.ID = new SelectList(db.Suppliers, "ID", "Name", book.ID);
            return View(book);
        }

        [Authorize(Roles = "Admin")]
        // GET: Books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = new SelectList(db.Suppliers, "ID", "Name", book.ID);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ISBN,PublicationDate,UserReview,Title,Price,ID")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID = new SelectList(db.Suppliers, "ID", "Name", book.ID);
            return View(book);
        }

        [Authorize(Roles = "Admin")]
        // GET: Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Books/Create
        public ActionResult Order(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = new SelectList(db.Suppliers, "ID", "Name", book.ID);
            return View(book);
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Order([Bind(Include = "ISBN,PublicationDate,UserReview,Title,Price,ID")] Book book)
        {
            if (ModelState.IsValid)
            {
                order.OrderDate = DateTime.Now;
                order.OrderValue = book.Title;

                var user = System.Web.HttpContext.Current.User.Identity.GetUserName();

                var customers = from c in db.Customers
                            select c;

                foreach (var customer in customers.Where(s => s.Username.StartsWith(user)))
                {
                    order.CustomerID = customer.CustomerID;
                }

                db.Orders.Add(order);
                db.SaveChanges();

                orderitem.ISBN = book.ISBN;
                orderitem.ItemPrice = book.Price;
                //orderitem.OrderID = order.OrderID;
                orderitem.OrderID = order.OrderID;

                db.OrderItems.Add(orderitem);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.ID = new SelectList(db.Suppliers, "ID", "Name", book.ID);
            return View(book);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public Boolean isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Admin")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}

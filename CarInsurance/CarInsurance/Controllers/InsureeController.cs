using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // GET: Insuree
        public ActionResult Index()
        {
            return View(db.Insurees.ToList());
        }

        
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
            return View();
        }

       
        [HttpPost] //Sees if Profile is valid
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                db.Insurees.Add(insuree);
                db.SaveChanges();
                CalculateQuote(insuree.Id);
                return RedirectToAction("Index");
            }

            return View(insuree);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insuree).State = EntityState.Modified;
                db.SaveChanges();
                CalculateQuote(insuree.Id);
                return RedirectToAction("Index");
            }
            return View(insuree);
        }

        // GET: Insuree/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insuree insuree = db.Insurees.Find(id);
            db.Insurees.Remove(insuree);
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

        public ActionResult CalculateQuote(int Id)
        {
            using (InsuranceEntities db = new InsuranceEntities())
            {
                var insuree = db.Insurees.Find(Id);
                var dateOfBirth = insuree.DateOfBirth;
                var carYear = insuree.CarYear;
                var carModel = insuree.CarModel;
                var carMake = insuree.CarMake;
                var speedingTickets = insuree.SpeedingTickets;
                var dui = insuree.DUI;
                var coverageType = insuree.CoverageType;

                var quote = 50.0M;

                if (dateOfBirth.Year >= 2003)
                {
                    quote += 100.00M;
                }
                else if (dateOfBirth.Year <= 2001 && dateOfBirth.Year >= 1996)
                {
                    quote += 50.0M;
                }
                else if (dateOfBirth.Year < 1996)
                {
                    quote += 25.0M;
                }

                if (carYear < 2000)
                {
                    quote += 25.0M;
                }
                else if (carYear > 2015)
                {
                    quote += 25.0M;
                }

                if (carMake == "Porsche")
                {
                    quote += 25.0M;
                }

                if (carMake == "Porsche" && carModel == "911 Carrera")
                {
                    quote += 25.0M;
                }

                quote = quote + (speedingTickets * 10.0M);

                if (dui == true)
                {
                    quote += (quote / 4.0M);
                }

                if (coverageType == true)
                {
                    quote += (quote / 2.0M);
                }
                insuree.Quote = quote;
                db.SaveChanges();

            }
            return View("Index");
        }

    }
}

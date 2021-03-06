﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ogsysCRM.Models;
using ogsysCRM.Services;
using ogsysCRM.ViewModels;
using AutoMapper;

namespace ogsysCRM.Controllers
{
    public class NoteController : Controller
    {
        private readonly CRMService _service;

        /// <summary>
        /// Initializes a new instance of the NoteController class.
        /// </summary>
        /// <param name="service"></param>
        public NoteController(CRMService service)
        {
            _service = service;
        }

        // GET: /Note/
        public ActionResult Index([Bind(Prefix="id")]int? customerId)
        {
            if (customerId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var customer = _service.GetCustomerById(customerId.Value);

            if (customer != null)
            {
                return View(Mapper.Map<DetailsCustomerViewModel>(customer));
            }

            return HttpNotFound();
        }

        // GET: /Note/Details/5
        public ActionResult Details([Bind(Prefix = "id")]int? customerId)
        {
            if (customerId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = _service.NoteById(customerId.Value);
            if (note == null)
            {
                return HttpNotFound();
            }
            var nvm = Mapper.Map<NoteViewModel>(note);
            return View(nvm);
        }

        // GET: /Note/Create
        public ActionResult Create([Bind(Prefix = "id")]int? customerId)
        {
            if (customerId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NoteViewModel nvm = new NoteViewModel()
            {
                CustomerId = customerId.Value
            };
            return View(nvm);
        }

        // POST: /Note/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NoteViewModel nvm)
        {
            if (ModelState.IsValid)
            {
                Note note = Mapper.Map<Note>(nvm);
                var customer = _service.GetCustomerById(nvm.CustomerId);
                var user = _service.GetUserByUserName(User.Identity.Name);
                note.Customer = customer;
                note.User = user;
                _service.AddNote(note);
                return RedirectToAction("Details", "Customer", new { id = nvm.CustomerId});
            }

            return View(nvm);
        }

        // GET: /Note/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = _service.NoteById(id.Value);
            if (note == null)
            {
                return HttpNotFound();
            }
            NoteViewModel nvm = Mapper.Map<NoteViewModel>(note);
            return View(nvm);
        }

        // POST: /Note/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NoteViewModel nvm)
        {
            if (ModelState.IsValid)
            {
                //Users can only update the body
                Note note = _service.NoteById(nvm.Id);
                note.Body = nvm.Body;

                _service.UpdateNote(note);
                return RedirectToAction("Details", "Customer", new { id = note.Customer.Id});
            }
            return View(nvm);
        }

        // GET: /Note/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = _service.NoteById(id.Value);
            if (note == null)
            {
                return HttpNotFound();
            }
            NoteViewModel nvm = Mapper.Map<NoteViewModel>(note);
            return View(nvm);
        }

        // POST: /Note/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(NoteViewModel nvm)
        {
            _service.DeleteNoteById(nvm.Id);
            return RedirectToAction("Details", "Customer", new { id = nvm.CustomerId });
        }
    }
}

﻿using ajaxTest.DAL;
using ajaxTest.Models;
using ajaxTest.ViewModels;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ajaxTest.Controllers
{
    public class ListController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: 
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult createList(List list)
        {
            try
            {
                db.Lists.Add(list);
                db.SaveChanges();
            }
            catch (DataException dex)
            {
                string message = dex.ToString();
                return Json(new { success = false, responseText = message, JsonRequestBehavior.AllowGet });
            }

            return Json(new { success = true, JsonRequestBehavior.AllowGet });
        }

        public JsonResult getList()
        {
            try
            {
                var lists = db.Lists.OrderByDescending(o => o.ID).Select(x => new ListViewModel
                {

                    ID = x.ID,
                    Name = x.Name,
                    Tasks = db.Tasks.Where(c => c.ListID == x.ID).OrderBy(o => o.ID).Select(y => new TaskViewModel
                    {
                        ID = y.ID,
                        Description = y.Description,
                        IsDone = y.IsDone

                    }).ToList()

                }).ToList();

                return Json(lists, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteList(int id)
        {
            try
            {
                List list = db.Lists.Find(id);
                db.Lists.Remove(list);
                db.SaveChanges();
            }
            catch (DataException dex)
            {
                string message = dex.ToString();
                return Json(new
                {
                    success = false,
                    responseText = message,
                    JsonRequestBehavior.AllowGet
                });
            }

            return Json(new { success = true, JsonRequestBehavior.AllowGet });
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
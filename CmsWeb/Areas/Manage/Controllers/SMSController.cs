﻿using System.Web.Mvc;
using CmsWeb.Models;

namespace CmsWeb.Areas.Manage.Controllers
{
    [RouteArea("Manage")]
    public class SMSController : Controller
    {
        [Route("~/SMS/List")]
        public ActionResult Index(SMSModel m)
        {
            if (m == null) 
                m = new SMSModel();
            else
                UpdateModel(m.Pager);
            return View(m);
        }

        [Route("~/SMS/Details/{id:int}")]
        public ActionResult Details( int id )
        {
            ViewBag.ListID = id;
            return View();
        }
    }
}

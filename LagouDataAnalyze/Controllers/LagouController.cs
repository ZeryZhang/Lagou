using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lagou.Repository;
using Newtonsoft.Json;
using Lagou.Repository.Entity;

namespace LagouDataAnalyze.Controllers
{
    public class LagouController : Controller
    {
        private JobRepository repository;
        public LagouController()
        {
            repository = new JobRepository();
        }

        //
        // GET: /Lagou/

        public ActionResult Index()
        {
            
            return View();
        }


        public string  QueryJCityNeedJobNum()
        {
            var result = repository.QueryJCityNeedJobNum();
            var json = JsonConvert.SerializeObject(result);

            return json;
        }

    }
}

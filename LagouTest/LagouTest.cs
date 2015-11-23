using System;
using Lagou.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LagouTest
{
    [TestClass]
    public class LagouTest
    {
        private LagouDataController controller;
        public LagouTest()
        {
            controller = new LagouDataController();
        }

        [TestMethod]
        public void QuerySalaryWorkYear()
        {
            controller.QueryWorkYearJobNum();
        }

        [TestMethod]
        public void QueryPositionNameSalary()
        {
            controller.QueryPositionNameSalary("java");
        }

        [TestMethod]
        public void QueryIndustrySalary()
        {
            controller.QueryIndustrySalary();
        }

        [TestMethod]
        public void QueryFinanceStage()
        {
            controller.QueryFinanceStage();
        }




    }
}

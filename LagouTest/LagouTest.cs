using System;
using Lagou.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LagouTest
{
    [TestClass]
    public class LagouTest
    {
        [TestMethod]
        public void QuerySalaryWorkYear()
        {
            LagouDataController controller = new LagouDataController();
            controller.QuerySalaryWorkYear("java");
        }
    }
}

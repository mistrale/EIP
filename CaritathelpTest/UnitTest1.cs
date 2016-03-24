using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Caritathelp.Common;
namespace CaritathelpTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void InscriptionSuccessTest()
        {
            Caritathelp.Inscription controller = new Caritathelp.Inscription();
            controller.Name.Text = "";
            controller.Register_click();
            Assert.AreEqual(controller.Warning.Text, "Name empty.");
        }
    }
}

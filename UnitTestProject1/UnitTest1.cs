using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprite;
using System.Windows.Forms;
using Microsoft.Win32;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SCConstructor()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.ShowDialog();
            SpriteContainer testSprite = new SpriteContainer(new Uri(openFile.FileName));
            Assert.AreEqual(testSprite.width, 120);
            Assert.AreEqual(testSprite.height, 40);
        }

        [TestMethod]
        public void SCXMLWriter()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.ShowDialog();
            SpriteContainer testSprite = new SpriteContainer(new Uri(openFile.FileName));
            XElement testXml = testSprite.ToXML();
            XElement compare = new XElement("Sprite");
            compare.Add(new XAttribute("Name", "ATest"));
            compare.Add(new XAttribute("X", 0));
            compare.Add(new XAttribute("Y", 0));
            compare.Add(new XAttribute("Width", 120));
            compare.Add(new XAttribute("Height", 40));
            Assert.AreEqual(testXml.Attribute("Name").Value, compare.Attribute("Name").Value);
            Assert.AreEqual(testXml.Attribute("X").Value, compare.Attribute("X").Value);
            Assert.AreEqual(testXml.Attribute("Y").Value, compare.Attribute("Y").Value);
            Assert.AreEqual(testXml.Attribute("Width").Value, compare.Attribute("Width").Value);
            Assert.AreEqual(testXml.Attribute("Height").Value, compare.Attribute("Height").Value);
        }
    }
}

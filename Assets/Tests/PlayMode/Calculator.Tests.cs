using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KevinSkyba
{
    namespace EGHG
    {
        public class CalculatorTests
        {
            private Calculator calculator;

            [SetUp]
            public void SetUp()
            {
                GameObject go = new GameObject();
                calculator = go.AddComponent<Calculator>();
            }

            [Test()]
            [TestCase(3, 8)]
            [TestCase(5, 6)]
            [TestCase(2, 9)]
            public void Test_Simple_Add(int a, int b)
            {
                Assert.AreEqual("0", calculator.CurrentText);

                calculator.OnButtonClick(a.ToString());
                Assert.AreEqual(a.ToString(), calculator.CurrentText);

                calculator.OnButtonClick("+");
                Assert.AreEqual(a.ToString(), calculator.CurrentText);

                calculator.OnButtonClick(b.ToString());
                Assert.AreEqual(b.ToString(), calculator.CurrentText);

                calculator.OnButtonClick("=");
                Assert.AreEqual("11", calculator.CurrentText);
            }

            [Test()]
            [TestCase(8, 3)]
            [TestCase(6, 1)]
            [TestCase(5, 0)]
            public void Test_Simple_Sub(int a, int b)
            {
                Assert.AreEqual("0", calculator.CurrentText);

                calculator.OnButtonClick(a.ToString());
                Assert.AreEqual(a.ToString(), calculator.CurrentText);

                calculator.OnButtonClick("-");
                Assert.AreEqual(a.ToString(), calculator.CurrentText);

                calculator.OnButtonClick(b.ToString());
                Assert.AreEqual(b.ToString(), calculator.CurrentText);

                calculator.OnButtonClick("=");
                Assert.AreEqual("5", calculator.CurrentText);
            }

            [Test()]
            [TestCase(3, 8)]
            [TestCase(8, 3)]
            [TestCase(6, 4)]
            public void Test_Simple_Mult(int a, int b)
            {
                Assert.AreEqual("0", calculator.CurrentText);

                calculator.OnButtonClick(a.ToString());
                Assert.AreEqual(a.ToString(), calculator.CurrentText);

                calculator.OnButtonClick("x");
                Assert.AreEqual(a.ToString(), calculator.CurrentText);

                calculator.OnButtonClick(b.ToString());
                Assert.AreEqual(b.ToString(), calculator.CurrentText);

                calculator.OnButtonClick("=");
                Assert.AreEqual("24", calculator.CurrentText);
            }

            [Test()]
            [TestCase(8, 2)]
            [TestCase(4, 1)]
            public void Test_Simple_Div(int a, int b)
            {
                Assert.AreEqual("0", calculator.CurrentText);

                calculator.OnButtonClick(a.ToString());
                Assert.AreEqual(a.ToString(), calculator.CurrentText);

                calculator.OnButtonClick("/");
                Assert.AreEqual(a.ToString(), calculator.CurrentText);

                calculator.OnButtonClick(b.ToString());
                Assert.AreEqual(b.ToString(), calculator.CurrentText);

                calculator.OnButtonClick("=");
                Assert.AreEqual("4", calculator.CurrentText);
            }

            [Test()]
            [TestCase(2, 4)]
            [TestCase(4, 5)]
            [TestCase(5, 100)]
            [TestCase(1, 2)]
            public void Test_Continous_Add(int a, int n)
            {
                Assert.AreEqual("0", calculator.CurrentText);

                calculator.OnButtonClick(a.ToString());
                Assert.AreEqual(a.ToString(), calculator.CurrentText);

                calculator.OnButtonClick("+");
                Assert.AreEqual(a.ToString(), calculator.CurrentText);

                for (int i = 0; i < n; i++)
                {
                    calculator.OnButtonClick(a.ToString());
                    Assert.AreEqual(a.ToString(), calculator.CurrentText);

                    calculator.OnButtonClick("+");
                    Assert.AreEqual((a * (i + 2)).ToString(), calculator.CurrentText);
                }
            }

            [Test()]
            [TestCase(5, 5)]
            [TestCase(2, 8)]
            [TestCase(8, 2)]
            [TestCase(1, 9)]
            public void Test_AC_Add(int a, int b)
            {
                Assert.AreEqual("0", calculator.CurrentText);

                calculator.OnButtonClick(a.ToString());
                Assert.AreEqual(a.ToString(), calculator.CurrentText);

                calculator.OnButtonClick("+");
                Assert.AreEqual(a.ToString(), calculator.CurrentText);

                calculator.OnButtonClick("AC");
                Assert.AreEqual("0", calculator.CurrentText);

                calculator.OnButtonClick(a.ToString());
                Assert.AreEqual(a.ToString(), calculator.CurrentText);

                calculator.OnButtonClick("+");
                Assert.AreEqual(a.ToString(), calculator.CurrentText);

                calculator.OnButtonClick(b.ToString());
                Assert.AreEqual(b.ToString(), calculator.CurrentText);

                calculator.OnButtonClick("=");
                Assert.AreEqual("10", calculator.CurrentText);
            }

            /*
            [Test()]
            [TestCase(8, 2)]
            [TestCase(5, 2)]
            [TestCase(4, 1)]
            public void Test_Div_Mult(int a, int b)
            {
                Assert.AreEqual("0", calculator.CurrentText);

                calculator.OnButtonClick(a.ToString());
                Assert.AreEqual(a.ToString(), calculator.CurrentText);

                calculator.OnButtonClick("x");
                Assert.AreEqual(a.ToString(), calculator.CurrentText);

                calculator.OnButtonClick(b.ToString());
                Assert.AreEqual(b.ToString(), calculator.CurrentText);

                calculator.OnButtonClick("/");
                Assert.AreEqual((a*b).ToString(), calculator.CurrentText);

                calculator.OnButtonClick(b.ToString());
                Assert.AreEqual(b.ToString(), calculator.CurrentText);

                calculator.OnButtonClick("=");
                Assert.AreEqual(a, calculator.CurrentText);
            }
            */
        }
    }
}
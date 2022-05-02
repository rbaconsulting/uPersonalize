using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace uPersonalize.Tests
{
    [TestClass]
    public class Initialize
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            Console.WriteLine("Inside AssemblyInitialize");
        }
    }

    public class DeInitialize
    {
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Console.WriteLine("Inside AssemblyCleanup");
        }
    }

    [TestClass]
    public class TestClass1
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Console.WriteLine("Inside ClassInitialize");
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Console.WriteLine("Inside ClassCleanup");
        }

        [TestMethod]
        public void Test_1()
        {
            Console.WriteLine("Inside TestMethod Test_1");
        }
    }

    [TestClass]
    public class TestClass2
    {
        [TestInitialize]
        public void TestInitialize()
        {
            Console.WriteLine("Inside TestInitialize");
        }

        [TestMethod]
        public void Test_2()
        {
            Console.WriteLine("Inside TestMethod Test_2");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Console.WriteLine("Inside TestCleanup");
        }
    }


    [TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
		}
	}
}

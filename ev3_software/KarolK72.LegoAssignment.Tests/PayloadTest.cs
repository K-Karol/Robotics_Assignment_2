using KarolK72.LegoAssignment.Library;

namespace KarolK72.LegoAssignment.Tests
{
    [TestClass]
    public class PayloadTest
    {
        [TestMethod]
        public void EqualsOperatorTest()
        {
            Payload payload1 = new Payload()
            {
                CommandID = 1,
                Paramaters = new Dictionary<string, string>()
            {
                { "test1" , "123" },
                { "test2" , "345" }
            }
            };
            Payload payload2 = new Payload()
            {
                CommandID = 1,
                Paramaters = new Dictionary<string, string>()
            {
                { "test1" , "123" },
                { "test2" , "345" }
            }
            };
            Assert.IsTrue(payload1.Equals(payload2));

            Payload payload3 = new Payload()
            {
                CommandID = 1,
                Paramaters = new Dictionary<string, string>()
            {
                { "test4" , "123" },
                { "test2" , "345" }
            }
            };

            Assert.IsFalse(payload1.Equals(payload3));

            Payload payload4 = new Payload()
            {
                CommandID = 1,
                Paramaters = new Dictionary<string, string>()
            {
                { "test1" , "123" },
            }
            };

            Assert.IsFalse(payload1.Equals(payload4));


            Payload payload5 = new Payload()
            {
                CommandID = 2,
                Paramaters = new Dictionary<string, string>()
            {
                { "test1" , "123" },
                { "test2" , "345" }
            }
            };


            Assert.IsFalse(payload1.Equals(payload5));

            Payload payload6 = new Payload()
            {
                CommandID = 1,
                Paramaters = new Dictionary<string, string>()
            {
                { "test1" , "123" },
                { "test2" , "345" },
                { "test3" , "345" }
            }
            };

            Assert.IsFalse(payload1.Equals(payload6));

        }

        [TestMethod]
        public void ConvertingStringToPayload()
        {
            string stringPayload1 = "#1|test1:123|test2:345;";
            Payload assertAgainstPayload1 = new Payload()
            {
                CommandID = 1,
                Paramaters = new Dictionary<string, string>()
            {
                { "test1" , "123" },
                { "test2" , "345" }
            }
            };
            Payload? payload = null;
            payload = Payload.Parse(stringPayload1);
            Assert.IsNotNull(payload);
            Assert.IsTrue(payload.Equals(assertAgainstPayload1));

            string stringPayload2 = "#123|Yes:True;";
            Payload assertAgainstPayload2 = new Payload()
            {
                CommandID = 123,
                Paramaters = new Dictionary<string, string>()
            {
                { "Yes" , "True" }
            }
            };
            payload = null;
            payload = Payload.Parse(stringPayload2);
            Assert.IsNotNull(payload);
            Assert.IsTrue(payload.Equals(assertAgainstPayload2));

            string stringPayload3 = "#12";
            payload = null;
            payload = Payload.Parse(stringPayload3);
            Assert.IsNull(payload);
        }
    }
}
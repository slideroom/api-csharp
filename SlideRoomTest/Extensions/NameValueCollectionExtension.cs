using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using System.Linq;

namespace SlideRoomTest.Extensions
{
    public static class NameValueCollectionExtension
    {
        public static void NotContains(this NameValueCollection list, string key)
        {
            Assert.IsFalse(list.AllKeys.Contains(key));
        }

        public static void Contains(this NameValueCollection list, string key)
        {
            Assert.IsTrue(list.AllKeys.Contains(key));
        }

        public static void ContainsAndEquals(this NameValueCollection list, string key, string value)
        {
            list.Contains(key);
            Assert.AreEqual(value, list[key]);
        }
    }
}

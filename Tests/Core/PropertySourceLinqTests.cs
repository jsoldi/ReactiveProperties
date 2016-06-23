using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Utils;
using ReactiveProperties;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Core
{
    [TestClass]
    public class PropertySourceLinqTests
    {
        [TestMethod]
        public void DistinctNotifiesOnceAtRawSubscription()
        {
            var property = new TestPropertySource<string>(null);
            int calls = 0;
            Action observer = () => calls++;

            using (var subs = property.Distinct().RawSubscribe(observer))
            {
                Assert.AreEqual(calls, 1);
            }
        }

        [TestMethod]
        public void DistinctUsesComparerWhenChangesFirst()
        {
            var property = new TestPropertySource<string>(null);
            int calls = 0;
            Action observer = () => calls++;

            using (var subs = property.Distinct(StringComparer.OrdinalIgnoreCase).RawSubscribe(observer))
            {
                property.Value = "abc";
                property.Notify();
                Assert.AreEqual(2, calls);

                property.Value = "ABC";
                property.Notify();
                Assert.AreEqual(2, calls);

                property.Value = "xyz";
                property.Notify();
                Assert.AreEqual(3, calls);
            }
        }

        [TestMethod]
        public void DistinctUsesComparerWhenDoesNotChangeFirst()
        {
            var property = new TestPropertySource<string>("abc");
            int calls = 0;
            Action observer = () => calls++;

            using (var subs = property.Distinct(StringComparer.OrdinalIgnoreCase).RawSubscribe(observer))
            {
                property.Value = "abc";
                property.Notify();
                Assert.AreEqual(1, calls);

                property.Value = "xyz";
                property.Notify();
                Assert.AreEqual(2, calls);

                property.Value = "XYZ";
                property.Notify();
                Assert.AreEqual(2, calls);
            }
        }

        [TestMethod]
        public void EagerNotifiesAtRawSubscription()
        {
            var property = new TestPropertySource<int>(0);
            int calls = 0;
            Action observer = () => calls++;

            using (var subs = property.Lazy().Eager().RawSubscribe(observer))
            {
                Assert.AreEqual(1, calls);
            }
        }

        [TestMethod]
        public void LazyDoesNotNotifyAtRawSubscription()
        {
            var property = new TestPropertySource<int>(0);
            int calls = 0;
            Action observer = () => calls++;

            using (var subs = property.Eager().Lazy().RawSubscribe(observer))
            {
                Assert.AreEqual(0, calls);
            }
        }

        [TestMethod]
        public void SelectManyGetsTheValueReturnedBySelector()
        {
            var property = new TestPropertySource<int>(0);
            var source = property.SelectMany(v => new TestPropertySource<string>("hello"));
            Assert.AreEqual("hello", source.Value);
        }

        [TestMethod]
        public void SelectManyFiresOnLeftChange()
        {
            var property = new TestPropertySource<int>(0);
            var source = property.SelectMany(v => new TestPropertySource<string>("hello"));
            bool called;
            Action observer = () => called = true;

            using (var subs = source.RawSubscribe(observer))
            {
                called = false;
                property.Value = 10;
                property.Notify();
                Assert.AreEqual(true, called);
            }
        }

        [TestMethod]
        public void SelectManyFiresOnRightChange()
        {
            var left = new TestPropertySource<int>(0);
            var right = new TestPropertySource<string>("hello");
            var source = left.SelectMany(v => right);
            bool called;
            Action observer = () => called = true;

            using (var subs = source.RawSubscribe(observer))
            {
                called = false;
                right.Value = "abc";
                right.Notify();
                Assert.AreEqual(true, called);
            }
        }

        [TestMethod]
        public void SelectManyPassesLeftValueToSelector()
        {
            var left = new TestPropertySource<int>(10);
            var oldRight = new TestPropertySource<string>("old");
            var newRight = new TestPropertySource<string>("new");
            var source = left.SelectMany(i => i == 20 ? newRight : oldRight);
            left.Value = 20;
            Assert.AreEqual("new", source.Value);
        }

        [TestMethod]
        public void SelectManyUpdatesRightSubscription()
        {
            var left = new TestPropertySource<bool>(false);
            var oldRight = new TestPropertySource<string>("old");
            var newRight = new TestPropertySource<string>("new");
            var source = left.SelectMany(b => b ? newRight : oldRight);
            bool called;
            Action observer = () => called = true;

            using (var subs = source.RawSubscribe(observer))
            {
                called = false;
                oldRight.SetAndNotify("old 2");
                Assert.AreEqual(true, called);

                Assert.IsNull(newRight.Notify);

                left.SetAndNotify(true);

                Assert.IsNull(oldRight.Notify);

                newRight.SetAndNotify("new 2");
                Assert.AreEqual(true, called);
            }
        }

        [TestMethod]
        public void SelectManyFlattensNested()
        {
            var nested = new TestPropertySource<TestPropertySource<string>>(null);
            nested.Value = new TestPropertySource<string>("one");
            var source = PropertySource.SelectMany(nested, a => a);
            bool called;
            Action observer = () => called = true;

            using (var subs = source.RawSubscribe(observer))
            {
                called = false;
                nested.Value.SetAndNotify("two");
                Assert.AreEqual(true, called);
                Assert.AreEqual("two", source.Value);

                called = false;
                nested.SetAndNotify(new TestPropertySource<string>("three"));
                Assert.AreEqual(true, called);
                Assert.AreEqual("three", source.Value);

                called = false;
                nested.Value.SetAndNotify("four");
                Assert.AreEqual(true, called);
                Assert.AreEqual("four", source.Value);
            }
        }

        [TestMethod]
        public void SelectWorks()
        {
            var left = new TestPropertySource<int>(10);
            var source = PropertySource.Select(left, a => a + 2);
            int value = 0;
            Action<int> observer = val => value = val;

            using (var subs = source.Subscribe(observer))
            {
                Assert.AreEqual(12, value);
                left.SetAndNotify(20);
                Assert.AreEqual(22, value);
            }
        }

        [TestMethod]
        public void MergeWorks()
        {
            var left = new TestPropertySource<string>("a");
            var right = new TestPropertySource<string>("b");
            var merged = PropertySource.Merge(left, right, (l, r) => l + ":" + r);
            string value = null;
            Action<string> observer = val => value = val;

            using (var subs = merged.Subscribe(observer))
            {
                Assert.AreEqual("a:b", value);
                left.SetAndNotify("A");
                Assert.AreEqual("A:b", value);
                right.SetAndNotify("B");
                Assert.AreEqual("A:B", value);
            }
        }

        [TestMethod]
        public void Merge3Works()
        {
            var left = new TestPropertySource<string>("a");
            var middle = new TestPropertySource<string>("b");
            var right = new TestPropertySource<string>("c");
            var merged = PropertySource.Merge(left, middle, right, (l, m, r) => l + ":" + m + ":" + r);
            string value = null;
            Action<string> observer = val => value = val;

            using (var subs = merged.Subscribe(observer))
            {
                Assert.AreEqual("a:b:c", value);
                left.SetAndNotify("A");
                Assert.AreEqual("A:b:c", value);
                middle.SetAndNotify("B");
                Assert.AreEqual("A:B:c", value);
                right.SetAndNotify("C");
                Assert.AreEqual("A:B:C", value);
            }
        }
    }
}

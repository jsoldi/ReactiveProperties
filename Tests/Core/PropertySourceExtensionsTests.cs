using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveProperties;
using System.Reactive.Disposables;
using System.Collections.Generic;
using Tests.Utils;

namespace Tests.Core
{
    [TestClass]
    public class PropertySourceExtensionsTests
    {
        [TestMethod]
        public void SubscribeNotifiesOnceAtSubscriptionWithGivenValue()
        {
            var property = new TestPropertySource<int>(10);
            int calls = 0;
            int value = 0;

            Action<int> observer = val =>
            {
                calls++;
                value = val;
            };

            using (var subs = property.Subscribe(observer))
            {
                Assert.AreEqual(1, calls);
                Assert.AreEqual(10, value);
            }
        }

        [TestMethod]
        public void SubscribeDoesNotNotifyAfterDispose()
        {
            var property = new TestPropertySource<int>(10);
            int calls = 0;
            Action<int> observer = val => calls++;

            using (var subs = property.Subscribe(observer))
            {
                Assert.AreEqual(1, calls);
            }

            Assert.AreEqual(1, calls);
        }

        [TestMethod]
        public void SubscribeOnlyNotifiesAtSubscriptionAndValueChanges()
        {
            var property = new TestPropertySource<int>(10);
            int calls = 0;
            Action<int> observer = val => calls++;

            using (var subs = property.Subscribe(observer))
            {
                Assert.AreEqual(1, calls);
                property.Notify();
                Assert.AreEqual(1, calls);
                property.Value = 20;
                property.Notify();
                Assert.AreEqual(2, calls);
            }
        }

        [TestMethod]
        public void SubscribeUsesComparer()
        {
            var property = new TestPropertySource<string>("abc");
            int calls = 0;
            Action<string> observer = val => calls++;

            using (var subs = property.Subscribe(observer, StringComparer.OrdinalIgnoreCase))
            {
                Assert.AreEqual(1, calls);
                property.Value = "abcd";
                property.Notify();
                Assert.AreEqual(2, calls);
                property.Value = "ABCD";
                property.Notify();
                Assert.AreEqual(2, calls);
                property.Value = "ABCDE";
                property.Notify();
                Assert.AreEqual(3, calls);
            }
        }

        [TestMethod]
        public void SubscribePassesCurrentValue()
        {
            var property = new TestPropertySource<int>(10);
            int latestValue = 0;
            Action<int> observer = val => latestValue = val;

            using (var subs = property.Subscribe(observer))
            {
                Assert.AreEqual(10, latestValue);
                property.SetAndNotify(20);
                Assert.AreEqual(20, latestValue);
            }
        }

        [TestMethod]
        public void SubscribeToChangesNotifiesOnceAtSubscription()
        {
            var property = new TestPropertySource<int>(0);
            int calls = 0;
            Action<ChangeInfo<int>> observer = change => calls++;

            using (var subs = property.SubscribeToChanges(observer))
            {
                Assert.AreEqual(1, calls);
            }

            Assert.AreEqual(1, calls);
        }

        [TestMethod]
        public void SubscribeToChangesNotifiesWithDefaultAndCurrentAtSubscription()
        {
            var property = new TestPropertySource<int>(10);
            ChangeInfo<int> latestChange = new ChangeInfo<int>();
            Action<ChangeInfo<int>> observer = change => latestChange = change;

            using (var subs = property.SubscribeToChanges(observer))
            {
                Assert.AreEqual(0, latestChange.Old);
                Assert.AreEqual(10, latestChange.New);
            }
        }

        [TestMethod]
        public void SubscribeToChangesNotifiesWithOldAndNewValues()
        {
            var property = new TestPropertySource<int>(10);
            ChangeInfo<int> latestChange = new ChangeInfo<int>();
            Action<ChangeInfo<int>> observer = change => latestChange = change;

            using (var subs = property.SubscribeToChanges(observer))
            {
                property.SetAndNotify(20);
                Assert.AreEqual(10, latestChange.Old);
                Assert.AreEqual(20, latestChange.New);
                property.SetAndNotify(30);
                Assert.AreEqual(20, latestChange.Old);
                Assert.AreEqual(30, latestChange.New);
            }
        }

        [TestMethod]
        public void SubscribeToChangesNotifiesOnlyIfValueChanges()
        {
            var property = new TestPropertySource<int>(10);
            int calls = 0;
            Action<ChangeInfo<int>> observer = change => calls++;

            using (var subs = property.SubscribeToChanges(observer))
            {
                property.Notify();
                Assert.AreEqual(1, calls);
                property.SetAndNotify(20);
                Assert.AreEqual(2, calls);
                property.Notify();
                Assert.AreEqual(2, calls);
            }
        }

        [TestMethod]
        public void SubscribeToChangesUsesComparer()
        {
            var property = new TestPropertySource<string>("abc");
            int calls = 0;
            ChangeInfo<string> latestChange = new ChangeInfo<string>();

            Action<ChangeInfo<string>> observer = change =>
            {
                latestChange = change;
                calls++;
            };

            using (var subs = property.SubscribeToChanges(observer, StringComparer.OrdinalIgnoreCase))
            {
                property.SetAndNotify("abcd");
                Assert.AreEqual(2, calls);
                Assert.AreEqual("abc", latestChange.Old);
                Assert.AreEqual("abcd", latestChange.New);
                
                property.SetAndNotify("ABCD");
                Assert.AreEqual(2, calls);
                
                property.SetAndNotify("ABCDE");
                Assert.AreEqual(3, calls);
                Assert.AreEqual("abcd", latestChange.Old);
                Assert.AreEqual("ABCDE", latestChange.New); 
            }
        }

        [TestMethod]
        public void MergeSubscribeNotifiesOnceAtSubscription()
        {
            var left = new TestPropertySource<int>(10);
            var right = new TestPropertySource<string>("abc");
            int calls = 0;

            Action<int, string> subscriber = (l, r) => calls++;

            using (var subs = left.MergeSubscribe(right, subscriber))
            {
                Assert.AreEqual(1, calls);
            }

            Assert.AreEqual(1, calls);
        }

        [TestMethod]
        public void MergeSubscribeNotifiesWhenAnyValueChanges()
        {
            var left = new TestPropertySource<int>(10);
            var right = new TestPropertySource<string>("abc");
            int calls = 0;

            Action<int, string> subscriber = (l, r) => calls++;

            using (var subs = left.MergeSubscribe(right, subscriber))
            {
                left.SetAndNotify(20);
                Assert.AreEqual(2, calls);

                right.SetAndNotify("abcd");
                Assert.AreEqual(3, calls);
            }
        }

        [TestMethod]
        public void MergeSubscribeDoesNotNotifyWhenValuesDoNotChange()
        {
            var left = new TestPropertySource<int>(10);
            var right = new TestPropertySource<string>("abc");
            int calls = 0;

            Action<int, string> subscriber = (l, r) => calls++;

            using (var subs = left.MergeSubscribe(right, subscriber))
            {
                left.Notify();
                right.Notify();
                Assert.AreEqual(1, calls);
            }
        }

        [TestMethod]
        public void MergeSubscribeUsesComparer()
        {
            var left = new TestPropertySource<int>(10);
            var right = new TestPropertySource<string>("abc");
            int calls = 0;
            Tuple<int, string> values = null;

            Action<int, string> subscriber = (l, r) =>
            {
                calls++;
                values = Tuple.Create(l, r);
            };

            using (var subs = left.MergeSubscribe(right, subscriber, EqualityComparer<int>.Default, StringComparer.OrdinalIgnoreCase))
            {
                right.Value = "ABC";
                left.Notify();
                right.Notify();
                Assert.AreEqual(1, calls);
                Assert.AreEqual(10, values.Item1);
                Assert.AreEqual("abc", values.Item2);

                left.Value = 20;
                left.Notify();
                Assert.AreEqual(2, calls);
                Assert.AreEqual(20, values.Item1);
                Assert.AreEqual("ABC", values.Item2);

                right.Value = "ABCD";
                right.Notify();
                Assert.AreEqual(3, calls);
                Assert.AreEqual(20, values.Item1);
                Assert.AreEqual("ABCD", values.Item2);
            }
        }

        [TestMethod]
        public void MergeSubscribe3NotifiesOnceAtSubscription()
        {
            var left = new TestPropertySource<int>(10);
            var middle = new TestPropertySource<bool>(true);
            var right = new TestPropertySource<string>("abc");
            int calls = 0;

            Action<int, bool, string> subscriber = (l, m, r) => calls++;

            using (var subs = left.MergeSubscribe(middle, right, subscriber))
            {
                Assert.AreEqual(1, calls);
            }

            Assert.AreEqual(1, calls);
        }

        [TestMethod]
        public void MergeSubscribe3NotifiesWhenAnyValueChanges()
        {
            var left = new TestPropertySource<int>(10);
            var middle = new TestPropertySource<bool>(true);
            var right = new TestPropertySource<string>("abc");
            int calls = 0;

            Action<int, bool, string> subscriber = (l, m, r) => calls++;

            using (var subs = left.MergeSubscribe(middle, right, subscriber))
            {
                left.Value = 20;
                left.Notify();
                Assert.AreEqual(2, calls);

                middle.Value = false;
                right.Notify();
                Assert.AreEqual(3, calls);

                right.Value = "abcd";
                right.Notify();
                Assert.AreEqual(4, calls);
            }
        }

        [TestMethod]
        public void MergeSubscribe3DoesNotNotifyWhenValuesDoNotChange()
        {
            var left = new TestPropertySource<int>(10);
            var middle = new TestPropertySource<bool>(true);
            var right = new TestPropertySource<string>("abc");
            int calls = 0;

            Action<int, bool, string> subscriber = (l, m, r) => calls++;

            using (var subs = left.MergeSubscribe(middle, right, subscriber))
            {
                left.Notify();
                middle.Notify();
                right.Notify();
                Assert.AreEqual(1, calls);
            }
        }

        [TestMethod]
        public void MergeSubscribe3UsesComparer()
        {
            var left = new TestPropertySource<int>(10);
            var middle = new TestPropertySource<bool>(true);
            var right = new TestPropertySource<string>("abc");
            int calls = 0;
            Tuple<int, bool, string> values = null;

            Action<int, bool, string> subscriber = (l, m, r) =>
            {
                calls++;
                values = Tuple.Create(l, m, r);
            };

            using (var subs = left.MergeSubscribe(middle, right, subscriber, EqualityComparer<int>.Default, EqualityComparer<bool>.Default, StringComparer.OrdinalIgnoreCase))
            {
                right.Value = "ABC";
                left.Notify();
                middle.Notify();
                right.Notify();
                Assert.AreEqual(1, calls);
                Assert.AreEqual(10, values.Item1);
                Assert.AreEqual(true, values.Item2);
                Assert.AreEqual("abc", values.Item3);

                left.Value = 20;
                left.Notify();
                Assert.AreEqual(2, calls);
                Assert.AreEqual(20, values.Item1);
                Assert.AreEqual(true, values.Item2);
                Assert.AreEqual("ABC", values.Item3);

                middle.Value = false;
                middle.Notify();
                Assert.AreEqual(3, calls);
                Assert.AreEqual(20, values.Item1);
                Assert.AreEqual(false, values.Item2);
                Assert.AreEqual("ABC", values.Item3);

                right.Value = "ABCD";
                right.Notify();
                Assert.AreEqual(4, calls);
                Assert.AreEqual(20, values.Item1);
                Assert.AreEqual(false, values.Item2);
                Assert.AreEqual("ABCD", values.Item3);

                right.Value = "aBcD";
                left.Notify();
                middle.Notify();
                right.Notify();
                Assert.AreEqual(4, calls);
                Assert.AreEqual(20, values.Item1);
                Assert.AreEqual(false, values.Item2);
                Assert.AreEqual("ABCD", values.Item3);
            }
        }
    }
}

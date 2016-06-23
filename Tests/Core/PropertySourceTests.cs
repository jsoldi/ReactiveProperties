using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveProperties;
using System.Reactive.Disposables;
using System.Windows.Forms;

namespace Tests.Core
{
    [TestClass]
    public class PropertySourceTests
    {
        private static void TestObserversAreCalledOncePerSubscription<T>(Action propertyChangedEvent, IPropertySource<T> propertySource)
        {
            int handlerCount = 0;

            using (var subs1 = propertySource.RawSubscribe(() => handlerCount += 1))
            {
                Assert.AreEqual(0, handlerCount);
                propertyChangedEvent();
                Assert.AreEqual(1, handlerCount);

                using (var subs2 = propertySource.RawSubscribe(() => handlerCount += 10))
                {
                    propertyChangedEvent();
                    Assert.AreEqual(12, handlerCount);
                    propertyChangedEvent();
                    Assert.AreEqual(23, handlerCount);
                }

                propertyChangedEvent();
                Assert.AreEqual(24, handlerCount);
            }
        }

        [TestMethod]
        public void ExplicitPropertySourceHasGivenValue()
        {
            var propertySource = PropertySource.Create(rawObserver => Disposable.Empty, () => 123);
            Assert.AreEqual(123, propertySource.Value);
        }

        [TestMethod]
        public void ExplicitPropertySource_RawSubscribeDoesNotInvokeObserver()
        {
            var propertySource = PropertySource.Create(rawObserver => Disposable.Empty, () => 123);
            Action observer = () => { throw new Exception("This is not supposed to run."); };
            using (var subs = propertySource.RawSubscribe(observer)) { }
        }

        [TestMethod]
        public void ExplicitPropertySource_RawSubscribeInvokesObserverEachTimeIsCalled()
        {
            int observerCount = 0;
            var propertySource = PropertySource.Create(rawObserver => { rawObserver(); rawObserver(); return Disposable.Empty; }, () => 123);
            Action observer = () => { observerCount++; };
            using (var subs = propertySource.RawSubscribe(observer)) { }
            Assert.AreEqual(2, observerCount);
        }

        [TestMethod]
        public void Create_AddsEventHandlersOncePerSubscription()
        {
            Action propertyChangedEvent = null;

            var propertySource = PropertySource.Create(
                () => 123,
                handler => propertyChangedEvent += handler,
                handler => propertyChangedEvent -= handler
            );

            TestObserversAreCalledOncePerSubscription(() => propertyChangedEvent(), propertySource);
            Assert.IsNull(propertyChangedEvent);
        }

        [TestMethod]
        public void Create_CreatesEventAndAddsEventHandlersOncePerSubscription()
        {
            MouseEventHandler mouseLocationChanged = null;

            var propertySource = PropertySource.Create(
                () => 123,
                observer => new MouseEventHandler((s, e) => observer()),
                handler => mouseLocationChanged += handler,
                handler => mouseLocationChanged -= handler
            );

            TestObserversAreCalledOncePerSubscription(() => mouseLocationChanged(null, null), propertySource);
            Assert.IsNull(mouseLocationChanged);
        }

        [TestMethod]
        public void Create_NotifyInvokesObserver()
        {
            Action notify;
            var propertySource = PropertySource.Create(() => 123, out notify);
            TestObserversAreCalledOncePerSubscription(notify, propertySource);
        }
    }
}

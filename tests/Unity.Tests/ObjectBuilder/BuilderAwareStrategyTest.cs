﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.Unity.TestSupport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.ObjectBuilder.Strategies;

namespace Microsoft.Practices.ObjectBuilder2.Tests
{
    [TestClass]
    public class BuilderAwareStrategyTest
    {
        [TestMethod]
        public void BuildCallsClassWithInterface()
        {
            var strategy = new BuilderAwareStrategy();
            var context = new MockBuilderContext();
            var obj = new Aware();

            context.Strategies.Add(strategy);

            context.ExecuteBuildUp(new NamedTypeBuildKey<Aware>(), obj);

            Assert.IsTrue(obj.OnBuiltUpWasCalled);
            Assert.IsFalse(obj.OnTearingDownWasCalled);
            Assert.AreEqual(new NamedTypeBuildKey<Aware>(), obj.OnBuiltUpBuildKey);
        }

        [TestMethod]
        public void BuildChecksConcreteTypeAndNotRequestedType()
        {
            var strategy = new BuilderAwareStrategy();
            var context = new MockBuilderContext();
            var obj = new Aware();

            context.Strategies.Add(strategy);

            context.ExecuteBuildUp(new NamedTypeBuildKey<Aware>(), obj);

            Assert.IsTrue(obj.OnBuiltUpWasCalled);
            Assert.IsFalse(obj.OnTearingDownWasCalled);
        }

        [TestMethod]
        public void BuildIgnoresClassWithoutInterface()
        {
            var strategy = new BuilderAwareStrategy();
            var context = new MockBuilderContext();
            var obj = new Ignorant();

            context.Strategies.Add(strategy);

            context.ExecuteBuildUp(new NamedTypeBuildKey<Ignorant>(), obj);

            Assert.IsFalse(obj.OnBuiltUpWasCalled);
            Assert.IsFalse(obj.OnTearingDownWasCalled);
        }

        private class Aware : Ignorant, IBuilderAware { }

        private class Ignorant
        {
            public bool OnBuiltUpWasCalled;
            public NamedTypeBuildKey OnBuiltUpBuildKey;
            public bool OnTearingDownWasCalled;

            public void OnBuiltUp(NamedTypeBuildKey buildKey)
            {
                OnBuiltUpWasCalled = true;
                OnBuiltUpBuildKey = buildKey;
            }

            public void OnTearingDown()
            {
                OnTearingDownWasCalled = true;
            }
        }
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
using System;
using Microsoft.Practices.ObjectBuilder2;
using Unity.Builder;
using Unity.Container;
using Unity.Events;
using Unity.Extension;
using Unity.Lifetime;
using Unity.ObjectBuilder.Strategies;
using Unity.Policy;
using Unity.Strategy;

namespace Unity.Tests.TestDoubles
{
    public class MyExtensionContext : ExtensionContext
    {
        private UnityContainer container;
        private int i = 0;

        public MyExtensionContext(UnityContainer container)
        {
            this.container = container;
            this.Strategies.Add(new LifetimeStrategy(), UnityBuildStage.Lifetime);
        }

        public override IUnityContainer Container
        {
            get { return this.container; }
        }

        public override IStagedStrategyChain<UnityBuildStage> Strategies
        {
            get { return new StagedStrategyChain<UnityBuildStage>(); }
        }

        public override IStagedStrategyChain<UnityBuildStage> BuildPlanStrategies
        {
            get { return new StagedStrategyChain<UnityBuildStage>(); }
        }

        public override IPolicyList Policies
        {
            get { return new PolicyList(); }
        }

        public override ILifetimeContainer Lifetime
        {
            get { return null; }
        }

        public override event EventHandler<RegisterEventArgs> Registering
        {
            add { this.i++; }
            remove { this.i--; }
        }

        /// <summary>
        /// This event is raised when the <see cref="UnityContainer.RegisterInstance(Type,string,object,LifetimeManager)"/> method,
        /// or one of its overloads, is called.
        /// </summary>
        public override event EventHandler<RegisterInstanceEventArgs> RegisteringInstance
        {
            add { this.i++; }
            remove { this.i--; }
        }

#pragma warning disable 67
        public override event EventHandler<ChildContainerCreatedEventArgs> ChildContainerCreated;
#pragma warning restore 67
    }
}

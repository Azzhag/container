﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Exceptions;
using Unity.Lifetime;
using Unity.Policy;

namespace Unity.ObjectBuilder.Strategies
{
    /// <summary>
    /// An <see cref="IBuilderStrategy"/> implementation that uses
    /// a <see cref="ILifetimePolicy"/> to figure out if an object
    /// has already been created and to update or remove that
    /// object from some backing store.
    /// </summary>
    public class LifetimeStrategy : BuilderStrategy
    {
        private readonly object _genericLifetimeManagerLock = new object();
        private static readonly TransientLifetimeManager TransientManager = new TransientLifetimeManager();

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override void PreBuildUp(IBuilderContext context)
        {
            if ((context ?? throw new ArgumentNullException(nameof(context))).Existing == null)
            {
                var lifetimePolicy = GetLifetimePolicy(context);
                if (lifetimePolicy is IRequiresRecovery recovery)
                {
                    context.RecoveryStack.Add(recovery);
                }

                var existing = lifetimePolicy.GetValue();
                if (existing != null)
                {
                    context.Existing = existing;
                    context.BuildComplete = true;
                }
            }
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override void PostBuildUp(IBuilderContext context)
        {
            // If we got to this method, then we know the lifetime policy didn't
            // find the object. So we go ahead and store it.
            ILifetimePolicy lifetimePolicy = GetLifetimePolicy(context ?? throw new ArgumentNullException(nameof(context)));
            lifetimePolicy.SetValue(context.Existing);
        }

        private ILifetimePolicy GetLifetimePolicy(IBuilderContext context)
        {
            ILifetimePolicy policy = context.Policies.GetNoDefault<ILifetimePolicy>(context.BuildKey, false);
            if (policy == null && context.BuildKey.Type.GetTypeInfo().IsGenericType)
            {
                policy = GetLifetimePolicyForGenericType(context);
            }

            if (policy == null)
            {
                policy = TransientManager;
                context.PersistentPolicies.Set(policy, context.BuildKey);
            }

            return policy;
        }

        private ILifetimePolicy GetLifetimePolicyForGenericType(IBuilderContext context)
        {
            var typeToBuild = context.BuildKey.Type;
            object openGenericBuildKey = new NamedTypeBuildKey(typeToBuild.GetGenericTypeDefinition(),
                                                               context.BuildKey.Name);

            var factoryPolicy = context.Policies
                                       .Get<ILifetimeFactoryPolicy>(openGenericBuildKey, out var factorySource);

            if (factoryPolicy != null)
            {
                // creating the lifetime policy can result in arbitrary code execution
                // in particular it will likely result in a Resolve call, which could result in locking
                // to avoid deadlocks the new lifetime policy is created outside the lock
                // multiple instances might be created, but only one instance will be used
                ILifetimePolicy newLifetime = factoryPolicy.CreateLifetimePolicy();

                lock (_genericLifetimeManagerLock)
                {
                    // check whether the policy for closed-generic has been added since first checked
                    var lifetime = factorySource.GetNoDefault<ILifetimePolicy>(context.BuildKey, false);
                    if (lifetime == null)
                    {
                        factorySource.Set(newLifetime, context.BuildKey);
                        lifetime = newLifetime;
                    }

                    return lifetime;
                }
            }

            return null;
        }
    }
}

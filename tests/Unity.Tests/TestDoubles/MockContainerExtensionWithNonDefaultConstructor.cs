﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Unity;
using Unity.Extension;

namespace Microsoft.Practices.Unity.Tests.TestDoubles
{
    public class ContainerExtensionWithNonDefaultConstructor : UnityContainerExtension
    {
        public ContainerExtensionWithNonDefaultConstructor(IUnityContainer container)
        {
        }

        protected override void Initialize()
        {
        }
    }
}

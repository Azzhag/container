﻿// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Builder;
using Unity.Policy;

namespace Unity.ObjectBuilder.Policies
{
    internal class OverriddenBuildPlanMarkerPolicy : IBuildPlanPolicy
    {
        /// <summary>
        /// Creates an instance of this build plan's type, or fills
        /// in the existing type if passed in.
        /// </summary>
        /// <param name="context">Context used to build up the object.</param>
        public void BuildUp(IBuilderContext context)
        {
            throw new InvalidOperationException(Constants.MarkerBuildPlanInvoked);
        }
    }
}

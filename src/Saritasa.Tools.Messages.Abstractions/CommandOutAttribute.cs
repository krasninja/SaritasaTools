﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

namespace Saritasa.Tools.Messages.Abstractions
{
    using System;

    /// <summary>
    /// Marks property as output. Is not for processing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CommandOutAttribute : Attribute
    {
    }
}
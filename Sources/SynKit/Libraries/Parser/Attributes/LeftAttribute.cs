﻿// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.SynKit.Parser.Attributes;

/// <summary>
/// An attribute to annotate a left-associative operator.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class LeftAttribute : Attribute
{
    /// <summary>
    /// The separators that should be left-associative.
    /// </summary>
    public object[] Separators { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LeftAttribute"/> class.
    /// </summary>
    /// <param name="separators">The separator elements that should be left-associative.</param>
    public LeftAttribute(params object[] separators)
    {
        this.Separators = separators;
    }
}

// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.SynKit.Lexer.Attributes;

/// <summary>
/// An attribute to mark a class to generate a lexer for token types.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class LexerAttribute : Attribute
{
    /// <summary>
    /// The enumeration type that is annotated with token attributes.
    /// The lexer will be generated based on the annotations on the enum fields.
    /// </summary>
    public Type TokenType { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LexerAttribute"/> class.
    /// </summary>
    /// <param name="tokenType">The token type to generate the lexer for.</param>
    public LexerAttribute(Type tokenType)
    {
        this.TokenType = tokenType;
    }
}

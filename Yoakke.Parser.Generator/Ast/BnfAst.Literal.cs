﻿using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Parser.Generator.Ast
{
    partial class BnfAst
    {
        /// <summary>
        /// A literal token match, either by text or by token kind.
        /// </summary>
        public class Literal : BnfAst
        {
            /// <summary>
            /// The value to match.
            /// </summary>
            public readonly object Value;

            public Literal(object value)
            {
                Value = value;
            }

            public override bool Equals(BnfAst other) => other is Literal lit
                && Value.Equals(lit.Value);
            public override int GetHashCode() => Value.GetHashCode();

            public override BnfAst Desugar() => this;

            public override string GetParsedType(RuleSet ruleSet, TokenKindSet tokens)
            {
                if (tokens.EnumType == null) return TypeNames.IToken;
                else return $"{TypeNames.Token}<{tokens.EnumType.ToDisplayString()}>";
            }
        }
    }
}

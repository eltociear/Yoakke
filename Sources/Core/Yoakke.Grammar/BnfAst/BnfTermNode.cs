// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Grammar.BnfAst
{
    /// <summary>
    /// A terminal node.
    /// </summary>
    public record BnfTermNode(object Value) : BnfNodeBase
    {
        /// <inheritdoc/>
        public override bool IsLeaf => true;

        /// <inheritdoc/>
        public override IEnumerable<KeyValuePair<int, IBnfNode>> TraverseLeaves(bool reverse, int offset)
        {
            yield return new(offset, this);
        }

        /// <inheritdoc/>
        protected override IBnfNode ReplaceChildrenByReference(IBnfNode find, IBnfNode replace) => this;
    }
}
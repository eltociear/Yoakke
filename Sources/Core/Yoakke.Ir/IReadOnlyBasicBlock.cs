// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Ir
{
    /// <summary>
    /// A basic block that can only be read.
    /// A basic block is a sequence of instructions that only contains branching at the end.
    /// This means that the code can only jump at the start of the block and only the last instruction
    /// can jump elsewhere.
    /// </summary>
    public interface IReadOnlyBasicBlock
    {
        /// <summary>
        /// The <see cref="IReadOnlyProcedure"/> this <see cref="IReadOnlyBasicBlock"/> belongs to.
        /// </summary>
        public IReadOnlyProcedure Procedure { get; }

        /// <summary>
        /// The optional name hint for this <see cref="IReadOnlyBasicBlock"/>.
        /// This can be useful to produce nicer, more readable IR code.
        /// </summary>
        public string? NameHint { get; }

        /// <summary>
        /// The list of <see cref="IInstruction"/>s in this <see cref="IReadOnlyBasicBlock"/>.
        /// </summary>
        public IReadOnlyList<IInstruction> Instructions { get; }
    }
}

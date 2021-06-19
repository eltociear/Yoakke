﻿// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Lsp.Model.LanguageFeatures
{
    /// <summary>
    /// Completion item tags are extra annotations that tweak the rendering of a
    /// completion item.
    /// </summary>
    [Since(3, 15, 0)]
    public enum CompletionItemTag
    {
        /// <summary>
        /// Render a completion as obsolete, usually using a strike-out.
        /// </summary>
        Deprecated = 1,
    }
}
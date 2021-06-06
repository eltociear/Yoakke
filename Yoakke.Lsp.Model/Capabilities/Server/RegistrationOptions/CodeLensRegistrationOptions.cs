﻿using Newtonsoft.Json;
using System.Collections.Generic;
using Yoakke.Lsp.Model.Basic;

namespace Yoakke.Lsp.Model.Capabilities.Server.RegistrationOptions
{
    public class CodeLensRegistrationOptions : CodeLensOptions, ITextDocumentRegistrationOptions
    {
        [JsonProperty("documentSelector")]
        public IReadOnlyList<DocumentFilter>? DocumentSelector { get; set; }
    }
}
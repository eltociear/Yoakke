// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Grammar.Lr
{
    /// <summary>
    /// A helper for printing the LR tables.
    /// </summary>
    internal static class LrTablePrinter
    {
        /// <summary>
        /// Implements <see cref="ILrParsingTable{TItem}.ToHtmlTable"/>.
        /// </summary>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <param name="table">The table to convert.</param>
        /// <returns>The HTML table representation of the table.</returns>
        public static string ToHtmlTable<TItem>(ILrParsingTable<TItem> table)
            where TItem : ILrItem
        {
            const string border = "border: 1px solid black";
            const string doubleRight = "border-right: 3px black double";
            const string doubleDown = "border-bottom: 3px black double";
            const string center = "text-align: center";

            var result = new StringBuilder();

            // Header with Action and Goto
            result
                .AppendLine("<table style=\"width: 100%; border-collapse: collapse\">")
                .AppendLine("  <tr>")
                .AppendLine("    <th></th>")
                .AppendLine($"    <th colspan=\"{table.Grammar.Terminals.Count}\">Action</th>")
                .AppendLine($"    <th colspan=\"{table.Grammar.Nonterminals.Count}\">Goto</th>")
                .AppendLine("  </tr>");

            // Header with state, terminals and nonterminals
            result.AppendLine("  <tr>");
            // First the state
            result.AppendLine($"    <td style=\"{border}; {doubleRight}; {doubleDown}; {center}\">State</td>");
            // Next the terminals
            var i = 0;
            foreach (var term in table.Grammar.Terminals)
            {
                ++i;
                var isLast = i == table.Grammar.Terminals.Count;
                var append = isLast ? $"; {doubleRight}" : string.Empty;
                result.AppendLine($"    <td style=\"{border}; {doubleDown}; {center}{append}\">{term}</td>");
            }
            // Finally the nonterminals
            foreach (var nonterm in table.Grammar.Nonterminals)
            {
                result.AppendLine($"    <td style=\"{border}; {doubleDown}; {center}\">{nonterm}</td>");
            }
            result.AppendLine("  </tr>");

            // Now we can actually print the contents state by state
            for (var state = 0; state < table.StateCount; ++state)
            {
                result.AppendLine("  <tr>");
                // First we print the state
                result.AppendLine($"    <td style=\"{border}; {doubleRight}; {center}\">{state}</td>");
                // We print all actions with terminals
                i = 0;
                foreach (var term in table.Grammar.Terminals)
                {
                    ++i;
                    var isLast = i == table.Grammar.Terminals.Count;
                    var append = isLast ? $"; {doubleRight}" : string.Empty;
                    var actions = table.Action[state, term];
                    result.AppendLine($"    <td style=\"{border}{append}\">{string.Join("<br>", actions)}</td>");
                }
                // We print all gotos for nonterminals
                foreach (var nonterm in table.Grammar.Nonterminals)
                {
                    var to = table.Goto[state, nonterm];
                    result.AppendLine($"    <td style=\"{border}\">{to}</td>");
                }
                result.AppendLine("  </tr>");
            }

            // Close table
            result.Append("</table>");

            result.Replace(" -> ", " → ");

            return result.ToString();
        }

        /// <summary>
        /// Implements <see cref="ILrParsingTable{TItem}.ToDotDfa"/>.
        /// </summary>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <param name="table">The table to convert.</param>
        /// <returns>The DOT DFA representation of the table.</returns>
        public static string ToDotDfa<TItem>(ILrParsingTable<TItem> table)
            where TItem : ILrItem
        {
            throw new NotImplementedException();
        }
    }
}

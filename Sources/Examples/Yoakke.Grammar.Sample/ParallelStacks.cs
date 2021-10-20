// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections.Graphs;
using Yoakke.Grammar.Cfg;
using Yoakke.Grammar.Lr;
using Yoakke.Grammar.ParseTree;

namespace Yoakke.Grammar.Sample
{
    public class ParallelStacks : INondetStack
    {
        public ILrParsingTable ParsingTable { get; }

        public int? CurrentState => this.heads.Count == 1 ? this.heads.First().State : null;

        public IEnumerable<IIncrementalTreeNode> Trees => this.heads
            .Where(h => h.PrevMap.Count > 0)
            .SelectMany(h => h.PrevMap.Values.Select(v => v.ParseTree));

        public int ShiftCount { get; private set; }

        public int ReduceCount { get; private set; }

        public int EdgeCount { get; private set; }

        public int VertexCount { get; private set; } = 1;

        // The heads
        private readonly List<StateVertex> heads = new();

        // Action tracking
        private readonly Stack<(StateVertex Vertex, Reduce Reduce)> remainingReduces = new();
        private readonly Stack<(StateVertex Vertex, Shift Shift)> remainingShifts = new();

        // Current terminal
        private IIncrementalTreeNode? currentNode;

        public ParallelStacks(ILrParsingTable table)
        {
            this.ParsingTable = table;
            // Initial state
            this.heads.Add(new());
        }

        public string ToDot()
        {
            var result = new StringBuilder();
            result.AppendLine("graph Parallel_Stacks {");
            result.AppendLine("  rankdir=RL;");

            // We number each vertex
            var vertexNames = new Dictionary<Vertex, int>();
            foreach (var v in this.heads.Cast<Vertex>().SelectMany(n => BreadthFirst.Search(n, v => v.Prev)))
            {
                if (vertexNames.ContainsKey(v)) continue;
                vertexNames.Add(v, vertexNames.Count);
            }

            // Define all nodes
            foreach (var vertex in vertexNames.Keys)
            {
                result.Append($"  {vertexNames[vertex]}[");
                if (vertex is StateVertex state)
                {
                    var reductionsOnHead = this.remainingReduces
                        .Where(r => ReferenceEquals(r.Vertex, state))
                        .Select(r => r.Reduce);
                    var shiftsOnHead = this.remainingShifts
                        .Where(s => ReferenceEquals(s.Vertex, state))
                        .Select(s => s.Shift);
                    var opsOnHead = reductionsOnHead.Cast<Lr.Action>().Concat(shiftsOnHead);
                    result.Append($"label=\"{state.State}\", shape=circle, xlabel=\"{string.Join(@"\l", opsOnHead)}\"");
                }
                else
                {
                    var symbol = (SymbolVertex)vertex;
                    result.Append($"label=\"{symbol.Symbol}\", shape=square");
                }
                result.AppendLine("]");
            }

            // Connections
            foreach (var v1 in vertexNames.Keys)
            {
                foreach (var v2 in v1.Prev) result.AppendLine($"  {vertexNames[v1]} -- {vertexNames[v2]}");
            }

            result.Append("}");

            result.Replace("->", "→");

            return result.ToString();
        }

        public void Feed(IIncrementalTreeNode currentNode)
        {
            // If there are remaining actions to perform, feeding another terminal is illegal
            if (this.remainingReduces.Count > 0 || this.remainingShifts.Count > 0) throw new InvalidOperationException("Not all actions are performed yet");

            // We store the terminal
            this.currentNode = currentNode;

            // We push each action for each head
            foreach (var head in this.heads) this.PushActions(head);
        }

        public bool Step()
        {
            // We always reduce everywhere first
            if (this.remainingReduces.TryPop(out var r))
            {
                this.Reduce(r.Vertex, r.Reduce);
                return true;
            }
            // We do the shifts all at once
            var result = this.remainingShifts.Count > 0;
            while (this.remainingShifts.TryPop(out var s)) this.Shift(s.Vertex, s.Shift);
            // If there are no more shifts, we are done
            return result;
        }

        private void Reduce(StateVertex vertex, Reduce reduce)
        {
            ++this.ReduceCount;
            this.heads.Remove(vertex);
            // Now we need to pop off |b| amount of symbol vertices for an X -> b reduction
            var reducedSubtrees = new List<IIncrementalTreeNode>();
            var newRoot = vertex;
            for (var i = 0; i < reduce.Production.Right.Count; ++i)
            {
                var (r, s) = Pop(newRoot);
                newRoot = r;
                reducedSubtrees.Add(s.ParseTree);
            }
            reducedSubtrees.Reverse();
            // We have the new root, act on it
            // Check what state we result in
            var stateGoto = this.ParsingTable.Goto[newRoot.State, reduce.Production.Left];
            // If nothing, we terminate this branch
            if (stateGoto is null) return;
            // Otherwise we push on the symbol and the state
            var tree = new ProductionIncrementalTreeNode(reduce.Production, newRoot.State, reducedSubtrees)
            {
                IsReusable = this.heads.Count == 1,
            };
            var pushedVertex = Push(newRoot, tree, stateGoto.Value);
            // We add it as a head
            this.heads.Add(pushedVertex);
            // Now we add all actions that can be performed on this new state for the current terminal for further processing
            this.PushActions(pushedVertex);
        }

        private void Shift(StateVertex vertex, Shift shift)
        {
            ++this.ShiftCount;
            Debug.Assert(this.currentNode is not null, "The current node cannot be null.");
            // The vertex is surely out of the heads now
            this.heads.Remove(vertex);
            // Now we try to push on the symbol and next state
            var newHead = this.Push(vertex, this.currentNode, shift.State);
            this.heads.Add(newHead);
        }

        private static (StateVertex State, SymbolVertex Symbol) Pop(StateVertex vertex)
        {
            Debug.Assert(vertex.PrevMap.Count == 1, "Parallel stacks can only have one back-edges per vertex.");
            var prevSymbol = vertex.PrevMap.Values.First();
            Debug.Assert(prevSymbol.Prev.Count == 1, "Parallel stacks can only have one back-edges per vertex.");
            return (prevSymbol.Prev.First(), prevSymbol);
        }

        private StateVertex Push(StateVertex vertex, IIncrementalTreeNode node, int state)
        {
            this.VertexCount += 2;
            this.EdgeCount += 2;
            return new(new(vertex, node), state);
        }

        private void PushActions(StateVertex vertex)
        {
            Debug.Assert(this.currentNode is not null, "The current node cannot be null.");
            if (this.currentNode is LeafIncrementalTreeNode leaf)
            {
                var actions = this.ParsingTable.Action[vertex.State, leaf.Terminal];
                var i = 0;
                foreach (var action in actions)
                {
                    var head = vertex;
                    // If this is a nondeterministic step, clone the stack and act on that
                    if (i > 0) head = this.CloneStack(vertex);
                    // Act on the chosen head
                    if (action is Shift s) this.remainingShifts.Push((head, s));
                    else if (action is Reduce r) this.remainingReduces.Push((head, r));
                    // NOTE: else it's an accept
                    ++i;
                }
            }
            else
            {
                var prod = (ProductionIncrementalTreeNode)this.currentNode;
                var stateGoto = this.ParsingTable.Goto[vertex.State, prod.Production.Left];
                if (stateGoto is not null) this.remainingShifts.Push((vertex, new(stateGoto.Value)));
            }
        }

        private StateVertex CloneStack(StateVertex vertex)
        {
            // Just for counting
            Vertex? it = vertex;
            while (it is not null)
            {
                ++this.VertexCount;
                if (it.Prev.Any())
                {
                    ++this.EdgeCount;
                    it = it.Prev.First();
                }
                else
                {
                    it = null;
                }
            }
            // Actual important things
            var newHead = vertex.Clone();
            this.heads.Add(newHead);
            return newHead;
        }
    }
}

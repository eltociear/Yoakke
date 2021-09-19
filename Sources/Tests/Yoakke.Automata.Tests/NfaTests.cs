// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Linq;
using Xunit;
using Yoakke.Automata.Discrete;

namespace Yoakke.Automata.Tests
{
    public class NfaTests : AutomatonTestBase
    {
        [Theory]
        [InlineData(new string[] { }, false)]
        [InlineData(new string[] { "0 -> A" }, false)]
        [InlineData(new string[] { "1 -> A, B" }, false)]
        [InlineData(new string[] { "0 -> A", "1 -> A, B" }, false)]
        [InlineData(new string[] { "1 -> A, B", "0 -> A, C" }, false)]
        [InlineData(new string[] { "1 -> A, B", "1 -> A, B" }, false)]
        [InlineData(new string[] { "1 -> A, B", "0 -> A, C", "0 -> A" }, false)]
        [InlineData(new string[] { "1 -> A, B", "0 -> A, C", "1 -> A, B, D" }, true)]
        [InlineData(new string[] { "1 -> A, B", "0 -> A, C", "1 -> A, B, D", "1 -> A, B, D" }, true)]
        [InlineData(new string[] { "1 -> A, B", "0 -> A, C", "1 -> A, B, D", "0 -> A, C, D" }, true)]
        [InlineData(new string[] { "1 -> A, B", "0 -> A, C", "1 -> A, B, D", "0 -> A, C, D", "0 -> A, D" }, true)]
        [InlineData(new string[] { "1 -> A, B", "0 -> A, C", "1 -> A, B, D", "0 -> A, C, D", "1 -> A, B, D" }, true)]
        [InlineData(new string[] { "1 -> A, B", "0 -> A, C", "1 -> A, B, D", "0 -> A, C, D", "0 -> A, D", "0 -> A, D" }, true)]
        [InlineData(new string[] { "1 -> A, B", "0 -> A, C", "1 -> A, B, D", "0 -> A, C, D", "0 -> A, D", "1 -> A, B, D" }, true)]
        public void Has101AcceptsTests(string[] transitionTexts, bool accepts)
        {
            var nfa = BuildHas101Nfa();

            var transitions = transitionTexts.Select(ParseTransition).ToList();

            var state = new StateSet<string>(new[] { nfa.InitialState });
            foreach (var (inputChar, expectedNextText) in transitions)
            {
                var expectedNext = ParseStateSet(expectedNextText).ToHashSet();
                var nextState = nfa.GetTransitions(state, inputChar);
                Assert.True(expectedNext.SetEquals(nextState));
                state = nextState;
            }

            var input = transitions.Select(t => t.Item1);
            Assert.Equal(accepts, nfa.Accepts(input));
        }

        [Fact]
        public void Has101Determinization()
        {
            var dfa = BuildHas101Nfa().Determinize();

            var expectedStates = new[] { "A", "A, B", "A, C", "A, B, D", "A, D", "A, C, D" }.Select(ParseStateSet);
            var gotStates = dfa.States.ToHashSet();

            var expectedAcceptingStates = new[] { "A, B, D", "A, D", "A, C, D" }.Select(ParseStateSet);
            var gotAcceptingStates = dfa.AcceptingStates.ToHashSet();

            Assert.True(gotStates.SetEquals(expectedStates));
            Assert.True(gotAcceptingStates.SetEquals(expectedAcceptingStates));

            AssertTransition(dfa, "A", '0', "A");
        }

        private static Nfa<string, char> BuildHas101Nfa()
        {
            var nfa = new Nfa<string, char>();
            nfa.InitialState = "A";
            nfa.AcceptingStates.Add("D");
            nfa.AddTransition("A", '0', "A");
            nfa.AddTransition("A", '1', "A");
            nfa.AddTransition("A", '1', "B");
            nfa.AddTransition("B", '0', "C");
            nfa.AddTransition("C", '1', "D");
            nfa.AddTransition("D", '0', "D");
            nfa.AddTransition("D", '1', "D");
            return nfa;
        }
    }
}

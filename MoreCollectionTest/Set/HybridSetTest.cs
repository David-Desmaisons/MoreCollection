﻿using MoreCollection.Set;
using MoreCollection.Set.Infra;
using MoreCollection.Extensions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;

namespace MoreCollectionTest.Set
{
    [Collection("Changing Default static set stategy")]
    public class HybridSetTest : IDisposable
    {
        private readonly ILetterSimpleSetFactory _LetterSimpleSetFactory;
        private readonly ILetterSimpleSetFactory _Factory;
        private readonly ILetterSimpleSet<string> _LetterSimpleSetSubstitute;
        private readonly IEnumerable<string> _Enumerable;
        private List<string> _GetDefaultParameter = null;     

        public HybridSetTest()
        {
            _Enumerable = Substitute.For<IEnumerable<string>>();
            _LetterSimpleSetSubstitute = Substitute.For<ILetterSimpleSet<string>>();
            _LetterSimpleSetFactory = Substitute.For<ILetterSimpleSetFactory>();
            _LetterSimpleSetFactory.GetDefault<string>().Returns(_LetterSimpleSetSubstitute);
            _LetterSimpleSetFactory.GetDefault<string>(Arg.Any<string>()).Returns(_LetterSimpleSetSubstitute);
            _LetterSimpleSetFactory.GetDefault<string>(Arg.Any<IEnumerable<string>>()).Returns(arg =>
            {
                _GetDefaultParameter = ((IEnumerable<string>)arg[0]).ToList();
                return _LetterSimpleSetSubstitute;
            });

            bool res;
            _LetterSimpleSetSubstitute.Add(Arg.Any<string>(), out res).Returns(_LetterSimpleSetSubstitute);

            _Factory = LetterSimpleSetFactoryBuilder.Factory;
            LetterSimpleSetFactoryBuilder.Factory = _LetterSimpleSetFactory;
        }

        public void Dispose()
        {
            LetterSimpleSetFactoryBuilder.Factory = _Factory;
        }

        private void SetUp(ISet<string> FakeCollection)
        {
            _LetterSimpleSetSubstitute.GetEnumerator().Returns(FakeCollection.GetEnumerator());
            _LetterSimpleSetSubstitute.Contains(Arg.Any<string>())
                                .Returns(argument => FakeCollection.Contains(argument[0]));
            _LetterSimpleSetSubstitute.Count.Returns(FakeCollection.Count);
        }

        [Fact]
        public void Constructor_WithoutParameters_Call_LetterSimpleSetFactory_GetDefault_WithoutParameters()
        {
            var PolyMorphSet = new HybridSet<string>();
            _LetterSimpleSetFactory.Received(1).GetDefault<string>();
        }

        [Fact]
        public void Constructor_WithoneElement_Call_LetterSimpleSetFactory_GetDefault_WithoutoneElement()
        {
            var PolyMorphSet = new HybridSet<string>("abcd");
            _LetterSimpleSetFactory.Received(1).GetDefault("abcd");
        }

        [Fact]
        public void Constructor_WithEnumerable_Call_LetterSimpleSetFactory_GetDefault_WithoutEnumerable()
        {
            var PolyMorphSet = new HybridSet<string>(_Enumerable);
            _LetterSimpleSetFactory.Received(1).GetDefault(_Enumerable);
        }

        [Fact]
        public void Add_Call_LetterSimpleSet_Add()
        {
            bool res;
            var PolyMorphSet = new HybridSet<string>();
            PolyMorphSet.Add("key");
            _LetterSimpleSetSubstitute.Received(1).Add("key", out res);
        }

        [Fact]
        public void Add_ICollection_Call_LetterSimpleSet_Add()
        {
            bool res;
            ICollection<string> PolyMorphSet = new HybridSet<string>();
            PolyMorphSet.Add("key");
            _LetterSimpleSetSubstitute.Received(1).Add("key", out res);
        }

        [Fact]
        public void Remove_Call_LetterSimpleSet_Remove()
        {
            bool res;
            var PolyMorphSet = new HybridSet<string>();
            PolyMorphSet.Remove("key");
            _LetterSimpleSetSubstitute.Received(1).Remove("key", out res);
        }

        [Fact]
        public void Contains_Call_LetterSimpleSet_Contains()
        {
            var PolyMorphSet = new HybridSet<string>();
            PolyMorphSet.Contains("key");
            _LetterSimpleSetSubstitute.Received(1).Contains("key");
        }

        [Fact]
        public void Count_Call_LetterSimpleSet_Count()
        {
            var PolyMorphSet = new HybridSet<string>();
            var count = PolyMorphSet.Count;
            var res = _LetterSimpleSetSubstitute.Received(1).Count;
        }

        [Fact]
        public void Clear_Call_LetterSimpleSetFactory_GetDefault()
        {
            var PolyMorphSet = new HybridSet<string>();
            _LetterSimpleSetFactory.ClearReceivedCalls();
            PolyMorphSet.Clear();
            _LetterSimpleSetFactory.Received(1).GetDefault<string>();
        }


        [Fact]
        public void GetEnumerator_Call_LetterSimpleSet_GetEnumerator()
        {
            var PolyMorphSet = new HybridSet<string>();
            PolyMorphSet.GetEnumerator();
            _LetterSimpleSetSubstitute.Received(1).GetEnumerator();
        }

        [Fact]
        public void Collections_GetEnumerator_Call_LetterSimpleSet_GetEnumerator()
        {
            System.Collections.IEnumerable PolyMorphSet = new HybridSet<string>();
            PolyMorphSet.GetEnumerator();
            _LetterSimpleSetSubstitute.Received(1).GetEnumerator();
        }

        [Fact]
        public void IsReadOnly_IsFalse()
        {
            var PolyMorphSet = new HybridSet<string>();
            PolyMorphSet.IsReadOnly.Should().BeFalse();
        }

        public static IEnumerable<IEnumerable<string>> RawData
        {
            get
            {
                yield return new[] { "a", "b" };
                yield return new[] { "a", "b", "c" };
                yield return new[] { "a", "b", "c", "d" };
                yield return new[] { "a", "b", "c", "a" };
            }
        }

        public static IEnumerable<object[]> Data => RawData.Select(collection => new[] { collection });

        public static IEnumerable<object[]> DataSquare => RawData.Cartesian(RawData, (rw1, rw2) => new[] { rw1, rw2 });

        [Theory, MemberData(nameof(Data))]
        public void UnionWith_Add_ElementsToCollection(string[] strings)
        {
            var PolyMorphSet = new HybridSet<string>();
            PolyMorphSet.UnionWith(strings);
            bool res;

            Received.InOrder(() =>
               strings.ForEach(s => _LetterSimpleSetSubstitute.Add(s, out res))
                );
        }

        private Tuple<HybridSet<string>, ISet<string>> SetUpAndBuild(string[] strings1)
        {
            ISet<string> set1 = new HashSet<string>(strings1);
            SetUp(set1);

            return new Tuple<HybridSet<string>, ISet<string>>(new HybridSet<string>(), set1);
        }

        private void ISetMethodTest(string[] strings1, string[] strings2, Action<ISet<string>, IEnumerable<string>> Build)
        {
            var SetUp = SetUpAndBuild(strings1);
            Build(SetUp.Item1, strings2);
            Build(SetUp.Item2, strings2);

            _LetterSimpleSetFactory.Received(1).GetDefault(Arg.Any<IEnumerable<string>>());

            var defaulthash = new HashSet<string>(_GetDefaultParameter);
            defaulthash.Should().BeEquivalentTo(SetUp.Item2);
        }

        private void ISetMethodTest(string[] strings1, string[] strings2, Func<ISet<string>, IEnumerable<string>, bool> Build)
        {
            var SetUp = SetUpAndBuild(strings1);
            var res = Build(SetUp.Item1, strings2);
            var res2 = Build(SetUp.Item2, strings2);

            res.Should().Be(res2);
        }

        [Theory, MemberData(nameof(DataSquare))]
        public void IntersectWith_ConstructANewLetter_withCorrectElements(string[] strings1, string[] strings2)
        {
            ISetMethodTest(strings1, strings2, (set, enumerable) => set.IntersectWith(enumerable));
        }

        [Theory, MemberData(nameof(DataSquare))]
        public void ExceptWith_ConstructANewLetter_withCorrectElements(string[] strings1, string[] strings2)
        {
            ISetMethodTest(strings1, strings2, (set, enumerable) => set.ExceptWith(enumerable));
        }

        [Theory, MemberData(nameof(DataSquare))]
        public void SymmetricExceptWith_ConstructANewLetter_withCorrectElements(string[] strings1, string[] strings2)
        {
            ISetMethodTest(strings1, strings2, (set, enumerable) => set.SymmetricExceptWith(enumerable));
        }

        [Theory, MemberData(nameof(DataSquare))]
        public void IsProperSubsetOf_ConstructANewLetter_withCorrectElements(string[] strings1, string[] strings2)
        {
            ISetMethodTest(strings1, strings2, (set, enumerable) => set.IsProperSubsetOf(enumerable));
        }

        [Theory, MemberData(nameof(DataSquare))]
        public void IsProperSupersetOf_ConstructANewLetter_withCorrectElements(string[] strings1, string[] strings2)
        {
            ISetMethodTest(strings1, strings2, (set, enumerable) => set.IsProperSupersetOf(enumerable));
        }

        [Theory, MemberData(nameof(DataSquare))]
        public void IsSubsetOf_ConstructANewLetter_withCorrectElements(string[] strings1, string[] strings2)
        {
            ISetMethodTest(strings1, strings2, (set, enumerable) => set.IsSubsetOf(enumerable));
        }

        [Theory, MemberData(nameof(DataSquare))]
        public void IsSupersetOf_ConstructANewLetter_withCorrectElements(string[] strings1, string[] strings2)
        {
            ISetMethodTest(strings1, strings2, (set, enumerable) => set.IsSupersetOf(enumerable));
        }

        [Theory, MemberData(nameof(DataSquare))]
        public void Overlaps_ConstructANewLetter_withCorrectElements(string[] strings1, string[] strings2)
        {
            ISetMethodTest(strings1, strings2, (set, enumerable) => set.Overlaps(enumerable));
        }

        [Theory, MemberData(nameof(DataSquare))]
        public void SetEquals_ConstructANewLetter_withCorrectElements(string[] strings1, string[] strings2)
        {
            ISetMethodTest(strings1, strings2, (set, enumerable) => set.SetEquals(enumerable));
        }

        [Theory, MemberData(nameof(Data))]
        public void CopyTo(string[] stringArray)
        {
            var SetUp = SetUpAndBuild(stringArray);
            var strings = new string[SetUp.Item2.Count];
            SetUp.Item1.CopyTo(strings, 0);
            strings.Should().BeEquivalentTo(SetUp.Item2);
        }
    }
}

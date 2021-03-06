﻿using System;
using System.Collections.Generic;
using System.Linq;
using MoreCollection.Extensions;
using Xunit;
using FluentAssertions;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MoreCollectionTest.Extensions
{
    public class ListExtensionTest
    {
        private IList<int> List => _List;
        private readonly IList<int> _List;
        private readonly IList<int> _FullList;
        private readonly IList<int> _FullListObservable;
        private readonly ObservableCollection<int> _RawFullListObservable;
        private readonly IList<int> _NullList=null;
        public ListExtensionTest()
        {
            _List = new List<int>();
            _FullList = new List<int>( new[]{0, 1, 2});
            _RawFullListObservable = new ObservableCollection<int>(new[] { 0, 1, 2 });
            _FullListObservable = _RawFullListObservable;
        }

        [Theory, MemberData(nameof(Data))]
        public void AddRange_AppendsElement_On_Empty_List(IEnumerable<int> enumerable)
        {
            var expected = new List<int>(enumerable ?? Enumerable.Empty<int>());
            List.AddRange(enumerable);
            List.Should().BeEquivalentTo(expected);
        }

        [Theory, MemberData(nameof(Data))]
        public void AddRange_AppendsElement(IEnumerable<int> enumerable)
        {
            var initialValues = new[] {100, 20};
            _List.AddRange(initialValues);
            var expected = initialValues.Concat(enumerable ?? Enumerable.Empty<int>());
            List.AddRange(enumerable);
            List.Should().BeEquivalentTo(expected);
        }

        [Theory, MemberData(nameof(Data))]
        public void AddRange_ReturnsCallingList(IEnumerable<int> enumerable)
        {
            var res = List.AddRange(enumerable);
            res.Should().BeSameAs(List);
        }

        public static IEnumerable<object[]> Data
        {
            get
            {
                IEnumerable<int> nullCollection = null;
                yield return new object[] { Enumerable.Empty<int>() };
                yield return new object[] { new List<int>() { 0, 5, 10 } };
                yield return new object[] { nullCollection };
            }
        }


        [Fact]
        public void AddRange_CalledOnNull_ThrowException()
        {
            Action Do = () => _NullList.AddRange(Enumerable.Empty<int>());
            Do.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Move_CalledOnNull_ThrowException()
        {
            Action Do = () => _NullList.Move(0, 27);
            Do.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Move_CalledWithWrongIndex_ThrowArgumentException()
        {
            Action Do = () => _FullList.Move(-1, 1);
            Do.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Move_CalledWithWrongIndex_2_ThrowArgumentException()
        {
            Action Do = () => _FullList.Move(10, 1);
            Do.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Move_CalledWithWrongIndex_3_ThrowArgumentException()
        {
            Action Do = () => _FullList.Move(0, -1);
            Do.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Move_CalledWithWrongIndex_4_ThrowArgumentException()
        {
            Action Do = () => _FullList.Move(0, 3);
            Do.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Move_Behave_AsExpected()
        {
            _FullList.Move(0, 2);
            _FullList.Should().Equal(1,2,0);
        }

        [Fact]
        public void Move_Behave_AsExpected_2()
        {
            _FullList.Move(2, 0);
            _FullList.Should().Equal(2, 0, 1);
        }

        [Fact]
        public void Move_CallObservableCollectionMove()
        {
            using (var monitor = _RawFullListObservable.Monitor()) 
            {
                var res = _FullListObservable.Move(2, 0);
                res.Should().BeSameAs(_FullListObservable);
                _FullListObservable.Should().Equal(2, 0, 1);
                monitor.Should().Raise("CollectionChanged")
                    .WithArgs<NotifyCollectionChangedEventArgs>(args => args.Action == NotifyCollectionChangedAction.Move);

            }
        }
    }
}

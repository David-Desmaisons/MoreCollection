﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using MoreCollection.Extensions;

namespace MoreCollectionTest.Extensions
{
    public class EnumerableAlgoExtenderTest
    {
        private class CompareMyObject : IComparer<MyObject>
        {
            public int Compare(MyObject x, MyObject y)
            {
                return x.Value - y.Value;
            }
        }

        private List<int> _MyList;

        public EnumerableAlgoExtenderTest()
        {
            _MyList = Enumerable.Range(0, 100000).ToList();
        }

        [Fact]
        public void Test_Null()
        {
            IEnumerable<int> res = null;
            Action ac = () => res.SortFirst(10);
            ac.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Test_Null_MinBy()
        {
            IEnumerable<MyObject> res = null;
            Action ac = () => res.MinBy();
            ac.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Test_Empty_MinBy()
        {
            IEnumerable<MyObject> res = Enumerable.Empty<MyObject>();
            MyObject mo = res.MinBy();
            mo.Should().BeNull();
        }

        [Fact]
        public void Test_MinBy_List_Classic()
        {
            List<MyObject> _MyList = Enumerable.Range(0, 1000000).Select(i => new MyObject(i.ToString(), i)).ToList();
            MyObject mo = _MyList.MinBy();
            mo.Should().NotBeNull();
            mo.Should().Be(_MyList[0]);
        }

        [Fact]
        public void Test_MaxBy_List_Classic()
        {
            List<MyObject> _MyList = Enumerable.Range(0, 1000000).Select(i => new MyObject(i.ToString(), i)).ToList();
            MyObject mo = _MyList.MaxBy();
            mo.Should().NotBeNull();
            mo.Should().Be(_MyList[999999]);
        }

        [Fact]
        public void Test_MinBy_List_Classic_Comparer()
        {
            List<MyObject> _MyList = Enumerable.Range(0, 1000000).Select(i => new MyObject(i.ToString(), i)).ToList();
            MyObject mo = _MyList.MinBy(Comparer<MyObject>.Create((a, b) => b.Value - a.Value));
            mo.Should().NotBeNull();
            mo.Should().Be(_MyList[999999]);
        }

        [Fact]
        public void Test_Negative()
        {
            IList<int> res = new List<int>();
            Action ac = () => res.SortFirst(-10);
            ac.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Test_Sort_All()
        {
            List<int> res = new List<int>() { 5, 45, 10, 2, 200, -1, 1 };
            var cp = res.SortFirst(100);
            res.Sort();
            cp.Should().Equal(res);
        }

        [Fact]
        public void Test_Sort_Int_Reverse()
        {
            ICollection<int> res = null;
            using (TimeTracer.TimeTrack("find 5 in a list of 100000"))
            {
                res = _MyList.SortLast(5);
            }
            using (TimeTracer.TimeTrack("full sort a list of 100000"))
            {
                _MyList.Sort();
            }

            res.Count.Should().Be(5);
            res.Should().Equal(99999, 99998, 99997, 99996, 99995);
        }

        [Fact]
        public void Test_Sort_Int_Parrellel()
        {
            ICollection<int> res = null;
            using (TimeTracer.TimeTrack("find 10 in a list of 100000"))
            {
                res = _MyList.SortFirstParallel(10);
            }

            res.Count.Should().Be(10);
            res.Should().Equal(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
        }

        [Fact]
        public void Test_Sort_Int_Parrellel_NonParelele()
        {
            _MyList = Enumerable.Range(0, 50).ToList();

            ICollection<int> res = null;
            using (TimeTracer.TimeTrack("find 10 in a list of 50"))
            {
                res = _MyList.SortFirstParallel(10);
            }

            res.Count.Should().Be(10);
            res.Should().Equal(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
        }

        [Fact]
        public void Test_Sort_Int_Parrellel_BadUsage()
        {
            IEnumerable<int> Null = null;
            Action prob = () => Null.SortFirstParallel(10);
            prob.Should().Throw<ArgumentNullException>();

            Action prob2 = () => _MyList.SortFirstParallel(-10);
            prob2.Should().Throw<ArgumentException>().WithMessage("iFirst");
        }

        [Fact]
        public void Test_Sort_Int()
        {
            ICollection<int> res = null;
            using (TimeTracer.TimeTrack("find 10 in a list of 100000"))
            {
                res = _MyList.SortFirst(10);
            }
            using (TimeTracer.TimeTrack("full sort a list of 100000"))
            {
                _MyList.Sort();
            }

            res.Count.Should().Be(10);
            res.Should().Equal(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);

            _MyList = Enumerable.Range(0, 100000).Reverse().ToList();
            res = null;
            using (TimeTracer.TimeTrack("find 10 in a list of 100000"))
            {
                res = _MyList.SortFirst(10);
            }
            using (TimeTracer.TimeTrack("full sort a list of 100000"))
            {
                _MyList.Sort();
            }

            res.Count.Should().Be(10);
            res.Should().Equal(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);

        }

        [Fact]
        public void Test_Sort_MyObject()
        {
            List<MyObject> _MyList = Enumerable.Range(0, 1000000).Select(i => new MyObject(i.ToString(), i)).ToList();
            ICollection<MyObject> res = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            using (TimeTracer.TimeTrack("find 10 in a list of 100000"))
            {
                res = _MyList.SortFirst(10);
            }
            using (TimeTracer.TimeTrack("full sort a list of 100000"))
            {
                _MyList.Sort();
            }

            res.Count.Should().Be(10);
            res.Should().Equal(_MyList.Take(10));
        }



        [Fact]
        public void Test_SortFull_MyObject()
        {
            List<MyObject> _MyList = Enumerable.Range(0, 2000000).Select(i => new MyObject(i.ToString(), i)).ToList();
            ICollection<MyObject> res, res2 = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            using (TimeTracer.TimeTrack("find 10 in a list of 2000000"))
            {
                res = _MyList.SortFirst(10);
            }
            using (TimeTracer.TimeTrack("find 10 parallel in a list of 2000000"))
            {
                res2 = _MyList.SortFirstParallel(10);
            }
            using (TimeTracer.TimeTrack("full sort a list of 2000000"))
            {
                _MyList.Sort();
            }

            res.Count.Should().Be(10);
            res.Should().Equal(_MyList.Take(10));

            res2.Count.Should().Be(10);
            res2.Should().Equal(_MyList.Take(10));
        }

        [Fact]
        public void Test_Sort_MyObjectFull_2()
        {

            CompareMyObject cm = new CompareMyObject();
            ICollection<MyObject> res, res2 = null;
            List<MyObject> fullexpected = null;
            List<MyObject> _MyList = Enumerable.Range(0, 200000).Select(i => new MyObject(i.ToString(), i)).Reverse().ToList();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            using (TimeTracer.TimeTrack("find 10 in a list of 200000"))
            {
                res = _MyList.SortFirst(10, cm);
            }
            using (TimeTracer.TimeTrack("find 10 parallel in a list of 2000000"))
            {
                res2 = _MyList.SortFirstParallel(10, cm);
            }
            using (TimeTracer.TimeTrack("full sort a list of 200000"))
            {
                fullexpected = _MyList.OrderBy(s => s, cm).ToList();
            }

            res.Count.Should().Be(10);
            res.Should().Equal(fullexpected.Take(10));

            res2.Count.Should().Be(10);
            res2.Should().Equal(fullexpected.Take(10));

        }

        [Fact]
        public void Test_Sort_MyObject_2()
        {
            CompareMyObject cm = new CompareMyObject();
            ICollection<MyObject> res = null;
            List<MyObject> fullexpected = null;
            List<MyObject> _MyList = Enumerable.Range(0, 1000000).Select(i => new MyObject(i.ToString(), i)).Reverse().ToList();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            using (TimeTracer.TimeTrack("find 10 in a list of 100000"))
            {
                res = _MyList.SortFirst(10, cm);
            }
            using (TimeTracer.TimeTrack("full sort a list of 100000"))
            {
                fullexpected = _MyList.OrderBy(s => s, cm).ToList();
            }

            res.Count.Should().Be(10);
            res.Should().Equal(fullexpected.Take(10));

        }
    }
}

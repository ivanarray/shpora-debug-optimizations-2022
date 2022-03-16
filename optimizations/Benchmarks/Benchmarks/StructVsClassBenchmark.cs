using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Benchmarks.Benchmarks
{
    public class C
    {
        public int N;
        public string Str;

        #region Equality members

        protected bool Equals(C other)
        {
            return N == other.N && string.Equals(Str, other.Str);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((C)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (N * 397) ^ (Str?.GetHashCode() ?? 0);
            }
        }

        #endregion
    }

    public struct S : IEquatable<S>
    {
        public override bool Equals(object? obj)
        {
            if (obj is not S s) return false;
            return s.N == N && s.Str.AsSpan().CompareTo(Str.AsSpan(), StringComparison.InvariantCulture) == 0;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(N, Str);
        }

        public int N;
        public string Str;

        public bool Equals(S other)
        {
            return N == other.N && Str == other.Str;
        }
    }

    [SimpleJob(warmupCount: 5, targetCount: 7)]
    public class StructVsClassBenchmark
    {
        private C[] classArr;
        private S[] structArr;

        [GlobalSetup]
        public void Setup()
        {
            classArr = Enumerable.Range(0, 1000).Select(x => new C { N = x, Str = Guid.NewGuid().ToString() })
                .ToArray();
            structArr = classArr.Select(x => new S { N = x.N, Str = x.Str }).ToArray();
        }

        [Benchmark]
        public bool Class() => classArr.Contains(new C { N = 100, Str = "something" });

        [Benchmark]
        public bool Struct() => structArr.Contains(new S { N = 100, Str = "something" });
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveProperties
{
    internal class MergedPropertyValueComparer<TLeft, TRight> : IEqualityComparer<MergedPropertyValue<TLeft, TRight>>
    {
        private readonly IEqualityComparer<TLeft> LeftComparer;
        private readonly IEqualityComparer<TRight> RightComparer;

        public MergedPropertyValueComparer(IEqualityComparer<TLeft> leftComparer, IEqualityComparer<TRight> rightComparer)
        {
            LeftComparer = leftComparer;
            RightComparer = rightComparer;
        }

        public bool Equals(MergedPropertyValue<TLeft, TRight> x, MergedPropertyValue<TLeft, TRight> y)
        {
            return LeftComparer.Equals(x.Left, y.Left) && RightComparer.Equals(x.Right, y.Right);
        }

        public int GetHashCode(MergedPropertyValue<TLeft, TRight> obj)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + LeftComparer.GetHashCode(obj.Left);
                hash = hash * 31 + RightComparer.GetHashCode(obj.Right);
                return hash;
            }
        }
    }

    internal struct MergedPropertyValue<TLeft, TRight>
    {
        public readonly TLeft Left;
        public readonly TRight Right;

        public MergedPropertyValue(TLeft left, TRight right)
        {
            Left = left;
            Right = right;
        }
    }

    internal class MergedPropertyValueComparer<TLeft, TMiddle, TRight> : IEqualityComparer<MergedPropertyValue<TLeft, TMiddle, TRight>>
    {
        private readonly IEqualityComparer<TLeft> LeftComparer;
        private readonly IEqualityComparer<TMiddle> MiddleComparer;
        private readonly IEqualityComparer<TRight> RightComparer;

        public MergedPropertyValueComparer(IEqualityComparer<TLeft> leftComparer, IEqualityComparer<TMiddle> middleComparer, IEqualityComparer<TRight> rightComparer)
        {
            LeftComparer = leftComparer;
            MiddleComparer = middleComparer;
            RightComparer = rightComparer;
        }

        public bool Equals(MergedPropertyValue<TLeft, TMiddle, TRight> x, MergedPropertyValue<TLeft, TMiddle, TRight> y)
        {
            return LeftComparer.Equals(x.Left, y.Left) && MiddleComparer.Equals(x.Middle, y.Middle) && RightComparer.Equals(x.Right, y.Right);
        }

        public int GetHashCode(MergedPropertyValue<TLeft, TMiddle, TRight> obj)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + LeftComparer.GetHashCode(obj.Left);
                hash = hash * 31 + MiddleComparer.GetHashCode(obj.Middle);
                hash = hash * 31 + RightComparer.GetHashCode(obj.Right);
                return hash;
            }
        }
    }

    internal struct MergedPropertyValue<TLeft, TMiddle, TRight>
    {
        public readonly TLeft Left;
        public readonly TMiddle Middle;
        public readonly TRight Right;

        public MergedPropertyValue(TLeft left, TMiddle middle, TRight right)
        {
            Left = left;
            Middle = middle;
            Right = right;
        }
    }
}

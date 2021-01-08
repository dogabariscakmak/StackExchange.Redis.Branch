using System.Collections.Generic;

namespace StackExchange.Redis.Branch.UnitTest.Fakes
{
    public class FakeRedisValueWrapper
    {
        public RedisValue Value { get; set; }
        public double Score { get; set; }
    }


    public class SortedFakeRedisValueWrapperComparar : IComparer<FakeRedisValueWrapper>
    {
        public int Compare(FakeRedisValueWrapper x, FakeRedisValueWrapper y)
        {
            if(x.Score == y.Score)
            {
                return x.Value.CompareTo(y.Value);
            }
            else
            {
                return x.Score.CompareTo(y.Score);
            }
        }
    }
}

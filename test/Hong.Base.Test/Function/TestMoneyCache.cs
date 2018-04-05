using Hong.Test.Function.Public;
using Xunit;

namespace Hong.Test.Function
{
    public class TestMemoryCache
    {
        [Fact]
        public void Standard()
        {
            var memoryHandle1 = new Cache.RuntimeCache.MemoryCacheConfiguration().CreateHandle<byte[]>();
            var memoryHandle2 = new Cache.RuntimeCache.MemoryCacheConfiguration().CreateHandle<string>();
            new TestCache().Standard(memoryHandle1, memoryHandle2);
        }
    }
}

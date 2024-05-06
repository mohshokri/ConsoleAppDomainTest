using ConsoleAppDomainTest;
using ConsoleAppDomainTest.Builder;
using System.IO;

namespace AppDomainUnitTest
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("Start=2024/4/29 10:00,Length=100,Dest1={M1;P(10-20);2024/4/29 10:10}", 1000)]
        [InlineData("Start=2024/5/29 11:00,Length=200,Dest1={M1;P(11-21);2024/5/29 11:10}", 2000)]
        [InlineData("Start=2024/6/29 12:00,Length=300,Dest1={M1;P(12-22);2024/6/29 12:10}", 3000)]
        [InlineData("Start=2024/7/29 13:00,Length=400,Dest1={M1;P(13-23);2024/7/29 13:10}", 4000)]
        [InlineData("Start=2024/8/29 14:00,Length=500,Dest1={M2;P(14-24);2024/8/29 14:10}", 5000)]
        public void Test1(string tripDesc, int expPrice)
        {
            var trip = new Director().Construct(tripDesc);
            int price = new Calculator().Calc(trip, new Rating { KMRate = 10 });

            Assert.Equal(expPrice, price);            
        }
    }
}
using Microsoft.AspNetCore.Mvc;

namespace customer_rank.Controllers
{

    [ApiController]
    public class CustomerController : ControllerBase
    {
        [HttpPost]
        [Route("/customer/{customerid}/score/{score}")]
        public decimal UpdateScore(ulong customerid, decimal score)
        {
            var c = Data.Customers.FirstOrDefault(t => t.CustomerID == customerid);

            if (c == null)
            {
                c = new Customer(customerid, score);
                var index = Data.Customers.FindPosition(c);
                Data.Customers.Insert(index, c);

                if (score > 0)
                {
                    index = Data.OutPut.FindPosition(c);
                    Data.OutPut.Insert(index, c);
                }
            }
            else // 已经存在，只是挪动。
            {
                if (c.Score > 0 && c.Score + score > 0) // 一直 为正值
                {
                    c.Score += score;
                    Data.Customers.MoveOptimized(c, score);
                    Data.OutPut.MoveOptimized(c, score);

                }
                else if (c.Score <= 0 && c.Score + score > 0) // 变为可视
                {
                    c.Score += score;
                    Data.Customers.MoveOptimized(c, score);
                    var index = Data.OutPut.FindPosition(c);// Data.Customers.BinarySearch(c,new CustomerCompare());
                    Data.OutPut.Insert(index, c);
                }
                else if (c.Score > 0 && c.Score + score <= 0) // 变为 hiden
                {
                    c.Score += score;
                    Data.Customers.MoveOptimized(c, score);
                    var index = Test.OutPut.FindIndex(t => t.CustomerID == customerid);
                    Data.OutPut.RemoveAt(index);
                }
                else
                { // 一直为负值
                    c.Score += score;
                    Data.Customers.MoveOptimized(c, score);
                }
            }
            return c.Score;
        }

        [HttpGet]
        [Route("/leaderboard/{customerid}")]//?high={high}&low={low}
        public Customer[] leaderboard(ulong customerid, int low, int high)
        {
            var i = Data.OutPut.FindIndex(t => t.CustomerID == customerid);

            if (i == -1)
            {
                return new Customer[0];
            }

            if (i - low > 0)
                return Data.OutPut.Skip(i - low).Take(high + low+1).ToArray();
            else
                return Data.OutPut.Take(high + i).ToArray();
        }

        [HttpGet]
        [Route("/leaderboard")] //?start={start}&end={end}  start 应该从1 开始
        public Customer[] leaderboard_range(int start, int end)
        {
            var result = Data.OutPut.Skip(start-1).Take(end-start+1).ToArray();
            return result;
        }
    }
}

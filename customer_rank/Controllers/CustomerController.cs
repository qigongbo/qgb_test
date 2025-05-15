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
            var c = Data.Customers.SingleOrDefault(t => t.CustomerID == customerid);

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
                if (c.Score > 0)    // 为正值
                {
                    c.Score += score;
                    Data.Customers.MoveOptimized(c, score);
                    if (c.Score > 0) // 保持正值
                    {
                        Data.OutPut.MoveOptimized(c, score);
                    }
                    else             // 变为 hiden
                    {
                        var index = Data.OutPut.FindIndex(t => t.CustomerID == customerid);
                        if (index >= 0)
                            Data.OutPut.RemoveAt(index);
                    }
                    
                }
                else // 为负值或0
                {
                    c.Score += score;
                    Data.Customers.MoveOptimized(c, score);
                    if (c.Score > 0) // 变为正值，可见
                    {
                        var index = Data.OutPut.FindPosition(c);
                        Data.OutPut.Insert(index, c);
                    }
                    //else  //still keep 负值或0，do nothing. just refresh the Score.
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
                return Data.OutPut.Skip(i - low).Take(high + low + 1).ToArray();
            else
                return Data.OutPut.Take(high + i).ToArray();
        }

        [HttpGet]
        [Route("/leaderboard")] //?start={start}&end={end}  start 应该从1 开始
        public Customer[] leaderboard_range(int start, int end)
        {
            var result = Data.OutPut.Skip(start - 1).Take(end - start + 1).ToArray();
            return result;
        }
    }
}

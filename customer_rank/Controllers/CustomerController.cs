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
            var c = Data.Customers.GetValueOrDefault(customerid);

            if (c == null)
            {
                c = new Customer(customerid, score);
                Data.Customers.Add(customerid, c);

                if (score > 0)
                {
                    c.Rank = Data.OutPut.FindPosition(c)+1;
                    Data.OutPut.Insert(c.Rank-1, c);

                    for (int i = c.Rank; i < Data.OutPut.Count; i++) {
                        Data.OutPut[i].Rank++;
                    }
                }
            }
            else if(score!=0)// 已经存在，只是挪动。
            {
                var compare_customer = new Customer(customerid, c.Score+score);
                compare_customer.Rank = c.Rank;

                if (c.Score > 0)    // 为正值
                {
                    if (compare_customer.Score > 0) // 保持正值
                    {
                        Data.OutPut.MoveOptimized(compare_customer, score);
                    }
                    else             // 变为 hiden
                    {
                        var index = Data.OutPut.FindIndex(t => t.CustomerID == customerid);
                        Data.OutPut.RemoveAt(index);
                    }
                }
                else // 为负值或0
                {
                    if (compare_customer.Score > 0) // 变为正值，可见
                    {
                        var index = Data.OutPut.FindPosition(compare_customer);
                        Data.OutPut.Insert(index, compare_customer);
                    }
                    //else  //still keep 负值或0，do nothing.  Customers 已经处理过，output 不需处理。
                }
                c.Score += score;

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

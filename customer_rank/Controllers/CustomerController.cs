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
            var c = Data.All.GetValueOrDefault(customerid);

            if (c == null)
            {
                c = new Customer(customerid, score);
                Data.All.Add(customerid, c);

                if (score > 0)
                {
                    var index = Data.Customer.FindPosition(c);
                    Data.Customer.Insert(index, c);
                }
            }
            else if(score!=0)// 已经存在，只是挪动。
            {
                var compare_customer = new Customer(customerid, c.Score+score);
                if (c.Score > 0)    // 为正值
                {
                    if (compare_customer.Score > 0) // 保持正值
                    {
                        Data.Customer.MoveOptimized(compare_customer, score);
                    }
                    else             // 变为 hiden
                    {
                        var index = Data.Customer.FindIndex(t => t.CustomerID == customerid);
                        Data.Customer.RemoveAt(index);
                    }
                }
                else // 为负值或0
                {
                    if (compare_customer.Score > 0) // 变为正值，可见
                    {
                        var index = Data.Customer.FindPosition(compare_customer);
                        Data.Customer.Insert(index, compare_customer);
                    }
                    //else  //still keep 负值或0，do nothing.  All 已经处理过，Customer 不需处理。
                }
                c.Score += score;

            }
            return c.Score;
        }

        [HttpGet]
        [Route("/leaderboard/{customerid}")]//?high={high}&low={low}
        public Customer[] leaderboard(ulong customerid, int low, int high)
        {
            var i = Data.Customer.FindIndex(t => t.CustomerID == customerid);

            if (i == -1)
            {
                return new Customer[0];
            }

            if (i - low > 0)
                return Data.Customer.Skip(i - low).Take(high + low + 1).ToArray();
            else
                return Data.Customer.Take(high + i).ToArray();
        }

        [HttpGet]
        [Route("/leaderboard")] //?start={start}&end={end}  start 应该从1 开始
        public Customer[] leaderboard_range(int start, int end)
        {
            var result = Data.Customer.Skip(start - 1).Take(end - start + 1).ToArray();
            return result;
        }
    }
}

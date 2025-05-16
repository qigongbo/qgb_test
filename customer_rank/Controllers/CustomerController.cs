using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

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
                    c.Rank = Data.Customers.FindPosition(c)+1;
                    Data.Customers.Insert(c.Rank-1, c);

                    for (int i = c.Rank; i < Data.Customers.Count; i++) {
                        Data.Customers[i].Rank++;
                    }
                }
            }
            else if(score!=0)       // 已经存在，只是挪动。
            {
                if (c.Score > 0)    // 为正值
                {
                    if (c.Score + score > 0) // 保持正值
                    {
                        Data.Customers.MoveOptimized(c, score);
                    }
                    else             // 变为 hiden
                    {
                        var index = c.Rank-1;
                        
                        Data.Customers.RemoveAt(index);
                        for (int i = index; i < Data.Customers.Count; i++)
                        {
                            Data.Customers[i].Rank--;
                        }
                    }
                }
                else // 为负值或0
                {
                    if (c.Score + score > 0) // 变为正值，可见
                    {
                        c.Rank = Data.Customers.FindPosition(c, score) +1;
                        Data.Customers.Insert(c.Rank-1, c);
                       
                        for (int i = c.Rank; i < Data.Customers.Count; i++)
                        {
                            Data.Customers[i].Rank++;
                        }
                    }
                    //else  //still keep 负值或0，do nothing.  All 已经处理过，Customers,不包含这个元素 不需处理。
                }
                c.Score = c.Score + score;
            }
            return c.Score;
        }

        [HttpGet]
        [Route("/leaderboard/{customerid}")]//?high={high}&low={low}
        public Customer[] leaderboard(ulong customerid, int low, int high)
        {
            var c = Data.All.GetValueOrDefault(customerid);

            if (c is null || c.Score <=0)
            {
                return new Customer[0];
            }

            var i = c.Rank - 1;

            if (i - low > 0)
                return Data.Customers.Skip(i - low).Take(high + low + 1).ToArray();
            else
                return Data.Customers.Take(high + i).ToArray();
        }

        [HttpGet]
        [Route("/leaderboard")] //?start={start}&end={end}  start 应该从1 开始
        public Customer[] leaderboard_range(int start, int end)
        {
            var result = Data.Customers.Skip(start - 1).Take(end - start + 1).ToArray();
            return result;
        }
    }
}

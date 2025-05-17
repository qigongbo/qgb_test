using Microsoft.AspNetCore.Mvc;

namespace customer_rank.Controllers
{

    [ApiController]
    public class CustomerController : ControllerBase
    {
        private static ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();

        [HttpPost]
        [Route("/customer/{customerid}/score/{score}")]
        public decimal UpdateScore(ulong customerid, decimal score)
        {
            _rwLock.EnterWriteLock();
            try
            {
                var c = Data.All.GetValueOrDefault(customerid);

                if (c == null)
                {
                    c = new Customer(customerid, score);
                    Data.All.TryAdd(customerid, c);

                    if (score > 0)
                    {
                        Data.Insert(c);
                    }
                }
                else if (score != 0)    // 已经存在，只是挪动。
                {
                    if (c.Score > 0)    // 为正值
                    {
                        if (c.Score + score > 0) // 保持正值
                        {
                            Data.Customers.MoveOptimized(c, score);
                        }
                        else             // 变为 hiden
                        {
                            var index = c.Rank - 1;

                            Data.Customers.RemoveAt(index);
                            for (int i = index; i < Data.Customers.Count; i++)
                            {
                                Data.Customers[i].Rank--;
                            }

                            var s = new List<Customer>(Data.Customers);
                        }
                    }
                    else // 为负值或0
                    {
                        if (c.Score + score > 0) // 变为正值，可见
                        {
                            Data.Insert(c, score);
                        }
                        //else  //still keep 负值或0，do nothing.  All 已经处理过，Customers,不包含这个元素 不需处理。
                    }
                    c.Score = c.Score + score;
                }
                return c.Score;
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        [HttpGet]
        [Route("/leaderboard/{customerid}")]//?high={high}&low={low}
        public Customer[] GetLeaderboardAroundCustomer(ulong customerid, int low, int high)
        {
            _rwLock.EnterReadLock();
            try
            {
                var c = Data.All.GetValueOrDefault(customerid);

                if (c is null || c.Score <= 0)
                {
                    return Array.Empty<Customer>();
                }

                var i = c.Rank - 1;
                if (i - low > 0)
                    return Data.Customers.Skip(i - low).Take(high + low + 1).ToArray();
                else
                    return Data.Customers.Take(high + i).ToArray();
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }

        [HttpGet]
        [Route("/leaderboard")] //?start={start}&end={end}  start 应该从1 开始
        public Customer[] GetLeaderboardRange(int start, int end)
        {
            _rwLock.EnterReadLock();
            try
            {
                return Data.Customers.Skip(start - 1).Take(end - start + 1).ToArray();
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }
    }
}

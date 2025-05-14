using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace customer_rank.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        [HttpPost]
        [Route("/customer/{customerid}/score/{score}")]
        public decimal UpdateScore(ulong customerid = 44, decimal score = 12)
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
    }
}

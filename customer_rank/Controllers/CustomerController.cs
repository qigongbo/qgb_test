using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace customer_rank.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var data = Data.Customers;

            var str = new StringBuilder($"Customer ID       Score      Rank");
            var tableHtml = $"""
                <table border='1' cellspacing='0' cellpadding='8' style='width:100%;border-collapse:collapse;'>
                    <thead style='background-color:#f2f2f2;'>
                        <tr>
                            <th>CustomerID</th>
                            <th>Score</th>
                            <th>Rank</th>
                        </tr>
                    </thead>
                    <tbody>
                        {string.Join("", data.Select((item, i) => $"""
                            <tr style='border-bottom:1px solid #ddd;'>
                                <td>{item.CustomerID}</td>
                                <td>{item.Score}</td>
                                <td>{i}</td>
                            </tr>
                        """))}
                    </tbody>
                </table>
            """;

            // 返回HTML内容
            return Content(tableHtml, "text/html");
        }


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

        [HttpPost]
        [Route("/init")]
        public string init()
        {
            Data.Customers.Clear();
            Test.Customers.Clear();
            Data.OutPut.Clear();
            Test.OutPut.Clear();

            //821,541
            //224,250
            //761,656
            UpdateScore(860, 1000);
            UpdateScore(593, 906);
            UpdateScore(260, 10);


            UpdateScore_correct(593, 906);
            UpdateScore_correct(260, 98);

            return "OK";
        }

        [HttpPost]
        [Route("/test")]
        public string test()
        {
            Data.Customers.Clear();
            Test.Customers.Clear();
            Data.OutPut.Clear();
            Test.OutPut.Clear();

            Random random = new Random();
            var count = 40000;
            var mid = 1000;
            var score_max = 1000;
            var d = DateTime.Now;
            for (int i = 0; i < count; i++)
            {
                var id = (ulong)random.NextInt64(mid);
                var score = (ulong)random.NextInt64(score_max);
                UpdateScore(id, score);
            }

            StringBuilder s = new StringBuilder("result adding time cost:" + DateTime.Now.Subtract(d).TotalSeconds);
            s.AppendLine();

            d = DateTime.Now;
            Data.Customers.Clear();
            Test.Customers.Clear();
            Data.OutPut.Clear();
            Test.OutPut.Clear();
            for (int i = 0; i < count; i++)
            {
                var id = (ulong)random.NextInt64(mid);
                var score = (ulong)random.NextInt64(score_max);
                UpdateScore_correct(id, score);
            }
            s.AppendLine("sort time cost:" + DateTime.Now.Subtract(d).TotalSeconds);

            Console.WriteLine($"----------");
            Data.Customers.Clear();
            Test.Customers.Clear();
            Data.OutPut.Clear();
            Test.OutPut.Clear();
            d = DateTime.Now;
            for (int i = 0; i < count; i++)
            {
                var id = (ulong)random.NextInt64(mid);
                var score = (ulong)random.NextInt64(score_max);
                Console.WriteLine($"{id},{score}");
                UpdateScore(id, score);

                UpdateScore_correct(id, score);
            }

            s.AppendLine("comprehensive time cost:" + DateTime.Now.Subtract(d).TotalSeconds);

            for (int i = 0; i < Data.Customers.Count(); i++)
            {
                if (Data.Customers[i].CustomerID != Test.Customers[i].CustomerID)
                {
                    throw new Exception("not equal");
                }
            }


            return s.ToString();
        }



        [HttpPost]
        [Route("/customer_correct/{customerid}/score/{score}")]
        public decimal UpdateScore_correct(ulong customerid, decimal score)
        {
            var c = Test.Customers.FirstOrDefault(t => t.CustomerID == customerid);

            if (c == null)
            {
                c = new Customer(customerid, score);
                Test.Customers.Add(new Customer(customerid, score));
                if (score > 0)
                {
                    Test.OutPut.Add(new Customer(customerid, score));
                }
            }
            else
            {
                if (c.Score <= 0 && c.Score + score > 0) // 由 小于等于0 变为 大于0
                {
                    Test.OutPut.Add(c);
                }
                else if (c.Score > 0 && c.Score + score <= 0) // 由 大于等于0   变为 小于0
                {
                    var index = Test.OutPut.FindIndex(t => t.CustomerID == customerid);
                    Test.OutPut.RemoveAt(index);
                }

                c.Score += score;

            }

            Test.Customers.Sort();
            Test.OutPut.Sort();
            return c.Score;
        }
    }
}

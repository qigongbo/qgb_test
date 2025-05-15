using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace customer_rank.Controllers
{
    [ApiController]
    [Route("/")]
    public class HomeController : ControllerBase
    {
        CustomerController Con = new CustomerController();
        [HttpPost]
        [Route("/test")]
        public string test()
        {
            clearData();

            Con.UpdateScore(860, 1000);
            Con.UpdateScore(593, 906);
            Con.UpdateScore(260, 10);

            return "OK";
        }

        [HttpPost]
        [Route("/verfiy")]
        public string verfiy()
        {
            clearData();

            Random random = new Random();
            var count = 40000;
            var id_max = 1000;

            var list = new List<(ulong id, decimal score)>();
            for (int i = 0; i < count; i++)
            {
                list.Add(((ulong)random.NextInt64(id_max), (decimal)random.NextInt64(-1000,1000)));
            }


            var d = DateTime.Now;
            foreach (var item in list)
            {
                Con.UpdateScore(item.id, item.score);
            }
            StringBuilder s = new StringBuilder($"result time cost:{DateTime.Now.Subtract(d).TotalSeconds}\n\n");
    

            d = DateTime.Now;
            foreach (var item in list)
            {
                UpdateScore_correct(item.id, item.score);
            }
            s.AppendLine("sort time cost:" + DateTime.Now.Subtract(d).TotalSeconds);

            Console.WriteLine($"----------");

            for (int i = 0; i < Test.OutPut.Count(); i++)
            {
                if (Data.OutPut[i].CustomerID != Test.OutPut[i].CustomerID)
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
                Test.Customers.Add(c);
                if (score > 0)
                {
                    Test.OutPut.Add(c);
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
                // 一直都是 正数，只用增加score; 一直都是负的，也只用刷新score;
                c.Score += score;
            }

            Test.Customers.Sort();
            Test.OutPut.Sort();
            return c.Score;
        }

        [HttpGet()]
        [Route("/get_data_table")]
        public IActionResult Get()
        {
            // 返回HTML内容
            return Content(Getstr(Data.OutPut), "text/html");
        }

        [HttpGet]
        [Route("/get_test_data_table")]
        public IActionResult GetTest()
        {
            // 返回HTML内容
            return Content(Getstr(Test.OutPut), "text/html");
        }

        private void clearData()
        {
            Data.Customers.Clear();
            Test.Customers.Clear();
            Data.OutPut.Clear();
            Test.OutPut.Clear();
        }

        private string Getstr(List<Customer> data)
        {
            var str = new StringBuilder($"Customer ID       Score      Rank");
            return $"""
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
                                <td>{i+1}</td>
                            </tr>
                        """))}
                    </tbody>
                </table>
            """;


        }
    }
}

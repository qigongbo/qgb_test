using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace customer_rank.Controllers
{
    [ApiController]
    [Route("/")]
    public class Home : ControllerBase
    {

        [HttpGet()]
        [Route("/")]
        public IActionResult Get()
        {
            // 返回HTML内容
            return Content(Getstr(Data.Customers), "text/html");
        }

        [HttpGet]
        [Route("/test")]
        public IActionResult GetTest()
        {
            // 返回HTML内容
            return Content(Getstr(Test.Customers), "text/html");
        }


        private string Getstr(List<Customer> data) {
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
                                <td>{i}</td>
                            </tr>
                        """))}
                    </tbody>
                </table>
            """;


        }
    }
}

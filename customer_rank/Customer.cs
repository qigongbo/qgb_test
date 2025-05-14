
namespace customer_rank
{
    public class Customer: IComparable<Customer>
    {
        public Customer(ulong id, decimal score)
        {
            CustomerID = id;
            Score = score;
        }

        public ulong CustomerID { get; init; }

        public int CompareTo(Customer? other)
        {
            if (other.Score == Score)
                return other.CustomerID > CustomerID ? -1 : 1;
            else
                return other.Score > Score ? 1 : -1;
        }

        // 重载 < 运算符
        public static bool operator <(Customer left, Customer right)
        {
            return left.CompareTo(right) < 0;
        }

        // 重载 > 运算符
        public static bool operator >(Customer left, Customer right)
        {
            return right < left; // 复用 < 运算符的逻辑
        }

        public decimal Score { get; set; }
    }

}

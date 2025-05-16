namespace customer_rank
{
    public static class Test
    {
        public static List<Customer> Customers = new List<Customer>();
        public static List<Customer> All = new List<Customer>();
    }
    public static class Data
    {
        public static List<Customer> Customers = new List<Customer>();
        public static Dictionary<ulong, Customer> All = new Dictionary<ulong, Customer>();


        /// <summary>
        ///  一个有序数组，要插入一个新元素，返回要插入的位置
        /// </summary>
        /// <param name="list"></param>
        /// <param name="customer">插入之后，元素的值不要改变。就是 插入之前就要改变</param>
        /// <returns></returns>
        public static int FindPosition(this List<Customer> list, Customer customer, decimal change = 0)
        {
            if (list.Count() == 0)
                return 0;

            var tmp = change == 0 ? customer : new Customer(customer.CustomerID, customer.Score + change);

            int left = 0, right = list.Count - 1;
            while (left <= right)
            {
                int mid = (left + right) >> 1; // 位运算代替除法
                if (list[mid] < tmp)
                    left = mid + 1;
                else
                    right = mid - 1;
            }
            return left;
        }

        /// <summary>
        ///  一个有序数组，要插入一个新元素，返回要插入的位置
        /// </summary>
        /// <param name="list"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="customer">插入之后，元素的值不要改变。就是 插入之前就要改变</param>
        /// <returns></returns>
        private static int FindPosition(this List<Customer> list, int left, int right, Customer customer)
        {
            if (list.Count() == 0)
                return 0;

            while (left <= right)
            {
                int mid = (left + right) >> 1; // 位运算代替除法
                if (list[mid] < customer)
                    left = mid + 1;
                else
                    right = mid - 1;
            }
            return left;
        }

        /// <summary>
        /// 一个有序数组，一个元素的值要有变化。变化之后，重新排序。
        /// </summary>
        /// <param name="list"></param>
        /// <param name="customer">变化之后的值</param>
        /// <param name="Changed_score"></param>
        public static void MoveOptimized(this List<Customer> list, Customer customer, decimal change)
        {
            var fromIndex = customer.Rank - 1;
            var tmp = new Customer(customer.CustomerID, customer.Score + change);
            var toIndex = 0;
            if (change > 0) //score 变大, 索引越小。这是倒排，大的在前。
            {
                toIndex = list.FindPosition(0, fromIndex, tmp);

                for (int i = fromIndex; i > toIndex; i--)
                {
                    list[i] = list[i - 1];
                    list[i].Rank = i + 1; // ++
                }
            }
            else           //score 变小, 索引越大。这是倒排，小的在后。
            {
                toIndex = list.FindPosition(0, list.Count() - 1, tmp) - 1;

                for (int i = fromIndex; i < toIndex; i++)
                {
                    list[i] = list[i + 1];
                    list[i].Rank = i + 1;
                }
            }

            list[toIndex] = customer; // 放置目标元素
            list[toIndex].Rank = toIndex + 1;
        }
    }
}

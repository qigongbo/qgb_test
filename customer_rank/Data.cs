namespace customer_rank
{
    public static class Test
    {
        public static List<Customer> OutPut = new List<Customer>();
        public static List<Customer> Customers = new List<Customer>();
    }
    public static class Data
    {
        public static List<Customer> OutPut = new List<Customer>();
        public static Dictionary<ulong, Customer> Customers = new Dictionary<ulong, Customer>();
        public static Dictionary<ulong, Customer> Hiddens = new Dictionary<ulong, Customer>();

        /// <summary>
        ///  一个有序数组，要插入一个新元素，返回要插入的位置
        /// </summary>
        /// <param name="list"></param>
        /// <param name="customer">插入之后，元素的值不要改变。就是 插入之前就要改变</param>
        /// <returns></returns>
        public static int FindPosition(this List<Customer> list, Customer customer)
        {
            if (list.Count() == 0)
                return 0;

            int left = 0, right = list.Count - 1;
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
        public static void MoveOptimized(this List<Customer> list, Customer customer, decimal Changed_score)
        {
            var fromIndex = list.FindIndex(t => t.CustomerID == customer.CustomerID);

            if (Changed_score > 0) //score 变大, 索引越小。这是倒排，大的在前。
            {
                var toIndex = list.FindPosition(0, fromIndex, customer);

                for (int i = fromIndex; i > toIndex; i--)
                {
                    list[i] = list[i - 1];
                }
                
                list[toIndex] = customer; // 放置目标元素
            }
            else    //score 变小, 索引越大。这是倒排，小的在后。
            {
                var toIndex = list.FindPosition(0, list.Count() - 1, customer)-1;


                //// 将中间元素依次前移
                for (int i = fromIndex; i < toIndex; i++)
                {
                    list[i] = list[i + 1];
                }

                list[toIndex] = customer; 
            }

        }
    }
}

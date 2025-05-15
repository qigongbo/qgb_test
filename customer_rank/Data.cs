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
        public static List<Customer> Customers = new List<Customer>();
        public static Dictionary<ulong, Customer> Dic = new Dictionary<ulong, Customer>();

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
            var m = list.SingleOrDefault(t => t.CustomerID == customer.CustomerID);
            var fromIndex = list.FindIndex(t => t.CustomerID == customer.CustomerID);

            Customer item = list[fromIndex];

            var toIndex = 0;
            // Changed_score 决定元素移动方式
            if (Changed_score < 0) 
            {
                toIndex = list.FindPosition(0, fromIndex, customer);
                // 将中间元素依次后移
                for (int i = fromIndex; i >= toIndex+1; i--)
                {
                    list[i] = list[i - 1];
                }
            }
            else  //score 变大
            {
                toIndex = list.FindPosition(fromIndex, list.Count() - 1, customer);

                // 将中间元素依次前移
                for (int i = fromIndex; i < toIndex-1; i++)
                {
                    list[i] = list[i + 1];
                }
            }

            if (fromIndex != toIndex)// 放置目标元素
            {
                list[toIndex] = item;
            }

            m = list.SingleOrDefault(t => t.CustomerID == customer.CustomerID);
        }
    }
}

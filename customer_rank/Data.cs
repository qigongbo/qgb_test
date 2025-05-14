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

        public static int FindPosition(this List<Customer> list, int left, int  right, Customer customer)
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

        public static void MoveOptimized(this List<Customer> list, Customer customer, decimal Changed_score)
        {
            var fromIndex = list.FindIndex(t => t.CustomerID == customer.CustomerID);

            Customer item = list[fromIndex];

            var toIndex = 0;

            // Changed_score 决定元素移动方式
            if (Changed_score > 0) //score 变大，向前
            {
                toIndex = list.FindPosition(0, fromIndex, customer);
                // 向前移动：将中间元素依次后移
                for (int i = fromIndex; i > toIndex; i--)
                {
                    list[i] = list[i - 1];
                }

            }
            else
            {
                toIndex = list.FindPosition(fromIndex, list.Count() - 1, customer);

                // 向后移动：将中间元素依次前移
                for (int i = fromIndex; i < toIndex; i++)
                {
                    list[i] = list[i + 1];
                }
            }

            if (fromIndex != toIndex)// 放置目标元素
            {
                list[toIndex] = item;
            }
        }
    }
}

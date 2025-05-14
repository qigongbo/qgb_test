using System;
using System.Collections;

namespace customer_rank
{
    public class Customer: IComparable<Customer>
    {
        public Customer(ulong id, decimal score)
        {
            CustomerID = id;
            Score = score;
        }

        public Customer(ulong id, Group group)
        {
            CustomerID = id;
            Group = group;
        }
        public ulong CustomerID { get; set; }
        public Group Group { get; set; }


        public int CompareTo(Customer? other)
        {
            if (other.Score == Score)
                return other.CustomerID > CustomerID ? -1 : 1;
            else
                return other.Score > Score ? 1 : -1;
        }

        // ?? < ???
        public static bool operator <(Customer left, Customer right)
        {
            return left.CompareTo(right) < 0;
        }

        // ?? > ???
        public static bool operator >(Customer left, Customer right)
        {
            return right < left; // ?? < ??????
        }

        public decimal Score;
    }
    public class CustomerCompare: IComparer<Customer>
    {
        public int Compare(Customer? s, Customer? t)
        {
            if (s.Score == t.Score)
                return s.CustomerID > t.CustomerID ? 0 : -1;
            else
                return s.Score > t.Score ? -1 : 0;
        }
    }
    public class Group
    {
        public static Dictionary<decimal, Group> Data = new Dictionary<decimal, Group>();
        public Group(decimal score)
        {
            Score = score;
        }

        public decimal Score { get; set; }

        public int Rank { get; set; }

        public int Start { get; set; }
        public int End { get; set; }

        public void Remove()
        {
            End--;
        }

        public void Add(ulong id)
        {
            End++;
        }
    }
}

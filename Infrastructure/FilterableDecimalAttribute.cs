using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Infrastructure
{
   
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FilterableDecimalAttribute : FilterableAttribute
    {
        public FilterableDecimalAttribute()
        {
            ExpressionProvider = new DecimalToIntFilterExpressionProvider();
        }
    }
}

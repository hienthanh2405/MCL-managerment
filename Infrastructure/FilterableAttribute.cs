using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FilterableAttribute : Attribute
    {
        public string EntityProperty { get; set; }

        public IFilterExpressionProvider ExpressionProvider { get; set; }
            = new DefaultFilterExpressionProvider();
    }
}

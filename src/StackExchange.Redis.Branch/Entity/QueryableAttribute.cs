using System;
using System.Collections.Generic;
using System.Text;

namespace StackExchange.Redis.Branch.Entity
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RedisQueryableAttribute : Attribute
    {
    }
}

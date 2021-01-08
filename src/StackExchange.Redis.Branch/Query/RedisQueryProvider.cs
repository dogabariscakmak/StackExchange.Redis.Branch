using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace StackExchange.Redis.Branch.Query
{
    public class RedisQueryProvider : QueryProvider
    {
        protected readonly IConnectionMultiplexer _redisConnectionMultiplexer;
        protected readonly IDatabase _redisDatabase;

        public RedisQueryProvider(IConnectionMultiplexer redisConnectionMultiplexer)
        {
            _redisConnectionMultiplexer = redisConnectionMultiplexer;
            _redisDatabase = _redisConnectionMultiplexer.GetDatabase();
        }

        /// <summary>
        /// Gets the Query text for given expression tree.
        /// </summary>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <returns>Query Text as postfix representation.</returns>
        public override string GetQueryText(Expression expression)
        {
            return Translate(expression);
        }

        /// <summary>
        /// Executes the query represented by a specified expression tree.
        /// </summary>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <returns>The value that results from executing the specified query.</returns>
        public override object Execute(Expression expression)
        {
            List<HashEntry[]> reader = new List<HashEntry[]>();
            string postfix = Translate(expression);
            foreach (string redisKey in EvaluatePostfix(postfix))
            {
                reader.Add(_redisDatabase.HashGetAll(redisKey));
            }
            Type elementType = TypeSystem.GetElementType(expression.Type);

            return Activator.CreateInstance(
                typeof(ObjectReader<>).MakeGenericType(elementType),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] { reader },
                null);
        }

        private string Translate(Expression expression)
        {
            expression = Evaluator.PartialEval(expression);
            return new QueryTranslator().Translate(expression);
        }

        private IEnumerable<string> EvaluatePostfix(string postfix)
        {
            string[] validOperators = { "Union", "Intersection" };

            Stack<Operand> stack = new Stack<Operand>();

            List<string> tokens = new List<string>();
            Regex myRegex = new Regex("{{(.*?)}}");
            Match matchResult = myRegex.Match(postfix);

            while (matchResult.Success)
            {
                tokens.Add(matchResult.Value.Replace("{{", "").Replace("}}", ""));
                matchResult = matchResult.NextMatch();
            }

            foreach (string token in tokens)
            {
                if (validOperators.Contains(token))
                {
                    Operand operand1 = stack.Pop();
                    Operand operand2 = stack.Pop();

                    switch (token)
                    {
                        case "Union":
                            stack.Push(operand1.UnionWith(operand2));
                            break;
                        case "Intersection":
                            stack.Push(operand1.IntersectWith(operand2));
                            break;
                        default:
                            throw new NotSupportedException(string.Format("The operator '{0}' is not supported", token));
                    }
                }
                else
                {
                    stack.Push(new Operand(_redisDatabase, token));
                }
            }

            return stack.Pop().KeySet;
        }

        class Operand
        {
            public double Start { get; private set; }
            public double Stop { get; private set; }
            public Exclude ExcludeType { get; private set; }
            public string RedisKey { get; private set; }

            private HashSet<string> _keySet;
            public HashSet<string> KeySet
            {
                get
                {
                    if (_keySet == default)
                    {
                        _keySet = new HashSet<string>(_redisDatabase.SortedSetRangeByScore(RedisKey, Start, Stop, ExcludeType).Select(i => $"{_entityName}:data:{i}"));
                    }
                    return _keySet;
                }
                private set
                {
                    _keySet = value;
                }
            }

            private readonly string _entityName;
            private readonly IDatabase _redisDatabase;

            public Operand(IDatabase redisDatabase, HashSet<string> keySet)
            {
                _redisDatabase = redisDatabase;
                KeySet = keySet;
            }

            public Operand(IDatabase redisDatabase, string expression)
            {
                _redisDatabase = redisDatabase;

                string[] expressionTokens = expression.Split(":");
                string[] memberInfo = expressionTokens[1].Split("-");

                _entityName = expressionTokens[0];

                if (expressionTokens.Count() == 4) //SortedSet
                {
                    List<RedisValue> keys = new List<RedisValue>();
                    double value = 0;
                    double.TryParse(expressionTokens[2], out value);

                    switch (expressionTokens[3])
                    {
                        case "=":
                            Start = value;
                            Stop = value;
                            ExcludeType = Exclude.None;
                            RedisKey = $"{expressionTokens[0]}:properties:{memberInfo[0]}";
                            break;
                        case "<>":
                            Start = value;
                            Stop = value;
                            ExcludeType = Exclude.Both;
                            RedisKey = $"{expressionTokens[0]}:properties:{memberInfo[0]}";
                            break;
                        case "<":
                            Start = double.NegativeInfinity;
                            Stop = value;
                            ExcludeType = Exclude.Stop;
                            RedisKey = $"{expressionTokens[0]}:properties:{memberInfo[0]}";
                            break;
                        case "<=":
                            Start = double.NegativeInfinity;
                            Stop = value;
                            ExcludeType = Exclude.None;
                            RedisKey = $"{expressionTokens[0]}:properties:{memberInfo[0]}";
                            break;
                        case ">":
                            Start = value;
                            Stop = double.PositiveInfinity;
                            ExcludeType = Exclude.Start;
                            RedisKey = $"{expressionTokens[0]}:properties:{memberInfo[0]}";
                            break;
                        case ">=":
                            Start = value;
                            Stop = double.PositiveInfinity;
                            ExcludeType = Exclude.None;
                            RedisKey = $"{expressionTokens[0]}:properties:{memberInfo[0]}";
                            break;
                        default:
                            throw new NotSupportedException(string.Format("The expression '{0}' is not supported", expression));
                    }

                }
                else if (expressionTokens.Count() == 3) //Set
                {
                    Start = double.NegativeInfinity;
                    Stop = double.PositiveInfinity;
                    ExcludeType = Exclude.None;
                    RedisKey = $"{expressionTokens[0]}:properties:{memberInfo[0]}:{expressionTokens[2]}";
                }
                else
                {
                    throw new NotSupportedException(string.Format("The expression '{0}' is not supported", expression));
                }
            }

            public Operand UnionWith(Operand other)
            {
                KeySet.UnionWith(other.KeySet);
                return this;
            }

            public Operand IntersectWith(Operand other)
            {
                KeySet.IntersectWith(other.KeySet);
                return this;
            }
        }
    }
}

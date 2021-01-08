using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Linq;

namespace StackExchange.Redis.Branch.Query
{
    internal class QueryTranslator : ExpressionVisitor
    {
        private StringBuilder _postfix;
        private SetOperand _setOperand;

        internal QueryTranslator()
        {
        }

        internal string Translate(Expression expression)
        {
            _postfix = new StringBuilder();
            _setOperand = new SetOperand();
            Visit(expression);
            return _postfix.ToString();
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }

            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {
                Visit(m.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                Visit(lambda.Body);
                return m;
            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            Visit(b.Left);
            Visit(b.Right);

            switch (b.NodeType)
            {
                case ExpressionType.AndAlso:
                    _postfix.Append("{{Intersection}}");
                    break;
                case ExpressionType.OrElse:
                    _postfix.Append("{{Union}}");
                    break;
                case ExpressionType.Equal:
                    _setOperand.SetExpression("=");
                    _postfix.Append($" {_setOperand} ");
                    _setOperand.Reset();
                    break;
                case ExpressionType.NotEqual:
                    _setOperand.SetExpression("<>");
                    _postfix.Append($" {_setOperand} ");
                    _setOperand.Reset();
                    break;
                case ExpressionType.LessThan:
                    _setOperand.SetExpression("<");
                    _postfix.Append($" {_setOperand} ");
                    _setOperand.Reset();
                    break;
                case ExpressionType.LessThanOrEqual:
                    _setOperand.SetExpression("<=");
                    _postfix.Append($" {_setOperand} ");
                    _setOperand.Reset();
                    break;
                case ExpressionType.GreaterThan:
                    _setOperand.SetExpression(">");
                    _postfix.Append($" {_setOperand} ");
                    _setOperand.Reset();
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    _setOperand.SetExpression(">=");
                    _postfix.Append($" {_setOperand} ");
                    _setOperand.Reset();
                    break;

                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
            }

            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;

            if (q != null)
            {
                _setOperand.SetEntityName(q.ElementType.Name);
            }
            else if (c.Value == null)
            {
                _setOperand.SetValue("", TypeCode.String);
            }
            else
            {
                TypeCode typeCode = Type.GetTypeCode(c.Value.GetType());
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                    case TypeCode.Char:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                    case TypeCode.String:
                        _setOperand.SetValue(Convert.ToString(c.Value), typeCode);
                        break;
                    case TypeCode.DateTime:
                        _setOperand.SetValue(Convert.ToString(DateTime.SpecifyKind(((DateTime)c.Value), DateTimeKind.Utc).Ticks), typeCode);
                        break;
                    default:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));
                }
            }

            return c;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                _setOperand.SetName(m.Member.Name);
                return m;
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }

        class SetOperand
        {
            public string EntityName { get; private set; }
            public string Value { get; private set; }
            public TypeCode ValueType { get; private set; }
            public string Name { get; private set; }
            public string Expression { get; private set; }

            public void SetEntityName(string entityName)
            {
                EntityName = entityName;
            }

            public void SetValue(string value, TypeCode valueType)
            {
                Value = value;
                ValueType = valueType;
            }

            public void SetName(string name)
            {
                Name = name;
            }

            public void SetExpression(string expression)
            {
                Expression = expression;
            }

            public override string ToString()
            {
                switch (ValueType)
                {
                    case TypeCode.Char:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                    case TypeCode.DateTime:
                        return $"{{{{{EntityName}:{Name}-{ValueType}:{Value}:{Expression}}}}}";
                    case TypeCode.Boolean:
                    case TypeCode.String:
                        return $"{{{{{EntityName}:{Name}-{ValueType}:{Value}}}}}";
                    default:
                        throw new NotSupportedException(string.Format("The type for '{0}' is not supported", ValueType));
                }
            }

            public void Reset()
            {
                Value = "";
                ValueType = TypeCode.String;
                Name = "";
                Expression = "";
            }
        }
    }
}

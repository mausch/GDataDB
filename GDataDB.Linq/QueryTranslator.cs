using System;
using System.Linq.Expressions;
using System.Text;

namespace GDataDB.Linq {
	public class QueryTranslator : ExpressionVisitor {
		private readonly StringBuilder sb = new StringBuilder();

		public string Translate(Expression e) {
			Visit(e);
			return sb.ToString();
		}

		protected override Expression VisitMethodCall(MethodCallExpression m) {
			if (m.Method.Name == "Where") {
				Visit(m.Arguments[1]);
				return m;
			}
			throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
		}

		protected override Expression VisitBinary(BinaryExpression b) {
			sb.Append("(");
			Visit(b.Left);
			switch (b.NodeType) {
				case ExpressionType.And:
				case ExpressionType.AndAlso:
					sb.Append("&&");
					break;
				case ExpressionType.Or:
				case ExpressionType.OrElse:
					sb.Append("||");
					break;
				case ExpressionType.Equal:
					sb.Append("=");
					break;
				case ExpressionType.NotEqual:
					sb.Append("!=");
					break;
				default:
					throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
			}
			Visit(b.Right);
			sb.Append(")");
			return b;
		}

		protected override Expression VisitMemberAccess(MemberExpression m) {
			if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter) {
				sb.Append(m.Member.Name.ToLowerInvariant());
				return m;
			}
			throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
		}

		protected override Expression VisitConstant(ConstantExpression c) {
			sb.Append(c.Value.ToString());
			return c;
		}
	}
}
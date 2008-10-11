using System.Linq.Expressions;

namespace GDataDB.Linq {
	public class QueryTranslator : ExpressionVisitor {
		private readonly Query q = new Query();

		public Query Translate(Expression e) {
			Visit(e);
			return q;
		}

		protected override Expression VisitMethodCall(MethodCallExpression m) {
			Visit(m.Arguments[0]);
			if (m.Method.Name == "Where") {
				q.StructuredQuery = new WhereTranslator().Translate(m);
			} else if (m.Method.Name == "OrderBy" || m.Method.Name == "OrderByDescending") {
				q.Order = new OrderTranslator().Translate(m);
			}
			return m;
		}
	}
}
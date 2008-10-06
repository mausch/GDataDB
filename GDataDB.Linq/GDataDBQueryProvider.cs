using System;
using System.Linq;
using System.Linq.Expressions;

namespace GDataDB.Linq {
	public class GDataDBQueryProvider<T> : QueryProvider {
		private readonly ITable<T> table;

		public GDataDBQueryProvider(ITable<T> table) {
			this.table = table;
		}

		public override string GetQueryText(Expression expression) {
			expression = Evaluator.PartialEval(expression);
			return new QueryTranslator().Translate(expression);
		}

		public override object Execute(Expression expression) {
			return table.FindStructured(GetQueryText(expression)).Select(r => r.Element);
		}
	}
}
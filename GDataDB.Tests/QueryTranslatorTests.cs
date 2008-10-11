using System.Linq;
using GDataDB.Linq;
using NUnit.Framework;

namespace GDataDB.Tests {
	[TestFixture]
	public class QueryTranslatorTests {
		private IQueryable<Entity> q;

		[TestFixtureSetUp]
		public void FixtureSetup() {
			var qt = new QueryTranslator();
			var t = new MockTable();
			q = new Query<Entity>(new GDataDBQueryProvider<Entity>(t));
		}

		[Test]
		public void QueryTranslator() {
			var iq = q.Where(e => e.Description == "pepe");
			Assert.AreEqual("(description=\"pepe\")", iq.ToString());
		}

		[Test]
		public void GreaterThanInt() {
			var iq = q.Where(e => e.Quantity > 5);
			Assert.AreEqual("(quantity>5)", iq.ToString());
		}

		[Test]
		public void LessThanInt() {
			var iq = q.Where(e => e.Quantity < 5);
			Assert.AreEqual("(quantity<5)", iq.ToString());
		}

		[Test]
		public void GreaterThanOrEqualInt() {
			var iq = q.Where(e => e.Quantity >= 5);
			Assert.AreEqual("(((quantity>5)||(quantity=5)))", iq.ToString());
		}

		[Test]
		public void LessThanOrEqualInt() {
			var iq = q.Where(e => e.Quantity <= 5);
			Assert.AreEqual("(((quantity<5)||(quantity=5)))", iq.ToString());
		}

		[Test]
		public void Equals() {
			var iq = q.Where(e => e.Quantity == 5);
			Assert.AreEqual("(quantity=5)", iq.ToString());
		}

		[Test]
		public void NotEquals() {
			var iq = q.Where(e => e.Quantity != 5);
			Assert.AreEqual("(quantity!=5)", iq.ToString());
		}

		[Test]
		public void Or() {
			var iq = q.Where(e => e.Quantity > 5 || e.Quantity < 5);
			Assert.AreEqual("((quantity>5)||(quantity<5))", iq.ToString());
		}

		[Test]
		public void And() {
			var iq = q.Where(e => e.Quantity > 5 && e.Quantity < 5);
			Assert.AreEqual("((quantity>5)&&(quantity<5))", iq.ToString());
		}
	}
}
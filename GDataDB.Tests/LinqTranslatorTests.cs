using System;
using System.Linq;
using GDataDB.Linq;
using NUnit.Framework;

namespace GDataDB.Tests {
	[TestFixture]
	public class LinqTranslatorTests {
		private IQueryable<Entity> q;

		[TestFixtureSetUp]
		public void FixtureSetup() {
			var t = new DummyTable();
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

		[Test]
		public void OrderBy() {
			var iq = (Query<Entity>) q.OrderBy(e => e.Quantity);
			var sq = iq.ToQuery();
			Assert.IsNotNull(sq.Order);
			Assert.IsFalse(sq.Order.Descending);
			Assert.AreEqual("quantity", sq.Order.ColumnName);
		}

		[Test]
		public void OrderByDescending() {
			var iq = (Query<Entity>) q.OrderByDescending(e => e.Quantity);
			var sq = iq.ToQuery();
			Assert.IsNotNull(sq.Order);
			Assert.IsTrue(sq.Order.Descending);
			Assert.AreEqual("quantity", sq.Order.ColumnName);
		}

		[Test]
		[ExpectedException(typeof (NotSupportedException))]
		public void OrderBy_With_comparer_is_not_supported() {
			var iq = (Query<Entity>) q.OrderBy(e => e.Quantity, new DummyComparer<int>());
			var sq = iq.ToQuery();
			Assert.IsNotNull(sq.Order);
			Assert.IsFalse(sq.Order.Descending);
			Assert.AreEqual("quantity", sq.Order.ColumnName);
		}

		[Test]
		public void Where_and_OrderBy() {
			var iq = (Query<Entity>) q
			                         	.Where(e => e.Quantity > 5)
			                         	.OrderBy(e => e.Quantity);
			var sq = iq.ToQuery();
			Assert.IsNotNull(sq.Order);
			Assert.IsNotNull(sq.StructuredQuery);
			Assert.AreEqual("(quantity>5)", sq.StructuredQuery);
			Assert.AreEqual("quantity", sq.Order.ColumnName);
		}

		[Test]
		public void Take() {
			var iq = (Query<Entity>) q
			                         	.Where(e => e.Quantity > 5)
			                         	.Take(5);
			var sq = iq.ToQuery();
			Assert.AreEqual(5, sq.Count);
		}

		[Test]
		public void Take_Expression() {
			var iq = (Query<Entity>)q
																.Where(e => e.Quantity > 5)
																.Take(5+5);
			var sq = iq.ToQuery();
			Assert.AreEqual(10, sq.Count);
			Assert.AreEqual("(quantity>5)", sq.StructuredQuery);			
		}
	}
}
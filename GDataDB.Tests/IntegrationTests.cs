using System;
using System.Linq;
using GDataDB.Linq;
using NUnit.Framework;

namespace GDataDB.Tests {
	[TestFixture]
	public class IntegrationTests {
		private ITable<IntegrationEntity> table;
	    private IDatabase db;

		private readonly IntegrationEntity e1 = new IntegrationEntity {
			DateTimeProp = new DateTime(2001, 1, 1, 5, 6, 7),
			IntProp = 1,
		};

		private readonly IntegrationEntity e2 = new IntegrationEntity {
			DateTimeProp = new DateTime(2005, 6, 7, 10, 6, 7),
			IntProp = 1000,
		};


		[TestFixtureSetUp]
		public void FixtureSetup() {
			Console.WriteLine("Connecting");
			var client = new DatabaseClient("you@gmail.com", "password");
			const string dbName = "IntegrationTests";
			Console.WriteLine("Opening or creating database");
			db = client.GetDatabase(dbName) ?? client.CreateDatabase(dbName);
			const string tableName = "IntegrationTests";
			Console.WriteLine("Opening or creating table");
			table = db.GetTable<IntegrationEntity>(tableName) ?? db.CreateTable<IntegrationEntity>(tableName);
		}

        [SetUp]
        public void setup() {
            table.Clear();
            table.Add(e1);
            table.Add(e2);
        }

        [TestFixtureTearDown]
        public void FixtureTearDown() {
            table.Delete();
            db.Delete();
        }

        [Test]
        public void Add() {
            table.Add(new IntegrationEntity {
                FloatProp = 123.45f,
                DecimalProp = 234.56m,
                DoubleProp = 333.44,
            });
        }

		[Test]
		public void LINQ_orderby_int() {
			var q = from r in table.AsQueryable()
			        orderby r.IntProp
			        select r;

			var l = q.ToList();
			Assert.AreEqual(2, l.Count);
			Assert.AreEqual(e1.IntProp, l[0].IntProp);
			Assert.AreEqual(e2.IntProp, l[1].IntProp);
		}

		[Test]
		public void LINQ_orderby_int_descending() {
			var q = from r in table.AsQueryable()
			        orderby r.IntProp descending
			        select r;

			var l = q.ToList();
			Assert.AreEqual(2, l.Count);
			Assert.AreEqual(e2.IntProp, l[0].IntProp);
			Assert.AreEqual(e1.IntProp, l[1].IntProp);
		}

		[Test]
		public void LINQ_orderby_datetime() {
			var q = from r in table.AsQueryable()
			        orderby r.DateTimeProp
			        select r;

			var l = q.ToList();
			Assert.AreEqual(2, l.Count);
			Assert.AreEqual(e1.DateTimeProp, l[0].DateTimeProp);
			Assert.AreEqual(e2.DateTimeProp, l[1].DateTimeProp);
		}

		[Test]
		public void LINQ_orderby_datetime_descending() {
			var q = from r in table.AsQueryable()
			        orderby r.DateTimeProp descending
			        select r;

			var l = q.ToList();
			Assert.AreEqual(2, l.Count);
			Assert.AreEqual(e2.DateTimeProp, l[0].DateTimeProp);
			Assert.AreEqual(e1.DateTimeProp, l[1].DateTimeProp);
		}

		[Test]
		public void LINQ_orderby_datetime_descending_take() {
			var q = table.AsQueryable()
				.OrderByDescending(r => r.DateTimeProp)
				.Take(1)
				.ToList();

			Assert.AreEqual(1, q.Count);
			Assert.AreEqual(e2.DateTimeProp, q[0].DateTimeProp);
		}

		[Test]
		public void LINQ_orderby_datetime_descending_take_skip() {
			var q = table.AsQueryable()
				.OrderByDescending(r => r.DateTimeProp)
				.Skip(1)
				.Take(1)
				.ToList();

			Assert.AreEqual(1, q.Count);
			Assert.AreEqual(e1.DateTimeProp, q[0].DateTimeProp);
		}
	}
}
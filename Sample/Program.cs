using System;
using System.IO;
using System.Linq;
using GDataDB;
using GDataDB.Linq;

namespace Sample {
	internal class Program {
		private static void Main(string[] args) {
			Console.WriteLine("Connecting");
			var client = new DatabaseClient("you@gmail.com", File.ReadAllBytes("key.p12"));
			const string dbName = "testing";
			Console.WriteLine("Opening or creating database");
			var db = client.GetDatabase(dbName) ?? client.CreateDatabase(dbName);
			const string tableName = "testtable";
			Console.WriteLine("Opening or creating table");
			var t = db.GetTable<Entity>(tableName) ?? db.CreateTable<Entity>(tableName);
			var all = t.FindAll();
			Console.WriteLine("{0} elements", all.Count);
			var r = all.Count > 0 ? all[0] : t.Add(new Entity {Conteudo = "some content", Amount = 5});
			Console.WriteLine("conteudo: {0}", r.Element.Conteudo);
			r.Element.Conteudo = "nothing at all";
			Console.WriteLine("updating row");
			r.Update();
			Console.WriteLine("Now there are {0} elements", t.FindAll().Count);
			{
				Console.WriteLine("executing a few queries");
				foreach (var q in new[] { "amount=5", "amount<6", "amount>0", "amount!=6", "amount<>6", "conteudo=\"nothing at all\"" }) {
					Console.Write("querying '{0}': ", q);
					var rows = t.FindStructured(q);
					Console.WriteLine("{0} elements found", rows.Count);
				}
			}
			{
				Console.WriteLine("Linq queries");
				var rows = from e in t.AsQueryable()
									 where e.Conteudo == r.Element.Conteudo
									 orderby e.Amount
									 select e;
				Console.WriteLine("{0} elements found", rows.ToList().Count());
			}
			Console.WriteLine("deleting row");
			r.Delete();
			Console.WriteLine("deleting table");
			t.Delete();
			Console.WriteLine("deleting database");
			db.Delete();
			Console.WriteLine("Press any key...");
			Console.ReadKey();
		}
	}
}

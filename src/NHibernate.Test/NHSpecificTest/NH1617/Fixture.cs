using System.Collections.Generic;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1617
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2005Dialect;
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = OpenSession())
			{
				using (ITransaction tran = session.BeginTransaction())
				{
					var newUser = new User();
					var newOrder1 = new Order();
					newOrder1.User = newUser;
					newOrder1.Status = true;
					var newOrder2 = new Order();
					newOrder2.User = newUser;
					newOrder2.Status = true;

					session.Save(newUser);
					session.Save(newOrder1);
					session.Save(newOrder2);
					tran.Commit();
				}
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = OpenSession())
			{
				using (ITransaction tran = session.BeginTransaction())
				{
					session.Delete("from Order");
					session.Delete("from User");
					tran.Commit();
				}
			}
		}

		[Test]
		public void CanUseDataTypeInFormulaWithCriteriaQuery()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tran = session.BeginTransaction())
				{
					string sql = "from User";
					IList<User> list = session.CreateQuery(sql).List<User>();
					Assert.That(list.Count, Is.EqualTo(1));
					Assert.That(list[0].OrderStatus, Is.EqualTo(2));
				}
			}
		}
	}
}
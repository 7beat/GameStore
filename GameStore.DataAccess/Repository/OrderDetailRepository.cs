using BookStore.DataAccess.Repository.IRepository;
using GameStore.DataAccess.Repository;
using GameStore.Models;
using GameStoreWeb.Data;

namespace BookStore.DataAccess.Repository
{
	public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
	{
		private ApplicationDbContext _db;

		public OrderDetailRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public void Update(OrderDetail obj)
		{
			_db.OrderDetails.Update(obj);
		}
	}
}

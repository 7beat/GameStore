using GameStore.DataAccess.Repository.IRepository;
using GameStore.Models;

namespace BookStore.DataAccess.Repository.IRepository
{
	public interface IOrderDetailRepository : IRepository<OrderDetail>
	{
		void Update(OrderDetail obj);
	}
}

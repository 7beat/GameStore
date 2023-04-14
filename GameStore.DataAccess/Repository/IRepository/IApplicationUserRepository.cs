using GameStore.DataAccess.Repository.IRepository;
using GameStore.Models.Identity;

namespace BookStore.DataAccess.Repository.IRepository
{
	public interface IApplicationUserRepository : IRepository<ApplicationUser>
	{
	}
}

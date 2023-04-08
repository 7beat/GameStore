using BookStore.DataAccess.Repository.IRepository;
using GameStore.DataAccess.Repository;
using GameStore.Models.Identity;
using GameStoreWeb.Data;

namespace BookStore.DataAccess.Repository
{
	public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
	{
		private ApplicationDbContext _db;

		public ApplicationUserRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

	}
}

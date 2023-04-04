using GameStore.DataAccess.Repository.IRepository;
using GameStore.Models;
using GameStoreWeb.Data;

namespace GameStore.DataAccess.Repository
{
    public class PlatformRepository : Repository<Platform>, IPlatformRepository
    {
        private ApplicationDbContext _db;

        public PlatformRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Platform obj)
        {
            _db.Platforms.Update(obj);
        }
    }
}

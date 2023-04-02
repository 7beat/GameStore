using GameStore.Models;

namespace GameStore.DataAccess.Repository.IRepository
{
    public interface IPlatformRepository : IRepository<Platform>
    {
        void Update(Platform obj);
    }
}

using GameStore.DataAccess.Repository.IRepository;
using GameStore.Models;
using GameStoreWeb.Data;
using System.Linq.Expressions;

namespace GameStore.DataAccess.Repository
{
    public class GenreRepository : Repository<Genre>, IGenreRepository
    {
        private ApplicationDbContext _db;

        public GenreRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Genre obj)
        {
            _db.Genres.Update(obj);
        }
    }
}

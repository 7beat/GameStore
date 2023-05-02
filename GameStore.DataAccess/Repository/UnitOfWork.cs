using BookStore.DataAccess.Repository;
using BookStore.DataAccess.Repository.IRepository;
using GameStore.DataAccess.Repository.IRepository;
using GameStoreWeb.Data;

namespace GameStore.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _disposed = false;
        private readonly ApplicationDbContext _dbContext;

        private IPlatformRepository? _platformRepository;
        private IGenreRepository? _genreRepository;
        private IProductRepository? _productRepository;
        private IShoppingCartRepository? _shoppingCartRepository;
        private IApplicationUserRepository? _applicationUserRepository;
        private IOrderHeaderRepository? _orderHeaderRepository;
        private IOrderDetailRepository? _orderDetailRepository;

        public IPlatformRepository Platform => _platformRepository ??= new PlatformRepository(_dbContext);
        public IGenreRepository Genre => _genreRepository ??= new GenreRepository(_dbContext);
        public IProductRepository Product => _productRepository ??= new ProductRepository(_dbContext);
        public IShoppingCartRepository ShoppingCart => _shoppingCartRepository ??= new ShoppingCartRepository(_dbContext);
        public IApplicationUserRepository ApplicationUser => _applicationUserRepository ??= new ApplicationUserRepository(_dbContext);
        public IOrderHeaderRepository OrderHeader => _orderHeaderRepository ??= new OrderHeaderRepository(_dbContext);
        public IOrderDetailRepository OrderDetail => _orderDetailRepository ??= new OrderDetailRepository(_dbContext);

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}

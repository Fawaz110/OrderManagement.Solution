using Core.Entities;
using Core.Repositories.Contract;
using Core.UnitsOfWork;
using OrderManagement.DbContexts;
using System.Collections;
using System.Security.Principal;

namespace Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreDbContext _context;
        private Hashtable _repositories;

        public UnitOfWork(StoreDbContext context)
        {
            _context = context;
            _repositories = new Hashtable();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var key = typeof(TEntity).Name;

            if(!_repositories.ContainsKey(key) )
            {
                var _repo = new GenericRepository<TEntity>(_context);

                _repositories.Add(key, _repo);
            }

            return _repositories[key] as IGenericRepository<TEntity>;
        }

        public async Task<int> CompleteAsync()
            => await _context.SaveChangesAsync();

        public async ValueTask DisposeAsync()
            => await _context.DisposeAsync();

    }
}

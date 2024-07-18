using Core.Entities;
using Core.Repositories.Contract;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;
using OrderManagement.DbContexts;

namespace Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly StoreDbContext _context;

        public GenericRepository(
            StoreDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(TEntity entity)
            => await _context.Set<TEntity>().AddAsync(entity);

        public void Delete(TEntity entity)
            => _context.Set<TEntity>().Remove(entity);

        public async Task<IEnumerable<TEntity>> GetAllAsync()
            => await _context.Set<TEntity>().ToListAsync();

        public async Task<IEnumerable<TEntity>> GetAllWithSpecAsync(ISpecifications<TEntity> spec)
            => await ApplySpecifications(spec).ToListAsync();

        public async Task<TEntity> GetByIdAsync(int id)
            => await _context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);

        public async Task<TEntity> GetWithSpecAsync(ISpecifications<TEntity> spec)
            => await ApplySpecifications(spec).FirstOrDefaultAsync();

        public void Update(TEntity entity)
            => _context.Update<TEntity>(entity);

        private IQueryable<TEntity> ApplySpecifications(ISpecifications<TEntity> spec)
            => SpecificationsEvaluator<TEntity>.GetQuery(_context.Set<TEntity>(), spec);
    }
}

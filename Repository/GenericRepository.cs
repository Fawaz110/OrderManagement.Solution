using Core.Entities;
using Core.Repositories.Contract;
using Microsoft.EntityFrameworkCore;
using OrderManagement.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<int> CompleteAsync()
            => await _context.SaveChangesAsync();

        public void Delete(TEntity entity)
            => _context.Set<TEntity>().Remove(entity);

        public async Task<IEnumerable<TEntity>> GetAllAsync()
            => await _context.Set<TEntity>().ToListAsync();

        public async Task<TEntity> GetByIdAsync(int id)
            => await _context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);

        public void Update(TEntity entity)
            => _context.Update<TEntity>(entity);
    }
}

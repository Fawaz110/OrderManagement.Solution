using Core.Entities;
using Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.Contract
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetAllWithSpecAsync(ISpecifications<TEntity> spec);
        Task<TEntity> GetByIdAsync(int id);
        Task<TEntity> GetWithSpecAsync(ISpecifications<TEntity> spec);
        Task<int> CompleteAsync();

    }
}

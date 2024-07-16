using Core.Entities;
using System.Linq.Expressions;

namespace Core.Specifications
{
    public interface ISpecifications<TEntity> where TEntity : BaseEntity
    {
        public Expression<Func<TEntity, bool>> Criteria { get; set; }
        public List<Expression<Func<TEntity, object>>> Includes { get; set; }
    }
}

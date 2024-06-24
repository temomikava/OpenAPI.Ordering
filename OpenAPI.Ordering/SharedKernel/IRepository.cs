
using System.Linq.Expressions;

namespace SharedKernel
{
    public interface IRepository<TEntity, in TId> where TEntity : class
    {
        Task<TEntity> CreateAsync(TEntity entity);
        Task<TEntity?> GetByIdAsync(TId id);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate,CancellationToken token);
        Task<List<TEntity>> GetAllAsync(CancellationToken token);
    }
}

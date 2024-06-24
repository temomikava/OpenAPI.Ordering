using Microsoft.EntityFrameworkCore;
using SharedKernel;
using System;
using System.Linq.Expressions;

namespace OpenAPI.Identity.Data
{
    public class Repository<TEntity, TId> : IRepository<TEntity,TId> where TEntity : class
    {
        private readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken token)
        {
            return await _context.Set<TEntity>().AnyAsync(predicate, token);
        }

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<List<TEntity>> GetAllAsync(CancellationToken token)
        {
            return await _context.Set<TEntity>().ToListAsync(token);
        }

        public async Task<TEntity?> GetByIdAsync(TId id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }
    }
}

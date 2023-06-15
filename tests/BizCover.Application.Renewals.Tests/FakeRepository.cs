using System.Linq.Expressions;
using BizCover.Framework.Application.Interfaces;
using BizCover.Framework.Application.Sort;

namespace BizCover.Application.Renewals.Tests
{
    public class FakeRepository<TEntity> : IRepository<TEntity> where TEntity : Framework.Domain.Entity
    {
        public IEnumerable<TEntity> Entities = new List<TEntity>();

        public Task AddManyAsync(IEnumerable<TEntity> entities, CancellationToken token = new())
        {
            throw new NotImplementedException();
        }

        public Task AddOneAsync(TEntity entity, CancellationToken token = new())
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Guid id, TEntity entity, CancellationToken token = new())
        {
            throw new NotImplementedException();
        }

        public Task UpsertAsync(Guid id, TEntity entity, CancellationToken token = new())
        {
            throw new NotImplementedException();
        }

        public Task<long> CountAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token = new())
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Expression<Func<TEntity, bool>> filter, CancellationToken token = new())
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter, List<Sort<TEntity>> sort = null, int? skip = null, int? take = null,
            CancellationToken token = new())
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter,
            CancellationToken token = new()) => await Task.Run(() => Entities.Where(filter.Compile()), token);

        public Task<TEntity> GetByIdAsync(Guid id, CancellationToken token = new())
        {
            throw new NotImplementedException();
        }
    }
}

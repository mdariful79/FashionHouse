using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FashionHouse.Domain.Contracts;

namespace FashionHouse.Infrastructure.Data
{
    public abstract class Repository<TAggregateRoot, TKey> : IRepository<TAggregateRoot, TKey>, IDisposable
        where TAggregateRoot : class, IAggregateRoot<TKey>
        where TKey : IComparable
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<TAggregateRoot> _dbSet;

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TAggregateRoot>();
        }

        public void Add(TAggregateRoot entity)
        {
            _dbSet.Add(entity);
        }

        public async Task AddAsync(TAggregateRoot entity, CancellationToken cancellationToken)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public void Edit(TAggregateRoot entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _dbContext.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public Task EditAsync(TAggregateRoot entityToUpdate, CancellationToken cancellationToken)
        {
            Edit(entityToUpdate);
            return Task.CompletedTask;
        }

        public void Update(TAggregateRoot entity)
        {
            _dbSet.Update(entity);
        }

        public IList<TAggregateRoot> GetAll()
        {
            return _dbSet.ToList();
        }

        public async Task<IList<TAggregateRoot>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        public TAggregateRoot GetById(TKey id)
        {
            return _dbSet.Find(id)!;
        }

        public async Task<TAggregateRoot?> GetByIdAsync(TKey id, CancellationToken cancellationToken)
        {
            return await _dbSet.FindAsync(new object?[] { id }, cancellationToken);
        }

        public int GetCount(Expression<Func<TAggregateRoot, bool>>? filter = null)
        {
            return filter is null ? _dbSet.Count() : _dbSet.Count(filter);
        }

        public async Task<int> GetCountAsync(Expression<Func<TAggregateRoot, bool>>? filter = null, CancellationToken cancellationToken = default)
        {
            return filter is null
                ? await _dbSet.CountAsync(cancellationToken)
                : await _dbSet.CountAsync(filter, cancellationToken);
        }

        public void Remove(TAggregateRoot entityToDelete)
        {
            if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public void Remove(TKey id)
        {
            var entity = GetById(id);
            if (entity is not null)
            {
                Remove(entity);
            }
        }

        public void Remove(Expression<Func<TAggregateRoot, bool>> filter)
        {
            var entities = _dbSet.Where(filter);
            _dbSet.RemoveRange(entities);
        }

        public async Task RemoveAsync(TAggregateRoot entityToDelete, CancellationToken cancellationToken)
        {
            Remove(entityToDelete);
            await Task.CompletedTask;
        }

        public async Task RemoveAsync(TKey id, CancellationToken cancellationToken)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity is not null)
            {
                Remove(entity);
            }
        }

        public async Task RemoveAsync(Expression<Func<TAggregateRoot, bool>>? filter, CancellationToken cancellationToken)
        {
            if (filter is not null)
            {
                var entities = await _dbSet.Where(filter).ToListAsync(cancellationToken);
                _dbSet.RemoveRange(entities);
            }
        }
        // Used by CategoryRepository/ProductRepository for DataTables-style dynamic paging
        protected virtual async Task<(IList<TAggregateRoot> data, int total, int totalDisplay)> GetDynamicAsync(
            Expression<Func<TAggregateRoot, bool>>? filter = null,
            string? orderBy = null,
            Func<IQueryable<TAggregateRoot>, IQueryable<TAggregateRoot>>? include = null,
            int pageIndex = 1,
            int pageSize = 10,
            bool isTrackingOff = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TAggregateRoot> query = _dbSet;
            var total = query.Count();
            var totalDisplay = total;

            if (filter is not null)
            {
                query = query.Where(filter);
                totalDisplay = query.Count();
            }

            if (include is not null)
                query = include(query);

            var paged = string.IsNullOrWhiteSpace(orderBy)
                ? query.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                : query.OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var data = isTrackingOff
                ? await paged.AsNoTracking().ToListAsync(cancellationToken)
                : await paged.ToListAsync(cancellationToken);

            return (data, total, totalDisplay);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
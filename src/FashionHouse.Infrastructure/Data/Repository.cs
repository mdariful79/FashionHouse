using FashionHouse.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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

        public async Task AddAsync(TAggregateRoot entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}

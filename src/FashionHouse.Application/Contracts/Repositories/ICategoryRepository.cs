using FashionHouse.Domain.Contracts;
using FashionHouse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Application.Contracts.Repositories
{
    public interface ICategoryRepository : IRepository<Category, Guid>
    {
        Task<bool> IsDuplicateCategoryName(string name, Guid? id, CancellationToken cancellationToken);
    }
}

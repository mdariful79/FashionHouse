using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Application.Features.Inventories.Query
{
    public class GetAllInventoriesByPagingQueryHandler
        : IQueryHandler<GetAllInventoriesByPagingQuery, (IList<Inventory>, int, int)>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public GetAllInventoriesByPagingQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(IList<Inventory>, int, int)> Handle(GetAllInventoriesByPagingQuery query,
            CancellationToken cancellationToken)
        {
            return await _unitOfWork.InventoryRepository.GetPagedInventoriesAsync(query, cancellationToken);
        }
    }
}

using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Application.Features.Inventories.Query
{
    public class GetInventoryByProductIdQueryHandler : IQueryHandler<GetInventoryByProductIdQuery, Inventory?>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public GetInventoryByProductIdQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Inventory?> Handle(GetInventoryByProductIdQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.InventoryRepository.GetByProductIdAsync(query.ProductId, cancellationToken);
        }
    }
}

using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Inventories.Query
{
    public class GetInventoryByIdQueryHandler : IQueryHandler<GetInventoryByIdQuery, Inventory?>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public GetInventoryByIdQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Inventory?> Handle(GetInventoryByIdQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.InventoryRepository.GetByIdAsync(query.Id, cancellationToken);
        }
    }
}
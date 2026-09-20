using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Orders.Query
{
    public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, Order?>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public GetOrderByIdQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Order?> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.OrderRepository.GetByIdWithItemsAsync(query.Id, cancellationToken);
        }
    }
}
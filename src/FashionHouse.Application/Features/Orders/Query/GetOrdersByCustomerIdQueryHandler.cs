using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Orders.Query
{
    public class GetOrdersByCustomerIdQueryHandler : IQueryHandler<GetOrdersByCustomerIdQuery, IList<Order>>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public GetOrdersByCustomerIdQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<Order>> Handle(GetOrdersByCustomerIdQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.OrderRepository.GetByCustomerIdAsync(query.CustomerId, cancellationToken);
        }
    }
}
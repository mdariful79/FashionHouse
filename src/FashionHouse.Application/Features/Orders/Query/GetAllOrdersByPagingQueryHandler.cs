using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Orders.Query
{
    public class GetAllOrdersByPagingQueryHandler : IQueryHandler<GetAllOrdersByPagingQuery, (IList<Order>, int, int)>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public GetAllOrdersByPagingQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(IList<Order>, int, int)> Handle(GetAllOrdersByPagingQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.OrderRepository.GetPagedOrdersAsync(query, cancellationToken);
        }
    }
}
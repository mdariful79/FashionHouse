using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;

namespace FashionHouse.Application.Features.Carts.Query
{
    public class GetCartByCustomerIdQueryHandler : IQueryHandler<GetCartByCustomerIdQuery, Domain.Entities.Cart>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public GetCartByCustomerIdQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Domain.Entities.Cart> Handle(GetCartByCustomerIdQuery query, CancellationToken cancellationToken)
        {
            var cart = await _unitOfWork.CartRepository.GetOrCreateForCustomerAsync(query.CustomerId, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            return cart;
        }
    }
}
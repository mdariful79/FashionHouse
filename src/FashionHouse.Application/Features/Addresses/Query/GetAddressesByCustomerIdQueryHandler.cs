using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Addresses.Query
{
    public class GetAddressesByCustomerIdQueryHandler : IQueryHandler<GetAddressesByCustomerIdQuery, IList<Address>>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public GetAddressesByCustomerIdQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<Address>> Handle(GetAddressesByCustomerIdQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.AddressRepository.GetByCustomerIdAsync(query.CustomerId, cancellationToken);
        }
    }
}
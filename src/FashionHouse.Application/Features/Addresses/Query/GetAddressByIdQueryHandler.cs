using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Addresses.Query
{
    public class GetAddressByIdQueryHandler : IQueryHandler<GetAddressByIdQuery, Address?>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public GetAddressByIdQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Address?> Handle(GetAddressByIdQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.AddressRepository.GetByIdAsync(query.Id, cancellationToken);
        }
    }
}
using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;
using FashionHouse.Domain.Entities;

namespace FashionHouse.Application.Features.Customers.Command
{
    public class CustomerAddCommandHandler : ICommandHandler<CustomerAddCommand, Customer>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public CustomerAddCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Customer> Handle(CustomerAddCommand command, CancellationToken cancellationToken)
        {
            var customer = new Customer
            {
                Id = command.Id,
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                Phone = command.Phone
            };

            await _unitOfWork.CustomerRepository.AddAsync(customer, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);

            return customer;
        }
    }
}
using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;

namespace FashionHouse.Application.Features.Categories.Command
{
    public class CategoryDeleteCommandHandler : ICommandHandler<CategoryDeleteCommand>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public CategoryDeleteCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CategoryDeleteCommand command, CancellationToken cancellationToken)
        {
            await _unitOfWork.CategoryRepository.RemoveAsync(command.Id, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
        }
    }
}
using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;

namespace FashionHouse.Application.Features.SubCategories.Command
{
    public class SubCategoryDeleteCommandHandler : ICommandHandler<SubCategoryDeleteCommand>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public SubCategoryDeleteCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(SubCategoryDeleteCommand command, CancellationToken cancellationToken)
        {
            await _unitOfWork.SubCategoryRepository.RemoveAsync(command.Id, cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
        }
    }
}
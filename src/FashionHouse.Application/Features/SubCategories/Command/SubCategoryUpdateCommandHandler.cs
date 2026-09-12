using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;
using FashionHouse.Application.Exceptions;
using FashionHouse.Domain.Entities;
using MapsterMapper;

namespace FashionHouse.Application.Features.SubCategories.Command
{
    public class SubCategoryUpdateCommandHandler : ICommandHandler<SubCategoryUpdateCommand, SubCategory>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubCategoryUpdateCommandHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SubCategory> Handle(SubCategoryUpdateCommand command, CancellationToken cancellationToken)
        {
            var isDuplicateName = await _unitOfWork.SubCategoryRepository.IsDuplicateSubCategoryName(
                command.Name, command.CategoryId, command.Id, cancellationToken);

            if (!isDuplicateName)
            {
                var subCategory = _unitOfWork.SubCategoryRepository.GetById(command.Id);
                subCategory = _mapper.Map(command, subCategory);

                await _unitOfWork.SubCategoryRepository.EditAsync(subCategory, cancellationToken);
                await _unitOfWork.SaveAsync(cancellationToken);

                return subCategory;
            }
            else
                throw new DuplicateDataException("Sub category name is duplicate for this category");
        }
    }
}
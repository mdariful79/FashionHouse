using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;
using FashionHouse.Application.Exceptions;
using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Utilities;
using MapsterMapper;

namespace FashionHouse.Application.Features.SubCategories.Command
{
    public class SubCategoryAddCommandHandler : ICommandHandler<SubCategoryAddCommand, SubCategory>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubCategoryAddCommandHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SubCategory> Handle(SubCategoryAddCommand command, CancellationToken cancellationToken)
        {
            var isDuplicateName = await _unitOfWork.SubCategoryRepository.IsDuplicateSubCategoryName(
                command.Name, command.CategoryId, null, cancellationToken);

            if (!isDuplicateName)
            {
                var subCategory = _mapper.Map<SubCategory>(command);
                subCategory.Id = IdentityGenerator.NewSequentialGuid();

                await _unitOfWork.SubCategoryRepository.AddAsync(subCategory, cancellationToken);
                await _unitOfWork.SaveAsync(cancellationToken);

                return subCategory;
            }
            else
                throw new DuplicateDataException("Sub category name is duplicate for this category");
        }
    }
}
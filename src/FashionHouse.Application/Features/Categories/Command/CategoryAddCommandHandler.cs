using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;
using FashionHouse.Application.Exceptions;
using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Utilities;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Application.Features.Categories.Command
{
    public class CategoryAddCommandHandler : ICommandHandler<CategoryAddCommand, Category>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryAddCommandHandler(IApplicationUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Category> Handle(CategoryAddCommand command, CancellationToken cancellationToken)
        {
            var isDuplicateName = await _unitOfWork.CategoryRepository.IsDuplicateCategoryName(command.Name, null,
                cancellationToken);

            if (!isDuplicateName)
            {
                var category = _mapper.Map<Category>(command);
                category.Id = IdentityGenerator.NewSequentialGuid();

                await _unitOfWork.CategoryRepository.AddAsync(category, cancellationToken);
                await _unitOfWork.SaveAsync(cancellationToken);

                return category;
            }
            else
                throw new DuplicateDataException("Category name is duplicate");
        }
    }
}

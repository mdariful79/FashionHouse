using FashionHouse.Application.Exceptions;
using FashionHouse.Application.Features.SubCategories.Command;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.SubCategories.Commands
{
    public class SubCategoryAddCommandHandlerTests : HandlerTestBase<SubCategoryAddCommandHandler>
    {
        [Test]
        public async Task Handle_UniqueName_AddsSubCategory()
        {
            // Arrange
            var command = new SubCategoryAddCommand
            {
                CategoryId = Guid.NewGuid(),
                Name = "Men",
                Slug = "men",
                Description = "Men's clothing",
                IsActive = true
            };

            var entity = new SubCategory
            {
                CategoryId = Guid.NewGuid(),
                Name = command.Name,
                Slug = command.Slug,
                Description = command.Description,
                IsActive = command.IsActive
            };

            _subCategoryRepository
                .Setup(x => x.IsDuplicateSubCategoryName(command.Name, command.CategoryId, null, _token))
                .ReturnsAsync(false);

            _mapper.Setup(x => x.Map<SubCategory>(command)).Returns(entity);

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.ShouldBeSameAs(entity),
                () => result.Id.ShouldNotBe(Guid.Empty),
                () => result.Name.ShouldBe(command.Name),
                () => result.Slug.ShouldBe(command.Slug),
                () => result.Description.ShouldBe(command.Description),
                () => result.IsActive.ShouldBe(command.IsActive));

            _subCategoryRepository.Verify(x => x.AddAsync(entity, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_DuplicateName_ThrowsDuplicateDataException()
        {
            // Arrange
            var command = new SubCategoryAddCommand
            {
                CategoryId = Guid.NewGuid(),
                Name = "Men",
                Slug = "men"
            };

            _subCategoryRepository
                .Setup(x => x.IsDuplicateSubCategoryName(command.Name, command.CategoryId, null, _token))
                .ReturnsAsync(true);

            // Act
            var ex = await Should.ThrowAsync<DuplicateDataException>(
                async () => await _handler.Handle(command, _token));

            // Assert
            ex.Message.ShouldBe("Sub category name is duplicate for this category");

            _mapper.Verify(x => x.Map<SubCategory>(It.IsAny<object>()), Times.Never());
            _subCategoryRepository.Verify(x => x.AddAsync(It.IsAny<SubCategory>(), It.IsAny<CancellationToken>()), Times.Never());
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}

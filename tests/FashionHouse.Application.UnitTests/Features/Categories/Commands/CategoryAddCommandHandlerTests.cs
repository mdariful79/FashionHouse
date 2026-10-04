using FashionHouse.Application.Exceptions;
using FashionHouse.Application.Features.Categories.Command;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Categories.Commands
{
    public class CategoryAddCommandHandlerTests : HandlerTestBase<CategoryAddCommandHandler>
    {
        [Test]
        public async Task Handle_UniqueName_AddsCategory()
        {
            // Arrange
            var command = new CategoryAddCommand
            {

                Name = "Men",
                Slug = "men",
                Description = "Men's clothing",
                IsActive = true
            };

            var entity = new Category
            {

                Name = command.Name,
                Slug = command.Slug,
                Description = command.Description,
                IsActive = command.IsActive
            };

            _categoryRepository
                .Setup(x => x.IsDuplicateCategoryName(command.Name, null, _token))
                .ReturnsAsync(false);

            _mapper.Setup(x => x.Map<Category>(command)).Returns(entity);

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

            _categoryRepository.Verify(x => x.AddAsync(entity, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_DuplicateName_ThrowsDuplicateDataException()
        {
            // Arrange
            var command = new CategoryAddCommand
            {

                Name = "Men",
                Slug = "men"
            };

            _categoryRepository
                .Setup(x => x.IsDuplicateCategoryName(command.Name, null, _token))
                .ReturnsAsync(true);

            // Act
            var ex = await Should.ThrowAsync<DuplicateDataException>(
                async () => await _handler.Handle(command, _token));

            // Assert
            ex.Message.ShouldBe("Category name is duplicate");

            _mapper.Verify(x => x.Map<Category>(It.IsAny<object>()), Times.Never());
            _categoryRepository.Verify(x => x.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never());
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}

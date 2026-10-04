using FashionHouse.Application.Exceptions;
using FashionHouse.Application.Features.Categories.Command;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Categories.Commands
{
    public class CategoryUpdateCommandHandlerTests : HandlerTestBase<CategoryUpdateCommandHandler>
    {
        [Test]
        public async Task Handle_UniqueName_UpdatesCategory()
        {
            // Arrange
            var command = new CategoryUpdateCommand
            {
                Id = Guid.NewGuid(),

                Name = "Women",
                Slug = "women",
                Description = "Women's clothing",
                IsActive = false
            };

            var existing = new Category { Id = command.Id, Name = "Old name" };
            var updated = new Category
            {
                Id = command.Id,

                Name = command.Name,
                Slug = command.Slug,
                Description = command.Description,
                IsActive = command.IsActive
            };

            _categoryRepository
                .Setup(x => x.IsDuplicateCategoryName(command.Name, command.Id, _token))
                .ReturnsAsync(false);
            _categoryRepository.Setup(x => x.GetById(command.Id)).Returns(existing);
            _mapper.Setup(x => x.Map(command, existing)).Returns(updated);

            // Act
            var result = await _handler.Handle(command, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.ShouldBeSameAs(updated),
                () => result.Name.ShouldBe(command.Name),
                () => result.Slug.ShouldBe(command.Slug),
                () => result.Description.ShouldBe(command.Description),
                () => result.IsActive.ShouldBeFalse());

            _categoryRepository.Verify(x => x.EditAsync(updated, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_DuplicateName_ThrowsDuplicateDataException()
        {
            // Arrange
            var command = new CategoryUpdateCommand
            {
                Id = Guid.NewGuid(),

                Name = "Women"
            };

            _categoryRepository
                .Setup(x => x.IsDuplicateCategoryName(command.Name, command.Id, _token))
                .ReturnsAsync(true);

            // Act
            var ex = await Should.ThrowAsync<DuplicateDataException>(
                async () => await _handler.Handle(command, _token));

            // Assert
            ex.Message.ShouldBe("Category name is duplicate");

            _categoryRepository.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never());
            _categoryRepository.Verify(x => x.EditAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never());
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}

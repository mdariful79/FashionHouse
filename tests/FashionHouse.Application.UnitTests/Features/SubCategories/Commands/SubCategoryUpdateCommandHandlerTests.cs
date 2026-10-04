using FashionHouse.Application.Exceptions;
using FashionHouse.Application.Features.SubCategories.Command;
using FashionHouse.Domain.Entities;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.SubCategories.Commands
{
    public class SubCategoryUpdateCommandHandlerTests : HandlerTestBase<SubCategoryUpdateCommandHandler>
    {
        [Test]
        public async Task Handle_UniqueName_UpdatesSubCategory()
        {
            // Arrange
            var command = new SubCategoryUpdateCommand
            {
                Id = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                Name = "Women",
                Slug = "women",
                Description = "Women's clothing",
                IsActive = false
            };

            var existing = new SubCategory { Id = command.Id, Name = "Old name" };
            var updated = new SubCategory
            {
                Id = command.Id,
                CategoryId = Guid.NewGuid(),
                Name = command.Name,
                Slug = command.Slug,
                Description = command.Description,
                IsActive = command.IsActive
            };

            _subCategoryRepository
                .Setup(x => x.IsDuplicateSubCategoryName(command.Name, command.CategoryId, command.Id, _token))
                .ReturnsAsync(false);
            _subCategoryRepository.Setup(x => x.GetById(command.Id)).Returns(existing);
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

            _subCategoryRepository.Verify(x => x.EditAsync(updated, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_DuplicateName_ThrowsDuplicateDataException()
        {
            // Arrange
            var command = new SubCategoryUpdateCommand
            {
                Id = Guid.NewGuid(),
                CategoryId = Guid.NewGuid(),
                Name = "Women"
            };

            _subCategoryRepository
                .Setup(x => x.IsDuplicateSubCategoryName(command.Name, command.CategoryId, command.Id, _token))
                .ReturnsAsync(true);

            // Act
            var ex = await Should.ThrowAsync<DuplicateDataException>(
                async () => await _handler.Handle(command, _token));

            // Assert
            ex.Message.ShouldBe("Sub category name is duplicate for this category");

            _subCategoryRepository.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never());
            _subCategoryRepository.Verify(x => x.EditAsync(It.IsAny<SubCategory>(), It.IsAny<CancellationToken>()), Times.Never());
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}

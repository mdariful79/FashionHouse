using FashionHouse.Application.Exceptions;
using FashionHouse.Application.Features.Orders.Command;
using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Enums;
using Moq;
using Shouldly;

namespace FashionHouse.Application.UnitTests.Features.Orders.Commands
{
    public class CheckoutCommandHandlerTests : HandlerTestBase<CheckoutCommandHandler>
    {
        private Guid _customerId;
        private Guid _addressId;
        private CheckoutCommand _command = null!;
        private Customer _customer = null!;
        private Address _address = null!;

        [SetUp]
        public void Setup()
        {
            _customerId = Guid.NewGuid();
            _addressId = Guid.NewGuid();

            _command = new CheckoutCommand
            {
                CustomerId = _customerId,
                AddressId = _addressId,
                PaymentMethod = "Cash on Delivery",
                Notes = "Leave at the door",
                ShippingFee = 10m,
                Discount = 20m
            };

            _customer = new Customer
            {
                Id = _customerId,
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com"
            };

            _address = new Address
            {
                Id = _addressId,
                CustomerId = _customerId,
                FullName = "Jane Doe",
                Phone = "01700000000",
                FullAddress = "House 1, Road 2, Dhanmondi",
                District = "Dhaka",
                PostalCode = "1209"
            };

            _addressRepository.Setup(x => x.GetByIdAsync(_addressId, _token)).ReturnsAsync(_address);
            _customerRepository.Setup(x => x.GetByIdAsync(_customerId, _token)).ReturnsAsync(_customer);
        }

        // ---------- helpers ----------

        private Cart CreateCart() => new() { Id = Guid.NewGuid(), CustomerId = _customerId };

        private void SetupCart(Cart? cart) =>
            _cartRepository.Setup(x => x.GetByCustomerIdAsync(_customerId, _token)).ReturnsAsync(cart);

        private static Product CreateProduct(string name, string sku, decimal price, decimal? discountedPrice = null) => new()
        {
            Id = Guid.NewGuid(),
            ProductName = name,
            SKU = sku,
            Price = price,
            DiscountedPrice = discountedPrice
        };

        private static void AddItem(Cart cart, Product product, int quantity) =>
            cart.CartItems.Add(new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                ProductId = product.Id,
                Product = product,
                Quantity = quantity
            });

        private Inventory SetupInventory(Guid productId, int quantity)
        {
            var inventory = new Inventory { Id = Guid.NewGuid(), ProductId = productId, Quantity = quantity };
            _inventoryRepository.Setup(x => x.GetByProductIdAsync(productId, _token)).ReturnsAsync(inventory);
            return inventory;
        }

        private (Cart cart, Product shirt, Product pants, Inventory shirtStock, Inventory pantsStock) SetupValidCheckout()
        {
            var cart = CreateCart();
            var shirt = CreateProduct("Shirt", "SHT-1", price: 100m, discountedPrice: 80m);
            var pants = CreateProduct("Pants", "PNT-1", price: 50m);
            AddItem(cart, shirt, 2);
            AddItem(cart, pants, 1);

            var shirtStock = SetupInventory(shirt.Id, 10);
            var pantsStock = SetupInventory(pants.Id, 1);
            SetupCart(cart);

            return (cart, shirt, pants, shirtStock, pantsStock);
        }

        // ---------- validation failures ----------

        [Test]
        public async Task Handle_CartDoesNotExist_ThrowsCartIsEmpty()
        {
            // Arrange
            SetupCart(null);

            // Act
            var ex = await Should.ThrowAsync<Exception>(async () => await _handler.Handle(_command, _token));

            // Assert
            ex.Message.ShouldBe("Your cart is empty.");
            _orderRepository.Verify(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never());
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task Handle_CartHasNoItems_ThrowsCartIsEmpty()
        {
            // Arrange
            SetupCart(CreateCart());

            // Act
            var ex = await Should.ThrowAsync<Exception>(async () => await _handler.Handle(_command, _token));

            // Assert
            ex.Message.ShouldBe("Your cart is empty.");
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task Handle_AddressNotFound_ThrowsInvalidShippingAddress()
        {
            // Arrange
            var (cart, _, _, _, _) = SetupValidCheckout();
            _addressRepository.Setup(x => x.GetByIdAsync(_addressId, _token)).ReturnsAsync((Address?)null);

            // Act
            var ex = await Should.ThrowAsync<Exception>(async () => await _handler.Handle(_command, _token));

            // Assert
            ex.Message.ShouldBe("Invalid shipping address.");
            cart.CartItems.Count.ShouldBe(2);
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task Handle_AddressBelongsToAnotherCustomer_ThrowsInvalidShippingAddress()
        {
            // Arrange
            SetupValidCheckout();
            _address.CustomerId = Guid.NewGuid();

            // Act
            var ex = await Should.ThrowAsync<Exception>(async () => await _handler.Handle(_command, _token));

            // Assert
            ex.Message.ShouldBe("Invalid shipping address.");
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task Handle_CustomerNotFound_ThrowsCustomerNotFound()
        {
            // Arrange
            SetupValidCheckout();
            _customerRepository.Setup(x => x.GetByIdAsync(_customerId, _token)).ReturnsAsync((Customer?)null);

            // Act
            var ex = await Should.ThrowAsync<Exception>(async () => await _handler.Handle(_command, _token));

            // Assert
            ex.Message.ShouldBe("Customer not found.");
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task Handle_QuantityExceedsStock_ThrowsInsufficientStockException()
        {
            // Arrange
            var cart = CreateCart();
            var shirt = CreateProduct("Blue Shirt", "SHT-1", 100m);
            AddItem(cart, shirt, 3);
            var stock = SetupInventory(shirt.Id, 1);
            SetupCart(cart);

            // Act
            var ex = await Should.ThrowAsync<InsufficientStockException>(
                async () => await _handler.Handle(_command, _token));

            // Assert
            ex.Message.ShouldBe("'Blue Shirt' only has 1 units left in stock.");
            stock.Quantity.ShouldBe(1);
            _orderRepository.Verify(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never());
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task Handle_ProductHasNoInventoryRecord_ThrowsInsufficientStockException()
        {
            // Arrange
            var cart = CreateCart();
            var shirt = CreateProduct("Blue Shirt", "SHT-1", 100m);
            AddItem(cart, shirt, 1);
            _inventoryRepository
                .Setup(x => x.GetByProductIdAsync(shirt.Id, _token))
                .ReturnsAsync((Inventory?)null);
            SetupCart(cart);

            // Act
            var ex = await Should.ThrowAsync<InsufficientStockException>(
                async () => await _handler.Handle(_command, _token));

            // Assert
            ex.Message.ShouldBe("'Blue Shirt' only has 0 units left in stock.");
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Test]
        public async Task Handle_SecondItemOutOfStock_DoesNotCreateOrderOrSave()
        {
            // Arrange
            var cart = CreateCart();
            var shirt = CreateProduct("Shirt", "SHT-1", 100m);
            var pants = CreateProduct("Pants", "PNT-1", 50m);
            AddItem(cart, shirt, 1);
            AddItem(cart, pants, 5);
            SetupInventory(shirt.Id, 10);
            SetupInventory(pants.Id, 2);
            SetupCart(cart);

            // Act
            var ex = await Should.ThrowAsync<InsufficientStockException>(
                async () => await _handler.Handle(_command, _token));

            // Assert
            ex.Message.ShouldContain("'Pants'");
            _orderRepository.Verify(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never());
            _cartRepository.Verify(x => x.EditAsync(It.IsAny<Cart>(), It.IsAny<CancellationToken>()), Times.Never());
            _unitOfWork.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        // ---------- success path ----------

        [Test]
        public async Task Handle_ValidCheckout_CreatesPendingOrderWithShippingSnapshotAndTotals()
        {
            // Arrange
            SetupValidCheckout();
            var before = DateTime.UtcNow;

            // Act
            var result = await _handler.Handle(_command, _token);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Id.ShouldNotBe(Guid.Empty),
                () => result.CustomerId.ShouldBe(_customerId),
                () => result.Status.ShouldBe(OrderStatus.Pending),
                () => result.PaymentStatus.ShouldBe(PaymentStatus.Pending),
                () => result.PaymentMethod.ShouldBe("Cash on Delivery"),
                () => result.Notes.ShouldBe("Leave at the door"),
                () => result.ShippingFullName.ShouldBe(_address.FullName),
                () => result.ShippingEmail.ShouldBe(_customer.Email),
                () => result.ShippingPhone.ShouldBe(_address.Phone),
                () => result.ShippingFullAddress.ShouldBe(_address.FullAddress),
                () => result.ShippingDistrict.ShouldBe(_address.District),
                () => result.ShippingPostalCode.ShouldBe(_address.PostalCode),
                () => result.SubTotal.ShouldBe(210m),       // (80 x 2) + (50 x 1)
                () => result.ShippingFee.ShouldBe(10m),
                () => result.Discount.ShouldBe(20m),
                () => result.GrandTotal.ShouldBe(200m),     // 210 + 10 - 20
                () => result.CreatedAt.ShouldBeGreaterThanOrEqualTo(before));

            _orderRepository.Verify(x => x.AddAsync(result, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_ValidCheckout_CreatesOrderItemsUsingDiscountedPriceWhenAvailable()
        {
            // Arrange
            var (_, shirt, pants, _, _) = SetupValidCheckout();

            // Act
            var result = await _handler.Handle(_command, _token);

            // Assert
            result.OrderItems.Count.ShouldBe(2);

            var shirtLine = result.OrderItems.Single(x => x.ProductId == shirt.Id);
            shirtLine.ShouldSatisfyAllConditions(
                () => shirtLine.Id.ShouldNotBe(Guid.Empty),
                () => shirtLine.OrderId.ShouldBe(result.Id),
                () => shirtLine.ProductName.ShouldBe("Shirt"),
                () => shirtLine.SKU.ShouldBe("SHT-1"),
                () => shirtLine.UnitPrice.ShouldBe(80m),     // discounted price wins
                () => shirtLine.Quantity.ShouldBe(2),
                () => shirtLine.LineTotal.ShouldBe(160m));

            var pantsLine = result.OrderItems.Single(x => x.ProductId == pants.Id);
            pantsLine.ShouldSatisfyAllConditions(
                () => pantsLine.OrderId.ShouldBe(result.Id),
                () => pantsLine.UnitPrice.ShouldBe(50m),     // no discount -> regular price
                () => pantsLine.Quantity.ShouldBe(1),
                () => pantsLine.LineTotal.ShouldBe(50m));
        }

        [Test]
        public async Task Handle_ValidCheckout_DeductsStockForEveryCartItem()
        {
            // Arrange
            var (_, _, _, shirtStock, pantsStock) = SetupValidCheckout();

            // Act
            await _handler.Handle(_command, _token);

            // Assert
            shirtStock.Quantity.ShouldBe(8);   // 10 - 2
            pantsStock.Quantity.ShouldBe(0);   // 1 - 1

            _inventoryRepository.Verify(x => x.EditAsync(shirtStock, _token), Times.Once());
            _inventoryRepository.Verify(x => x.EditAsync(pantsStock, _token), Times.Once());
        }

        [Test]
        public async Task Handle_ValidCheckout_ClearsCartAndPersistsEverythingInOneSave()
        {
            // Arrange
            var (cart, _, _, _, _) = SetupValidCheckout();
            var before = DateTime.UtcNow;

            // Act
            await _handler.Handle(_command, _token);

            // Assert
            cart.CartItems.ShouldBeEmpty();
            cart.UpdatedAt.ShouldBeGreaterThanOrEqualTo(before);

            _cartRepository.Verify(x => x.EditAsync(cart, _token), Times.Once());
            _unitOfWork.Verify(x => x.SaveAsync(_token), Times.Once());
        }

        [Test]
        public async Task Handle_ValidCheckout_GeneratesOrderNumberInExpectedFormat()
        {
            // Arrange
            SetupValidCheckout();
            var dateBefore = DateTime.UtcNow.ToString("yyyyMMdd");

            // Act
            var result = await _handler.Handle(_command, _token);

            // Assert
            var dateAfter = DateTime.UtcNow.ToString("yyyyMMdd");   // guards against a midnight rollover
            result.OrderNumber.ShouldMatch(@"^FH-\d{8}-\d{4}$");
            new[] { dateBefore, dateAfter }.ShouldContain(result.OrderNumber.Substring(3, 8));
        }

        [Test]
        public async Task Handle_OrderNumberAlreadyExists_RetriesUntilUnique()
        {
            // Arrange
            SetupValidCheckout();

            _orderRepository
                .SetupSequence(x => x.OrderNumberExistsAsync(It.IsAny<string>(), _token))
                .ReturnsAsync(true)
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(_command, _token);

            // Assert
            result.OrderNumber.ShouldNotBeNullOrEmpty();
            _orderRepository.Verify(x => x.OrderNumberExistsAsync(It.IsAny<string>(), _token), Times.Exactly(3));
            _orderRepository.Verify(x => x.AddAsync(result, _token), Times.Once());
        }
    }
}

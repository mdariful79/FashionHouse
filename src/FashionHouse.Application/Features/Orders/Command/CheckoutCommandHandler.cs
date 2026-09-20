using Cortex.Mediator.Commands;
using FashionHouse.Application.Contracts;
using FashionHouse.Application.Exceptions;
using FashionHouse.Domain.Entities;
using FashionHouse.Domain.Enums;
using FashionHouse.Domain.Utilities;

namespace FashionHouse.Application.Features.Orders.Command
{
    public class CheckoutCommandHandler : ICommandHandler<CheckoutCommand, Order>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public CheckoutCommandHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Order> Handle(CheckoutCommand command, CancellationToken cancellationToken)
        {
            var cart = await _unitOfWork.CartRepository.GetByCustomerIdAsync(command.CustomerId, cancellationToken);

            if (cart is null || !cart.CartItems.Any())
                throw new Exception("Your cart is empty.");

            var address = await _unitOfWork.AddressRepository.GetByIdAsync(command.AddressId, cancellationToken);

            if (address is null || address.CustomerId != command.CustomerId)
                throw new Exception("Invalid shipping address.");

            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(command.CustomerId, cancellationToken);

            if (customer is null)
                throw new Exception("Customer not found.");

            // Re-validate stock at checkout time (it may have changed since items were added to cart)
            // and deduct it as part of this same transaction.
            var orderItems = new List<OrderItem>();

            foreach (var cartItem in cart.CartItems)
            {
                var inventory = await _unitOfWork.InventoryRepository.GetByProductIdAsync(cartItem.ProductId, cancellationToken);
                var available = inventory?.Quantity ?? 0;

                if (cartItem.Quantity > available)
                    throw new InsufficientStockException(
                        $"'{cartItem.Product.ProductName}' only has {available} units left in stock.");

                inventory!.Quantity -= cartItem.Quantity;
                inventory.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.InventoryRepository.EditAsync(inventory, cancellationToken);

                var unitPrice = cartItem.Product.DiscountedPrice ?? cartItem.Product.Price;

                orderItems.Add(new OrderItem
                {
                    Id = IdentityGenerator.NewSequentialGuid(),
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.Product.ProductName,
                    SKU = cartItem.Product.SKU,
                    UnitPrice = unitPrice,
                    Quantity = cartItem.Quantity,
                    LineTotal = unitPrice * cartItem.Quantity
                });
            }

            var subTotal = orderItems.Sum(x => x.LineTotal);
            var grandTotal = subTotal + command.ShippingFee - command.Discount;

            var order = new Order
            {
                Id = IdentityGenerator.NewSequentialGuid(),
                OrderNumber = await GenerateOrderNumberAsync(cancellationToken),
                CustomerId = command.CustomerId,
                Status = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                PaymentMethod = command.PaymentMethod,

                ShippingFullName = address.FullName,
                ShippingEmail = customer.Email,
                ShippingPhone = address.Phone,
                ShippingFullAddress = address.FullAddress,
                ShippingDistrict = address.District,
                ShippingPostalCode = address.PostalCode,

                SubTotal = subTotal,
                ShippingFee = command.ShippingFee,
                Discount = command.Discount,
                GrandTotal = grandTotal,

                Notes = command.Notes,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var item in orderItems)
                item.OrderId = order.Id;

            order.OrderItems = orderItems;

            await _unitOfWork.OrderRepository.AddAsync(order, cancellationToken);

            // Clear the cart now that its contents have become an order
            cart.CartItems.Clear();
            cart.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.CartRepository.EditAsync(cart, cancellationToken);

            await _unitOfWork.SaveAsync(cancellationToken);

            return order;
        }

        private async Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken)
        {
            string orderNumber;

            do
            {
                orderNumber = $"FH-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
            }
            while (await _unitOfWork.OrderRepository.OrderNumberExistsAsync(orderNumber, cancellationToken));

            return orderNumber;
        }
    }
}
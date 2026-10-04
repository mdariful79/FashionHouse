using Autofac.Extras.Moq;
using FashionHouse.Application.Contracts;
using FashionHouse.Application.Contracts.Repositories;
using MapsterMapper;
using Moq;

namespace FashionHouse.Application.UnitTests
{
    public abstract class HandlerTestBase<THandler> where THandler : class
    {
        private CancellationTokenSource _cts = null!;

        protected AutoMock _autoMock = null!;
        protected THandler _handler = null!;
        protected CancellationToken _token;

        protected Mock<IApplicationUnitOfWork> _unitOfWork = null!;
        protected Mock<IMapper> _mapper = null!;

        protected Mock<ICategoryRepository> _categoryRepository = null!;
        protected Mock<ISubCategoryRepository> _subCategoryRepository = null!;
        protected Mock<IProductRepository> _productRepository = null!;
        protected Mock<IProductImageRepository> _productImageRepository = null!;
        protected Mock<IInventoryRepository> _inventoryRepository = null!;
        protected Mock<ICustomerRepository> _customerRepository = null!;
        protected Mock<IAddressRepository> _addressRepository = null!;
        protected Mock<ICartRepository> _cartRepository = null!;
        protected Mock<IOrderRepository> _orderRepository = null!;

        [SetUp]
        public void BaseSetUp()
        {
            _autoMock = AutoMock.GetLoose();
            _cts = new CancellationTokenSource();
            _token = _cts.Token;

            _unitOfWork = _autoMock.Mock<IApplicationUnitOfWork>();
            _mapper = _autoMock.Mock<IMapper>();

            _categoryRepository = _autoMock.Mock<ICategoryRepository>();
            _subCategoryRepository = _autoMock.Mock<ISubCategoryRepository>();
            _productRepository = _autoMock.Mock<IProductRepository>();
            _productImageRepository = _autoMock.Mock<IProductImageRepository>();
            _inventoryRepository = _autoMock.Mock<IInventoryRepository>();
            _customerRepository = _autoMock.Mock<ICustomerRepository>();
            _addressRepository = _autoMock.Mock<IAddressRepository>();
            _cartRepository = _autoMock.Mock<ICartRepository>();
            _orderRepository = _autoMock.Mock<IOrderRepository>();

            _unitOfWork.SetupGet(x => x.CategoryRepository).Returns(_categoryRepository.Object);
            _unitOfWork.SetupGet(x => x.SubCategoryRepository).Returns(_subCategoryRepository.Object);
            _unitOfWork.SetupGet(x => x.ProductRepository).Returns(_productRepository.Object);
            _unitOfWork.SetupGet(x => x.ProductImageRepository).Returns(_productImageRepository.Object);
            _unitOfWork.SetupGet(x => x.InventoryRepository).Returns(_inventoryRepository.Object);
            _unitOfWork.SetupGet(x => x.CustomerRepository).Returns(_customerRepository.Object);
            _unitOfWork.SetupGet(x => x.AddressRepository).Returns(_addressRepository.Object);
            _unitOfWork.SetupGet(x => x.CartRepository).Returns(_cartRepository.Object);
            _unitOfWork.SetupGet(x => x.OrderRepository).Returns(_orderRepository.Object);

            _handler = _autoMock.Create<THandler>();
        }

        [TearDown]
        public void BaseTearDown()
        {
            _autoMock.Dispose();
            _cts.Dispose();
        }
    }
}

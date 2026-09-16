using System;
using System.Collections.Generic;
using System.Text;
using Cortex.Mediator.Queries;
using FashionHouse.Application.Contracts;

namespace FashionHouse.Application.Features.Products.Query
{
    public class CheckSkuExistsQueryHandler : IQueryHandler<CheckSkuExistsQuery, bool>
    {
        private readonly IApplicationUnitOfWork _unitOfWork;

        public CheckSkuExistsQueryHandler(IApplicationUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CheckSkuExistsQuery query, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductRepository.IsDuplicateSku(query.SKU, query.ExcludeId, cancellationToken);
        }
    }
}
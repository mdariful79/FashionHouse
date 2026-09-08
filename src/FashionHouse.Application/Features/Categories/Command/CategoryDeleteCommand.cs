using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Application.Features.Categories.Command
{
    public class CategoryDeleteCommand : ICommand
    {
        public Guid Id { get; set; }
    }
}

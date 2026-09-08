using Cortex.Mediator.Commands;
using FashionHouse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FashionHouse.Application.Features.Categories.Command
{
    public class CategoryAddCommand : ICommand<Category>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

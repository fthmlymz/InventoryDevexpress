﻿using Domain.Common;

namespace Application.Features.Categories.Commands.UpdateCategory
{
    public class CategoryUpdatedEvent : BaseEvent
    {
        public Domain.Entities.Category Category { get; }
        public CategoryUpdatedEvent(Domain.Entities.Category category)
        {
            Category = category;
        }
    }
}

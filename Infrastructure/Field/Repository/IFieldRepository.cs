﻿using Infrastructure.Field.Entity;

namespace Infrastructure.Field.Repository
{
    public interface IFieldRepository
    {
        Task<int> Create(TemplateFormFields fields);

        Task<IEnumerable<TemplateFormFields>> GetByQuery(string where = null);
    }
}
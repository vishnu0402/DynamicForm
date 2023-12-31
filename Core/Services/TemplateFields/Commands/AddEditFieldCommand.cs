﻿using AutoMapper;
using Core.Services.TemplateFields.Requests;
using Infrastructure.Field.Entity;
using Infrastructure.Field.Repository;
using MediatR;
using Shared.Result;

namespace Core.Services.TemplateFields.Commands
{
    public record AddEditFieldCommand(FieldRequest Field) : IRequest<Result<string>>
    {

    }

    internal class AddEditFieldCommandHandler : IRequestHandler<AddEditFieldCommand, Result<string>>
    {
        private readonly IMapper _mapper;
        private readonly IFieldRepository _fieldRepository;

        public AddEditFieldCommandHandler(IMapper mapper, IFieldRepository fieldRepository)
        {
            _mapper = mapper;
            _fieldRepository = fieldRepository;
        }

        public async Task<Result<string>> Handle(AddEditFieldCommand command, CancellationToken cancellationToken)
        {
            try
            {
                if (Convert.ToInt32(command.Field.Id) == 0)
                {
                    TemplateFormFields fields = new()
                    {
                        Id = command.Field.Id,
                        TemplateFormId = command.Field.TemplateFormId,
                        Name = command.Field.Name,
                        ControlId = command.Field.ControlId,
                        OrderNo = command.Field.OrderNo,
                        DefaultValue = command.Field.DefaultValue,
                        IsRequired = command.Field.IsRequired,
                        RequiredMessage = command.Field.RequiredMessage,
                        RegExValue = command.Field.RegExValue,
                        RegExMessage = command.Field.RegExMessage,
                        Status = 1,
                        CreatedBy = command.Field.CreatedBy,
                        CreatedDate = DateTime.UtcNow
                    };

                    var rtn = await _fieldRepository.Create(fields);

                    if (rtn == 0)
                    {
                        return await Result<string>.FailAsync("Failed to create field");
                    }
                    else
                    {
                        return await Result<string>.SuccessAsync(rtn.ToString(), "Field is created successfully");
                    }
                }

                else
                {
                    var existingObj = await _fieldRepository.GetById(Convert.ToInt32(command.Field.Id));

                    if (existingObj == null)
                    {
                        return await Result<string>.FailAsync("Failed to update form");
                    }
                    else
                    {
                        TemplateFormFields fields = new()
                        {
                            Id = command.Field.Id,
                            TemplateFormId = command.Field.TemplateFormId,
                            Name = command.Field.Name,
                            ControlId = command.Field.ControlId,
                            OrderNo = command.Field.OrderNo,
                            DefaultValue = command.Field.DefaultValue,
                            IsRequired = command.Field.IsRequired,
                            RequiredMessage = command.Field.RequiredMessage,
                            RegExValue = command.Field.RegExValue,
                            RegExMessage = command.Field.RegExMessage,
                            Status = command.Field.Status,
                            CreatedBy = existingObj.CreatedBy,
                            CreatedDate = existingObj.CreatedDate,
                            ModifiedBy = command.Field.ModifiedBy,
                            ModifiedDate = DateTime.UtcNow
                        };

                        var rtn = await _fieldRepository.Update(fields);

                        if (rtn == 0)
                        {
                            return await Result<string>.FailAsync("Failed to update field");
                        }
                        else
                        {
                            return await Result<string>.SuccessAsync(rtn.ToString(), "Field is updated successfully");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return await Result<string>.FailAsync(ex.Message);
            }
        }
    }
}

﻿using Core.Services.FieldOptions.Commands;
using Core.Services.FieldOptions.Requests;
using Core.Services.TemplateFields.Commands;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;

namespace DynamicForm.Controllers
{
    public class FieldOptionsController : BaseController<FieldsController>
    {
        public async Task<IActionResult> SaveOptions(FieldOptionsRequest model)
        {
            var command = new AddEditFieldOptionsCommand(model);
            var response = await _mediator.Send(command);
            dynamic result = new ExpandoObject();
            result.error = response.Succeeded;
            result.message = response.Messages.FirstOrDefault();

            if (response.Succeeded)
            {
                result.error = false;
            }
            else
            {
                result.error = true;
            }

            result.message = response.Messages.FirstOrDefault();

            return Json(result);
        }
        public async Task<IActionResult> AddOptions()
        {

            return PartialView("_AddOptions");
        }
    }
}

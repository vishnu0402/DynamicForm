﻿using Core.Services.ControlFields.Queries;
using Core.Services.TemplateFields.Commands;
using Core.Services.TemplateFields.Queries;
using Core.Services.TemplateFields.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Dynamic;

namespace DynamicForm.Controllers
{
    public class FieldsController : BaseController<FieldsController>
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await LoadControlTypes();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveField(FieldRequest model)
        {
            var command = new AddEditFieldCommand(model);
            var response = await _mediator.Send(command);

            dynamic result = new ExpandoObject();
            result.error = response.Succeeded;
            result.message = response.Messages.FirstOrDefault();

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFields()
        {
            try
            {
                dynamic res = new ExpandoObject();
                FieldRequest formRequest = new FieldRequest();

                var mediatorResponse = await _mediator.Send(new GetAllFieldsQuery() { TemplateFormId = null });

                return Json(mediatorResponse);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region | Private Methods |

        private async Task LoadControlTypes()
        {
            var data = new List<SelectListItem>();

            try
            {
                var result = await _mediator.Send(new GetAllControlTypesQuery());

                data = result.Data.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }).ToList();
            }
            catch
            {
                //TODO: record ex into logger
            }

            ViewBag.ControlTypes = data;
        }

        #endregion
    }
}

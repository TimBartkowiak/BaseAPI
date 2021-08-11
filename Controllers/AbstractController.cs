using System;
using BaseAPI.Entities;
using BaseAPI.Exceptions;
using BaseAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BaseAPI.Controllers
{
    public abstract class AbstractController<K, V> : ControllerBase where K : AbstractModel where V : AbstractEntity
    {
        protected abstract AbstractService<K, V> getService();

        protected IActionResult add(K model)
        {
            try
            {
                model.scrubAndValidate(RequiredModelAttribute.RequiredActionEnum.CREATE);
            }
            catch (InvalidModelException ime)
            {
                return BadRequest(ime.Message);
            }

            Guid? id = getService().add(model);

            return Created(new Uri($"{Request.Path}/{id}", UriKind.Relative), id);
        }

        protected IActionResult update(K model, string id)
        {
            try
            {
                model.scrubAndValidate(RequiredModelAttribute.RequiredActionEnum.UPDATE);
            }
            catch (InvalidModelException ime)
            {
                return BadRequest(ime.Message);
            }

            getService().update(model, id);

            return Ok();
        }

        //TODO: Implement Pagination
        
        protected K getById(string id)
        {
            return getService().getById(id);
        }
    }
}
using System;
using BaseAPI.Entities;
using BaseAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BaseAPI.Controllers
{
    public abstract class AbstractController<K> : ControllerBase where K : AbstractModel
    {
        protected abstract AbstractService<K, AbstractEntity> getService();

        protected IActionResult add(K model)
        {
            model.scrubAndValidate(RequiredModelAttribute.RequiredActionEnum.CREATE);
            Guid id = getService().add(model);

            return Created(new Uri($"{Request.Path}/{id}", UriKind.Relative), id);
        }

        protected IActionResult update(K model, string id)
        {
            model.scrubAndValidate(RequiredModelAttribute.RequiredActionEnum.UPDATE);
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
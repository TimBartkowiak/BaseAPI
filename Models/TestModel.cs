using System;
using BaseAPI.Entities;

namespace BaseAPI.Models
{
    public class TestModel : AbstractModel
    {
        [RequiredModel(Type = RequiredModelAttribute.RequiredActionEnum.BOTH)]
        public int? Number { get; set; }
        public string Data { get; set; }
        
    }
}
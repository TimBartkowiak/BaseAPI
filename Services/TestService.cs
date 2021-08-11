using AutoMapper;
using BaseAPI.Database;
using BaseAPI.Entities;
using BaseAPI.Models;

namespace BaseAPI
{
    public class TestService : AbstractService<TestModel, TestEntity>
    {
        private readonly IMapper _autoMapper;
        
        public TestService(BaseDbContext baseDbContext, IMapper autoMapper) : base(baseDbContext)
        {
            _autoMapper = autoMapper;
        }

        protected override TestEntity convertToEntityForAdd(TestModel model)
        {
            return _autoMapper.Map<TestEntity>(model);
        }

        protected override TestEntity convertToEntityForSearch(TestModel model)
        {
            return convertToEntityForAdd(model);
        }

        protected override TestEntity populateEntityForUpdate(TestEntity entity, TestModel model)
        {
            entity.Data = model.Data;
            entity.Number = model.Number;
            return entity;
        }

        protected override TestModel convertToModel(TestEntity entity)
        {
            return _autoMapper.Map<TestModel>(entity);
        }

        protected override TestModel convertToModelForList(TestEntity entity)
        {
            return convertToModel(entity);
        }
    }
}
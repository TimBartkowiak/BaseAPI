using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BaseAPI.Database;
using BaseAPI.Entities;
using BaseAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BaseAPI
{
    public class UserService : AbstractService<UserModel, UserEntity>
    {
        private readonly IMapper _autoMapper;
        private readonly BaseDbContext _dbContext;

        public UserService(BaseDbContext baseDbContext, IMapper autoMapper) : base(baseDbContext)
        {
            _autoMapper = autoMapper;
            _dbContext = baseDbContext;
        }

        public List<UserModel> getAll()
        {
            return _dbContext.Set<UserModel>().AsNoTracking().ToList();
        }

        protected override UserEntity convertToEntityForAdd(UserModel model)
        {
            return _autoMapper.Map<UserEntity>(model);
        }

        protected override UserEntity convertToEntityForSearch(UserModel model)
        {
            return convertToEntityForAdd(model);
        }

        protected override UserEntity populateEntityForUpdate(UserEntity entity, UserModel model)
        {
            throw new System.NotImplementedException();
        }

        protected override UserModel convertToModel(UserEntity entity)
        {
            return _autoMapper.Map<UserModel>(entity);
        }

        protected override UserModel convertToModelForList(UserEntity entity)
        {
            return convertToModel(entity);
        }
    }
}
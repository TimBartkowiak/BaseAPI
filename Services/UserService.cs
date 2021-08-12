using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BaseAPI.Database;
using BaseAPI.Entities;
using BaseAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BaseAPI
{
    public class UserService : AbstractService<UserModel, UserEntity>
    {
        private readonly IMapper _autoMapper;

        public UserService(BaseDbContext baseDbContext, IHttpContextAccessor httpContext, IMapper autoMapper) : base(baseDbContext, httpContext)
        {
            _autoMapper = autoMapper;
        }

        public List<UserModel> getAll()
        {
            return dbContext.Set<UserEntity>().AsNoTracking().AsEnumerable().Select(convertToModelForList).ToList();
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
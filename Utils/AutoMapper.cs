using AutoMapper;
using BaseAPI.Entities;
using BaseAPI.Models;

namespace BaseAPI.Utils
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            //Model -> Entity
            CreateMap<TestModel, TestEntity>();
            CreateMap<UserModel, UserEntity>();
            
            //Entity -> Model
            CreateMap<TestEntity, TestModel>();
            CreateMap<UserEntity, UserModel>();
        }
    }
}
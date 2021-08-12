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
            CreateMap<UserModel, UserEntity>();
            
            //Entity -> Model
            CreateMap<UserEntity, UserModel>();
        }
    }
}
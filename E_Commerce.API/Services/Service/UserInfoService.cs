using AutoMapper;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Models.Responses;
using E_Commerce.API.Repositories.IRepository;
using E_Commerce.API.Services.IService;

namespace E_Commerce.API.Services.Service
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IUserInfoRepository _repo;
        private readonly IMapper _mapper;
        public UserInfoService(IUserInfoRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<UserInfoDto?> GetByUserIdAsync(string userId)
        {
            var userInfo = await _repo.GetUserByIdAsync(userId);
            return userInfo == null ? null : _mapper.Map<UserInfoDto>(userInfo);
        }
        public async Task<UserInfoDto> CreateAsync(UserInfoDto dto)
        {
            var userInfo = _mapper.Map<UserInfo>(dto);
            var created = await _repo.CreateAsync(userInfo);
            return _mapper.Map<UserInfoDto>(created);
        }
        public async Task<UserInfoDto?> UpdateAsync(string userId, UserInfoDto dto)
        {
            var updated = await _repo.UpdateAsync(userId, _mapper.Map<UserInfo>(dto));
            return updated == null ? null : _mapper.Map<UserInfoDto>(updated);
        }
    }
}

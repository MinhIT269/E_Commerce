using AutoMapper;
using AutoMapper.QueryableExtensions;
using E_Commerce.API.Models.Responses;
using E_Commerce.API.Repositories.IRepository;
using E_Commerce.API.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API.Services.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<List<UserAdminDto>> GetFilteredUsers(int page, int pageSize, string searchQuery, string sortCriteria, bool isDescending)
        {
            var query = _userRepository.GetFilteredUsers(searchQuery, sortCriteria, isDescending);

            var pagedUsers = await query.Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .ProjectTo<UserAdminDto>(_mapper.ConfigurationProvider)
                              .ToListAsync();
            return pagedUsers;
        }
        public async Task<int> GetTotalUserAsync(string searchQuery)
        {
            var query = _userRepository.GetFilteredUsers(searchQuery, "name", false);
            return await query.CountAsync();
        }

        public async Task<UserDto> GetUser(string username)
        {
            var user = await _userRepository.FindByNameAsync(username);
            return _mapper.Map<UserDto>(user);
        }
    }
}

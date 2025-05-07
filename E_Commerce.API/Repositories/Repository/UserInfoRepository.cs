using E_Commerce.API.Data;
using E_Commerce.API.Models.Domain;
using E_Commerce.API.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.API.Repositories.Repository
{
    public class UserInfoRepository : IUserInfoRepository
    {
        private readonly DataContext _context;
        public UserInfoRepository(DataContext dbContext)
        {
            _context = dbContext;
        }
        public async Task<UserInfo?> GetUserByIdAsync(string userId)
        {
            return await _context.UserInfos.Include(ui => ui.User).FirstOrDefaultAsync(ui => ui.UserId == userId);
        }
        public async Task AddAsync(UserInfo userInfo)
        {
            await _context.UserInfos.AddAsync(userInfo); 
            await SaveChangesAsync();          
        }
        public async Task<UserInfo> CreateAsync(UserInfo userInfo)
        {
            await _context.UserInfos.AddAsync(userInfo);
            await SaveChangesAsync();
            return userInfo;
        }
        public async Task<UserInfo?> UpdateAsync(string userId, UserInfo updatedInfo)
        {
            var existing = await _context.UserInfos.FirstOrDefaultAsync(x => x.UserId == userId);
            if (existing == null) return null;

            existing.FirstName = updatedInfo.FirstName;
            existing.LastName = updatedInfo.LastName;
            existing.Address = updatedInfo.Address;
            existing.Gender = updatedInfo.Gender;

            await SaveChangesAsync();
            return existing;
        }
        private async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

using Group7_SE1733_A01_BE.Service.DTOs;
using Microsoft.Extensions.Configuration;
using Repositories;
using Repositories.Models;

namespace Services
{
    public interface ISystemAccountService
    {
        Task<List<SystemAccount>> GetAll();
        Task<SystemAccount> GetById(int id);
        Task<int> Create(SystemAccountDTO SystemAccount);
        Task<int> Update(short id, SystemAccountDTO SystemAccount);
        Task<bool> Delete(int id);
        Task<List<SystemAccount>> Search(string AccountName, string AccountEmail);
        Task<SystemAccount> GetUserAccountAsync(string email, string password);

    }
    public class SystemAccountService : ISystemAccountService
    {
        private readonly SystemAccountRepository _repository;
        private readonly NewsArticleRepository _newsArticleRepository;
        private readonly IConfiguration _configuration;
        public SystemAccountService(IConfiguration _configuration)
        {
            this._configuration = _configuration;
            _newsArticleRepository = new NewsArticleRepository();
            _repository = new SystemAccountRepository();
        }
        public async Task<int> Create(SystemAccountDTO SystemAccount)
        {

            if (string.IsNullOrWhiteSpace(SystemAccount.AccountName))
                throw new ArgumentException("AccountName cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(SystemAccount.AccountEmail))
                throw new ArgumentException("AccountEmail cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(SystemAccount.AccountPassword))
                throw new ArgumentException("AccountPassword cannot be null or empty.");

            if (!SystemAccount.AccountRole.HasValue || SystemAccount.AccountRole < 0 || SystemAccount.AccountRole > 2)
                throw new ArgumentOutOfRangeException(nameof(SystemAccount.AccountRole), "AccountRole must be between 0 and 2.");

            if (SystemAccount.AccountEmail.Length > 100)
                throw new ArgumentException("AccountEmail cannot exceed 100 characters.");

            if (SystemAccount.AccountPassword.Length < 6 || SystemAccount.AccountPassword.Length > 20)
                throw new ArgumentException("AccountPassword must be between 6 and 20 characters.");
            // Take last Id 
            short accountId = 1;
            var lastAccount = await _repository.GetAll();
            if (lastAccount.Count > 0)
            {
                accountId = (short)(lastAccount.Max(x => x.AccountId) + 1);
            }
            else
            {
                accountId = 1;
            }
            var entity = new SystemAccount
            {
                AccountId = accountId,
                AccountName = SystemAccount.AccountName,
                AccountEmail = SystemAccount.AccountEmail,
                AccountRole = SystemAccount.AccountRole,
                AccountPassword = SystemAccount.AccountPassword
            };


            return await _repository.CreateAsync(entity);
        }

        public async Task<bool> Delete(int id)
        {
            // Check user in articles
            var articles =  _newsArticleRepository.GetAll();
            if (articles.Any(x => x.CreatedById == id))
            {
                throw new InvalidOperationException("Cannot delete account because it is associated with existing articles.");
            }

            var item = await _repository.GetByIdAsync(id);
            if (item == null)
            {
                return false;
            }

            return await _repository.RemoveAsync(item);
        }

        public async Task<List<SystemAccount>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<SystemAccount> GetById(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<SystemAccount>> Search(string AccountName, string AccountEmail)
        {
            return await _repository.Search(AccountName, AccountEmail);
        }

        public async Task<int> Update(short AccountId, SystemAccountDTO SystemAccount)
        {
            var existingAccount = await _repository.GetByIdAsync(AccountId);
            if (existingAccount == null)
            {
                throw new InvalidOperationException("Account is not existed");
            }
            if (string.IsNullOrWhiteSpace(SystemAccount.AccountName))
                throw new ArgumentException("AccountName cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(SystemAccount.AccountEmail))
                throw new ArgumentException("AccountEmail cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(SystemAccount.AccountPassword))
                throw new ArgumentException("AccountPassword cannot be null or empty.");

            if (!SystemAccount.AccountRole.HasValue || SystemAccount.AccountRole < 0 || SystemAccount.AccountRole > 2)
                throw new ArgumentOutOfRangeException(nameof(SystemAccount.AccountRole), "AccountRole must be between 0 and 2.");

            if (SystemAccount.AccountEmail.Length > 100)
                throw new ArgumentException("AccountEmail cannot exceed 100 characters.");

            if (SystemAccount.AccountPassword.Length < 6 || SystemAccount.AccountPassword.Length > 20)
                throw new ArgumentException("AccountPassword must be between 6 and 20 characters.");
            existingAccount.AccountName = SystemAccount.AccountName;
            existingAccount.AccountEmail = SystemAccount.AccountEmail;
            existingAccount.AccountRole = SystemAccount.AccountRole;
            existingAccount.AccountPassword = SystemAccount.AccountPassword;
          
           


            return await _repository.UpdateAsync(existingAccount);
        }
        public async Task<SystemAccount> GetUserAccountAsync(string email, string password)
        {
            // Check isAdmin
            var usernameAdmin = _configuration["AdminAccount:Email"];
            var passwordAdmin = _configuration["AdminAccount:Password"];
            if (email == usernameAdmin && password == passwordAdmin)
            {
                return new SystemAccount
                {
                    AccountId = 0,
                    AccountName = "Admin",
                    AccountEmail = usernameAdmin,
                    AccountPassword = passwordAdmin,
                    AccountRole = 0,
                };
            }

            return await _repository.GetUserAccountAsync(email, password);
        }
    }

}

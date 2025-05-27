using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class SystemAccountRepository : GenericRepository<SystemAccount>
    {
        public SystemAccountRepository() { }

        public async Task<SystemAccount> GetUserAccountAsync(string email, string password)
        {



            return await _context.SystemAccounts.FirstOrDefaultAsync(
                x => x.AccountEmail == email && x.AccountPassword == password);

        }
        public async Task<List<SystemAccount>> GetAll()
        {
            var items = await _context.SystemAccounts.ToListAsync();
            return items;
        }
        public async Task<List<SystemAccount>> Search(string AccountName, string AccountEmail)
        {

            AccountName = AccountName?.Trim();
            AccountEmail = AccountEmail?.Trim();

            var items = await _context.SystemAccounts
                .Where(x =>
                (
                x.AccountName.Contains(AccountName) || string.IsNullOrEmpty(AccountName))
                && (x.AccountEmail.Contains(AccountEmail) || string.IsNullOrEmpty(AccountEmail))
                ).ToListAsync();
            return items;
        }
        public async Task<SystemAccount> GetByIdAsync(int id)
        {
            var item = await _context.SystemAccounts.FirstOrDefaultAsync(x => x.AccountId == id);
            return item;
        }


    }
}

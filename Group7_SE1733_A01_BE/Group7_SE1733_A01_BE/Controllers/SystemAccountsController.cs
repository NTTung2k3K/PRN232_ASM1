using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Group7_SE1733_A01_BE.Models;
using Group7_SE1733_A01_BE.Service.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories.Models;
using Services;

namespace Group7_SE1733_A01_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemAccountsController : ControllerBase
    {
        private readonly ISystemAccountService _systemAccountService;

        public SystemAccountsController(ISystemAccountService systemAccountService)
        {
            _systemAccountService = systemAccountService;
        }
        [HttpPost("login")]
        public async Task<ActionResult<SystemAccount>> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                if (loginRequest == null || string.IsNullOrEmpty(loginRequest.AccountEmail) || string.IsNullOrEmpty(loginRequest.AccountPassword))
                {
                    return BadRequest("Invalid login request.");
                }
                var account = await _systemAccountService.GetUserAccountAsync(loginRequest.AccountEmail, loginRequest.AccountPassword);
                if (account == null)
                {
                    return BadRequest("Invalid email or password.");
                }
                return Ok(account);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
         
        }

        // GET: api/SystemAccounts
        [HttpGet("get-all")]
        public async Task<ActionResult<IEnumerable<SystemAccount>>> GetSystemAccounts()
        {
            try
            {
                return await _systemAccountService.GetAll();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET: api/SystemAccounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SystemAccount>> GetSystemAccount(short id)
        {
            try
            {
                var systemAccount = await _systemAccountService.GetById(id);

                if (systemAccount == null)
                {
                    return NotFound();
                }

                return systemAccount;
            }
            catch(Exception e)
            {

                return BadRequest(e.Message);
            }
         
        }

        // PUT: api/SystemAccounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSystemAccount(short id, SystemAccountDTO systemAccount)
        {
          
            

            try
            {
                var existingAccount = await _systemAccountService.GetById(id);
                if (existingAccount == null)
                {
                    return NotFound("Account not found.");
                }

                var rs = await _systemAccountService.Update(id,systemAccount);
                if(rs == 0)
                {
                    return BadRequest("Failed to update account. Please check the input data.");
                }
                return Ok("Account updated successfully.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        // POST: api/SystemAccounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create")]
        public async Task<ActionResult<SystemAccount>> PostSystemAccount(SystemAccountDTO systemAccount)
        {
            
            try
            {
                var rs = await _systemAccountService.Create(systemAccount);
                if (rs == 0 )
                {
                    return BadRequest("Failed to create account. Please check the input data.");
                }
                return Ok("Account created successfully.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message) ;
            }

        }

        // DELETE: api/SystemAccounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSystemAccount(short id)
        {
            try
            {
                var systemAccount = await _systemAccountService.GetById(id);
                if (systemAccount == null)
                {
                    return NotFound();
                }

                var rs = await _systemAccountService.Delete(id);
                if (!rs)
                {
                    return BadRequest("Failed to delete account. Please check the input data.");
                }
                return Ok("Account deleted successfully.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
         
        }

        
    }
}

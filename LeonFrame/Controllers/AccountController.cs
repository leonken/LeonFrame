using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LeonFrameAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        /// <summary>
        /// 登陆
        /// </summary> 
        /// <returns></returns>
        [HttpGet]
        //public IActionResult Login()
        public IActionResult Login(string username  ,string pwd )
        {
            //string username = "1"; string pwd = "1";
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(pwd))
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                    new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddMinutes(30)).ToUnixTimeSeconds()}"),
                    new Claim(ClaimTypes.Name, username) //这个节点可以使得HttpContext.User.Identity.Name获取用户名
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("LeonKeykey1111111111111111111"));//不能少于16位
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: "http://www.baidu.com",
                    audience: "http://www.baidu.com",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(15),
                    signingCredentials: creds);

                return Ok(new
                {
                    tokenResult = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }

            return Ok();
        }

        /// <summary>
        /// 获取登录人姓名
        /// </summary>
        /// <returns></returns>
        [HttpGet] 
        [Authorize]
        public IActionResult GetLoginUserName()
        {
            string username = HttpContext.User.Identity.Name;

            return Ok(username);
        }
    }
}

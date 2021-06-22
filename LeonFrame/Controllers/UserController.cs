using Application.Interfaces;
using Application.ViewModel.In.User;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LeonFrameAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController] 
    public class UserController : ControllerBase
    {
        IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 创建、编辑用户1
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateUpdateUser(UpdateUserRequest req)
        {
            //验证请求可以放到过滤器或者MediatR
            await _userService.UpdateUser(req);

            //通过消息通知总线判断是否有异常

            return Ok();
        } 
    }
}

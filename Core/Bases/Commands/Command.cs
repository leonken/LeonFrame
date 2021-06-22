using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/11/26 18:11:00
*描述：
*
***********************************************************/
namespace Core.Bases.Commands
{
    /// <summary>
    /// 命令基类
    /// </summary>
    public class Command : IRequest<bool>
    {
    }
}

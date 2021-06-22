using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/10/21 15:48:22
*描述：
*
***********************************************************/
namespace Application.Mapper
{
    public class MapperRegister
    {
        public static MapperConfiguration RegisterMappings()
        {
            //创建AutoMapperConfiguration, 提供静态方法Configure，一次加载所有层中Profile定义 
            //MapperConfiguration实例可以静态存储在一个静态字段中，也可以存储在一个依赖注入容器中。 一旦创建，不能更改/修改。
            return new MapperConfiguration(cfg =>
            {
                //这个是领域模型 -> 视图模型的映射
                cfg.AddProfile(new CmdToEntity());

                //视图模型 ->命令模型的映射            
                cfg.AddProfile(new EntityToVm());
                //命令模型-->领域模型映射
                cfg.AddProfile(new VmToCmd());
                //Dto-->VM
                cfg.AddProfile(new VmToDto());
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Application.ViewModel.In.User;
using FluentValidation;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/10/20 20:12:46
*描述：
*
***********************************************************/
namespace Application.Validations
{
    public class UpdateUserInfoVMValidation : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserInfoVMValidation()
        {
            RuleFor(r => r.Id).NotEmpty().GreaterThan(0).WithMessage("用户ID不能为空");
            RuleFor(r => r.UserName).NotEmpty().WithMessage("用户名不能为空");
            RuleFor(r => r.UserAddresses).NotEmpty().WithMessage("用户地址不能为空");
            RuleForEach(r => r.UserAddresses).NotEmpty().WithMessage("请添加地址信息").SetValidator(new UpdateUserAddressVMValidation());
        }
    }

    public class UpdateUserAddressVMValidation:AbstractValidator<UpdateUserRequest.UpdateUserAddressVM>
    {
        public UpdateUserAddressVMValidation()
        {
            RuleFor(r => r.AddressTitle).NotEmpty().WithMessage("地址名称不能为空");
            RuleFor(r => r.Area).NotEmpty().WithMessage("区域不能为空");
            RuleFor(r => r.City).NotEmpty().WithMessage("城市不能为空");
            RuleFor(r => r.Country).NotEmpty().WithMessage("国家不能为空");
            RuleFor(r => r.Province).NotEmpty().WithMessage("省不能为空");
        }
    }
}

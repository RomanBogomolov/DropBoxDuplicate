using System;
using System.Collections.Generic;
using FluentValidation;
using Ninject;
using Ninject.Planning.Bindings;

namespace DropBoxDuplicate.Api.Services
{
    public class NinjectValidatorFactory : ValidatorFactoryBase
    {
        public NinjectValidatorFactory(IKernel kernel)
        {
            Kernel = kernel;
        }

        public IKernel Kernel { get; set; }

        public override IValidator CreateInstance(Type validatorType)
        {
            if (((IList<IBinding>)Kernel.GetBindings(validatorType)).Count == 0)
            {
                return null;
            }

            return Kernel.Get(validatorType) as IValidator;
        }
    }
}
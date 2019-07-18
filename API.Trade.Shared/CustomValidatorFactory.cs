using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace API.Trade.Shared
{
    public class CustomValidatorFactory : ValidatorFactoryBase
    {
        private static IServiceProvider _serviceProvider;

        public CustomValidatorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            var service = _serviceProvider.GetService(validatorType) as IValidator;

            return service;
        }
    }
}

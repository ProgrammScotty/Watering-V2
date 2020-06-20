using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Watering2.Validators
{
    class DoubleValidator : ConfigurationValidatorBase
    {
        public double MinValue { get; private set; }
        public double MaxValue { get; private set; }

        public DoubleValidator(double minValue, double maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public override bool CanValidate(Type type)
        {
            return type == typeof(double);
        }

        public override void Validate(object obj)
        {
            double value;
            try
            {
                value = Convert.ToDouble(obj);
            }
            catch (Exception)
            {
                throw new ArgumentException();
            }

            if (value < MinValue)
            {
                throw new ConfigurationErrorsException($"Value too low, minimum value allowed: {MinValue}");
            }

            if (value > MaxValue)
            {
                throw new ConfigurationErrorsException($"Value too high, maximum value allowed: {MaxValue}");
            }
        }
    }
    class DoubleValidatorAttribute : ConfigurationValidatorAttribute
    {
        public double MinValue { get; set; }
        public double MaxValue { get; set; }

        public DoubleValidatorAttribute(double minValue, double maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public override ConfigurationValidatorBase ValidatorInstance => new DoubleValidator(MinValue, MaxValue);
    }
}

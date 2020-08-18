using EnterpriseBot.VK.Exceptions;
using Newtonsoft.Json;
using System;

namespace EnterpriseBot.VK.Models.MenuRelated
{
    public class MenuParameter : ICloneable
    {
        public MenuParameter() { }

        public MenuParameter(object value, string name = null)
        {
            this.Value = value;
            this.Name = name;
        }

        [JsonIgnore]
        public Type Type
        {
            get
            {
                if (Value is null)
                    throw new NullParameterValueException("Value is null, thus cannot get its type");
                return Value.GetType();
            }
        }

        public object Value { get; set; }

        public string Name { get; set; }
        
        public object Clone()
        {
            object value;
            
            if (!(Value is string) && Value is ICloneable cloneable)
            {
                value = cloneable.Clone();
            }
            else
            {
                value = Value;
            }
            
            return new MenuParameter
            {
                Value = value,
                Name = Name
            };
        }
    }
}

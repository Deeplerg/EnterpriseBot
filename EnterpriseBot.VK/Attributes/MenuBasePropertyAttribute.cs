using System;

namespace EnterpriseBot.VK.Attributes
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class MenuBasePropertyAttribute : Attribute
    {
        //readonly bool required;

        //public MenuBasePropertyAttribute(bool required = true)
        //{
        //    this.required = required;
        //}

        //public bool Required
        //{
        //    get { return required; }
        //}

        public MenuBasePropertyAttribute() { }
    }
}

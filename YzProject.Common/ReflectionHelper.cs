using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Common
{
    /// <summary>
    /// 反射帮助类
    /// </summary>
    public class ReflectionHelper
    {
        #region Methods

        /// <summary>
        /// <para>return property name string by lamabda selector</para>
        /// <para>if in C# 6.0, can use nameof operator instead of this</para>
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="express"></param>
        /// <returns></returns>
        public static string GetPropertyName<TProperty>(Expression<Func<TProperty>> express)
        {
            var memberExpress = express.Body as MemberExpression;
            if (memberExpress == null)
            {
                throw new ArgumentException("Not is MemberExpression", nameof(express));
            }

            return memberExpress.Member.Name;
        }

        /// <summary>
        /// <para>return property name string by lamabda selector</para>
        /// <para>if in C# 6.0, can use nameof operator instead of this</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="express"></param>
        /// <returns></returns>
        public static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> express)
        {
            var memberExpress = express.Body as MemberExpression;
            if (memberExpress == null)
            {
                throw new ArgumentException("Not is MemberExpression", nameof(express));
            }

            return memberExpress.Member.Name;
        }

        public static string GetPropertyFullName<TProperty>(Expression<Func<TProperty>> express)
        {
            var memberExpress = express.Body as MemberExpression;
            if (memberExpress == null)
            {
                throw new ArgumentException("Not is MemberExpression", nameof(express));
            }

            return GetClassName(memberExpress.Member.ReflectedType) + "." + GetPropertyName(express);
        }

        public static string GetPropertyFullName<T, TProperty>(Expression<Func<T, TProperty>> express)
        {
            return GetClassName(typeof(T)) + "." + GetPropertyName(express);
        }

        public static string GetClassName(Type type)
        {
            return type.Name;
        }

        /// <summary>
        /// Tries to gets an of attribute defined for a class member and it's declaring type including inherited attributes.
        /// Returns default value if it's not declared at all.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute</typeparam>
        /// <param name="memberInfo">MemberInfo</param>
        /// <param name="defaultValue">Default value (null as default)</param>
        /// <param name="inherit">Inherit attribute from base classes</param>
        public static TAttribute GetSingleAttributeOrDefault<TAttribute>(MemberInfo memberInfo, TAttribute defaultValue = default(TAttribute), bool inherit = true)
            where TAttribute : Attribute
        {
            //Get attribute on the member
            if (memberInfo.IsDefined(typeof(TAttribute), inherit))
            {
                return memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().First();
            }

            return defaultValue;
        }

        #endregion Methods
    }

    public static class ReflectionHelperExtension
    {
        public static string GetPropertyName<T, TProperty>(this T obj, Expression<Func<T, TProperty>> express)
        {
            return ReflectionHelper.GetPropertyName(express);
        }

        public static string GetPropertyFullName<T, TProperty>(this T obj, Expression<Func<T, TProperty>> express)
        {
            return ReflectionHelper.GetPropertyFullName(express);
        }
    }
}

using System;
using System.Reflection;
using JetBrains.Annotations;

namespace dotRMDY.TestingTools
{
	[PublicAPI]
    public static class ReflectionUtil
    {
        private const BindingFlags ALL_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

        /// <summary>
        /// Gets the value of (static) field with name "fieldName" of an object
        /// </summary>
        /// <param name="obj">The object containing the target field</param>
        /// <param name="fieldName">The name of the target field</param>
        /// <typeparam name="TTarget">The type of object containing the target field. Use this explicitly if the target type to target the baseclass if the field is defined on that one</typeparam>
        /// <typeparam name="TReturn">The type of the returned value</typeparam>
        /// <exception cref="MissingFieldException">Thrown when field can't be found on target object</exception>
        /// <returns>The typed value of the field</returns>
        public static TReturn? GetField<TTarget, TReturn>(this TTarget obj, string fieldName)
            => (TReturn?)GetField(obj, fieldName);

        /// <summary>
        /// Gets the value of (static) field with name "fieldName" of an object
        /// </summary>
        /// <param name="obj">The object containing the target field</param>
        /// <param name="fieldName">The name of the target field</param>
        /// <typeparam name="TTarget">The type of object containing the target field. Use this explicitly if the target type to target the baseclass if the field is defined on that one</typeparam>
        /// <exception cref="MissingFieldException">Thrown when field can't be found on target object</exception>
        /// <returns>The raw value of the field</returns>
        public static object? GetField<TTarget>(this TTarget obj, string fieldName)
        {
            var fieldInfo = typeof(TTarget).GetField(fieldName, ALL_BINDING_FLAGS);
            if (fieldInfo == null)
            {
                throw new MissingFieldException($"Field {fieldName} not found on target object of type {typeof(TTarget).Name}");
            }

            return fieldInfo.GetValue(obj);
        }

        /// <summary>
        /// Sets the (static) field with name "fieldName" of an object
        /// </summary>
        /// <param name="obj">The object containing the target field</param>
        /// <param name="fieldName">The name of the target field</param>
        /// <param name="value">The new value for the target field</param>
        /// <typeparam name="TTarget">The type of object containing the target field. Use this explicitly if the target type to target the baseclass if the field is defined on that one</typeparam>
        /// <exception cref="MissingFieldException">Thrown when field can't be found on target object</exception>
        public static void SetField<TTarget>(this TTarget obj, string fieldName, object value)
        {
            var fieldInfo = typeof(TTarget).GetField(fieldName, ALL_BINDING_FLAGS);
            if (fieldInfo == null)
            {
                throw new MissingFieldException($"Field {fieldName} not found on target object of type {typeof(TTarget).Name}");
            }

            fieldInfo.SetValue(obj, value);
        }

        /// <summary>
        /// Gets the value of (static) property with name "propertyName" of an object
        /// </summary>
        /// <param name="obj">The object containing the target property</param>
        /// <param name="propertyName">The name of the target property</param>
        /// <typeparam name="TTarget">The type of object containing the target property. Use this explicitly if the target type to target the baseclass if the property is defined on that one.</typeparam>
        /// <typeparam name="TReturn">The type of the returned value</typeparam>
        /// <exception cref="MissingMemberException">Thrown when property can't be found on target object</exception>
        /// <returns>The typed value of the property</returns>
        public static TReturn? GetProperty<TTarget, TReturn>(this TTarget obj, string propertyName)
        {
            return (TReturn?)GetProperty(obj, propertyName);
        }

        /// <summary>
        /// Gets the value of (static) property with name "propertyName" of an object
        /// </summary>
        /// <param name="obj">The object containing the target property</param>
        /// <param name="propertyName">The name of the target property</param>
        /// <typeparam name="TTarget">The type of object containing the target property. Use this explicitly if the target type to target the baseclass if the property is defined on that one</typeparam>
        /// <exception cref="MissingMemberException">Thrown when property can't be found on target object</exception>
        /// <returns>The raw value of the property</returns>
        public static object? GetProperty<TTarget>(this TTarget obj, string propertyName)
        {
            var propertyInfo = typeof(TTarget).GetProperty(propertyName, ALL_BINDING_FLAGS);
            if (propertyInfo == null)
            {
                throw new MissingMemberException($"Property {propertyName} not found on target object of type {typeof(TTarget).Name}");
            }

            return propertyInfo.GetValue(obj);
        }

        /// <summary>
        /// Sets the (static) property with name "propertyName" of an object
        /// </summary>
        /// <param name="obj">The object containing the target property</param>
        /// <param name="propertyName">The name of the target property</param>
        /// <param name="value">The new value for the target property</param>
        /// <typeparam name="TTarget">The type of object containing the target property. Use this explicitly if the target type to target the baseclass if the property is defined on that one</typeparam>
        /// <exception cref="MissingMemberException">Thrown when property can't be found on target object</exception>
        public static void SetProperty<TTarget>(this TTarget obj, string propertyName, object value)
        {
            var propertyInfo = typeof(TTarget).GetProperty(propertyName, ALL_BINDING_FLAGS);
            if (propertyInfo == null)
            {
                throw new MissingMemberException($"Property {propertyName} not found on target object of type {typeof(TTarget).Name}");
            }

            propertyInfo.SetValue(obj, value, null);
        }

        /// <summary>
        /// Converts the property name to the compiler-generated backing field.
        /// This can be used for the field-based reflection when you want to set the value of get-only property
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// <returns>Name of the backing field</returns>
        /// <remarks>
        /// Only works for properties with compiler-generated backing fields.
        /// This is only a simple method and doesn't have any guarantees to work 100% of the time across different compilers/runtimes.
        /// See <a href="https://github.com/dotnet/roslyn/blob/894751ff28d4ce19b8a02672f71ae4ce996c7fd2/src/Compilers/CSharp/Portable/Symbols/Synthesized/GeneratedNames.cs#L24-L28">this link</a> for more info.
        /// </remarks>
        public static string ToCompilerGeneratedBackingField(string propertyName) => $"<{propertyName}>k__BackingField";
    }
}
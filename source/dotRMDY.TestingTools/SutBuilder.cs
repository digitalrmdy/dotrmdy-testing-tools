using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FakeItEasy.Sdk;
using JetBrains.Annotations;

namespace dotRMDY.TestingTools
{
	[PublicAPI]
	public class SutBuilder
	{
		private readonly Type _sutType;

		private ConstructorInfo? _constructorInfo;
		protected ConstructorInfo ConstructorInfo => _constructorInfo ??= GetConstructor();

		private readonly Dictionary<string, object> _dependencies;

		public SutBuilder(Type sutType)
		{
			_sutType = sutType;
			_dependencies = new Dictionary<string, object>();
		}

		public object BuildRaw()
		{
			var constructorParameterInfos = ConstructorInfo.GetParameters();
			var constructorParameters = new object[constructorParameterInfos.Length];
			for (var i = 0; i < constructorParameterInfos.Length; i++)
			{
				var parameterInfo = constructorParameterInfos[i];
				if (_dependencies.TryGetValue(parameterInfo.Name!, out var customDependency))
				{
					constructorParameters[i] = customDependency;
				}
				else
				{
					if (!parameterInfo.ParameterType.IsInterface)
					{
						throw new NotSupportedException("Construction of type with non-interface parameters isn't supported");
					}

					constructorParameters[i] = Create.Fake(parameterInfo.ParameterType);
				}
			}

			return ConstructorInfo.Invoke(constructorParameters);
		}

		public TDep AddFakedDependency<TDep>() where TDep : class
		{
			var dependency = A.Fake<TDep>();
			return AddDependency(dependency);
		}

		public TDep AddDependency<TDep>(TDep dependency) where TDep : notnull
		{
			var parameters = GetParametersOfType(dependency);
			if (parameters.Length > 1)
			{
				throw new InvalidOperationException("There are multiple constructor arguments of the specified type. Please use AddNamedDependency(dependency, parameterName) instead.");
			}

			var parameterName = parameters.First().Name;
			if (string.IsNullOrWhiteSpace(parameterName))
			{
				throw new InvalidOperationException("Couldn't determine parameterName of dependency with type: " + typeof(TDep));
			}

			return AddNamedDependencyInternal(dependency, parameterName);
		}

		public TDep AddNamedDependency<TDep>(TDep dependency, string parameterName) where TDep : notnull
		{
			var parameters = GetParametersOfType(dependency);

			var parameter = parameters.FirstOrDefault(p => p.Name == parameterName);
			if (parameter == null)
			{
				throw new InvalidOperationException("Unable to find a constructor argument with name: " + parameterName);
			}

			return AddNamedDependencyInternal(dependency, parameterName);
		}

		private TDep AddNamedDependencyInternal<TDep>(TDep dependency, string paramName) where TDep : notnull
		{
			_dependencies.Add(paramName, dependency);
			return dependency;
		}

		private ParameterInfo[] GetParametersOfType<TDep>(TDep dependency) where TDep : notnull
		{
			var typeParams = ConstructorInfo.GetParameters().Where(p => p.ParameterType.IsInstanceOfType(dependency));
			var parameters = typeParams as ParameterInfo[] ?? typeParams.ToArray();
			if (typeParams == null || !parameters.Any())
			{
				throw new InvalidOperationException("Unable to find a constructor argument assignable to the specified type: " + typeof(TDep));
			}

			return parameters;
		}

		private ConstructorInfo GetConstructor()
		{
			var constructor = _sutType.GetConstructors().OrderByDescending(ctor => ctor.GetParameters().Length).FirstOrDefault();

			if (constructor == null)
			{
				throw new NotSupportedException("Construction of type without constructor isn't supported");
			}

			return constructor;
		}
	}

	[PublicAPI]
	public class SutBuilder<TSut> : SutBuilder
	{
		public SutBuilder() : base(typeof(TSut))
		{
		}

		public TSut Build()
		{
			return (TSut) BuildRaw();
		}
	}
}
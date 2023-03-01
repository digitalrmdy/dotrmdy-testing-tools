using System;
using JetBrains.Annotations;

namespace dotRMDY.TestingTools
{
	[PublicAPI]
	public abstract class SutSupportingTest
	{
		private readonly Type _sutType;

		protected object SutRaw { get; private set; } = default!;

		protected SutSupportingTest(Type sutType)
		{
			_sutType = sutType;
			CreateSut();
		}

		protected virtual void SetupCustomSutDependencies(SutBuilder builder)
		{
		}

		protected virtual void OnSutCreating()
		{
		}

		protected virtual void OnSutCreated()
		{
		}

		protected TSut SutAsType<TSut>()
		{
			return (TSut) SutRaw;
		}

		private void CreateSut()
		{
			var sutBuilder = new SutBuilder(_sutType);

			SetupCustomSutDependencies(sutBuilder);

			OnSutCreating();

			SutRaw = sutBuilder.BuildRaw();

			OnSutCreated();
		}
	}

	[PublicAPI]
	public abstract class SutSupportingTest<TSut> : SutSupportingTest
	{
		protected TSut Sut => SutAsType<TSut>();

		protected SutSupportingTest() : base(typeof(TSut))
		{
		}
	}
}
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace dotRMDY.TestingTools
{
	[PublicAPI]
	public class PropertyChangedTester
	{
		public List<string?> Changes { get; }

		public PropertyChangedTester(INotifyPropertyChanged target)
		{
			Changes = new List<string?>();

			target.PropertyChanged += (_, e) => Changes.Add(e.PropertyName);
		}

		public void AssertChangedProperties(params string[] propertyNames)
		{
			var missingProperties = FormatPropertyNames(propertyNames.Except(Changes));

			missingProperties.Should().BeEmpty("the following properties have not been raised");
		}

		public void AssertChangedPropertiesStrict(params string[] propertyNames)
		{
			var missingProperties = FormatPropertyNames(propertyNames.Except(Changes));
			var unexpectedChanges = FormatPropertyNames(Changes.Except(propertyNames));

			var failureMessageBuilder = new StringBuilder();

			if (missingProperties.Length > 0)
			{
				failureMessageBuilder.AppendLine($"The following properties have not been raised but were expected: {missingProperties}.");
			}

			if (unexpectedChanges.Length > 0)
			{
				failureMessageBuilder.AppendLine($"The following properties have been raised unexpectedly: {unexpectedChanges}.");
			}

			if (failureMessageBuilder.Length > 0)
			{
				Assert.Fail(failureMessageBuilder.ToString());
			}
		}

		private static string FormatPropertyNames(IEnumerable<string?> propertyNames)
		{
			return string.Join(", ", propertyNames.Select(propertyName => $"\"{propertyName}\""));
		}

		public void ResetChanges() => Changes.Clear();
	}
}
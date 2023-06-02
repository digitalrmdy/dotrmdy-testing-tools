using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using JetBrains.Annotations;

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
			missingProperties.Should().HaveLength(0, "the following properties have not been raised but were expected: {0}.", missingProperties);

			var unexpectedChanges = FormatPropertyNames(Changes.Except(propertyNames));
			unexpectedChanges.Should().HaveLength(0, "the following properties have been raised unexpectedly: {0}.", unexpectedChanges);
		}

		private static string FormatPropertyNames(IEnumerable<string?> propertyNames)
		{
			return string.Join(", ", propertyNames.Select(propertyName => $"\"{propertyName}\""));
		}

		public void ResetChanges() => Changes.Clear();
	}
}
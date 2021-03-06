﻿using System;
using System.Linq;
using System.Reflection;

namespace Morestachio.Formatter.Framework.Attributes
{
	/// <summary>
	///		When decorated by a function, it can be used to format in morestachio
	/// </summary>
	/// <seealso cref="System.Attribute" />
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
	public class MorestachioFormatterAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MorestachioFormatterAttribute"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="description">The description.</param>
		public MorestachioFormatterAttribute(string name, string description)
		{
			Name = name;
			Description = description;
			IsSourceObjectAware = true;
		}

		/// <summary>
		///		Gets or Sets whoever an Formatter should apply the <see cref="SourceObjectAttribute"/> to its first argument if not anywhere else present
		/// <para>If its set to true and no argument has an <see cref="SourceObjectAttribute"/>, the first argument will be used to determinate the source value</para>
		/// <para>If its set to false the formatter can be called globally without specifying and object first. This ignores the <see cref="SourceObjectAttribute"/></para>
		/// </summary>
		/// <value>Default true</value>
		public bool IsSourceObjectAware { get; set; }

		/// <summary>
		///		What is the "header" of the function in morestachio.
		/// </summary>
		public string Name { get; private set; }
		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		public string Description { get; private set; }
		/// <summary>
		/// Gets or sets the return hint.
		/// </summary>
		/// <value>
		/// The return hint.
		/// </value>
		public string ReturnHint { get; set; }
		/// <summary>
		/// Gets or sets the type of the output.
		/// </summary>
		/// <value>
		/// The type of the output.
		/// </value>
		public Type OutputType { get; set; }

		/// <summary>
		///		Validates the name of the formatter
		/// </summary>
		/// <returns></returns>
		public virtual bool ValidateFormatterName()
		{
			if (string.IsNullOrWhiteSpace(Name))
			{
				return true;
			}

			return MorestachioFormatterService.ValidateFormatterNameRegEx.IsMatch(Name);
		}

		///  <summary>
		/// 		Validates the formatter
		///  </summary>
		///  <param name="method"></param>
		///  <returns></returns>
		public virtual void ValidateFormatter(MethodInfo method)
		{
			if (!ValidateFormatterName())
			{
				throw new InvalidOperationException(
					$"The name '{Name}' is invalid. An Formatter may only contain letters and cannot start with an digit");
			}
		}

		/// <summary>
		///		Gets all parameters from an method info
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public virtual MultiFormatterInfo[] GetParameters(MethodInfo method)
		{
			var arguments = method.GetParameters().Select((e, index) =>
				new MultiFormatterInfo(
					e.ParameterType,
					e.GetCustomAttribute<FormatterArgumentNameAttribute>()?.Name ?? e.Name,
					e.IsOptional,
					index,
					e.GetCustomAttribute<ParamArrayAttribute>() != null ||
					e.GetCustomAttribute<RestParameterAttribute>() != null)
				{
					IsSourceObject = IsSourceObjectAware &&
					                 e.GetCustomAttribute<SourceObjectAttribute>() != null,
					FormatterValueConverterAttribute =
						e.GetCustomAttributes<FormatterValueConverterAttribute>().ToArray(),
					IsInjected = e.GetCustomAttribute<ExternalDataAttribute>() != null
				}).ToArray();

			//if there is no declared SourceObject then check if the first object is of type what we are formatting and use this one.
			if (!arguments.Any(e => e.IsSourceObject) && arguments.Any() && IsSourceObjectAware)
			{
				arguments[0].IsSourceObject = true;
			}

			var sourceValue = arguments.FirstOrDefault(e => e.IsSourceObject);
			if (sourceValue != null)
			{
				//if we have a source value in the arguments reduce the index of all following 
				//this is important as the source value is never in the formatter string so we will not "count" it 
				for (var i = sourceValue.Index; i < arguments.Length; i++)
				{
					arguments[i].Index--;
				}

				sourceValue.Index = -1;
			}

			return arguments;
		}
	}
}
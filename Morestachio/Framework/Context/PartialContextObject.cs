﻿using System.Collections.Generic;
using JetBrains.Annotations;
using Morestachio.Document;
using Morestachio.Framework.Expression;
using Morestachio.Framework.Expression.Framework;

namespace Morestachio.Framework.Context
{
	/// <summary>
	///		The context object that will be used for the root of 
	/// </summary>
	public class PartialContextObject : ContextObject
	{
		/// <inheritdoc />
		public PartialContextObject([NotNull] ParserOptions options, [NotNull] string key, [CanBeNull] ContextObject parent, object value) : base(options, key, parent, value)
		{
		}

		/// <inheritdoc />
		protected override ContextObject HandlePathContext(
			Traversable elements,
			KeyValuePair<string, PathType> currentElement,
			IMorestachioExpression morestachioExpression, 
			ScopeData scopeData)
		{
			if (currentElement.Value != PathType.DataPath || !currentElement.Key.StartsWith("$"))
			{
				return null;
			}
			object value = null;
			if (currentElement.Key.Equals("$first"))
			{
				value = scopeData.PartialDepth.Count;
			}
			return value == null ? null : Options.CreateContextObject(currentElement.Key, CancellationToken, value, this);
		}
	}
}
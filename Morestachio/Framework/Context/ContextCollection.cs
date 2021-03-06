﻿using System.Collections.Generic;
using JetBrains.Annotations;
using Morestachio.Document;
using Morestachio.Framework.Expression;
using Morestachio.Framework.Expression.Framework;

namespace Morestachio.Framework.Context
{
	/// <summary>
	///     A context object for collections that is generated for each item inside a collection
	/// </summary>
	public class ContextCollection : ContextObject
	{
		/// <summary>
		///     ctor
		/// </summary>
		/// <param name="index">the current index of the item inside the collection</param>
		/// <param name="last">true if its the last item</param>
		/// <param name="options"></param>
		/// <param name="key"></param>
		public ContextCollection(long index, 
			bool last, 
			[NotNull] ParserOptions options, 
			string key, 
			[CanBeNull]ContextObject parent,
			object value) 
			: base(options, key, parent, value)
		{
			Index = index;
			Last = last;
		}

		/// <summary>
		///     The current index inside the collection
		/// </summary>
		public long Index { get; internal set; }

		/// <summary>
		///     True if its the last item in the current collection
		/// </summary>
		public bool Last { get; internal set; }

		/// <inheritdoc />
		protected override ContextObject HandlePathContext(Traversable elements,
			KeyValuePair<string, PathType> path,
			IMorestachioExpression expression, ScopeData scopeData)
		{
			if (path.Value != PathType.DataPath || !path.Key.StartsWith("$"))
			{
				return null;
			}
			
			object value = null;
			if (path.Key.Equals("$first"))
			{
				value = Index == 0;
			}
			else if (path.Key.Equals("$index"))
			{
				value = Index;
			}
			else if (path.Key.Equals("$middel"))
			{
				value = Index != 0 && !Last;
			}
			else if (path.Key.Equals("$last"))
			{
				value = Last;
			}
			else if (path.Key.Equals("$odd"))
			{
				value = Index % 2 != 0;
			}
			else if (path.Key.Equals("$even"))
			{
				value = Index % 2 == 0;
			}
			return value == null ? null : Options.CreateContextObject(path.Key, CancellationToken, value, this);
		}
	}
}
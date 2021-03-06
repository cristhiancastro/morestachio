﻿#if ValueTask
using ItemExecutionPromise = System.Threading.Tasks.ValueTask<System.Collections.Generic.IEnumerable<Morestachio.Document.Contracts.DocumentItemExecution>>;
using Promise = System.Threading.Tasks.ValueTask;
#else
using ItemExecutionPromise = System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<Morestachio.Document.Contracts.DocumentItemExecution>>;
#endif
using System;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using Morestachio.Document.Contracts;
using Morestachio.Document.Items.Base;
using Morestachio.Document.Visitor;
using Morestachio.Framework.Context;
using Morestachio.Framework.IO;
using Morestachio.Helper;

namespace Morestachio.Document.Items
{
	/// <summary>
	///		Defines an else Expression. This expression MUST come ether directly or only separated by <see cref="ContentDocumentItem"/> after an <see cref="IfExpressionScopeDocumentItem"/> or an <see cref="InvertedExpressionScopeDocumentItem"/>
	/// </summary>
	[Serializable]
	public class ElseExpressionScopeDocumentItem : DocumentItemBase
	{       
		/// <summary>
		///		Used for XML Serialization
		/// </summary>
		public ElseExpressionScopeDocumentItem()
		{
			
		}
		
		/// <inheritdoc />
		[UsedImplicitly]
		protected ElseExpressionScopeDocumentItem(SerializationInfo info, StreamingContext c) : base(info, c)
		{
		}
		
		/// <inheritdoc />
		public override ItemExecutionPromise Render(IByteCounterStream outputStream, ContextObject context, ScopeData scopeData)
		{
			if (scopeData.ExecuteElse)
			{
				scopeData.ExecuteElse = false;
				return Children.WithScope(context).ToPromise();
			}

			scopeData.ExecuteElse = false;
			return Enumerable.Empty<DocumentItemExecution>().ToPromise();
		}
		
		/// <inheritdoc />
		public override void Accept(IDocumentItemVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
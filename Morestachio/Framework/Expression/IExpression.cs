﻿using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Morestachio.Document;

namespace Morestachio.Framework.Expression
{
	/// <summary>
	///		Defines an Morestachio expression that contains dynamic data
	/// </summary>
	public interface IExpression : ISerializable, IXmlSerializable, IEquatable<IExpression>
	{
		/// <summary>
		///		Where in the original template was this expression located
		/// </summary>
		CharacterLocation Location { get; set; }

		/// <summary>
		///		The method for obtaining the Value
		/// </summary>
		/// <param name="contextObject"></param>
		/// <param name="scopeData"></param>
		/// <returns></returns>
		Task<ContextObject> GetValue(ContextObject contextObject, ScopeData scopeData);
	}
}
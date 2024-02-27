﻿using System.Linq.Expressions;
using Jellyfin.Plugin.SmartPlaylist.Models;

namespace Jellyfin.Plugin.SmartPlaylist.QueryEngine.Operators;

public class IsNullOperator : IOperator
{
	private static readonly ConstantExpression NullExpression = Expression.Constant(null);
	/// <inheritdoc />
	public EngineOperatorResult ValidateOperator<T>(SmartPlExpression   plExpression,
													MemberExpression    sourceExpression,
													ParameterExpression parameterExpression,
													Type                parameterPropertyType) {
		if (plExpression.OperatorAsLower is ("null" or "isnull")) {
			return EngineOperatorResult.Success();
		}

		return EngineOperatorResult.NotAValidFor();
	}

	/// <inheritdoc />
	public Expression GetOperator<T>(SmartPlExpression   plExpression,
									 MemberExpression    sourceExpression,
									 ParameterExpression parameterExpression,
									 Type                parameterPropertyType) {
		return Expression.MakeBinary(ExpressionType.Equal, sourceExpression, NullExpression);
	}
}

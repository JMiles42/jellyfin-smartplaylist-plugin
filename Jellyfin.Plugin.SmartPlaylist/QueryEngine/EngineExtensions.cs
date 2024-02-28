﻿using System.Linq.Expressions;
using System.Reflection;

namespace Jellyfin.Plugin.SmartPlaylist.QueryEngine;

internal static class EngineExtensions {
	public static readonly MethodInfo StringArrayContainsMethodInfo = typeof(EngineExtensions)!.GetMethod(nameof(StringArrayContains), BindingFlags.Static | BindingFlags.Public);

	public static bool StringArrayContains(this IReadOnlyCollection<string> l, string r, StringComparison stringComparison) {
		return l.Any(a => a.Contains(r, stringComparison));
	}

	public static Expression InvertIfTrue(this Expression expression, bool invert) => invert ? Expression.Not(expression) : expression;

	public static Expression ToConstantExpressionAsType<T>(this object value) {
		if (value is T tValue) {
			return Expression.Constant(tValue);
		}

		return Expression.Constant(Convert.ChangeType(value, typeof(T)));
	}

	public static Expression ToConstantExpressionAsType(this object value, Type type) {
		if (value?.GetType() == type) {
			return Expression.Constant(value);
		}

		return Expression.Constant(Convert.ChangeType(value, type));
	}

	public static Expression ToConstantExpression<T>(this T value) => Expression.Constant(value);

	private static readonly DateTime _origin = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

	public static double ConvertToUnixTimestamp(this DateTime date) {
		var diff = date.ToUniversalTime() - _origin;

		return Math.Floor(diff.TotalSeconds);
	}
}
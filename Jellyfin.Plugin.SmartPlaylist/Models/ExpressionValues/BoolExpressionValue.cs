﻿namespace Jellyfin.Plugin.SmartPlaylist.Models.ExpressionValues;

public record BoolExpressionValue: ExpressionValue<bool>
{

	public static BoolExpressionValue True { get; } = new(true);

	public static BoolExpressionValue False { get; } = new(false);

	public BoolExpressionValue(bool value): base(value) { }
}

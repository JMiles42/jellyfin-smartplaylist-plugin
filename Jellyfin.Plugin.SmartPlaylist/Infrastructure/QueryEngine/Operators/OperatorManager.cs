﻿namespace Jellyfin.Plugin.SmartPlaylist.Infrastructure.QueryEngine.Operators;

public class OperatorManager
{
    public IOperator[] EngineOperators { get; } =
    {
        new TimeSpanOperator(),
        new StringOperator(),
        new IsNullOperator(),
        new StringListContainsSubstringOperator(),
        new StringListContainsOperator(),
        new RegexOperator(),
        new ExpressionTypeOperator(),
    };
}

﻿namespace Jellyfin.Plugin.SmartPlaylist.Infrastructure.QueryEngine.Containers;

public class ParsedValueExpressions : IReadOnlyCollection<ParsedValueExpressionResult>
{

    private readonly List<ParsedValueExpressionResult> _builtExpressionResult;

    public SmartPlExpression PlaylistExpression { get; init; }

    public MatchMode Match => PlaylistExpression.Match;

    public IReadOnlyList<ParsedValueExpressionResult> BuiltExpressionResult => _builtExpressionResult;

    public ParsedValueExpressions(SmartPlExpression playlistExpression)
    {
        PlaylistExpression = playlistExpression;
        _builtExpressionResult = new();
    }

    public ParsedValueExpressions(SmartPlExpression playlistExpression, ParsedValueExpressionResult singleResult)
    {
        PlaylistExpression = playlistExpression;
        _builtExpressionResult = new() { singleResult, };
    }

    public ParsedValueExpressions(SmartPlExpression playlistExpression, IEnumerable<ParsedValueExpressionResult> builtRange)
    {
        PlaylistExpression = playlistExpression;
        _builtExpressionResult = new(builtRange);
    }

    public void Add(ParsedValueExpressionResult expression) => _builtExpressionResult.Add(expression);

    /// <inheritdoc />
    public IEnumerator<ParsedValueExpressionResult> GetEnumerator() => BuiltExpressionResult.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public int Count => BuiltExpressionResult.Count;
}

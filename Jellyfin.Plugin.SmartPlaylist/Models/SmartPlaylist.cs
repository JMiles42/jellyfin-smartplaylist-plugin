using Jellyfin.Data.Entities;
using Jellyfin.Data.Enums;
using Jellyfin.Plugin.SmartPlaylist.Models.Dto;
using Jellyfin.Plugin.SmartPlaylist.QueryEngine.Ordering;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;

namespace Jellyfin.Plugin.SmartPlaylist.Models;

public class SmartPlaylist {
    public string Id { get; set; }

    public string Name { get; set; }

    public string FileName { get; set; }

    public string User { get; set; }

    public IReadOnlyList<ExpressionSet> ExpressionSets { get; set; }

    public int MaxItems { get; set; }
    public bool IsReadonly { get; set; }

    public OrderStack Order { get; set; }

    public BaseItemKind[] SupportedItems { get; set; }

    public SmartPlaylist(SmartPlaylistDto dto) {
        Id = dto.Id;
        Name = dto.Name;
        FileName = dto.FileName;
        User = dto.User;
        ExpressionSets = Engine.FixRuleSets(dto.ExpressionSets);

        if (dto.MaxItems > 0) {
            MaxItems = dto.MaxItems;
        }
        else {
            MaxItems = 1000;
        }

        Order = GenerateOrderStack(dto.Order);
        SupportedItems = dto.SupportedItems;
    }

    private static OrderStack GenerateOrderStack(OrderByDto dtoOrder) {
        var result = new List<Order>(1 + (dtoOrder.ThenBy?.Count ?? 0)) {
                OrderManager.GetOrder(dtoOrder)
        };

        if (dtoOrder.ThenBy?.Count > 0) {
            foreach (var order in dtoOrder.ThenBy) {
                result.Add(OrderManager.GetOrder(order));
            }
        }

        return new(result.ToArray());
    }

    internal List<List<Func<Operand, bool>>> CompileRuleSets() {
        var compiledRuleSets = new List<List<Func<Operand, bool>>>();

        foreach (var set in ExpressionSets) {
            compiledRuleSets.Add(set.Expressions.Select(Engine.CompileRule<Operand>).ToList());
        }

        return compiledRuleSets;
    }

    // Returns the BaseItems that match the filter, if order is provided the IDs are sorted.
    public IEnumerable<Guid> FilterPlaylistItems(IEnumerable<BaseItem> items,
                                                     ILibraryManager libraryManager,
                                                     User user) {

        var results = new List<BaseItem>();

        var compiledRules = CompileRuleSets();
        foreach (var i in items) {
            var operand = OperandFactory.GetMediaType(libraryManager, i, user);

            if (compiledRules.Any(set => set.All(rule => rule(operand)))) {
                results.Add(i);
            }
        }

        var enumerable = Order.OrderItems(results);

        return enumerable.Select(bi => bi.Id);
    }
}

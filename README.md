# Smart Playlist 2 Playlist Harder Plugin for Jellyfin

This is a more recent forked & upgraded version of [ankenyr](https://github.com/ankenyr/jellyfin-smartplaylist-plugin) plugin, which hadn't had updates in quite a while, and I found it a quite useful plugin and wanted to improve it.

## Overview

This plugin allows you to setup a series of rules to auto generate and update playlists within Jellyfin.
With the current implementation these are limited to an individual user.

The json format currently in use, may not be the final version while I continue to work on this forked version.
Currently I am trying to keep backwards compatibility in mind. For example with the ordering, instead of changing the base OrderBy to an array I added the nested ThenBy field, to allow old ones to work.
If I do make breaking changes, for my own sake as well, I will try to allow the older version to be compatible as well, but no guarantees. 


## 2.2 ExpressionVars, more values less writing
WARNING: This update technically breaks any TargetValues that is a single string that starts with `$(` and ends with `)`. I currently don't have an option to escape this.
In this update adds reusable vars, you can now set any uniquely named entries in the ExpressionVars section, then to use set the TargetValue of "$(KeyName)",
Where KeyName is the key you chose to assign the value ExpressionValue you would like to use. See the now updated json example below.
I slightly tweaked how the files are parsed, based on my limited testing, all existing files should still work identically, however may save slightly differently, by adding the new ExpressionVars


## Warning as of the release of 2.* the save format has had changes.
The format has had some updates, to the best of my knowledge the old formats should still be readable, and saved they will be written in the new format.
Changes to the format include
 - Removal of "FileName" element. It now users the path relative to the smartplaylist folder, and no longer relies on this field.
 - GUIDs eg the ID, now saves with dashes
 - Order, is now an array, rather then an Object with a ThenBy array.
   - Order objects can be read as a single string of the order property name, this will default to be ascending
 - Removed the OperandMember enum, replaced with just a string, which gets validated on inital read.

## How to Install

To use this plugin download the DLL and place it in your plugin directory. Once launched you should find in your data directory a folder called "smartplaylist" (as of 2.* configurable via the plugin config page). Your JSON files describing a playlist go in here, or any subfolders.

## Configuration

To create a new playlist, create a json file in this directory having a format such as the following.

```json
{
  "Id": "7be77ebb-859b-34ee-ecfd-f56a7cc8bdd4",
  "Name": "Unplayed Youtube or X user",
  "User": "JMiles42",
  "ExpressionSets": [
    {
      "Expressions": [
        {
          "MemberName": "Directors",
          "Operator": "StringListContainsSubstring",
          "TargetValue": "CGP Grey",
          "StringComparison": "OrdinalIgnoreCase"
        },
        {
          "MemberName": "IsPlayed",
          "Operator": "Equal",
          "TargetValue": "False"
        }
      ],
      "Match": "All"
    },
    {
      "Expressions": [
        {
          "MemberName": "Directors",
          "Operator": "StringListContainsSubstring",
          "TargetValue": "$(DirectorNames)",
          "StringComparison": "OrdinalIgnoreCase",
          "Match": "Any"
        }
      ]
    }
  ],
  "ExpressionVars"{
    "DirectorNames": [
        "Nerdwriter1",
        "The Spiffing Brit"
    ]
  },
  "Order": [
    "ReleaseDate",
    "OriginalTitle",
    {
      "Name": "RunTimeTicks",
      "Ascending": false
    },
    {
      "Name": "HasSubtitles",
      "Ascending": true
    }
  ],
  "SupportedItems": [
    "Audio",
    "Episode",
    "Movie"
  ]
}
```

- Id: This field is created after the playlist is first rendered. Do not fill this in yourself.
- Name: Name of the playlist as it will appear in Jellyfin
- FileName: The actual filename. If you create a file named cgpgrey_playlist.json then this should be cgpgrey_playlist
- User: Name of the user for the playlist
- ExpressionSets: This is a list of Expressions. Each expression is OR'ed together.
- ExpressionVars: This is a dictionary of values to be used inplace of in line TargetValues with the "$(KeyName)" syntax
- Expressions: This is the meat of the plugin. Expressions are a list of maps containing MemberName, Operator, and TargetValue. I am working on a list of all valid things. Currently there are three sets of expressions:

  - Universal LINQ expression operators: [This link](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.expressiontype?redirectedfrom=MSDN&view=net-6.0) is a list of all valid operators within expression trees but only a subset are valid for a specific operand.
  - String operators: Equals, StartsWith, EndsWith, Contains.
  - Regex operators: MatchRegex, NotMatchRegex.
  - StringListContainsSubstring: This is basically a string contains, but searches all entries in a list, such as Directors, Genres or Tags.

- MemberName: This is a reference to the properties in [Operand](https://github.com/JMiles42/jellyfin-smartplaylist2playlistharder-plugin/blob/master/Jellyfin.Plugin.SmartPlaylist/Infrastructure/QueryEngine/Operand.cs "Operand"). You set this string to one of the property names to reference what you wish to filter on.
- Operator: An operation used to compare the TargetValue to the property of each piece of media. The above example would match anything with the director set as CGP Grey with a Premiere Date less than 2020/07/01
- TargetValue: The value to be compared to. Most things are converted into strings, booleans, or numbers. A date in the above example is converted to seconds since epoch.
- InvertResult: This allows you to invert any Expression, regardless of it's type, name or operator.
- StringComparison: Only used specifically on string comparisons, this can allow you to ignore case, when using a value such as OrdinalIgnoreCase [Microsoft Docs](https://learn.microsoft.com/en-us/dotnet/api/system.stringcomparer.ordinalignorecase?view=net-6.0)


- Order: Provides the type of sorting you wish for the playlist. The following are a list of valid sorting methods so far.
- These generally match the property name in the BaseItem object, if it accepts multiple names, they will  be split by |
  - Album
  - ChannelId
  - Container
  - DateLastRefreshed
  - DateLastSaved
  - DateModified
  - EndDate
  - ForcedSortName
  - Height
  - Id
  - MediaType
  - Name
  - NoOrder
  - OriginalTitle
  - Overview
  - Path
  - ProductionYear|Year
  - Release Date|ReleaseDate|PremiereDate
  - SortName
  - Tagline
  - Width
  - RandomOrder|Random|RNG|DiceRoll;

## Future work
- Add Top X to limit playlist size (✅ Implimented)
- Add Library filter to only allow specific libraries in the Playlist
- Add AND clause to existing ORs, eg any current caluse, AND a new one.
- Maybe: Add Dynamic Playlist Generater
  - This should allow you to setup rules to generate many playlists automatically.
  - eg, you want a playlist of the the last 5 days of any {Director}s videos
    this would generate a playlist for every {Director} that has a video uploaded in the past 5 days.

-Optimize playlist generators by pre grouping libaray query modes, eg user and item types, so it's not retriving the whole library for every playlist.

-Playlist refresh changes
  - Add IsReadonly (✅ Implimented)
  - Playlist specifc refresh rate (Unsure)
- More tests
  - An aim to have tests to test comparing expressions with actual JellyFin library documents.
  - More parsing and saving checks
- Add in more properties to be matched against. Please file a feature request if you have ideas.
- Document all operators that are valid for each property.
- Explore creating custom property types with custom operators.
- Once Jellyfin allows for custom web pages beyond the configuration page, explore how to allow configuration from the web interface rather than JSON files.
- CICD pipeline with automatic updates to latest JellyFin realease if there arn't breaking changes.

## Credits

Original plugin this fork was based off of [ankenyr](https://github.com/ankenyr/jellyfin-smartplaylist-plugin)
Rule engine was inspired by [this](https://stackoverflow.com/questions/6488034/how-to-implement-a-rule-engine "this") post in Stack Overflow.
Initially wanted to convert [ppankiewicz's plugin](https://github.com/ppankiewicz/Emby.SmartPlaylist.Plugin "ppankiewicz's plugin") but found it to be too incompatible and difficult to work with. I did take some bits of code mostly around interfacing with the filesystem.

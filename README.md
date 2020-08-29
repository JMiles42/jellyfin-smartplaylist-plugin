# Smart Playlist Plugin for Jellyfin
## Overview
This is an attempt to make a smart playlist similar to what iTunes, Plex, and other media players have. It is still in early development and has some limitations which will not be able to be resolved until Jellyfin impliments some features.

## How to Install
To use this plugin download the DLL and place it in your plugin directory.  Once launched you should find in your data directory a folder called "smartplaylist". Your JSON files describing a playlist go in here.

## Configuration
To create a new playlist, create a json file in this directory having a format such as the following.

```json
{
   "Name":"Works of CGP Grey",
   "FileName":"my_cgp_playlist",
   "User":"rob",
   "Expressions":[
      {
         "MemberName":"Directors",
         "Operator":"Contains",
         "TargetValue":"CGP Grey"
      },
      {
         "MemberName":"PremiereDate",
         "Operator":"LessThan",
         "TargetValue":"2020/07/01"
      }
   ]
}
```
- Name: Name of the playlist as it will appear in Jellyfin
- FileName: The actual filename. If you create a file named cgpgrey_playlist.json then this should be cgpgrey_playlist
- User: Name of the user for the playlist
- Expressions: This is the meat of the plugin. Expressions are a list of maps containing MemberName, Operator, and TargetValue. I am working on a list of all valid things. [This link](msdn.microsoft.com/en-us/library/bb361179.aspx "This link") is a list of all valid operators within expression trees but only a subset are valid for this.

- MemberName: This is a reference to the properties in [Operand](https://github.com/ankenyr/jellyfin-smartplaylist-plugin/blob/master/Jellyfin.Plugin.SmartPlaylist/QueryEngine/Operand.cs "Operand"). You set this string to one of the property names to reference what you wish to filter on.
- Operator: An operation used to compare the TargetValue to the property of each piece of media. The above example would match anything with the director set as CGP Grey with a Premiere Date less than 2020/07/01
- Target Value: The value to be compared to. Most things are converted into strings, booleans, or numbers. A date in the above example is converted to seconds since epoch.

## Future work
- Add in more properties to be matched against. Please file aa feature request if you have ideas.
- Document all operators that are valid for each property.
- Explore creating custom property types with custom operators.
- Once Jellyfin allows for custom web pages beyond the configuration page, explore how to allow configuration from the web interface rather than JSON files.
- Add ability to provide sorting method for playlists

##Credits
Rule engine was inspired by [this](https://stackoverflow.com/questions/6488034/how-to-implement-a-rule-engine "this") post in Stack Overflow.
Initially wanted to convert [ppankiewicz's plugin](https://github.com/ppankiewicz/Emby.SmartPlaylist.Plugin "ppankiewicz's plugin") but found it to be too incompatible and difficult to work with. I did take some bits of code mostly around interfacing with the filesystem.
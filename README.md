# PlexMatch File Generator
This application generates a .plexmatch file in the directory of all shows and movies added to your Plex Server. This is especially useful for migrating storage devices if you have some shows that needed a custom match.

# Usage
The command is expecting 2 arguments from the command line, it will prompt for these values if you do not provide them
- Plex Server Token (-t or --token)
- Plex Server Url (-u or --url)

For information on how to get your Plex token, see this support link: https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/

## Examples

`./PlexMatchGenerator-Linux-x64 -u http://192.168.0.3:32400 -t ABCD12345`

`./PlexMatchGenerator-Linux-x64 --url http://192.168.0.3:32400 --token ABCD12345`

`PlexMatchGenerator-Windows-x86.exe -u http://192.168.0.3:32400 -t ABCD12345`

`PlexMatchGenerator-Windows-x86.exe --url http://192.168.0.3:32400 --token ABCD12345`

# 3rd Party Packages Used
- [RestSharp](https://restsharp.dev/)
- [Newtonsoft.Json](https://www.newtonsoft.com/json)

# Development Environment Setup
- Clone repo
- Open folder with VS Code
- Ensure C# support is added (it should prompt if not)
- Restore Nuget packages (command line: dotnet restore)
- Application should build immediately out of box

# Contributing
Contributions are welcome!
- Fork the repo
- Make your change
- Submit a Pull Request, Ensure to include details of what problem is fixed, what is improved, etc. If it is in response to an issue, tag the issue on the PR
- Respond to and/or make changes based on PR comments
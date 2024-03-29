# PlexMatch File Generator

This application generates a .plexmatch file in the directory of all shows and movies added to your Plex Server. This is especially useful for migrating storage devices if you have some shows that needed a custom match.

## Usage

The command is expecting 2 arguments from the command line

- Plex Server Token (-t or --token)
- Plex Server Url (-u or --url)

For information on how to get your Plex token, see this support link: [https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/](https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/)

As of release 0.9.2-rc2 the default behavior for TV Show processing is to perform per season processing when episode ordering is set to anything other than library default. This action preserves the episode ordering as currently set.

### Optional Parameters

- Modify root path (-r or --root)
  - Use this option to set the root path used to be different than what your Plex server returns. For instance if your Plex container has the media mounted to /media and the computer running the application has it mounted to /mnt/media
  - Usage: -r /path/on/host:/path/on/plex
- Set log path (-l or --log)
  - This will output the log to file at the path specified, log file will be named `plexmatch.log` in the directory specified. The path specified must exist! If this option is not specified, no log file will be output.
  - Usage: -l /home/user/logs
- Per season processing of TV Shows (-sp or --seasonprocessing)
  - This will process each season of each TV Show individually writing a plexmatch file for both the show as a whole and each season
  - This behavior is the default for any show with the episode sorting set to anything other than the library default
  - Usage: -sp
- Restrict to specific libraries (-lib or --library)
  - This parameter can be specified multiple times
  - Each library specified is added to the allow list
  - If even 1 library is added, every library not matching will be ignored
  - Library names are **not** case-sensitive
  - Usage: -lib TV
  - Usage: -lib "TV Shows"
- Restrict to specific media items (-s or --show)
  - Despite the parameter name, this can be used on any library and is not restricted to TV Shows
  - This works best when specified with -lib as otherwise this will have a significant performance impact on larger libraries
  - This parameter can be specified multiple times
  - Each media item specified is added to the allow list
  - If even 1 media item is added, every media item not matching will be ignored
  - Media item names are **not** case-sensitive
  - Usage: -s firefly
- Get version (--version)
  - This will output the version of the executable being run
- Disable Automatic Overwrite (--nooverwrite or -no)
  - This will skip writing any file if it already exists instead of overwriting it.
- Set page size for batch processing (--pagesize or -ps)
  - This will change the default batch processing size. Default is 20 items
  - Usage: -ps 10
- View help (--help)
  - This will output the parameter help

### Examples

`./PlexMatchGenerator-Linux-x64 -u http://192.168.0.3:32400 -t ABCD12345`

`./PlexMatchGenerator-Linux-x64 --url http://192.168.0.3:32400 --token ABCD12345`

`PlexMatchGenerator-Windows-x86.exe -u http://192.168.0.3:32400 -t ABCD12345`

`PlexMatchGenerator-Windows-x86.exe --url http://192.168.0.3:32400 --token ABCD12345`

## Donations

Donations are always accepted but never required. Currently I accept PayPal using the button below.

[![Paypal Donation Image Button](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/donate/?business=XPYMV5XQG8JCN&no_recurring=0&currency_code=USD)

## 3rd Party Packages Used

- [RestSharp](https://restsharp.dev/)
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
- [Serilog](https://serilog.net/)

## Development Environment Setup

- Clone repo
- Open folder with VS Code
- Ensure C# support is added (it should prompt if not)
- Restore Nuget packages (command line: dotnet restore)
- Application should build immediately out of box

## Contributing

Contributions are welcome!

- Fork the repo
- Make your change
- Submit a Pull Request, Ensure to include details of what problem is fixed, what is improved, etc. If it is in response to an issue, tag the issue on the PR
- Submit your PR to merge into the Develop branch, merge requests to the main branch will be denied
- Respond to and/or make changes based on PR comments

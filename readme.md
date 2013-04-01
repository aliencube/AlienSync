# AlienSync #

This will help your team's source code repository up-to-date by synchronising the repository with a website where developers (either front-end or back-end) directly handle source files (mainly CSS, image and javascript files), who are not interested in a source code management system. In addition to the source code repository synchronisation, this will also help databases get synchronised.


## Prerequisites ##

**AlienSync** requires the following to be installed on your box depending on your purpose of use:

### Common ###

- [Micrisoft .NET Framework 4.5](http://www.microsoft.com/en-us/download/details.aspx?id=30653)

### AlienSync for SCP ###

- [WinSCP](http://winscp.net/eng/download.php)

### AlienSync for Git

- [WinSCP](http://winscp.net/eng/download.php)
- [Git for Windows](https://code.google.com/p/msysgit/downloads/list?q=full+installer+official+git)

### AlienSync for Hg (To be implemented) ###

- [WinSCP](http://winscp.net/eng/download.php)
- [Mercurial](http://mercurial.selenic.com/downloads)

### AlienSync for MS-SQL ###

- [MS-SQL Server 2008 or Express or later](http://www.microsoft.com/en-us/sqlserver/get-sql-server/try-it.aspx)


## Download Builds ##

Download pre-built applications are available at [BitBucket](https://bitbucket.org/aliencube/aliensync/downloads).


## Getting Started ##

In order to run any of **AlienSync**, a configuration file - `AlienSync.Scp.exe.config`, `AlienSync.Git.exe.config`, `AlienSync.Hg.exe.config` or `AlienSync.MsSql.exe.config` - needs to be setup beforehand.


### AlienSync for SCP ###

Once you setup the configuration, simply run `AlienSync.Scp.exe` and it will do the rest for you.


### AlienSync for Git ###

Once you setup the configuration, simply run `AlienSync.Git.exe` and it will do the rest for you. You might be asked to enter the password to access to the remote repository, if you use HTTP(S) connection for your repository. In order not to be prompted in this case, you should modify your `.git/config` file having your password, which is less secure.


### AlienSync for Hg (To be implemented) ###

To be documented.


### AlienSync for MS-SQL ###

Once you setup the configuration, simply run `AlienSync.MsSql.exe` and it will do the rest for you.


## Documentation ##

**AlienSync** is documented in this [wiki](https://github.com/aliencube/AlienSync/wiki) page.


## Issue Trackers ##

Should you have any issue in regards to **AlienSync**, please raise it on this [issue](https://github.com/aliencube/AlienSync/issues) page.


## Change History ##

### 0.8.0.0 ###

- MS-SQL Synchronization tool added
- Existing codes optimized.

### 0.7.0.0 ###

- Initial release


## License ##

**AlienSync** is released under the [MIT License](http://opensource.org/licenses/MIT).

> The MIT License (MIT)
> Copyright (c) 2013 [aliencube.org](http://aliencube.org)
> 
> Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
> 
> The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
> 
> THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


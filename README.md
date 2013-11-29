Pablo.Gallery
=============

Ansi/ascii/rip/image gallery using the PabloDraw or other conversion engines as a backend.


Prerequisites
-------------

### OS X and Linux

- [Mono 3.2.x](http://www.go-mono.com/mono-downloads/) or higher
- [Xamarin Studio / MonoDevelop 4.2](http://monodevelop.com/Download) (for development)
- [Nuget add-in](https://github.com/mrward/monodevelop-nuget-addin) for Xamarin Studio / MonoDevelop
- PostgreSQL

#### Ubuntu

In ubuntu 12.04 or 13.x, add the following ppa to your sources:

	ppa:v-kukol/mono-testing
	
Then, install:

	sudo apt-get install mono-complete mono-xsp4 mono-fastcgi-server4


You can then run `xsp4` in the web directory to start the application for testing, or use the fastcgi server from nginx for production.


### Windows

- .NET Framework 4.0 or 4.5
- Visual Studio 2012/2013 or Visual Studio Express 2013 for Web
- SQL Server / Express, or PostgreSQL

Database Setup
--------------

First, you must create a database. Follow the corresponding section for the database you'd like to use.  Then, update the &lt;connectionStrings&gt; section to have a "Gallery" connection string that points to your database.

### PostgreSQL

For PostgreSQL, create a new user and database.  Create a schema called 'gallery'. Run the script located at `scripts/gallery-postgresql.sql` on that schema.

### SQL Server / Express

To create a SQL Server database, run the `Update-Database` command from the package manager console.

### MySQL

MySQL has not been tested yet nor set up, however theoretically you should be able to use MySQL or derivitave.  If the MySQL .NET driver doesn't support migrations, the database would have to be created manually (no script is provided).


Configuration Setup
-------------------

There are a few changes in the Web.config to make for the system to run. Update the following in the &lt;appSettings&gt; section to suit your environment:

- **MonoPath** - If running in a mono environment, the path to the mono executable, or blank if running in windows.
- **7zipPath** - Path to the 7Zip executable (defaults to /usr/bin/7z)
- **UnzipPath** - Path to the unzip executable (defaults to /usr/bin/unzip)
- **16c:ArchiveLocation** - Location of the 16c Archive, organized in year folders
- **16c:CacheLocation** - Location to place cached conversion output

*Note: If you do not have 7Zip or unzip installed, you need to comment out the corresponding Extractors in the Logic\Extractors\ExtractorFactory.cs file. This will be configurable in the future.
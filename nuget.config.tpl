<?xml version="1.0" encoding="utf-8"?>
<configuration>
	<config>
		<add key="defaultPushSource" value="https://nuget.pkg.github.com/bizcover/index.json" />
	</config>
	<packageSources>
		<add key="github" value="https://nuget.pkg.github.com/bizcover/index.json" />
	</packageSources>
	<packageSourceCredentials>
		<github>
			<add key="Username" value="bizcover-bot" />
			<add key="ClearTextPassword" value="GITHUB_PKG_TOKEN" />
		</github>
	</packageSourceCredentials>
</configuration>
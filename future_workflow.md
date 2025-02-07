# RELEASE RECIPE

- `git pull` in main repo
- Edit version in ModShardLauncher.csproj
- `git tag vX.XX.X.X`
- `git push origin vX.XX.X.X` last modification
- `git log` to check for properly tagged commit
- `git pull` for safety
- dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:IncludeAllContentForSelfExtract=true /p:PublishReadyToRun=true --self-contained
- locate `publish` folder in `bin/release/net6.0-windows\win-x64\`
- Move required dlls (everything in `ModReference` + `Reference` folders) in the folder
- Run `ModShardLauncher.exe` once
- Verify there is no unexpected file / folder / mods
- Archive to msl.zip
- Test in sandbox environment (no need to install .NET, use a vanilla.win + a mod you **__KNOW__** works) and check proper injection with UTMTCE
- Add archive to release
- Publish release
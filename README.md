# pp
## Get started
1. Install dotnet core package. 
2. Clone repository. 
3. Open folder in VS Code.
4. Add plugins in Program.cs. 
5. Run `dotnet run` in terminal. 

## Add to powershell
1. ``notepad $PROFILE``
2. Add the following snippet: 
```
function pp {
    cd "<path to directory>\pp"
    dotnet run
}
```
3. ``. $PROFILE``

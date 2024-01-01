# paper

This is a .Net and Typscript project that uses SignalR to create a multi-player game.

To build the client-side typescript application (held in the src directory) run the following: 

```bash
npm run build
```

To start the server-side application, which also then serves the JS files built from the above, run the following:

```bash
dotnet watch
```

Then browse to http://localhost:5128/index.html to join the game.

If you want to specify a non-localhost URL then use the command line:

```bash
dotnet watch  --urls=https://example.com/index.html
```

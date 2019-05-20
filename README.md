# Crypteron .NET Sample Applications

This is the Crypteron .NET Sample Applications repository to demonstrate usage of CipherDB, CipherObject and CipherStor in a quick and easy manner. Each agent library (or plugin) has it's own little sample application and we cover .NET Framework and .NET Core (as well as Entity Framework and Entity Framework Core)

Looking for Java sample applications? You can find them at [github.com/crypteron/sample-apps-java](https://github.com/crypteron/sample-apps-java).

## How do I run this?

1. Clone this repository :)
2. Open the `Crypteron Sample Applications.sln` file in Visual Studio, say Visual Studio 2015
3. You'll see multiple sample apps (i.e. multiple projects) and each demonstrates a particular plugin (e.g. Crypteron CipherDB or CipherObject) 
4. Get the AppSecret from your [Crypteron Dashboard](https://my.crypteron.com/) and plug that back into your `web.config` or `app.config`

For example if using the `ConsoleCipherDb-EF-DbFirst` sample app, open its `App.config` and look for following to put your AppSecret in
```
<crypteronConfig>
    <myCrypteronAccount appSecret="Replace_this_with_app_secret_from_https://my.crypteron.com" />
</crypteronConfig>
```

## But how do I get the AppSecret? 

1. Signup at [crypteron.com](https://www.crypteron.com). It's free!
2. The wizard kicks off to help create your first app (or just hit 'Add New App')
3. Click on the app and hit 'show' for AppSecret

## Got more info?

Sure! Check out the [documentation](https://www.crypteron.com/docs) for the philosophy behind and guidance on Crypteron's developer friendly data-security platform.

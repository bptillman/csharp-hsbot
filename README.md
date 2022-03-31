# Headspring Bot (hsbot)

It's your good buddy hsbot, now in C# (robots don't care for coffee) and on Slack.

# Developing Locally

Copy the appSettings.json file in Application\Hsbot.Hosting.Web to Application\Hsbot.Hosting.Web\appSettings.Development.json.

To create the necessary JIRA API key use `LINQPad` and the following C# snippet:
```csharp
Convert.ToBase64String(Encoding.UTF8.GetBytes($"{JIRA Username}:{JIRA Authorization Token}"))
```

Update the file with the *devbot* API key and you're off to the races.

When testing out things like brags/HVA's where you have to tag a person, to get the regex to match you need to enter the user as <@alias> with the <> otherwise it does not work with the Regex match.

# Licensing

Note that some code in this project was adapted from [Noobot](https://github.com/noobot/noobot).  Even though this project is internal only, below is license text just to be safe.
Take heed in case there's ever a push to make this public.

The MIT License (MIT)

Copyright (c) 2015 noobot

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.


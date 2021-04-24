# BrowserSelectMod
A **modified (simplified)** version of [Browser Select](https://github.com/stevehoek/BrowserSelect) (2.0.0): a utility to dynamically select the browser you want instead of just having one default for all links. Similar to the Choosy app on Mac. It may not be useful for everyone but it helps when you use multiple browsers for different things (e.g. one for work, another for personal) and open many links from other applications (e.g. Messengers).

![screenshot1](https://github.com/ngrcld/BrowserSelectMod/raw/master/screenshots/screenshot.png)

Instead of having to copy the link, open the desired (non-default) browser then pasting the link, all you need to do is to click on the link and this prompt will open allowing you to choose the browser you want. It automatically detects installed browsers. It does not require administrative rights and can be installed as a restricted user.

You may click on the desired browser. You may also press Esc (or click the X) to not open the URL.

BrowserSelectMod has been tested on Windows 10. Requires **.NET Framework 4**.

### My modifications ###
* More compact: removed "Always" buttons and browser labels, removed shortcuts, smaller icons
* Removed support for browser profiles (did not work for me)
* Register for .html and .htm file types
* Portable app (saves settings in the same folder of the exe)

# Related links

[AlternativeTo](http://alternativeto.net/software/choosy/)

[Choosy](https://www.choosyosx.com/)

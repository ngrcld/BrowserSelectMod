using System.Reflection;

using Microsoft.Win32;

namespace BrowserSelectMod
{
    //=============================================================================================================
    static class BrowserSelectSetup
    //=============================================================================================================
    {
        //-------------------------------------------------------------------------------------------------------------
        public static void RegisterAsBrowser()
        //-------------------------------------------------------------------------------------------------------------
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string exe = asm.Location;

            /*
                [HKEY_CURRENT_USER\SOFTWARE\Clients\StartMenuInternet\BrowserSelectMod.exe]
                @="Browser Select"
            */
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(
                    @"SOFTWARE\Clients\StartMenuInternet\BrowserSelectMod.exe"))
            {
                key.SetValue("", "BrowserSelectMod");
            }

            /*
                [HKEY_CURRENT_USER\SOFTWARE\Clients\StartMenuInternet\BrowserSelectMod.exe\Capabilities]
                "ApplicationDescription"="Select which browser to open links with"
                "ApplicationIcon"="D:\\DEV\\GitHub\\BrowserSelectMod\\BrowserSelectMod\\bin\\Debug\\BrowserSelectMod.exe,0"
                "ApplicationName"="BrowserSelectMod"
            */
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(
                    @"SOFTWARE\Clients\StartMenuInternet\BrowserSelectMod.exe\Capabilities"))
            {
                string value = exe + ",0";
                key.SetValue("ApplicationName", "BrowserSelectMod");
                key.SetValue("ApplicationDescription", "Select which browser to open links with");
                key.SetValue("ApplicationIcon", value);
            }

            /*
                [HKEY_CURRENT_USER\SOFTWARE\Clients\StartMenuInternet\BrowserSelectMod.exe\Capabilities\StartMenu]
                "StartMenuInternet"="BrowserSelectMod.exe"
            */
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(
                    @"SOFTWARE\Clients\StartMenuInternet\BrowserSelectMod.exe\Capabilities\StartMenu"))
            {
                key.SetValue("StartMenuInternet", "BrowserSelectMod.exe");
            }

            /*
                [HKEY_CURRENT_USER\SOFTWARE\Clients\StartMenuInternet\BrowserSelectMod.exe\Capabilities\URLAssociations]
                "http"="BrowserSelectModURL"
                "https"="BrowserSelectModURL"
            */
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(
                    @"SOFTWARE\Clients\StartMenuInternet\BrowserSelectMod.exe\Capabilities\URLAssociations"))
            {
                key.SetValue("http", "BrowserSelectModURL");
                key.SetValue("https", "BrowserSelectModURL");
            }

            /*
                [HKEY_CURRENT_USER\SOFTWARE\Clients\StartMenuInternet\BrowserSelectMod.exe\DefaultIcon]
                @="D:\\DEV\\GitHub\\BrowserSelectMod\\BrowserSelectMod\\bin\\Debug\\BrowserSelectMod.exe,0"
            */
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(
                    @"SOFTWARE\Clients\StartMenuInternet\BrowserSelectMod.exe\DefaultIcon"))
            {
                string value = exe + ",0";
                key.SetValue("", value);
            }

            /*
                [HKEY_CURRENT_USER\SOFTWARE\Clients\StartMenuInternet\BrowserSelectMod.exe\shell]
                [HKEY_CURRENT_USER\SOFTWARE\Clients\StartMenuInternet\BrowserSelectMod.exe\shell\open]
                [HKEY_CURRENT_USER\SOFTWARE\Clients\StartMenuInternet\BrowserSelectMod.exe\shell\open\command]
                @="\"D:\\DEV\\GitHub\\BrowserSelectMod\\BrowserSelectMod\\bin\\Debug\\BrowserSelectMod.exe\""
            */
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(
                    @"SOFTWARE\Clients\StartMenuInternet\BrowserSelectMod.exe\shell\open\command"))
            {
                key.SetValue("", exe);
            }

            /*
                [HKEY_CURRENT_USER\SOFTWARE\RegisteredApplications]
                "BrowserSelectMod"="Software\\Clients\\StartMenuInternet\\BrowserSelectMod.exe\\Capabilities"
            */
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(
                    @"SOFTWARE\RegisteredApplications"))
            {
                key.SetValue("BrowserSelectMod", @"Software\Clients\StartMenuInternet\BrowserSelectMod.exe\Capabilities");
            }

            /*
                [HKEY_CURRENT_USER\SOFTWARE\Classes\BrowserSelectModURL]
                @="BrowserSelectMod Url"
            */
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(
                    @"SOFTWARE\Classes\BrowserSelectModURL"))
            {
                key.SetValue("", "BrowserSelectMod Url");
            }

            /*
                [HKEY_CURRENT_USER\SOFTWARE\Classes\BrowserSelectModURL\shell]
                [HKEY_CURRENT_USER\SOFTWARE\Classes\BrowserSelectModURL\shell\open]
                [HKEY_CURRENT_USER\SOFTWARE\Classes\BrowserSelectModURL\shell\open\command]
                @="\"D:\\DEV\\GitHub\\BrowserSelectMod\\BrowserSelectMod\\bin\\Debug\\BrowserSelectMod.exe\" \"%1\""
            */
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(
                    @"SOFTWARE\Classes\BrowserSelectModURL\shell\open\command"))
            {
                string value = "\"" + exe + "\"" + " " + "\"" + "%1" + "\"";
                key.SetValue("", value);
            }
        }
    }
}

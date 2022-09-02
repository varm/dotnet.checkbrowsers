/*
 * Created on Fri Sep 02 2022
 *
 * Copyright (c) 2022 - Zerow
 */

using System.Diagnostics;
using Microsoft.Win32;

public class BrowserProcess
{
    public static void GetAllBrowsers()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("********************");
        Console.WriteLine("List of all browsers:");
        Console.WriteLine("********************");
        Console.ForegroundColor = ConsoleColor.White;
        foreach (BrowserEntity browser in BrowserProcess.GetBrowsers())
        {
            Console.WriteLine(string.Format("{0}: \n\tPath: {1} \n\tVersion: {2} \n\tIcon: {3}", browser.Name, browser.Path, browser.Version, browser.IconPath));
            System.Console.WriteLine("--------------------");
        }
    }

    private static List<BrowserEntity> GetBrowsers()
    {
        RegistryKey browserKeys;
        var browsersList = new List<BrowserEntity>();
        //on 64bit the browsers are in a different location
        if (System.OperatingSystem.IsWindows())
        {
            browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Clients\StartMenuInternet");
            if (browserKeys == null)
                browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");
            string[] browserNames = browserKeys.GetSubKeyNames();
            for (int i = 0; i < browserNames.Length; i++)
            {
                BrowserEntity browser = new BrowserEntity();
                RegistryKey browserKey = browserKeys.OpenSubKey(browserNames[i]);
                browser.Name = (string)browserKey.GetValue(null);
                RegistryKey browserKeyPath = browserKey.OpenSubKey(@"shell\open\command");
                browser.Path = (string)browserKeyPath.GetValue(null).ToString().StripQuotes();
                RegistryKey browserIconPath = browserKey.OpenSubKey(@"DefaultIcon");
                browser.IconPath = (string)browserIconPath.GetValue(null).ToString().StripQuotes();
                browsersList.Add(browser);
                if (browser.Path != null)
                    browser.Version = FileVersionInfo.GetVersionInfo(browser.Path).FileVersion;
                else
                    browser.Version = "unknown";
            }
        }
        return browsersList;

    }
}

internal static class Extensions
{
    ///
    /// if string begins and ends with quotes, they are removed
    ///
    internal static String StripQuotes(this String s)
    {
        if (s.EndsWith("\"") && s.StartsWith("\""))
        {
            return s.Substring(1, s.Length - 2);
        }
        else
        {
            return s;
        }
    }
}
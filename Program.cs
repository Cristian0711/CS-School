using System;
using System.ComponentModel;
using System.Threading;
using System.IO;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Skema Orar || Make#1197";

            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;

            ChromeOptions options = new ChromeOptions();
            options.AddArguments("use-fake-ui-for-media-stream", "--disable-notifications");

            OSchool school = new OSchool();
            CUtils utils = new CUtils();

            school.loadAccount();

            IWebDriver driver = new ChromeDriver(chromeDriverService, options);

            if (!school.Login(driver, school.mail, school.passwd))
            {
                Console.WriteLine("Login has failed!");
                return;
            }
            else
            {
                Console.WriteLine("You have successfully logged in!");
            }

            if (!school.GetSchedule(driver, ref school.oreOrar, ref school.programOre, school.nClass))
            {
                Console.WriteLine("We couldn't take the schedule!");
                return;
            }
            else
            {
                Console.WriteLine("The schedule has been taken succesfully!");
            }

            school.loadLinks();

            while (true)
            {
                int currentDay = (int)DateTime.Now.DayOfWeek - 1;
                for (int i = 0; i < school.oreOrar[currentDay].hours.Count; i++)
                {
                    if (school.oreOrar[currentDay].hours[i] == "")
                        continue;

                    int currentTimeInMin = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                    int startTimeInMin = school.programOre[i].startHour * 60 + school.programOre[i].startMin + 3;
                    int endTimeInMin = school.programOre[i].endHour * 60 + school.programOre[i].endMin - 5;

                    string link = "", name = "";
                    bool linkFound = false;

                    foreach (OSchool.Links m_link in school.links)
                    {
                        if (school.oreOrar[currentDay].hours[i].Contains(m_link.className))
                        {
                            link = m_link.classLink;
                            name = m_link.className;
                            linkFound = !linkFound;
                            break;
                        }
                    }

                    if (!linkFound)
                        continue;

                    for (int j = i + 1; j < school.oreOrar[currentDay].hours.Count; j++)
                    {
                        if (school.oreOrar[currentDay].hours[i] == school.oreOrar[currentDay].hours[j])
                        {
                            endTimeInMin = school.programOre[j].endHour * 60 + school.programOre[j].endMin;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (currentTimeInMin >= startTimeInMin && endTimeInMin > currentTimeInMin)
                    {
                        if (school.JoinClass(driver, link, startTimeInMin, endTimeInMin))
                        {
                            Console.WriteLine("You've joined " + name + ", good luck!");

                            while (currentTimeInMin >= startTimeInMin && endTimeInMin > currentTimeInMin)
                            {
                                //update the time
                                currentTimeInMin = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                                Thread.Sleep(500);
                            }
                            ////*[@id="ow3"]/div[1]/div/div[5]/div[3]/div[9]/div[2]/div[2]/div
                            Console.WriteLine("The class has been finished succesfully!");
                            driver.Navigate().GoToUrl("https://www.google.com/");

                        }
                        else
                        {
                            Console.WriteLine("Failed to join " + name + "!");
                        }
                    }
                }
            }
        }
    }
}
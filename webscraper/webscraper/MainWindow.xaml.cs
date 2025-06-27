using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace webscraper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string toSearch = "";

        public static ObservableCollection<Product> mainList = new ObservableCollection<Product>();
        public MainWindow()
        {
            InitializeComponent();
            //write();
            
        }

        public async Task scroll(WebDriver browser)
        {
            By product = By.ClassName("offer-box");
            ReadOnlyCollection<IWebElement> el = null;
            int HowMuch = 0;
            try
            {
                while (browser.FindElement(By.ClassName("skeleton")).Displayed && HowMuch <= 10)
                {
                    IWebElement element = browser.FindElement(By.ClassName("skeleton"));
                    ((IJavaScriptExecutor)browser).ExecuteScript("arguments[0].scrollIntoView(true);", element);
                    Thread.Sleep(200);
                    HowMuch++;
                };
            }
            catch { }

        }

        public async Task<List<Product>> MediaExpert()
        {
            toSearch = SearchBox.Text;

            var options = new ChromeOptions()
            {
                BinaryLocation = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
            };

            /*            options.AddArguments(new List<string>() {
                                        "--headless=new"
                                    });*/
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddArgument("--user-agent=Mozilla/5.0 (iPad; CPU OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5355d Safari/8536.25");
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            var browser = new ChromeDriver(service, options);

            browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            browser.Manage().Window.Minimize();

            string Url1 = "https://www.amazon.pl/s?k=";
            string Url2 = "&crid=1OSCZ9WEIMEF1&sprefix=%2Caps%2C72&ref=nb_sb_ss_recent_1_0_recent";
            string fullUrl = Url1 + toSearch + Url2;
            browser.Navigate().GoToUrl(fullUrl);
            Thread.Sleep(500);
   /*         var timeout = 1000; //Maximum wait time of 10 seconds
            var wait = new WebDriverWait(browser, TimeSpan.FromMilliseconds(timeout));
            wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
            Thread.Sleep(500);*/

            //await scroll(browser);
            
            var el = browser.FindElements(By.CssSelector("div[class='a-section a-spacing-base']"));
            Console.WriteLine("Total number of elements are " + el.Count);


            List<Product> list = new List<Product>();

            var ex = toSearch.Split(' ');
            string rg = "";
            for(int i = 0; i < ex.Length; i++)
            {
                rg += "(?=.*" + ex[i] + ")";
            }
            Regex regex = new Regex(rg, RegexOptions.IgnoreCase);
            
            for (int i = 0; i < el.Count; i++)
            {
                try
                {
                    Product p = new Product();
                    p.Name = el[i].FindElement(By.CssSelector("span[class='a-size-base-plus a-color-base a-text-normal']")).Text;
                    p.Price = el[i].FindElement(By.CssSelector("div[class='a-row a-size-base a-color-base']")).Text;
                    p.Link = el[i].FindElement(By.CssSelector("a[class='a-link-normal s-underline-text s-underline-link-text s-link-style a-text-normal']")).GetAttribute("href");
                    p.PhotoSrc = el[i].FindElement(By.CssSelector("img[class='s-image']")).GetAttribute("src");
                    string trim = Regex.Replace(p.Name, @"\s", "");

                    if (regex.IsMatch(trim))
                    list.Add(p);
                }
                catch
                {
                    i++;
                    continue;
                }
            }

            browser.Close();
            browser.Quit();

            return list;         
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            
            List<Product> List = await MediaExpert();

            //label.Content = "Total number of elements are " + List.Count;
            for (int i = 0; i < List.Count; i++)
            {
                //label.Content += List[i].Price + Environment.NewLine + List[i].Name + Environment.NewLine + Environment.NewLine;
                /*       Hyperlink link = new Hyperlink();
                       link.NavigateUri = new Uri(List[i].Link, UriKind.Absolute);
                       label.Content = link;
                       label.Content = link;*/
               /* var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(List[i].PhotoSrc); ;
                bitmapImage.EndInit();
                img.Source = bitmapImage*/;
                mainList.Add(List[i]);
            }

            
            myListView.ItemsSource = mainList;












            /*            string fullUrl = "https://www.mediaexpert.pl/komputery-i-tablety/laptopy-i-ultrabooki/laptopy";
                        List<string> programmerLinks = new List<string>();

                        var options = new ChromeOptions()
                        {
                            BinaryLocation = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
                        };

                        options.AddArguments(new List<string>() {
                            "--headless=new"
                        });
                        options.AddArgument("--user-agent=Mozilla/5.0 (iPad; CPU OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5355d Safari/8536.25");
                        ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                        service.HideCommandPromptWindow = true;
                        var browser = new ChromeDriver(service, options);
                        //List<Products> products = new List<Products>();
                        Thread.Sleep(1000);
                        browser.Navigate().GoToUrl(fullUrl);
                        Thread.Sleep(1000);
                        var names = browser.FindElements(By.ClassName("offer-box"));
                        Thread.Sleep(1000);
                        label.Content = names[1].FindElement(By.ClassName("name")).Text;*/








        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Xbox.Services.Leaderboard;
//using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xbox.Services.Statistics.Manager;
using Microsoft.Xbox.Services.System;
using Microsoft.Xbox.Services;
//using System.Diagnostics;

// Install-Package Microsoft.Xbox.Live.SDK.WinRT.UWP -Version 2018.6.20181010.2
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Math_Drills
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        String opstr = "+";
        Dictionary<int, int> clist = new Dictionary<int, int>();
        Dictionary<int, int> dlist = new Dictionary<int, int>();
        int questionsasked = 0;
        int correct = 0;
        int correctnontime = 0;
        DateTime StartTime = DateTime.Now;
        XboxLiveUser primaryUser;
        Microsoft.Xbox.Services.XboxLiveContext xboxLiveContext;
        String Mode = "2";
        StatisticManager statManager = StatisticManager.SingletonInstance;
        public MainPage()
        {
            this.InitializeComponent();
            this.Operation.Items.Add("+");
            this.Operation.Items.Add("*");
            this.Operation.Items.Add("/");
            this.Operation.Items.Add("-");
            this.Operation.SelectedIndex = 0;
            this.MathQuestion.Text = "";
            this.MathAnswer.Text = "";
            //Task Waiter = 
            SignIn();
            //Waiter.Wait(TimeSpan.FromMinutes(5));
            Randomize();

        }
        private void Randomize ()
        {
            int a, b, c;
            Random Rand = new Random();
            for (int i = 0; i < (opstr == "/" ? 90 : 100);i++)
            {
                do
                {
                    if (opstr == "/")
                    {
                        a = Rand.Next(1, 10);//Get-Random -Maximum 10 -Minimum 1
                        b = Rand.Next(0, 10);//Get-Random -Maximum 10 -Minimum 0
                    }
                    else
                    {
                        a = Rand.Next(0, 10);//Get-Random -Maximum 10 -Minimum 0
                        b = Rand.Next(0, 10);//Get-Random -Maximum 10 -Minimum 0
                    }
                    c = a * 10 + b;
                } while (clist.ContainsKey(c));
            clist.Add(c, i);
            dlist.Add(i, c);
            }
        }
        private void SetQuestion()
        {
                this.MathQuestion.FontSize = 72;
                this.MathQuestion.Text = "";
                this.MathAnswer.Text = "";
                if (opstr == "+")
                {
                    this.MathQuestion.Text = (dlist.GetValueOrDefault(questionsasked) / 10).ToString() + opstr +  (dlist.GetValueOrDefault(questionsasked) % 10).ToString() + "=";
                }
                else if (opstr == "-")
                {
                    this.MathQuestion.Text = ((dlist.GetValueOrDefault(questionsasked) / 10) + (dlist.GetValueOrDefault(questionsasked) % 10)).ToString() + opstr + (dlist.GetValueOrDefault(questionsasked) / 10).ToString() + "=";
                }
                    else if (opstr == "*")
                {
                    this.MathQuestion.Text = (dlist.GetValueOrDefault(questionsasked) / 10).ToString() + opstr + (dlist.GetValueOrDefault(questionsasked) % 10).ToString() + "=";
                }
                else if (opstr == "/")
                {
                    this.MathQuestion.Text = ((dlist.GetValueOrDefault(questionsasked) / 10) * (dlist.GetValueOrDefault(questionsasked) % 10)).ToString() + opstr + (dlist.GetValueOrDefault(questionsasked) / 10).ToString() + "=";
                }
        }
        private void Score(int answer)
        {

            if ((opstr == "+") && (dlist.GetValueOrDefault(questionsasked) / 10) + dlist.GetValueOrDefault(questionsasked) % 10 == answer)
            {
                correct++;
                if ((DateTime.Now - this.StartTime).TotalSeconds <= 15) { correctnontime++; }
                if (File.Exists(@"C:\Windows\Media\ding.wav"))
                {
                    this.Media.Source = new Uri(@"C:\Windows\Media\ding.wav", UriKind.RelativeOrAbsolute);
                    this.Media.Play();
                }
            }
            else if ((opstr == "-") && (dlist.GetValueOrDefault(questionsasked) % 10 == answer))
            {
                correct++;
                if ((DateTime.Now - this.StartTime).TotalSeconds <= 15) { correctnontime++; }
                if (File.Exists(@"C:\Windows\Media\ding.wav"))
                {
                    this.Media.Source = new Uri(@"C:\Windows\Media\ding.wav", UriKind.RelativeOrAbsolute);
                    this.Media.Play();
                }
            }
            else if ((opstr == "*") && (dlist.GetValueOrDefault(questionsasked) / 10) * (dlist.GetValueOrDefault(questionsasked) % 10) == answer)
            {

                correct++;
                if ((DateTime.Now - this.StartTime).TotalSeconds <= 15) { correctnontime++; }
                if (File.Exists(@"C:\Windows\Media\ding.wav"))
                {
                    this.Media.Source = new Uri(@"C:\Windows\Media\ding.wav", UriKind.RelativeOrAbsolute);
                    this.Media.Play();
                }
            }
            else if ((opstr == "/") && (dlist.GetValueOrDefault(questionsasked) % 10 == answer))
            {
                correct++;
                if ((DateTime.Now - this.StartTime).TotalSeconds <= 15) { correctnontime++; }
                if (File.Exists(@"C:\Windows\Media\ding.wav"))
                {
                    this.Media.Source = new Uri(@"C:\Windows\Media\ding.wav", UriKind.RelativeOrAbsolute);
                    this.Media.Play();
                }
            }
        }

        private void AnswerKeyDown(object sender, KeyRoutedEventArgs e)
        {
            int Score;
            
            Int32 IQ = 100;
            
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if ((((questionsasked >= 19 && opstr == "/") || questionsasked >= 19) && Mode == "2") ||
                    ((questionsasked >= 89 && opstr == "/") || questionsasked >= 99))
                {
                    this.Progress.Value = 100.0;
                    if (int.TryParse(this.MathAnswer.Text, out Score) && 
                        ((((questionsasked == 19 && opstr == "/") || questionsasked == 19) && Mode == "2") ||
                        ((questionsasked == 89 && opstr == "/") || questionsasked == 99)))
                    {
                        this.Score(Score);
                        questionsasked++;
                    }
                    Double Elapsed = (DateTime.Now - this.StartTime).TotalSeconds;
                    //this.MathQuestion.Text = (correct + "/" + questionsasked + "  ✓ in " + (DateTime.Now - this.StartTime).TotalMinutes.ToString("0.0") + " min");
                    if ((questionsasked - correct <= 20 && Mode == "1") || (questionsasked - correct <= 2 && Mode == "2"))
                    {
                        UpdateLeaderboard(Elapsed, Mode);
                        switch (correctnontime)
                        {
                            case 1: IQ = 49; break;
                            case 2: IQ = 56; break;
                            case 3: IQ = 62; break;
                            case 4: IQ = 68; break;
                            case 5: IQ = 75; break;
                            case 6: IQ = 82; break;
                            case 7: IQ = 89; break;
                            case 8: IQ = 95; break;
                            case 9: IQ = 102; break;
                            case 10: IQ = 110; break;
                            case 11: IQ = 117; break;
                            case 12: IQ = 125; break;
                            case 13: IQ = 135; break;
                            case 14: IQ = 144; break;
                            case 15: IQ = 154; break;
                            case 16: IQ = 155; break;
                            case 17: IQ = 155; break;
                            case 18: IQ = 155; break;
                            case 19: IQ = 155; break;
                            case 20: IQ = 155; break;
                            default: IQ = 100; break;
                        }
                        //if (Elapsed <= questionsasked / .65 ){IQ = 130;}
                        //else if (Elapsed <= questionsasked / .36 ){IQ = 120;}
                        //else if (Elapsed <= questionsasked / .09 ){IQ = 110;}
                        //https://www.frontiersin.org/articles/10.3389/fpsyg.2018.01498/full
                        //Wais-3 15 seconds per problem
                    }
                    //Thread.Sleep(TimeSpan.FromSeconds(1));
                    double[] Max = new double[2];
                    Max[0] = 0.01;
                    Max[1] = 1000;
                    Max = GetPercentage(Mode);
                    double Percentile = Max[0];
                    Double Best = 1;
                    Best = GetPersonalBest(Mode);
                    //if (Best == Elapsed) Best = .01;

                    this.MathAnswer.Text = "";
                    this.MathQuestion.FontSize = 48;
                    if (Percentile != .01)
                    {
                        this.MathQuestion.Text = (correct + "/" + questionsasked + "  ✓ in " + Elapsed.ToString("0.0") + " seconds Xbox:" + Percentile.ToString("0.00%") + " IQ:" + IQ);
                    }
                    else if (Best != 1)
                    {
                        this.MathQuestion.Text = (correct + "/" + questionsasked + "  ✓ in " + Elapsed.ToString("0.0") + " seconds Best:" + Best.ToString("0.0") + " IQ:" + IQ + " Click me");
                    }
                    else
                    {
                        this.MathQuestion.Text = (correct + "/" + questionsasked + "  ✓ in " + Elapsed.ToString("0.0") + " seconds Xbox:Err IQ:" + IQ);
                    }
                    //  97/100 ✓ in 3.5 min Xbox:30.0% IQ:110
                }
                else if (int.TryParse(this.MathAnswer.Text,out Score))
                {
                    this.Score(Score);
                    questionsasked++;
                    int questionsmax = 0;
                    if (Mode == "2") questionsmax = 20;
                    else if (opstr == "/") questionsmax = 90;
                    else questionsmax = 100;
                    this.Progress.Value = 100.0 * (double)questionsasked / (double)questionsmax;
                    this.SetQuestion();
                }
            }
        }
        //
        private Double GetPersonalBest(String Mode)
        {
            Double Time = 1;
            try
            {
                //if (!primaryUser.IsSignedIn) SignIn();
                IReadOnlyList<string> Names = (IReadOnlyList<string>)statManager.GetStatisticNames(primaryUser);
                Int32 ColumnCount = Names.Count;
                String FirstColumn = Names.FirstOrDefault();
                if (ColumnCount == 0)
                {
                    return Time;
                }
                StatisticValue Value = statManager.GetStatistic(primaryUser, Mode);
                Time = Value.AsNumber;
                return Time;
            }
            catch { }
            return Time;
        }
        //https://docs.microsoft.com/en-us/gaming/xbox-live/features/player-data/stats-leaderboards/title-managed/how-to/live-leaderboards-tm-howto
        private double[] GetPercentage(String Mode)
        {
            IReadOnlyList<StatisticEvent> Events = null;
            double[] Max = new double[2];
            Max[0] = 0.01;
            Max[1] = 1000;
            //bool HasNext = true;
            //int Pages = 0;
            try
            {
                //if (!primaryUser.IsSignedIn) SignIn();
                IReadOnlyList<string> Names = (IReadOnlyList<string>)statManager.GetStatisticNames(primaryUser);
                Int32 ColumnCount = Names.Count;
                String FirstColumn = Names.FirstOrDefault();
                if (ColumnCount == 0)
                {
                    return Max;
                }
                LeaderboardService boardService = xboxLiveContext.LeaderboardService;
                LeaderboardQuery Query = new LeaderboardQuery();
                Query.Order = Microsoft.Xbox.Services.Leaderboard.SortOrder.Ascending;
                Query.MaxItems = 10;
                Query.SkipResultToMe = true;
                
                statManager.GetLeaderboard(primaryUser, Mode, Query);
                //statManager.RequestFlushToService(primaryUser, true);
                Events = statManager.DoWork();
                //for (int j = 0; (Events.Count == 0 || (Events.Count == 1 && Events.First().EventType != StatisticEventType.GetLeaderboardComplete)) && j < 10; j++)
                //    Task.Delay(TimeSpan.FromSeconds(10)).Wait();
                foreach (StatisticEvent Event in Events)
                {
                    if (Event.EventType == StatisticEventType.GetLeaderboardComplete
                        && Event.ErrorCode == 0)
                    {
                        try
                        {
                            LeaderboardResultEventArgs leaderArgs = (LeaderboardResultEventArgs)Event.EventArgs;
                            LeaderboardResult leaderboardResult = leaderArgs.Result;
                            foreach (LeaderboardRow Row in leaderboardResult.Rows)
                            {
                                if (Row.Gamertag == primaryUser.Gamertag && Row.Percentile > Max[0])
                                {
                                    Max[0] = Row.Percentile;
                                    Max[1] = Row.Rank;
                                }
                            }
                        }
                        catch (Exception e) { }
                        //swallows a 404 leaderboard not found exception
                    }
                }
            }
            catch ( Exception e )
            {
                this.MathQuestion.Text = e.Message;
                //Thread.Sleep(TimeSpan.FromSeconds(5));
                Task.Delay(TimeSpan.FromSeconds(5)).Wait();
            }
            return Max;
        }
        //https://docs.microsoft.com/en-us/gaming/xbox-live/features/identity/auth/auth-uwp/live-auth-for-uwp-projects
        private async Task SignIn()
        {
            bool signedIn = false;

            // Get a list of the active Windows users.
            IReadOnlyList<Windows.System.User> users = await Windows.System.User.FindAllAsync();

            // Acquire the CoreDispatcher which will be required for SignInSilentlyAsync and SignInAsync.
            Windows.UI.Core.CoreDispatcher UIDispatcher = Windows.UI.Xaml.Window.Current.CoreWindow.Dispatcher;

            try
            {
                // 1. Create an XboxLiveUser object to represent the user
                primaryUser = new XboxLiveUser(users[0]);
                // 2. Sign-in silently to Xbox Live
                SignInResult signInSilentResult = await primaryUser.SignInSilentlyAsync(UIDispatcher);
                switch (signInSilentResult.Status)
                {
                    case SignInStatus.Success:
                        signedIn = true;
                        break;
                    case SignInStatus.UserInteractionRequired:
                        //3. Attempt to sign-in with UX if required
                        SignInResult signInLoud = await primaryUser.SignInAsync(UIDispatcher);
                        switch (signInLoud.Status)
                        {
                            case SignInStatus.Success:
                                signedIn = true;
                                break;
                            case SignInStatus.UserCancel:
                                // present in-game UX that allows the user to retry the sign-in operation. (For example, a sign-in button)
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                if (signedIn)
                {
                    // 4. Create an Xbox Live context based on the interacting user
                    xboxLiveContext = new Microsoft.Xbox.Services.XboxLiveContext(primaryUser);
                    statManager.AddLocalUser(primaryUser);
                    //statManager.RequestFlushToService(primaryUser, true);
                    statManager.DoWork();

                    //add the sign out event handler
                    XboxLiveUser.SignOutCompleted += OnSignOut;
                }
            }
            catch (Exception e)
            {
                this.MathQuestion.Text = e.Message;
                //Thread.Sleep(TimeSpan.FromSeconds(5));
                Task.Delay(TimeSpan.FromSeconds(5)).Wait();
            }

        }

        private void OnSignOut(object sender, SignOutCompletedEventArgs e)
        {
            statManager.RemoveLocalUser(primaryUser);
            statManager.DoWork();
            // 6. When the game exits or the user signs-out, release the XboxLiveUser object and XboxLiveContext object by setting them to null
            primaryUser = null;
            xboxLiveContext = null;
        }
        private void EventDebugger(IReadOnlyList<StatisticEvent> Events)
        {
            //for (int i = 0; Events.Count == 0 && i < 10;i++) Thread.Sleep(TimeSpan.FromSeconds(1));
            for (int i = 0; Events.Count == 0 && i < 10; i++) Task.Delay(TimeSpan.FromSeconds(1)).Wait();
            if (Events.Count == 0) return;
            StatisticEvent One = Events.First();
            Int32 Count = Events.Count;
            if (One.EventType == StatisticEventType.GetLeaderboardComplete)
            {
                LeaderboardResultEventArgs Args = (LeaderboardResultEventArgs)One.EventArgs;
                LeaderboardResult Board = Args.Result;
                Count = Board.Rows.Count;
                LeaderboardRow Row = Board.Rows.First();
                String Tag = Row.Gamertag;
            }
            else if (One.EventType == StatisticEventType.StatisticUpdateComplete)
            {
                return;
            }
            else if (One.EventType == StatisticEventType.LocalUserAdded)
            {
                return;
            }
            else if (One.EventType == StatisticEventType.LocalUserRemoved)
            {
                return;
            }
        }
        //https://docs.microsoft.com/en-us/gaming/xbox-live/features/player-data/stats-leaderboards/title-managed/how-to/live-stats-tm-updating
        private void UpdateLeaderboard(Double Time, String Mode)
        {
            try
            {
                IReadOnlyList<StatisticEvent> Events = null;
                //if (!primaryUser.IsSignedIn) SignIn();
                IReadOnlyList<string> Names = (IReadOnlyList<string>)statManager.GetStatisticNames(primaryUser);
                if (Names.Count > 0 && Names.Contains(Mode))
                {
                    StatisticValue Value = statManager.GetStatistic(primaryUser, Mode);
                    if (Value.AsNumber > Time)
                    {
                        //Better time
                        statManager.SetStatisticNumberData(primaryUser, Mode, Time);
                        //statManager.RequestFlushToService(primaryUser, true);
                        Events = statManager.DoWork();
                        EventDebugger(Events);
                        this.SetQuestion();
                        if (File.Exists(@"C:\Windows\Media\tada.wav"))
                        {
                            this.Media.Source = new Uri(@"C:\Windows\Media\tada.wav", UriKind.RelativeOrAbsolute);
                            this.Media.Play();
                        }
                    }
                }
                else
                {
                    //Empty
                    statManager.SetStatisticNumberData(primaryUser, Mode, Time);
                    //statManager.RequestFlushToService(primaryUser, true);
                    Events = statManager.DoWork();
                    EventDebugger(Events);
                }
            }
            catch (Exception e)
            {
                this.MathQuestion.Text = e.Message;
                //Thread.Sleep(TimeSpan.FromSeconds(5));
                Task.Delay(TimeSpan.FromSeconds(5)).Wait();
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            StartTime = DateTime.Now;
            if (this.Fast.IsChecked.Value) Mode = "2";
            else Mode = "1";
            this.opstr = this.Operation.SelectedItem.ToString();
            clist = new Dictionary<int, int>();
            dlist = new Dictionary<int, int>();
            Randomize();
            questionsasked = 0;
            correct = 0;
            this.SetQuestion();
            this.MathAnswer.Focus(FocusState.Pointer);
            //questionsasked++;
        }

        private void MathQuestion_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (MathQuestion.Text.Contains("Best"))
            {
                double[] Max = new double[2];
                Max[0] = 0.01;
                Max[1] = 1000;
                Max = GetPercentage(Mode);
                double Percentile = Max[0];
                long Rank = (long)Max[1];
                this.MathAnswer.Text = "";
                this.MathQuestion.FontSize = 48;
                if (Percentile != .01)
                {
                    this.MathQuestion.Text = (correct + "/" + questionsasked + "  ✓ Percentile:" + Percentile.ToString("0.00%") + " Rank:" + Rank);
                }
            }
        }
    }
}

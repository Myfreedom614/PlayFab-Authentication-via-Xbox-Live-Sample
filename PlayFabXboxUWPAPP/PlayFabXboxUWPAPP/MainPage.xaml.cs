using Microsoft.Xbox.Services;
using Microsoft.Xbox.Services.System;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PlayFabXboxTestAPP1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private XboxLiveContext xlc;
        private XboxLiveUser xlu;

        public MainPage()
        {
            this.InitializeComponent();
            this.viewModel.XblSignedIn = false;
        }

        private async void BtnSignin_ClickAsync(object sender, RoutedEventArgs e)
        {
            await XBLSignIn();
        }

        #region Xbox Live

        public async Task XBLSignIn()
        {
            // Get a list of the active Windows users.
            IReadOnlyList<Windows.System.User> users = await Windows.System.User.FindAllAsync();

            // Acquire the CoreDispatcher which will be required for SignInSilentlyAsync and SignInAsync.
            Windows.UI.Core.CoreDispatcher UIDispatcher = Windows.UI.Xaml.Window.Current.CoreWindow.Dispatcher;

            try
            {
                // 1. Create an XboxLiveUser object to represent the user
                xlu = new XboxLiveUser(users[0]);

                // 2. Sign-in silently to Xbox Live
                SignInResult signInSilentResult = await xlu.SignInSilentlyAsync(UIDispatcher);
                switch (signInSilentResult.Status)
                {
                    case SignInStatus.Success:
                        HandleSuccessSignIn();
                        break;
                    case SignInStatus.UserInteractionRequired:
                        this.viewModel.XblOutput = "Attempt to sign-in with UX";
                        //3. Attempt to sign-in with UX if required
                        SignInResult signInLoud = await xlu.SignInAsync(UIDispatcher);
                        switch (signInLoud.Status)
                        {
                            case SignInStatus.Success:
                                HandleSuccessSignIn();
                                break;
                            case SignInStatus.UserCancel:
                                // present in-game UX that allows the user to retry the sign-in operation. (For example, a sign-in button)
                                this.viewModel.XblOutput = "User cancelled";
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                if (this.viewModel.XblSignedIn)
                {
                    // 4. Create an Xbox Live context based on the interacting user
                    xlc = new Microsoft.Xbox.Services.XboxLiveContext(xlu);

                    //add the sign out event handler
                    XboxLiveUser.SignOutCompleted += OnSignOut;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                this.viewModel.XblOutput = e.Message;
            }

        }

        private void HandleSuccessSignIn()
        {
            this.viewModel.GameTag = xlu.Gamertag;
            this.viewModel.XblSignedIn = true;
            this.viewModel.XblOutput = $"Success, welcome {this.viewModel.GameTag}";
        }

        public void OnSignOut(object sender, SignOutCompletedEventArgs e)
        {
            // 6. When the game exits or the user signs-out, release the XboxLiveUser object and XboxLiveContext object by setting them to null
            xlu = null;
            xlc = null;
            this.viewModel.XblSignedIn = false;
            this.viewModel.XboxToken = string.Empty;
            this.viewModel.HasXblToken = false;
            this.viewModel.XblOutput = string.Empty;
            this.viewModel.XblTokenOutput = string.Empty;
            this.viewModel.PFOutput = string.Empty;
            this.viewModel.GameTag = string.Empty;
        }

        #endregion

        private async void BtnGetToken_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                this.viewModel.XboxToken = string.Empty;
                this.viewModel.XblTokenOutput = "Waiting for response......";
                var result = await xlu.GetTokenAndSignatureAsync("POST", "https://playfabapi.com/ ", "");
                this.viewModel.XboxToken = result.Token;
                if (!string.IsNullOrEmpty(this.viewModel.XboxToken))
                {
                    this.viewModel.HasXblToken = true;
                    this.viewModel.XblTokenOutput = this.viewModel.XboxToken;
                }
                else
                {
                    this.viewModel.XblTokenOutput = "Empty for some reason";
                }
            }
            catch(Exception ex)
            {
                this.viewModel.XblTokenOutput = ex.Message;
            }
         }

        private async void BtnLoginWithXbox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.viewModel.PFOutput = "Waiting for response......";
                var challengeResponse = await PlayFab.PlayFabClientAPI.LoginWithXboxAsync(new PlayFab.ClientModels.LoginWithXboxRequest
                {
                    XboxToken = this.viewModel.XboxToken,
                    TitleId = PlayFab.PlayFabSettings.staticSettings.TitleId,
                    CreateAccount = true
                });
                Debug.WriteLine(JsonConvert.SerializeObject(challengeResponse.Result, Formatting.Indented));
                this.viewModel.PFOutput = JsonConvert.SerializeObject(challengeResponse.Result, Formatting.Indented);
            }
            catch (Exception ex)
            {
                this.viewModel.PFOutput = ex.Message;
            }
        }

        private void BtnSignout_Click(object sender, RoutedEventArgs e)
        {
            OnSignOut(null, null);
        }
    }
}

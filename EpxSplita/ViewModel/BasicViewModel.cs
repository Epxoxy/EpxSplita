using GuetModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

namespace EpxSplita
{
    class GuetWebRequestViewModel : NotificationObject
    {
        #region Basic mode

        private Model.WebRequestModel webModel = new Model.WebRequestModel();
        private Model.LoginModel loginModel = new Model.LoginModel();
        private GuetModelProvider modelProvider = new GuetModelProvider();
        private WebSerives.NetRequestProvider netRequestProvider;

        #endregion

        #region Model properities

        #region Login model

        public string UserName
        {
            get
            {
                return loginModel.UserName;
            }
            set
            {
                if (loginModel.UserName == value) return;
                loginModel.UserName = value;
                RaisePropertyChanged("UserName");

                IsStoredUser = false;
                IsStoredUser = UserNameList.Contains(value);
            }
        }

        public List<string> UserNameList
        {
            get
            {
                return loginModel.UserNameList;
            }
            set
            {
                loginModel.UserNameList = value;
                RaisePropertyChanged("UserNameList");
            }
        }
        
        public bool IsStoredUser
        {
            get
            {
                return loginModel.IsStoredUser;
            }
            set
            {
                if (loginModel.IsStoredUser == value) return;
                loginModel.IsStoredUser = value;
                RaisePropertyChanged("IsStoredUser");
            }
        }

        private bool rememberFailUser;
        public bool RememberFailUser
        {
            get
            {
                return rememberFailUser;
            }
            set
            {
                rememberFailUser = value;
                RaisePropertyChanged("RememberFailUser");
            }
        }

        public bool RememberUser
        {
            get
            {
                return loginModel.RememberUser;
            }
            set
            {
                loginModel.RememberUser = value;
                RaisePropertyChanged("RememberUser");
            }
        }

        public string SiteName
        {
            get
            {
                return loginModel.SiteName;
            }
            set
            {
                loginModel.SiteName = value;
                loadUserList();
                RaisePropertyChanged("SiteName");
            }
        }
        
        #endregion

        #region Request model
        
        public int DelayTime
        {
            get
            {
                return webModel.DelayTime;
            }
            set
            {
                if (value < 2000) webModel.DelayTime = 2000;
                else webModel.DelayTime = value;
                RaisePropertyChanged("DelayTime");
            }
        }

        public bool IsRequestRunning
        {
            get
            {
                return webModel.IsRequestRunning;
            }
            set
            {
                if (webModel.IsRequestRunning == value) return;
                webModel.IsRequestRunning = value;
                RaisePropertyChanged("IsRequestRunning");
                RequestCommand.RaiseCanExecuteChanged();
                LoginCommand.RaiseCanExecuteChanged();
                LogoutCommand.RaiseCanExecuteChanged();
                StopRequestCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsAutoRepeat
        {
            get
            {
                return webModel.IsAutoRepeat;
            }
            set
            {
                if (webModel.IsAutoRepeat == value) return;
                webModel.IsAutoRepeat = value;
                RaisePropertyChanged("IsAutoRepeat");
            }
        }
        
        public string RequestResult
        {
            get
            {
                return webModel.RequestResult;
            }
            set
            {
                webModel.RequestResult = value;
                RaisePropertyChanged("RequestResult");
            }
        }

        public string ConsoleMessage
        {
            get
            {
                return webModel.RunMessage;
            }
            set
            {
                webModel.RunMessage = DateTime.Now.ToString() + "\n\n" + value;
                RaisePropertyChanged("ConsoleMessage");
            }
        }

        private System.Net.CookieContainer cookieContainer
        {
            get
            {
                if (webModel.cookieContainer == null)
                    webModel.cookieContainer = netRequestProvider.MCookieContainer;
                return webModel.cookieContainer;
            }
            set
            {
                webModel.cookieContainer = value;
                netRequestProvider.MCookieContainer = null;
            }
        }

        public bool IsHttpPost
        {
            get
            {
                return webModel.IsHttpPost;
            }
            set
            {
                if (webModel.IsHttpPost == value) return;
                webModel.IsHttpPost = value;
                RaisePropertyChanged("IsHttpPost");
            }
        }

        #endregion
        
        private Dictionary<string, string> loginList = null;
        public Dictionary<string, string> LoginList
        {
            get
            {
                if (loginList == null)
                {
                    loginList = new Dictionary<string, string>();
                    loginList.Add("Hello0", "12324343");
                    loginList.Add("Hello1", "12324343");
                    loginList.Add("Hello2", "12324343");
                }
                return loginList;
            }
            set
            {
                if (loginList == value) return;
                loginList = value;
                RaisePropertyChanged("LoginList");
            }
        }

        private string selectedKey = string.Empty;
        public string SelectedKey
        {
            get
            {
                return selectedKey;
            }
            set
            {
                if (selectedKey == value) return;
                selectedKey = value;
                LogoutCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged("SelectedKey");
            }
        }

        private int selectedRowIndex = -1;
        public int SelectedRowIndex
        {
            get
            {
                return selectedRowIndex;
            }
            set
            {
                if (selectedRowIndex == value) return;
                selectedRowIndex = value;
                HandRequestCommand.RaiseCanExecuteChanged();
                RaisePropertyChanged("SelectedRowIndex");
            }
        }

        #endregion
        
        #region Additional properities

        private BindingList<OptionalItem<string, string>[]> postList = 
            new BindingList<OptionalItem<string, string>[]>();
        public BindingList<OptionalItem<string, string>[]> PostList
        {
            get
            {
                return postList;
            }
            set
            {
                postList = value;
                RaisePropertyChanged("PostList");
            }
        }


        private int postSelectedIndex = 0;
        public int PostSelectedIndex
        {
            get
            {
                return postSelectedIndex;
            }
            set
            {
                postSelectedIndex = value;
                RaisePropertyChanged("PostSelectedIndex");
            }
        }

        public List<string> MenuItems
        {
            get
            {
                return modelProvider.getMenuList();
            }
        }

        public List<string> SiteList
        {
            get
            {
                return modelProvider.getSiteList();
            }
        }
        
        private DataTable dataTable = new DataTable();
        public DataTable MyTable
        {
            get
            {
                return dataTable;
            }
            set
            {
                dataTable = value;
                RaisePropertyChanged("MyTable");
            }
        }

        #endregion

        #region Developer

        private bool developer = true;
        public bool Developer
        {
            get
            {
                return developer;
            }
            set
            {
                developer = value;
                RaisePropertyChanged("Developer");
            }
        }

        //Handle message when developer mode
        private void developerMessage(string value)
        {
            if (!Developer) return;
            System.Diagnostics.Debug.WriteLine(value);
        }

        //Update errormessage
        private void sendToConsole(string sdata)
        {
            ConsoleMessage = sdata;
        }

        private List<string> teststring;
        public List<string> TestString
        {
            get
            {
                return teststring;
            }set
            {
                teststring = value;
                RaisePropertyChanged("TestString");
            }
        }


        #endregion
        
        #region Command
        public DelegateCommand RequestCommand { get; set; } = new DelegateCommand();
        public DelegateCommand LoginCommand { get; set; } = new DelegateCommand();
        public DelegateCommand LogoutCommand { get; set; } = new DelegateCommand();
        public DelegateCommand StopRequestCommand { get; set; } = new DelegateCommand();
        public DelegateCommand SetRequestCommand { get; set; } = new DelegateCommand();
        public DelegateCommand ClearUserCommand { get; set; } = new DelegateCommand();
        public DelegateCommand HandRequestCommand { get; set; } = new DelegateCommand();
        public DelegateCommand UpdateSelectionCommand { get; set; } = new DelegateCommand();

        private void initializeCommand()
        {
            SetRequestCommand.ExecuteCommand = o => SetRequest(o);

            RequestCommand.CanExecuteCommand = o => { return !IsRequestRunning; };
            RequestCommand.ExecuteCommand = o => Request(o);

            StopRequestCommand.CanExecuteCommand = o => { return IsRequestRunning; };
            StopRequestCommand.ExecuteCommand = o => { this.IsRequestRunning = false; };

            LoginCommand.CanExecuteCommand = o => { return !IsRequestRunning; };
            LoginCommand.ExecuteCommand = o =>
            {
                Action<object> action = LoginRequest;
                System.Windows.Application.Current.Dispatcher.BeginInvoke(action, o);
            };

            LogoutCommand.CanExecuteCommand = o =>
            {
                return !string.IsNullOrEmpty(SelectedKey);
            };
            LogoutCommand.ExecuteCommand = o => Logout(o);

            ClearUserCommand.ExecuteCommand = o =>
            {
                UserSerives.StoreUserData.deleteAllEncrypted();
                loadUserList();
            };

            HandRequestCommand.CanExecuteCommand = o => { return SelectedRowIndex > -1; };
            HandRequestCommand.ExecuteCommand = o =>
            {
                //var b = o as DataRowView;
                //if (b != null)
                //{
                //    string result = b.Row[0].ToString();
                //    string[] rowName = modelProvider.getHeaders();
                //}
            };
            UpdateSelectionCommand.ExecuteCommand = o => updatePostSelect(o);
        }

        private Dictionary<int, string> postparams = new Dictionary<int, string>();
        private void updatePostSelect(object o)
        {
            if (o == null) return;
            else if (o is object[])
            {
                var param = (object[])o;
                foreach (var item in param)
                {
                    if (item == null) { postparams = new Dictionary<int, string>(); return; }
                }
                int index = PostList.IndexOf(param[0] as OptionalItem<string, string>[]);

                if (param.Length != 2) return;
                if (postparams.ContainsKey(index)) postparams[index] = param[1].ToString();
                else postparams.Add(index, param[1].ToString());

            }
        }

        private string[] getPostValue()
        {
            string str = string.Empty;
            if (postparams.Count < 1) return null;

            List<string> postList = new List<string>();
            if (postparams.Count == 1)
            {
                postList.Add(postparams[0]);
                str = postparams[0];
            }
            else
            {
                foreach (var item in postparams.OrderBy(x => x.Key))
                {
                    postList.Add(item.Value);
                    str += item.Key.ToString() + "," + item.Value + "\n";
                }
            }
            developerMessage("\nCurrent data\n" + str);

            return postList.ToArray();
        }

        #endregion

        #region Action

        #region User assistant
        
        private void storeUser(ref byte[] bytes)
        {
            //TODO save encrypt user data use tempBytes
            UserSerives.StoreUserData.encryptUserData(UserName, bytes, SiteName);
            bytes = null;
            UserNameList.Add(UserName);
        }

        private void loadUserList()
        {
            List<string> savedPswUserList = new List<string>();
            var allList = UserSerives.StoreUserData.getUserList(SiteName);
            if (allList != null)
            {
                foreach (var item in allList)
                {
                    savedPswUserList.Add(item.ToString());
                }
                UserNameList = savedPswUserList;
            }
        }

        #endregion

        #region Login assistant

        /// <summary>
        /// Check if the user name is right input
        /// </summary>
        /// <param name="userName">User name</param>
        /// <returns></returns>
        private bool isRightUserName(string userName)
        {
            if (userName == null)
            {
                sendToConsole("Login error : UserName is empty!");
            }
            else if (userName.Length < 10)
            {
                sendToConsole("Login error : UserName is not match ten!");
            }
            else if (!Regex.IsMatch(userName, "^[0-9]+$"))
            {
                sendToConsole("Login error : UserName is not all number!");
            }
            else
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Request data to login page
        /// </summary>
        /// <param name="o"></param>
        private async void LoginRequest(object o)
        {
            if (isRightUserName(UserName))
            {
                string siteName = SiteName;
                //0.SET MODEL TO LOGIN STATE
                modelProvider.useSite(siteName.ToLower() == "szhxy" ? StoredSiteName.Szhxy : StoredSiteName.Bkjw);
                modelProvider.setLogin();
                netRequestProvider.encoding = modelProvider.Encoding;

                //1.LOGOUT LOGINED USER
                if (LoginList.ContainsKey(siteName))
                {
                    RequestResult = await netRequestProvider.HttpGet(modelProvider.LogoutUrl, "");
                    LoginList.Remove(siteName);
                }

                //2.GET PASSWORD AND SAVE BYTE[] FOR STORE USER DATA
                byte[] tempBytes = null;
                IntPtr p = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(((o as System.Windows.Controls.PasswordBox).SecurePassword));
                string psw = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(p);
                if (RememberUser || rememberFailUser)
                {
                    tempBytes = UTF8Encoding.UTF8.GetBytes(psw);
                    if (rememberFailUser) storeUser(ref tempBytes);
                }

                //3.COMBINE POST DATA AND SHOW DEVELOPER MESSAGE
                string data = modelProvider.postFormat(UserName, psw);
                developerMessage(modelProvider.getUrl() + "\n" + data);

                //4.REQUEST POST ACTION
                RequestResult = await basicHttpRuquest(new RunHttpRequest(netRequestProvider.HttpPost), modelProvider.LoginUrl, data);

                //5.HAND RESULT BY CHECK LOGIN STATE
                bool loginState = false;
                if (siteName == "Szhxy")
                {
                    modelProvider.usePage(SpecialPage.szhxyCheckLogin);

                    string rule = modelProvider.getParsingRules();
                    string result = await basicHttpRuquest(new RunHttpRequest(netRequestProvider.HttpGet), modelProvider.getUrl(), string.Empty);
                    loginState = Regex.IsMatch(result, rule);
                }
                else
                {
                    loginState = checkBkjwLogin(RequestResult, UserName);
                }
                if (loginState)
                {
                    Dictionary<string, string> tempList = new Dictionary<string, string>(loginList);
                    tempList.Add(siteName, UserName);
                    LoginList = tempList;
                    if (RememberUser) storeUser(ref tempBytes);

                    SetRequest(MenuItems[0]);
                    sendToConsole(UserName+"("+siteName+") Login successful!");
                }
            }
        }

        private Task<string> basicRun(string url, string data)
        {
            return netRequestProvider.HttpGet(url, data);
        }

        /// <summary>
        /// Logout and clear some data
        /// </summary>
        private async void Logout(object o)
        {
            if (o is KeyValuePair<string, string>)
            {
                string key = ((KeyValuePair<string, string>)o).Key;
                Dictionary<string, string> tempList = new Dictionary<string, string>(loginList);
                if (tempList.ContainsKey(key))
                {
                    //Set model
                    string siteName = modelProvider.CurrentSiteName;
                    modelProvider.useSite(key);

                    //Request to logout
                    tempList.Remove(key);
                    RequestResult = await netRequestProvider.HttpGet(modelProvider.LogoutUrl, string.Empty);

                    //Restore model
                    modelProvider.useSite(SiteName);

                    //netRequestProvider.MCookieContainer = null;
                    //this.cookieContainer = null;

                    LoginList = tempList;
                }
            }
        }

        #region Check login

        /// <summary>
        /// Check if it's login successed
        /// </summary>
        /// <param name="data">data to check</param>
        /// <param name="userName">username that need check</param>
        /// <returns></returns>
        public bool checkBkjwLogin(string data, string userName)
        {
            string rightPattern = modelProvider.getParsingRules(SpecialPage.Login);
            string wrongPattern = modelProvider.getParsingRules(SpecialPage.LoginFail);

            if (string.IsNullOrWhiteSpace(data) || data.Length < 10)
            {   //Check if received data is error
                sendToConsole("Received data error/nCheckBkjwLogin().");
            }
            else
            {   //Check data match
                MatchCollection matches = data.rHtmlHead(rightPattern);
                if (matches.Count == 1)
                {
                    GroupCollection group = matches[0].Groups;
                    if (group[2].Value == userName) return true;
                    sendToConsole("User name no match, Request name : " + group[1].Value + "Input name : " + userName+ "/nCheckBkjwLogin()");
                }
                else
                {
                    sendToConsole(Regex.IsMatch(data, wrongPattern) ? "Wrong user name or password/nCheckBkjwLogin()." : "Data no match wrong pattern/nCheckBkjwLogin().");
                }
            }
            return false;
        }
       
        #endregion

        #endregion

        #region Request assistant

        /// <summary>
        /// Request data to current special url
        /// </summary>
        /// <param name="o">request data</param>
        private void Request(object o)
        {
            //Check login
            if (!LoginList.ContainsKey(modelProvider.CurrentSiteName))
            {
                ($"Please Login Site : { modelProvider.CurrentSiteName } \n{DateTime.Now.ToString()}").showInMessageBox();
                return;
            }
            //Set encoding
            if(netRequestProvider.encoding != modelProvider.Encoding)
            {
                netRequestProvider.encoding = modelProvider.Encoding;
            }
            //Request
            if (IsHttpPost)
            {
                string[] postdata = getPostValue();
                if(postdata == null || postdata.Length < 1)
                {
                    developerMessage("Post string error, action not do");
                }
                else
                {
                    string poststring = string.Empty;
                    poststring = modelProvider.postFormat(postdata);
                    requestWith(modelProvider.getUrl(), poststring);
                }
            }
            else
            {
                requestWith(modelProvider.getUrl());
            }
        }

        //Testing replace
        /// <summary>
        /// Select request model
        /// </summary>
        /// <param name="o">object that can covert to string key</param>
        private void SetRequest(object o)
        {
            //SelectedItem
            if (o is string)
            {
                string cmd = o as string;
                //Set usePage, request type and get optionalitems
                modelProvider.usePage(cmd);
                developerMessage(o.ToString() + "\n" + modelProvider.CurrentSiteName + "\n" + modelProvider.Encoding.WebName);
                var list = modelProvider.getOptionals();
                if (list != null)
                {
                    IsHttpPost = true;

                    var tempPostList = new BindingList<OptionalItem<string, string>[]>();
                    foreach (var itemlist in list)
                    {
                        tempPostList.Add(itemlist);
                    }
                    PostList = tempPostList;

                }
                else
                {
                    IsHttpPost = false;
                }
            }
        }

        #region Async run http request

        public delegate Task<string> RunHttpRequest(string param, string rule);
        private RunHttpRequest delegateHttpRequest = null;

        /// <summary>
        /// Request with url and data
        /// </summary>
        /// <param name="url">Target url</param>
        /// <param name="data">Request data</param>
        /// <param name="isGet">If use httpGet method</param>
        private void requestWith(string url, string data = "")
        {
            if (string.IsNullOrEmpty(data)) delegateHttpRequest = netRequestProvider.HttpGet;
            else delegateHttpRequest = netRequestProvider.HttpPost;

            Action<RunHttpRequest, string, string> action = null;
            action = new Action<RunHttpRequest, string, string>(ctlHttpRequest);

            object[] _params = new object[] { delegateHttpRequest, url, data};
            System.Windows.Application.Current.Dispatcher.BeginInvoke(action, _params);
        }

        private async void ctlHttpRequest(RunHttpRequest runHttpRequest, string url, string data)
        {
            do
            {
                RequestResult = await basicHttpRuquest(runHttpRequest, url, data);
                if (!string.IsNullOrEmpty(RequestResult))
                {
                    handRequestResult();
                }
                if (IsAutoRepeat) await Task.Delay(DelayTime);
                else break;

            } while (IsAutoRepeat);
        }

        private async Task<string> basicHttpRuquest(RunHttpRequest runHttpRequest, string url, string data)
        {
            IsRequestRunning = true;
            string result = string.Empty;

            //Await run
            result = await runHttpRequest(url, data);
            developerMessage("Task Run : " + runHttpRequest.Method.Name);

            //Result handle and check
            if (string.IsNullOrEmpty(result)) sendToConsole(netRequestProvider.errorMessages);

            IsRequestRunning = false;
            return result;
        }

        #endregion

        #region Result handle

        public int maxMatches = 100;//Max matches, prevent matches error

        /// <summary>
        /// Make data to a datatable
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="pattern">match pattern</param>
        /// <param name="headers">datatable's headers</param>
        /// <returns></returns>
        public DataTable dataToTable(string data, string pattern, string[] headers)
        {
            DataTable dt = new DataTable("TestDT");
            if (string.IsNullOrEmpty(data)) return dt;
            IsRequestRunning = false;

            if (string.IsNullOrEmpty(pattern)) return dt;
            MatchCollection matches = data.rHtml(pattern);

            if (matches != null && matches.Count > 0 && matches.Count < maxMatches)
            {
                //Add headers to datatable
                for (int m = 0; m < headers.Length; m++)
                {
                    dt.Columns.Add(headers[m], typeof(string));
                }

                //Add matches data to datatable
                for (int i = 0; i < matches.Count; i++)
                {
                    GroupCollection group = matches[i].Groups;
                    dt.Rows.Add();
                    for (int j = 1; j < group.Count; j++)
                    {
                        string value = group[j].Value.replaceGtLt();
                        dt.Rows[i][j - 1] = value;
                    }
                }

                //Make every column readOnly
                for (int m = 0; m < headers.Length; m++)
                    dt.Columns[m].ReadOnly = true;
            }
            else
            {
                sendToConsole("data to table error, match count is " + matches.Count);
            }
            return dt;
        }

        private void handRequestResult()
        {
            MyTable = dataToTable(RequestResult, modelProvider.getParsingRules(), modelProvider.getHeaders());
        }

        #endregion

        #endregion

        #endregion

        #region Constructor

        public GuetWebRequestViewModel()
        {
            netRequestProvider = new WebSerives.NetRequestProvider(Encoding.GetEncoding("GBK"));
            cookieContainer = netRequestProvider.MCookieContainer;
            initializeCommand();
            loadUserList();
        }

        #endregion

        #region Not useful now

        /// <summary>
        /// Make data to a string array
        /// </summary>
        /// <param name="data">Input data</param>
        /// <param name="pattern">match pattern</param>
        /// <returns></returns>
        public string[,] dataToArray(string data, string pattern)
        {
            if (!string.IsNullOrEmpty(pattern)) return null;

            MatchCollection matches = data.rHtml(pattern);
            string[,] result = null;

            if (matches != null && matches.Count > 0 && matches.Count < maxMatches)
            {
                result = new string[matches.Count, matches[0].Groups.Count];
                GroupCollection group = null;
                for (int i = 0; i < matches.Count; i++)
                {
                    group = matches[i].Groups;
                    for (int j = 1; j < group.Count; j++)
                    {
                        result[i, j] = group[j].Value;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Async run http request method
        /// </summary>
        /// <param name="getresult"></param>
        /// <param name="Url"></param>
        /// <param name="rule"></param>
        private async void asyncHttpRequest(RunHttpRequest runHttpRequest, string url, string data, bool defaultHand)
        {
            IsRequestRunning = true;

            do
            {
                //Await run
                RequestResult = await runHttpRequest(url, data);
                developerMessage("Task Run : " + runHttpRequest.Method.Name);

                //Result handle and check
                if (string.IsNullOrEmpty(RequestResult)) sendToConsole(netRequestProvider.errorMessages);
                else if (defaultHand) handRequestResult();
                else IsRequestRunning = false;

                if (!IsRequestRunning) break;

                //Delay for repeat
                if (IsAutoRepeat) await Task.Delay(DelayTime);
            } while (IsRequestRunning && IsAutoRepeat);

            IsRequestRunning = false;
        }

        #endregion
    }
}

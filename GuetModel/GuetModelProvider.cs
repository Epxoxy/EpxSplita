using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace GuetModel
{
    [Serializable]
    public class SiteModel
    {
        public string SiteName { get; set; }
        public string LoginUrl => LoginModel.Url;
        public string LogoutUrl { get; set; }
        public Encoding RequestEncoding { get; set; }
        public RequestBaseModel LoginModel { get; set; }

        public Dictionary<string, RequestBaseModel> RequestDictionary { get; set; }
    }

    [Serializable]
    public class ProviderModel
    {
        public List<string> MenuList { get; set; }
        public List<string> PostList { get; set; }

        public Dictionary<string, SiteModel> SiteModelDictionary { get; set; }

    }
    
    public class GuetModelProvider
    {
        #region Private Members
        
        private List<string> menuList;
        private List<string> postList;
        private List<string> siteList;
        private string nullStr = string.Empty;

        private Dictionary<string, SiteModel> bigDictionary;
        private SiteModel CurrentSiteModel { get; set; }
        private Dictionary<string, RequestBaseModel> RequestDictionary => CurrentSiteModel.RequestDictionary;

        #endregion

        #region Public Members

        public string CurrentSiteName { get; set; } = "Bkjw";
        public string CurrentPage { get; private set; }
        public string LoginUrl => CurrentSiteModel.LoginUrl;
        public string LogoutUrl => CurrentSiteModel.LogoutUrl;
        public RequestBaseModel LoginModel => CurrentSiteModel.LoginModel;
        public Encoding Encoding => CurrentSiteModel.RequestEncoding;
        public RequestBaseModel CurrentModel => RequestDictionary[CurrentPage];

        #endregion

        #region Contructor

        public void OutputDebug()
        {
            ModelLoader.debugWrite(bigDictionary);
        }
        
        private void Serialize(string dir, ProviderModel providerModel)
        {
            using (FileStream stream = new FileStream(dir + "/Provider.dat", FileMode.OpenOrCreate))
            {
                BinaryFormatter bFormat = new BinaryFormatter();
                bFormat.Serialize(stream, providerModel);
            }
        }

        public GuetModelProvider(string dictResPath)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "storeddata";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            
            ProviderModel providerModel = null;
            //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            //stopwatch.Start();

            //Deserialize model
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            using (Stream stream = asm.GetManifestResourceStream("GuetModel.Resources.Provider.dat"))
            {
                if(stream != null)
                {
                    BinaryFormatter bFormat = new BinaryFormatter();
                    providerModel = (ProviderModel)bFormat.Deserialize(stream);
                }
            }

            //Initilize rootDictionary and other option
            ModelLoader.saveDir = dir;
            if(providerModel == null)
            {
                System.Diagnostics.Debug.WriteLine("Deserialize fail, providerModel == null, use defalut initilize way");
                bigDictionary = new Dictionary<string, SiteModel>();
                ModelLoader.initDictionary(dictResPath, ref bigDictionary, ref menuList, ref postList);
                providerModel = new ProviderModel() { MenuList = menuList, PostList = postList, SiteModelDictionary = bigDictionary };
                Serialize(dir, providerModel);
            }
            else
            {
                bigDictionary = providerModel.SiteModelDictionary;
                menuList = providerModel.MenuList;
                postList = providerModel.PostList;
            }

            //Initilize current state
            CurrentSiteModel = bigDictionary[CurrentSiteName];
            //stopwatch.Stop();
            //System.Diagnostics.Debug.WriteLine("Used time for create data" + stopwatch.ElapsedMilliseconds);

            OutputDebug();
        }
        
        public GuetModelProvider() : this("GuetModel.Resources.List.xml")
        {
        }

        #endregion
        
        #region Basic outside function

        public RequestBaseModel getModel(string pageName)
        {
            if (!RequestDictionary.ContainsKey(pageName))
            {
                foreach (var site in bigDictionary)
                {
                    if (site.Value.RequestDictionary.ContainsKey(pageName))
                    {
                        return site.Value.RequestDictionary[pageName];
                    }
                }
                return null;
            }
            return RequestDictionary[pageName];
        }
        //public RequestBaseModel getModel(SpecialPage pageName)
        //{
        //    return getModel(pageName.getDescription());
        //}

        /// <summary>
        /// Select operate key
        /// </summary>
        /// <param name="key">key name</param>
        /// <returns></returns>
        public bool usePage(string pageName)
        {
            if (CurrentPage == pageName) return true;
            if (!RequestDictionary.ContainsKey(pageName))
            {
                foreach (var site in bigDictionary)
                {
                    if (site.Value.RequestDictionary.ContainsKey(pageName))
                    {
                        CurrentSiteName = site.Key;
                        CurrentSiteModel = site.Value;
                        goto SetKey;
                    }
                }
                return false;
            }

            //Set command key
            SetKey:
            {
                CurrentPage = pageName;
                return true;
            }
        }
        //public bool usePage(SpecialPage pageName)
        //{
        //    return usePage( pageName.getDescription());
        //}

        public bool isPageSelected(string pageName)
        {
            return CurrentPage == pageName;
        }
        //public bool isPageSelected(SpecialPage pageName)
        //{
        //    return CurrentPage == pageName.getDescription();
        //}

        public bool useSite(string siteName)
        {
            if (CurrentSiteName == siteName) return true;
            if (bigDictionary.ContainsKey(siteName))
            {
                CurrentSiteName = siteName;
                CurrentSiteModel = bigDictionary[siteName];
                return true;
            }
            return false;
        }
        //public bool useSite(StoredSiteName siteName)
        //{
        //    return useSite(siteName.getDescription());
        //}
        
        public bool setLogin()
        {
            CurrentPage = SpecialPage.Login;
            //CurrentPage = SpecialPage.Login.getDescription();
            return true;
        }
        public bool setLogin(string siteName)
        {
            if (useSite(siteName)) return setLogin();
            return false;
        }
        //public bool setLogin(StoredSiteName siteName)
        //{
        //    if (useSite(siteName)) return setLogin();
        //    return false;
        //}

        public bool isLoginBeSet()
        {
            return CurrentPage == SpecialPage.Login;
            //return CurrentPage == SpecialPage.Login.getDescription();
        }
        public bool isLoginBeSet(string siteName)
        {
            if (useSite(siteName)) return isLoginBeSet();
            return false;
        }
        //public bool isLoginBeSet(StoredSiteName siteName)
        //{
        //    if (useSite(siteName)) return isLoginBeSet();
        //    return false;
        //}

        #endregion
        
        public MatchCollection matchReceived(string input, string pattern)
        {
            string dataTmp = string.Empty;
            dataTmp = Regex.Replace(input, @"<![\s\S]*?-->", string.Empty);
            dataTmp = Regex.Replace(dataTmp, @"<script[\s\S]*?script>", string.Empty);
            dataTmp = Regex.Replace(dataTmp, @"<html[\s\S]*?<body>", string.Empty);
            dataTmp = Regex.Replace(dataTmp, @">\s+<", "><");

            Regex r = new Regex(pattern);
            dataTmp = Regex.Replace(dataTmp, "\n+\n", " ");
            return r.Matches(dataTmp);
        }

        #region Get basic model data
        
        public string getNextPage()
        {
            string pageName = string.Empty;
            if (CurrentModel is PostModel)
            {
                pageName = (CurrentModel as PostModel).NextKey;
            }
            return pageName;
        }

        /// <summary>
        /// Get all func item
        /// </summary>
        /// <returns></returns>
        public List<string> getMenuList()
        {
            if (menuList.Count > 0)
                return menuList;
            return null;
        }
        public List<string> getSiteList()
        {
            if(siteList == null) siteList = new List<string>(bigDictionary.Keys);
            return siteList;
        }

        public string[] getRowName(string pageName)
        {
            if (pageName == SpecialPage.SelectCourse) return new string[] { "ClassNo" };
            return null;
        }

        /// <summary>
        /// Get page if it is post item
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        public bool isPost(string pageName)
        {
            if (!RequestDictionary.ContainsKey(pageName)) return false;
            return !(CurrentModel is RequestBaseModel);
        }
        //public bool isPost(SpecialPage pageName)
        //{
        //    return isPost(pageName.getDescription());
        //}
        public bool isPost()
        {
            return isPost(CurrentPage);
        }

        /// <summary>
        /// Get the request url of page
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        public string getUrl(string pageName)
        {
            if (!RequestDictionary.ContainsKey(pageName)) return null;
            return RequestDictionary[pageName].Url;
        }
        //public string getUrl(SpecialPage pageName)
        //{
        //    return getUrl(pageName.getDescription());
        //}
        public string getUrl()
        {
            if (CurrentModel == null) return nullStr;
            return CurrentModel.Url;
        }
        public string getNextUrl()
        {
            return getUrl(getNextPage());
        }

        /// <summary>
        /// Get the request page's parsing rules
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        public string getParsingRules(string pageName)
        {
            if (!RequestDictionary.ContainsKey(pageName)) return null;
            return RequestDictionary[pageName].ParsingRules;
        }
        //public string getParsingRules(SpecialPage pageName)
        //{
        //    return getParsingRules(pageName.getDescription());
        //}
        public string getParsingRules()
        {
            if (CurrentModel == null) return nullStr;
            return CurrentModel.ParsingRules;
        }
        public string getNextParsingRules()
        {
            return getParsingRules(getNextPage());
        }

        /// <summary>
        /// Get item's headers of page for initalize table
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        public string[] getHeaders(string pageName)
        {
            if (!RequestDictionary.ContainsKey(pageName)) return null;
            return RequestDictionary[pageName].DataHeaders;
        }
        //public string[] getHeaders(SpecialPage pageName)
        //{
        //    return getHeaders(pageName.getDescription());
        //}
        public string[] getHeaders()
        {
            if (CurrentModel == null) return null;
            return CurrentModel.DataHeaders;
        }
        public string[] getNextHeaders()
        {
            return getHeaders(getNextPage());
        }

        #endregion

        #region Get optional array

        /// <summary>
        /// Get the model's selector data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public BindingList<OptionalItem<string, string>[]> getOptionals(RequestBaseModel model)
        {
            if (model is PostModel)
            {
                return ((PostModel)model).Selector;
            }
            return null;
        }
        public BindingList<OptionalItem<string,string>[]> getOptionals(string pageName)
        {
            if (!RequestDictionary.ContainsKey(pageName)) return null;
            return getOptionals(RequestDictionary[pageName]);
        }
        //public BindingList<OptionalItem<string, string>[]> getOptionals(SpecialPage pageName)
        //{
        //    return getOptionals(pageName.getDescription());
        //}
        public BindingList<OptionalItem<string, string>[]> getOptionals()
        {
            return getOptionals(CurrentModel);
        }
        public BindingList<OptionalItem<string, string>[]> getNextOptionals()
        {
            return getOptionals(getNextPage());
        }

        #endregion

        #region Complete the post data string

        /// <summary>
        ///  Format post data
        /// </summary>
        /// <param name="model"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public string postFormat(RequestBaseModel model, params string[] param)
        {
            string data = string.Empty;
            if (model == null) return data;

            if (model is PostModel)
            {
                string formatString = ((PostModel)model).FormatString;
                return string.Format(formatString, param);

                //if (formula.Length > 2) return data;
                //data = formula[0] + param;

                //if (formula.Length == 2) data += formula[1];
            }
            return data;
        }
        public string postFormat(ref string pageName, params string[] param)
        {
            string data = string.Empty;
            if (!RequestDictionary.ContainsKey(pageName)) return data;
            return postFormat(RequestDictionary[pageName], param);
        }
        public string postFormat(params string[] param)
        {
            return postFormat(CurrentModel, param);
        }
        public string formatNextKey(params string[] param)
        {
            string pageName = getNextPage();
            if (string.IsNullOrEmpty(pageName)) return pageName;
            return postFormat(getModel(pageName), param);
        }

        #endregion

    }
}

#region REMOVED DATA

/*
/// <summary>
/// Combine the complete post data with model
/// </summary>
/// <param name="model"></param>
/// <param name="param"></param>
/// <returns></returns>
public string mixMuliPostData(RequestBaseModel model, string[] param)
{
    StringBuilder data = new StringBuilder();
    if (model == null) return data.ToString();

    if (model is MuliParamPostModel)
    {
        string[] formula = ((MuliParamPostModel)model).PostParam;

        if (formula.Length - param.Length > 1) return data.ToString();

        for (int i = 0; i < param.Length; i++)
        {
            data.Append(formula[i]);
            data.Append(param[i]);
            //data += formula[i] + param[i];
        }
        if (param.Length == (formula.Length - 1))
        {
            data.Append(formula[param.Length]);
            //data += formula[param.Length];
        }
    }
    return data.ToString();
}
public string mixMuliPostData(string pageName, string[] param)
{
    string data = string.Empty;
    if (!RequestDictionary.ContainsKey(pageName)) return data;
    return mixMuliPostData(RequestDictionary[pageName], param);
}
public string mixMuliPostData(SpecialPage pageName, string[] param)
{
    return mixMuliPostData(pageName.getDescription(), param);
}
public string mixMuliPostData(string[] param)
{
    return mixMuliPostData(CurrentModel, param);
}
public string mixMuliWithNextKey(string[] param)
{
    string pageName = getNextPage();
    if (string.IsNullOrEmpty(pageName)) return pageName;
    return mixMuliPostData(pageName, param);
}
*/

/*
        /// <summary>
        /// Test only
        /// </summary>
        public static void testWrite()
        {
            using (System.IO.Stream stream = new System.IO.FileStream("C:/Users/xiaox/Desktop/list.xml", System.IO.FileMode.OpenOrCreate))
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load(stream);

                System.Xml.XmlNodeList nodeList = doc.SelectSingleNode("requestitem").ChildNodes;
                foreach (System.Xml.XmlNode item in nodeList)
                {
                    if (item.Name == "postitem")
                    {
                        PostModel pm = new PostModel();
                        string key = string.Empty;
                        key = item.Attributes["key"].ToString();
                        pm.ParsingRules = item.Attributes["parsingRules"].ToString();
                        pm.Url = item.Attributes["url"].ToString();

                        System.Xml.XmlNodeList child = item.ChildNodes;
                        List<string> postdata = new List<string>();
                        List<string> header = new List<string>();
                        foreach (System.Xml.XmlNode childitem in child)
                        {
                            if (childitem.Name == "postdata")
                            {
                                postdata.Add(childitem.Attributes["data"].ToString());
                            }
                            if (childitem.Name == "header")
                            {
                                postdata.Add(childitem.Attributes["header"].ToString());
                            }
                        }
                        if (header.Count > 0) pm.DataHeaders = header.ToArray();
                    }
                    else if (item.Name == "getitem")
                    {
                        RequestBaseModel rbm = new RequestBaseModel();
                        string key = string.Empty;
                        key = item.Attributes["key"].ToString();
                        rbm.ParsingRules = item.Attributes["parsingRules"].ToString();
                        rbm.Url = item.Attributes["url"].ToString();

                        System.Xml.XmlNodeList child = item.ChildNodes;
                        List<string> postdata = new List<string>();
                        List<string> header = new List<string>();
                        foreach (System.Xml.XmlNode childitem in child)
                        {
                            if (childitem.Name == "header")
                            {
                                postdata.Add(childitem.Attributes["header"].ToString());
                            }
                        }
                        if (header.Count > 0) rbm.DataHeaders = header.ToArray();
                    }
                }
            }
        }
     */
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace GuetModel
{
    public class ModelLoader
    {
        #region Load data

        /// <summary>
        /// Load  optional term
        /// </summary>
        /// <param name="beginYear"></param>
        /// <param name="endYear"></param>
        /// <returns></returns>
        public static OptionalItem<string, string>[] loadTerm(int beginYear, int endYear)
        {
            List<OptionalItem<string, string>> xTerm = new List<OptionalItem<string, string>>();
            for (int i = endYear; i >= beginYear; i--)
            {
                string s = i + "-" + (i + 1);
                xTerm.Add(new OptionalItem<string, string>(s + "_2", s + "Second Term"));
                xTerm.Add(new OptionalItem<string, string>(s + "_1", s + "First Term"));
            }
            return xTerm.ToArray();
        }

        /// <summary>
        ///  Load  optional year
        /// </summary>
        /// <param name="beginYear"></param>
        /// <param name="endYear"></param>
        /// <returns></returns>
        public static OptionalItem<string, string>[] loadYear(int beginYear, int endYear)
        {
            List<OptionalItem<string, string>> xYear = new List<OptionalItem<string, string>>();
            for (int i = endYear; i >= beginYear; i--)
            {
                string s = i + "-" + (i + 1);
                xYear.Add(new OptionalItem<string, string>(s, s + "Year"));
            }
            return xYear.ToArray();
        }

        private static OptionalItem<string, string>[] loadGrades(int fromYear, int toYear)
        {
            OptionalItem<string, string>[] grades = new OptionalItem<string, string>[toYear - fromYear + 1];
            for (int i = 0; i < grades.Length; i++)
            {
                string year = (fromYear + i).ToString();
                grades[i] = new OptionalItem<string, string>(year, year);
            }
            return grades;
        }

        /// <summary>
        /// Load list of optional with value and display name
        /// </summary>
        /// <param name="isResource">If it is content resource</param>
        /// <param name="path">file path</param>
        /// <returns></returns>
        public static bool loadOptionalList(string resourcesName, string path, ref OptionalItem<string, string>[] opList)
        {
            List<OptionalItem<string, string>> xList = new List<OptionalItem<string, string>>();
            string content = string.Empty;
            if (!File.Exists(path))
            {
                System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
                using (Stream stream = asm.GetManifestResourceStream(resourcesName))
                {
                    using (StreamReader sr = new StreamReader(stream, System.Text.Encoding.UTF8))
                    {
                        byte[] bytes = System.Text.UTF8Encoding.UTF8.GetBytes(sr.ReadToEnd());
                        using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                        {
                            fs.Write(bytes, 0, bytes.Length);
                        }
                    }
                }
            }
            using (System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.OpenOrCreate))
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8))
                {
                    content = sr.ReadToEnd();
                }
            }
            MatchCollection matches = Regex.Matches(content, "<option value=\"(.+?)\">(.+?)</option>");
            foreach (Match match in matches)
            {
                GroupCollection group = match.Groups;
                if (group.Count > 2)
                    xList.Add(new OptionalItem<string, string>(group[1].Value, group[2].Value));
            }
            if (xList.Count > 0) opList = xList.ToArray();
            return xList.Count > 0;
        }
        
        /// <summary>
        ///  Read test only
        /// </summary>
        public static void initDictionary(string resName, ref Dictionary<string, SiteModel> modelDictionary, ref List<string> menuList, ref List<string> postList)
        {
            Dictionary<string, SiteModel> cacheModelDict = new Dictionary<string, SiteModel>();
            List<string> cachePostList = new List<string>();
            List<string> cacheMenuList = new List<string>();

            if (majorList == null) updateMajorList(saveDir + "/MajorList.txt");
            if (courseProperties == null) updateClassProperties(saveDir + "/CourseProperties.txt");

            #region Read xml and initalize dictionary
            try
            {
                System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
                using (Stream stream = asm.GetManifestResourceStream(resName))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(stream);
                    XmlNodeList rootNodeList = doc.SelectSingleNode("Requestitem").ChildNodes;
                    foreach(XmlNode childNode in rootNodeList)
                    {
                        //Get basic attribute of site name and encoding name
                        XmlAttribute siteNameAttribute = childNode.Attributes["siteName"];
                        XmlAttribute encodingAttribute = childNode.Attributes["encodingName"];
                        string siteName = siteNameAttribute == null ? "Common" : siteNameAttribute.Value;
                        string encodingName = encodingAttribute == null ? "utf-8" : encodingAttribute.Value;

                        //Initilize request dictionary
                        Dictionary<string, RequestBaseModel> requestDictionary = new Dictionary<string, RequestBaseModel>();
                        List<string> headerTmp;

                        foreach (XmlNode item in childNode.ChildNodes)
                        {
                            if (item.Name == "#comment") continue;
                            string keyTmp = string.Empty, ruleTmp = string.Empty, urlTmp = string.Empty,
                                handHeader = string.Empty, getHandRule = string.Empty,
                                formatString = string.Empty, nextKey = string.Empty, nomenu = string.Empty, itemName = item.Name;
                            
                            System.Diagnostics.Debug.WriteLine(siteName + " -->" +itemName);
                            ////Get and set basic attributes value
                            foreach (XmlAttribute attribute in item.Attributes)
                            {
                                string value = attribute.Value;
                                switch (attribute.Name)
                                {
                                    case "key": { keyTmp = value; } break;
                                    case "parsingRules": { ruleTmp = value; } break;
                                    case "url": { urlTmp = value; } break;
                                    case "nextKey": { nextKey = value; } break;
                                    case "nomenu": { nomenu = value; } break;
                                    case "formatString": { formatString = value; } break;
                                    case "handHeader": { handHeader = value; } break;
                                    case "getHandRule": { getHandRule = value; }break;
                                }
                            }
                            
                            //initialize helper array and list
                            //Get child node value
                            //Assignment postparamArray and headerArray
                            string[] headers = null;
                            headerTmp = new List<string>();
                            foreach (XmlNode childitem in item.ChildNodes)
                            {
                                 if(childitem.Name == "header") headerTmp.Add(childitem.Attributes["data"].Value);
                            }
                            if (headerTmp.Count > 0) headers = headerTmp.ToArray();

                            //Create model with item name
                            if (itemName == "getitem" || itemName == "checkitem")
                            {
                                RequestBaseModel rbm = new RequestBaseModel(urlTmp, ruleTmp, headers);
                                requestDictionary.Add(keyTmp, rbm);
                            }
                            else if(itemName == "postitem")
                            {
                                //If itemName is postitem
                                string optionalValue = string.Empty;
                                if (item.Attributes["optional"] != null)
                                    optionalValue = item.Attributes["optional"].Value;

                                BindingList<OptionalItem<string, string>[]> selectorsTmp;
                                linkMuliOptional(optionalValue, '|', out selectorsTmp);

                                //Add item to postlist
                                PostModel pm = new PostModel(urlTmp, ruleTmp, headers, selectorsTmp, formatString, handHeader, getHandRule);
                                pm.NextKey = nextKey;
                                requestDictionary.Add(keyTmp, pm);
                                cachePostList.Add(keyTmp);
                            }
                            //Add item to allItem collecion
                            if (!string.IsNullOrEmpty(nomenu)) nomenu = string.Empty;
                            else cacheMenuList.Add(keyTmp);
                        }
                        RequestBaseModel loginModel = requestDictionary.ContainsKey("Login") ? requestDictionary["Login"] : null;
                        string logoutUrl = requestDictionary.ContainsKey("Logout") ? requestDictionary["Logout"].Url : string.Empty;
                        Encoding encoding = Encoding.GetEncoding(encodingName);

                        //Initilize siteModel
                        SiteModel siteModel = new SiteModel()
                        {
                            SiteName = siteName,
                            LoginModel = loginModel,
                            LogoutUrl = logoutUrl,
                            RequestEncoding = encoding,
                            RequestDictionary = requestDictionary
                        };
                        //Add model to dictionary
                        cacheModelDict.Add(siteName, siteModel);
                    }
                }
                modelDictionary = cacheModelDict;
                postList = cachePostList;
                menuList = cacheMenuList;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Initialize fail with error (ModelLoader.cs, Line:213)\n " + e.Message + e.StackTrace);
            }
            #endregion
        }
        
        /// <summary>
        /// Output dictionary data to console
        /// </summary>
        /// <param name="dict"></param>
        public static string debugWrite(Dictionary<string, SiteModel> modelDictionary)
        {
            StringBuilder sb = new StringBuilder("ModelDictionary count " + modelDictionary.Count);

            foreach(var dict in modelDictionary)
            {
                Dictionary<string, RequestBaseModel> requestDict = dict.Value.RequestDictionary;
                sb.Append(
                    "\nRequestDict count " + requestDict.Count 
                    + "\nSite -> " + dict.Key 
                    + "\n  Site Name -> " + dict.Value.SiteName 
                    + "\n  Encoding  -> " + dict.Value.RequestEncoding.WebName);
                foreach (var item in requestDict)
                {
                    RequestBaseModel rbm = item.Value;
                    sb.Append(
                          "\n    key -> " + item.Key 
                        + "\n      Url -> " + rbm.Url 
                        + "\n      ParsingRules -> " + rbm.ParsingRules);

                    string[] headers;
                    if ((headers = rbm.DataHeaders) != null)
                    {
                        sb.Append("\n      Headers");
                        foreach (var header in headers)
                        {
                            sb.Append("\n        > " + header);
                        }
                    }
                    if (item.Value is PostModel)
                    {
                        string formatString;
                        if ((formatString = ((PostModel)item.Value).FormatString) != null)
                        {
                            sb.Append("\n      FormatString");
                            sb.Append("\n        > " + formatString);
                        }
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine(sb.ToString());
            return sb.ToString();
        }

        #region Assignment optional array
        /// <summary>
        /// Assignment the mulitiple optional array from optionalStringValue
        /// </summary>
        /// <param name="optionalValue">Value the get optional array</param>
        /// <param name="cSplit">Char value to splite string to array</param>
        /// <param name="optionalItems"></param>
        private static void linkMuliOptional(string optionalValue, char cSplit, out BindingList<OptionalItem<string, string>[]> optionalItems)
        {
            optionalItems = new BindingList<OptionalItem<string, string>[]>();
            if (string.IsNullOrEmpty(optionalValue)) return;
            string[] array = optionalValue.Split(cSplit);
            foreach (var item in array)
            {
                optionalItems.Add(linkOptional(item));
            }
        }

        /// <summary>
        /// Assignment the optional array from optionalStringValue
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private static OptionalItem<string, string>[] linkOptional(string param)
        {
            if (string.IsNullOrEmpty(param)) return null;
            switch (param)
            {
                case "term": return term;
                case "year": return year;
                case "courseProperties": return courseProperties;
                case "type": return type;
                case "grade": return grade;
                case "majorList": return majorList;
                case "selecttype": return selectTyle;
                default: return null;
            }
        }

        #endregion

        public static bool updateMajorList(string path)
        {
            if (majorList == null) majorList = new OptionalItem<string, string>[1];
            return loadOptionalList("GuetModel.Resources.MajorList.txt", path, ref majorList);
        }

        public static bool updateClassProperties(string path)
        {
            if (courseProperties == null) courseProperties = new OptionalItem<string, string>[1];
            return loadOptionalList("GuetModel.Resources.CourseProperties.txt", path, ref courseProperties);
        }


        #endregion

        #region Helper data

        private static OptionalItem<string, string>[] term = loadTerm(2012, DateTime.Now.Year);
        private static OptionalItem<string, string>[] year = loadYear(2012, DateTime.Now.Year);
        private static OptionalItem<string, string>[] grade = loadGrades(2012, DateTime.Now.Year); 
        private static OptionalItem<string, string>[] majorList = null;
        private static OptionalItem<string, string>[] courseProperties = null;
        private static OptionalItem<string, string>[] type = new OptionalItem<string, string>[] {
            new OptionalItem<string, string>("0", "normal"),
            new OptionalItem<string, string>("1", "Retake")  };
        private static OptionalItem<string, string>[] selectTyle = new OptionalItem<string, string>[] {
            new OptionalItem<string, string>("%d5%fd%b3%a3", "normal"),
            new OptionalItem<string, string>("%d6%d8%d0%de", "Retake")  };
        public static string saveDir = string.Empty;

        #endregion
    }
}

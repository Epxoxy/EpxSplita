using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuetModel
{
    //    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    //    public sealed class EnumDescriptionAttribute : Attribute
    //    {
    //        private string description;
    //        public string Description { get { return description; } }

    //        public EnumDescriptionAttribute(string description) : base()
    //        {
    //            this.description = description;
    //        }
    //    }
    //    public static class EnumHelper
    //    {
    //        public static string GetDescription(Enum value)
    //        {
    //            if (value == null)
    //            {
    //                throw new ArgumentException("value");
    //            }
    //            string description = value.ToString();
    //            var fieldInfo = value.GetType().GetField(description);
    //            var attributes =
    //                (EnumDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
    //            if (attributes != null && attributes.Length > 0)
    //            {
    //                description = attributes[0].Description;
    //            }
    //            return description;
    //        }
    //    }

    public static class EnumUtil
    {
        /// <summary>
        /// Get description of enum
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <param name="nameInstead">If use enum name when description is null, defalut value is true</param>
        /// <returns></returns>
        public static string getDescription(this Enum value, bool nameInstead = true)
        {
            Type type = value.GetType();

            string name = Enum.GetName(type, value);
            if (name == null) return null;

            var field = type.GetField(name);
            if (field == null) return nameInstead ? name : null;

            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute), false) as DescriptionAttribute;

            if (attribute == null) return nameInstead ? name : null;

            return attribute.Description;
        }

        /// <summary>
        /// Convert enum to dictionary
        /// </summary>
        /// <param name="enumType">Enum type</param>
        /// <param name="getText">Get text func</param>
        /// <returns>Dictionary with key is Enum's key and value is Enum'Text</returns>
        public static Dictionary<Int32, string> enumToDictionary(Type enumType, Func<Enum, String> getText)
        {
            if (!enumType.IsEnum) throw new ArgumentException("Type must be enum", "enumType");

            Dictionary<Int32, string> enumDic = new Dictionary<int, string>();
            var enumValues = Enum.GetValues(enumType);
            foreach(Enum enumValue in enumValues)
            {
                Int32 key = Convert.ToInt32(enumValue);
                string value = getText(enumValue);
                enumDic.Add(key, value);
            }
            return enumDic;
        }
    }

    public class SpecialPage
    {
        public const string Login = "Login";
        public const string LoginFail = "LoginFail";
        public const string Title = "Title";
        public const string Option = "Option";
        public const string UnSelectCourse = "UnSelectCourse";
        public const string SelectCourse = "SelectCourse";
        public const string CreditsPoint2 = "CreditsPoint2";
        public const string SelectCourseAction = "SelectCourseAction";
        public const string UnSelectCourseAction = "UnSelectCourseAction";
        public const string Logout = "Logout";
        public const string szhxyGetScore = "szhxyGetScore";
        public const string szhxyCheckLogin = "CheckLogin";
        public const string szhxyLogout = "szhxyLogout";
    }

    public class StoredSiteName
    {
        public const string Szhxy = "Szhxy";
        public const string Bkjw = "Bkjw";
    }


    public enum SpecialPage2
    {
        [Description("Login")]
        Login,
        [Description("LoginFail")]
        LoginFail,
        [Description("Title")]
        Title,
        [Description("Option")]
        Option,
        [Description("UnSelectCourse")]
        UnSelectCourse,
        [Description("SelectCourse")]
        SelectCourse,
        [Description("CreditsPoint2")]
        CreditsPoint2,
        [Description("SelectCourseAction")]
        SelectCourseAction,
        [Description("UnSelectCourseAction")]
        UnSelectCourseAction,
        [Description("Logout")]
        Logout,
        [Description("szhxyGetScore")]
        szhxyGetScore,
        [Description("CheckLogin")]
        szhxyCheckLogin,
        [Description("szhxyLogout")]
        szhxyLogout
    }

    public enum StoredSiteName2
    {
        [Description("Szhxy")]
        Szhxy,
        [Description("Bkjw")]
        Bkjw
    }
}

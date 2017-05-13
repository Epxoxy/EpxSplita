using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EpxSplita
{
    static class Extension
    {
        public static void showInMessageBox(this string value)
        {
            (Application.Current.MainWindow as MainWindow)?.showMessage(value);
        }

        public static System.Text.RegularExpressions.MatchCollection rHtml(this String input, string rule)
        {
            if (string.IsNullOrEmpty(input)) return null;
            input = System.Text.RegularExpressions.Regex.Replace(input, @"<![\s\S]*?-->", string.Empty);
            input = System.Text.RegularExpressions.Regex.Replace(input, @"<script[\s\S]*?script>", string.Empty);
            input = System.Text.RegularExpressions.Regex.Replace(input, @"<html[\s\S]*?<body>", string.Empty);
            input = System.Text.RegularExpressions.Regex.Replace(input, @">\s+<", "><");
            return input.rHtmlNoReplace(rule);
        }
        public static System.Text.RegularExpressions.MatchCollection rHtmlNoReplace(this String input, string rule)
        {
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(rule);
            input = System.Text.RegularExpressions.Regex.Replace(input, "\n+\n", " ");
            return r.Matches(input);
        }
        public static System.Text.RegularExpressions.MatchCollection rHtmlHead(this String input, string rule)
        {
            input = System.Text.RegularExpressions.Regex.Replace(input, @"<![\s\S]*?-->", string.Empty);
            input = System.Text.RegularExpressions.Regex.Replace(input, @"<script[\s\S]*?</script>", string.Empty);
            input = System.Text.RegularExpressions.Regex.Replace(input, @">\s+<", "><");
            return input.rHtmlNoReplace(rule);
        }
        public static string replaceGtLt(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            input = System.Text.RegularExpressions.Regex.Replace(input, @"<.+?>", "\n");
            input = System.Text.RegularExpressions.Regex.Replace(input, @"&nbsp;", string.Empty);
            return input;
        }
    }
}

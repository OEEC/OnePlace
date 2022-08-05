using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlace.Server.Helpers
{
    public class EliminarCaracteres
    {
        /// <summary>
        /// Returns a copy of the original string containing only the set of whitelisted characters.
        /// </summary>
        /// <param name="value">The string that will be copied and scrubbed.</param>
        /// <param name="alphas">If true, all alphabetical characters (a-zA-Z) will be preserved; otherwise, they will be removed.</param>
        /// <param name="numerics">If true, all numeric characters (0-9) will be preserved; otherwise, they will be removed.</param>
        /// <param name="dashes">If true, all dash characters (-) will be preserved; otherwise, they will be removed.</param>
        /// <param name="underlines">If true, all underscore characters (_) will be preserved; otherwise, they will be removed.</param>
        /// <param name="spaces">If true, all whitespace (e.g. spaces, tabs) will be preserved; otherwise, they will be removed.</param>
        /// <param name="periods">If true, all dot characters (".") will be preserved; otherwise, they will be removed.</param>

        //quitar todos los caracteres usando linq
        //(variable = new string(variable.Where(x => char.IsWhiteSpace(x) || char.IsLetterOrDigit(x)).ToArray()));
        public static string RemoveExcept(string value, bool alphas = false, bool numerics = false, bool dashes = false, bool underlines = false, bool spaces = false, bool periods = false, bool doubledot = false, bool quote = false, bool uniquequote = false)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            if (new[] { alphas, numerics, dashes, underlines, spaces, periods, doubledot, quote, uniquequote }.All(x => x == false)) return value;

            var whitelistChars = new HashSet<char>(string.Concat(
                alphas ? "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" : "",
                numerics ? "0123456789" : "",
                dashes ? "-" : "",
                underlines ? "_" : "",
                periods ? "." : "",
                spaces ? " " : "",
                doubledot ? ":" : "",
                quote ? "," : "",
                uniquequote ? " ' " : ""
            ).ToCharArray());

            var scrubbedValue = value.Aggregate(new StringBuilder(), (sb, @char) => {
                if (whitelistChars.Contains(@char)) sb.Append(@char);
                return sb;
            }).ToString();

            return scrubbedValue;
        }
    }
}

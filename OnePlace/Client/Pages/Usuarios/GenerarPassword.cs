﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OnePlace.Client.Pages.Usuarios
{
    public class GenerarPassword
    {
        /// <summary>
        /// Generates a Random Password
        /// respecting the given strength requirements.
        /// </summary>
        /// <param name="opts">A valid PasswordOptions object
        /// containing the password strength requirements.</param>
        /// <returns>A random password</returns>

        //si funciona pero querian contraseñas mas faciles
        //public static string GenerateRandomPassword(PasswordOptions opts = null)
        //{
        //    if (opts == null) opts = new PasswordOptions()
        //    {
        //        RequiredLength = 8,
        //        RequiredUniqueChars = 1,
        //        RequireDigit = true,
        //        RequireLowercase = true,
        //        RequireNonAlphanumeric = true,
        //        RequireUppercase = true
        //    };

        //    string[] randomChars = new[] {
        //    "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
        //    "abcdefghijkmnopqrstuvwxyz",    // lowercase
        //    "0123456789",                   // digits
        //    "!@$?_-"                        // non-alphanumeric
        //};

        //    Random rand = new Random(Environment.TickCount);
        //    List<char> chars = new List<char>();

        //    if (opts.RequireUppercase)
        //        chars.Insert(rand.Next(0, chars.Count),
        //            randomChars[0][rand.Next(0, randomChars[0].Length)]);

        //    if (opts.RequireLowercase)
        //        chars.Insert(rand.Next(0, chars.Count),
        //            randomChars[1][rand.Next(0, randomChars[1].Length)]);

        //    if (opts.RequireDigit)
        //        chars.Insert(rand.Next(0, chars.Count),
        //            randomChars[2][rand.Next(0, randomChars[2].Length)]);

        //    if (opts.RequireNonAlphanumeric)
        //        chars.Insert(rand.Next(0, chars.Count),
        //            randomChars[3][rand.Next(0, randomChars[3].Length)]);

        //    for (int i = chars.Count; i < opts.RequiredLength
        //        || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
        //    {
        //        string rcs = randomChars[rand.Next(0, randomChars.Length)];
        //        chars.Insert(rand.Next(0, chars.Count),
        //            rcs[rand.Next(0, rcs.Length)]);
        //    }

        //    return new string(chars.ToArray());
        //}
        public static string GenerateRandomPassword(int Numero, string Nombre)
        {
            //generamos numeros random
            //Random myObject = new Random();
            //int ranNum = myObject.Next(100, 150);

            //generar numeros random con caracteres
            //var characters = "0123456789!@$?_-";
            //var Charsarr = new char[8];
            //var random = new Random();

            //for (int i = 0; i < Charsarr.Length; i++)
            //{
            //    Charsarr[i] = characters[random.Next(characters.Length)];
            //}

            //var resultString = new String(Charsarr);

            //quitamos los espacios al nombre por que vienen los dos 
            string NewNombre = Regex.Replace(Nombre, @"\s", "");

            return new string(NewNombre + Numero + "*");
        }
    }
}

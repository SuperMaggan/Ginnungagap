using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bifrost.Common.IntegrationTests.Utilities
{
    public static class TestDataUtility
    {
        public static string GetCurrentDirectoryPath()
        {
            return Directory.GetCurrentDirectory();
        }

        public static IList<string> GenerateRandomStringList(int listLength, int stringLength)
        {
            var r = new Random();
            var list = new List<string>(listLength);
            for (var i = 0; i < listLength; i++)
                list.Add(GenerateString(stringLength, r));
            return list;
        }

        public static string GenerateString(int size, Random rnd)
        {
            var str = new StringBuilder();
            for (var i = 0; i < size; ++i)
            {
                var ch = Convert.ToChar(rnd.Next(65, 122));
                str.Append(ch);
            }
            return str.ToString();
        }
    }
}
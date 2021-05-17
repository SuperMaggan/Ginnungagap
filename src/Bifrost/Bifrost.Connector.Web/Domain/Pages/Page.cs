using System;
using System.Security.Cryptography;
using System.Text;

namespace Bifrost.Connector.Web.Domain.Pages
{
    public abstract class Page
    {
        protected Page(string id)
        {
            Id = id;
            DownloadDate = DateTime.UtcNow;
        }
        public Uri Url { get; set; }

        /// <summary>
        /// Usually the same as the Url but can sometimes differ, eg if the page has a canonical link defined
        /// </summary>
        public string Id { get; set; }
        
        public DateTime DownloadDate { get; set; }

        /// <summary>
        /// WHen this page should be checked for updates again
        /// </summary>
        public DateTime VerifyDate { get; set; }

        /// <summary>
        /// Number of jumps to get to this page starting from the StartUrl
        /// eg. if this page is linked from the Page defined by StartUrl, its depth would be 1
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Type of content this response contains (eg. application/pdf)
        /// </summary>
        public string ContentType { get; set; }


        private string _hashValue;
        public string HashValue
        {
            get
            {
                if (string.IsNullOrEmpty(_hashValue))
                    _hashValue = CalculateHash();
                return _hashValue;
            }
        }
        protected abstract string CalculateHash();


        protected static string CalculateMD5Hash(string input)
        {

            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();

        }

        protected static string CalculateMD5Hash(byte[] input)
        {

            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] hash = md5.ComputeHash(input);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();

        }
    }
}
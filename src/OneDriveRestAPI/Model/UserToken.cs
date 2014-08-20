using System;

namespace OneDriveRestAPI.Model
{
    public class UserToken
    {
        private DateTime _expires;
        private long _expiresIn;

        public string Access_Token { get; set; }
        public string Refresh_Token { get; set; }
        public string Scope { get; set; }
        public string Token_Type { get; set; }

        public long Expires_In
        {
            get { return _expiresIn; }
            set
            {
                _expiresIn = value;
                _expires = DateTime.UtcNow.AddSeconds(value);
            }
        }

        public DateTime Expires { get { return _expires; } }

    }
}
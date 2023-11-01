using Google.Apis.Auth;

namespace AuthorVerseServer.Services
{
    public class DecodeGoogleTokenService
    {
        static public GoogleJsonWebSignature.Payload? VerifyGoogleIdToken(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { "535136865130-874a69c1agfpvrvdhmfe27s6f0ucg2be.apps.googleusercontent.com" }
                };

                var payload = GoogleJsonWebSignature.ValidateAsync(idToken, settings).Result;

                return payload;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public class AuthRequestModel
    {
        public string Token { get; set; }
    }
}

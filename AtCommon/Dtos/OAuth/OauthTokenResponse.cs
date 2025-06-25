namespace AtCommon.Dtos.OAuth
{
    public class OauthTokenResponse
    {
        public string Token { get; set; }
        public int ExpiresIn { get; set; }
    }
}
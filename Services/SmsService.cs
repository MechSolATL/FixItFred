using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Services
{
    /// <summary>
    /// Provides SMS functionality via Twilio for optional verification.
    /// This is currently not active but kept for future integration.
    /// </summary>
    public class SmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromPhone;

        public SmsService(IConfiguration configuration)
        {
            // Guard clauses ensure environment is properly configured if activated
            _accountSid = configuration["Twilio:AccountSID"]
                ?? throw new InvalidOperationException("Twilio AccountSID is missing in configuration.");

            _authToken = configuration["Twilio:AuthToken"]
                ?? throw new InvalidOperationException("Twilio AuthToken is missing in configuration.");

            _fromPhone = configuration["Twilio:FromPhoneNumber"]
                ?? throw new InvalidOperationException("Twilio FromPhoneNumber is missing in configuration.");
        }

        /// <summary>
        /// Sends a verification code via SMS to the specified phone number.
        /// Returns true if successfully sent.
        /// </summary>
        public async Task<bool> SendVerificationCodeAsync(string phoneNumber, string code)
        {
            try
            {
                TwilioClient.Init(_accountSid, _authToken);

                MessageResource message = await MessageResource.CreateAsync(
                    body: $"?? Your Service-Atlanta verification code is: {code}",
                    from: new Twilio.Types.PhoneNumber(_fromPhone),
                    to: new Twilio.Types.PhoneNumber(phoneNumber)
                );

                return message.ErrorCode == null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SMS error: {ex.Message}");
            }
            {
                // Optional: log ex with ILogger for audit
                return false;
            }
        }
    }
}

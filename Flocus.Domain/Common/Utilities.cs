namespace Flocus.Domain.Common;

public static class Utilities
{
    public static DateTime UnixTimeStampStringToDateTime(string timestampString)
    {
        if (!long.TryParse(timestampString, out long timestamp))
        {
            throw new ArgumentException($"Invalid Unix timestamp string: '{timestampString}'", nameof(timestampString));
        }

        DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return unixEpoch.AddSeconds(timestamp);
    }
}

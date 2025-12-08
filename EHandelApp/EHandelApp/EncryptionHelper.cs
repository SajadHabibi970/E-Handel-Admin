namespace EHandelApp;

public class EncryptionHelper
{
    private const byte Key = 0x42;

    public static string Encrypt(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }
        
        var bytes = System.Text.Encoding.UTF8.GetBytes(text);
        for (int i = 0;  i < bytes.Length; i++)
        {
            bytes[i] = (byte)(bytes[i] ^ Key);
        }
        return Convert.ToBase64String(bytes);
    }

    public static string Decrypt(string krypteradText)
    {
        if (string.IsNullOrEmpty(krypteradText))
        {
            return krypteradText;
        }

        try
        {
            var bytes = Convert.FromBase64String(krypteradText);

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(bytes[i] ^ Key);
            }
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
        catch (FormatException)
        {
            return krypteradText;
        }
    }
}
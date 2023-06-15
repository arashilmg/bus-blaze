using System.Text;

namespace BizCover.Blaze.Infrastructure.Bus.Upgrade.Internals
{
    public static class ByteArrayExtenstions
    {
        public static string ConvertToString(this byte[] toConvert)
            => Encoding.UTF8.GetString(toConvert, 0, toConvert.Length);
    }
}

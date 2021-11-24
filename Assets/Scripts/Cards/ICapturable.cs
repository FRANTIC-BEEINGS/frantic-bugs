using System.Text.RegularExpressions;

namespace Cards
{
    public interface ICapturable
    {
        public void Capture(ulong captorId);
    }
}
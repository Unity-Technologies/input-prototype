namespace UnityEngine.Experimental.Input.Utilities
{
    /// <summary>
    /// Four character code.
    /// </summary>
    public struct FourCC
    {
        int m_Code;

        public FourCC(char a, char b, char c, char d)
        {
            m_Code = (a << 24) | (b << 16) | (c << 8) | d;
        }

        public FourCC(string str)
            : this()
        {
            Debug.Assert(str.Length == 4, "FourCC string must be exactly four characters long!");
            m_Code = (str[0] << 24) | (str[1] << 16) | (str[2] << 8) | str[3];
        }

        public static implicit operator int(FourCC fourCC)
        {
            return fourCC.m_Code;
        }

        public override string ToString()
        {
            return string.Format("'{0}{1}{2}{3}'",
                (char)(m_Code >> 24), (char)((m_Code & 0xff0000) >> 16), (char)((m_Code & 0xff00) >> 8), (char)(m_Code & 0xff));
        }
    }
}

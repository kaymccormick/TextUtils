namespace TextUtils
{
    public struct TextSpan
    {
        public int Start { get; set; }
        public int Length { get; set; }
        public int End => Start + Length;
        public TextSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }

        public bool Contains(in int value)
        {
            return value >= Start && value < End;
        }
    }
}
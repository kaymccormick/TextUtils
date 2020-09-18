namespace TextUtils
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICustomSpan
    {
        /// <summary>
        /// 
        /// </summary>
       TextSpan Span {
            get;
        }

        bool Partial { get; }

        bool FinalPartial { get; }
    }
}
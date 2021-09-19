using System.Numerics;

namespace imSSOUtils.customs
{
    /// <summary>
    /// Basic Console Message structure
    /// </summary>
    internal struct ConsoleMessage
    {
        /// <summary>
        /// The prefix to be used for system messages.
        /// </summary>
        public string system_prefix;

        /// <summary>
        /// The text of the message
        /// </summary>
        public readonly string text;

        /// <summary>
        /// The colour of the message
        /// </summary>
        public Vector4 colour;

        /// <summary>
        /// Is this a system message?
        /// </summary>
        public readonly bool system;

        /// <summary>
        /// Creates a new instance of <see cref="ConsoleMessage"/>.
        /// </summary>
        /// <param name="text">The message to be displayed</param>
        /// <param name="colour">The colour to be used for the message</param>
        /// <param name="system">If this is true, the message will be displayed differently.</param>
        /// <param name="system_prefix">The prefix to be used for system messages.</param>
        public ConsoleMessage(string text, Vector4 colour, bool system, string system_prefix)
        {
            this.text = text;
            this.colour = colour;
            this.system = system;
            this.system_prefix = system_prefix;
        }
    }
}
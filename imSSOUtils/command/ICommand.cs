namespace imSSOUtils.command
{
    /// <summary>
    /// Basic command interface.
    /// </summary>
    internal interface ICommand
    {
        /// <summary>
        /// Push a command.
        /// </summary>
        /// <param name="args">The command arguments, 0 is the command itself.</param>
        protected internal void push(string[] args);
    }
}
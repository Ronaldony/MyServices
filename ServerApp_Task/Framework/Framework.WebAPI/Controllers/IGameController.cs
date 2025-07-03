namespace Framework.WebAPI.Controllers
{
    /// <summary>
    /// IGameController.
    /// </summary>
    interface IGameController
    {
        /// <summary>
        /// Pid.
        /// </summary>
        string Pid { get; set; }

        /// <summary>
        /// Uid.
        /// </summary>
        string Uid { get; set; }

        /// <summary>
        /// Uid.
        /// </summary>
        string Address { get; set; }
    }
}

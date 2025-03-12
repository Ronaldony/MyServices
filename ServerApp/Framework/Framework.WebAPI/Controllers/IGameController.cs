namespace Framework.WebAPI.Controllers
{
    /// <summary>
    /// IGameController.
    /// </summary>
    interface IGameController
    {
        string UserId { get; }

        string Pid { get; }

        string Uid { get; }
    }
}

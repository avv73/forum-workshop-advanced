namespace Forum.App.Commands
{
    using Contracts;

    public class LogInCommand : ICommand
    {
        private IMenuFactory menuFactory;
        private IUserService userService;

        public LogInCommand(IMenuFactory menuFactory, IUserService userService)
        {
            this.menuFactory = menuFactory;
            this.userService = userService;
        }

        public IMenu Execute(params string[] args)
        {
            string userName = args[0];
            string password = args[1];

            bool isLoginSuccessful = userService.TryLogInUser(userName, password);

            if (!isLoginSuccessful)
            {
                throw new System.InvalidOperationException("Invalid login!");
            }

            return this.menuFactory.CreateMenu("MainMenu");
        }
    }
}

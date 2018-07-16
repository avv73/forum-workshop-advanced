namespace Forum.App.Commands
{
    using Contracts;

    public class AddReplyCommand : ICommand
    {
        private IMenuFactory menuFactory;
        private IPostService postService;
        private ICommandFactory commandFactory;

        public AddReplyCommand(IMenuFactory factory, IPostService postService, ICommandFactory commandFactory)
        {
            this.menuFactory = factory;
            this.postService = postService;
            this.commandFactory = commandFactory;
        }

        public IMenu Execute(params string[] args)
        {
            int postId = int.Parse(args[0]);

            string commandName = this.GetType().Name;
            string menuName = commandName.Substring(0, commandName.Length - "Command".Length);

            IMenu menu = this.menuFactory.CreateMenu(menuName + "Menu");

            IIdHoldingMenu menuAsId = (IIdHoldingMenu)menu;
            menuAsId.SetId(postId);

            return menu;
        }
    }
}

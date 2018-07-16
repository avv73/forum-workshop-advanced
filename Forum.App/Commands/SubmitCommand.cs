namespace Forum.App.Commands
{
    using Contracts;

    public class SubmitCommand : ICommand
    {
        private IPostService postService;
        private IMenuFactory menuFactory;

        public SubmitCommand(IPostService postService, IMenuFactory menuFactory)
        {
            this.postService = postService;
            this.menuFactory = menuFactory;
        }

        public IMenu Execute(params string[] args)
        {
            int postId = int.Parse(args[0]);
            string replyText = args[1];
            int userId = int.Parse(args[2]);

            this.postService.AddReplyToPost(postId, replyText, userId);
            IMenu menu = menuFactory.CreateMenu("ViewPostMenu");
            IIdHoldingMenu menuAsId = (IIdHoldingMenu)menu;
            menuAsId.SetId(postId);

            return menu;
        }
    }
}

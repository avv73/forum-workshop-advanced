namespace Forum.App.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;
    using Forum.App.ViewModels;
    using Forum.Data;
    using Forum.DataModels;

    public class PostService : IPostService
    {
        private ForumData forumData;
        private IUserService userService;

        public PostService(ForumData forumData, IUserService userService)
        {
            this.forumData = forumData;
            this.userService = userService;
        }

        public int AddPost(int userId, string postTitle, string postCategory, string postContent)
        {
            bool emptyCategory = string.IsNullOrWhiteSpace(postCategory);
            bool emptyTitle = string.IsNullOrWhiteSpace(postTitle);
            bool emptyContent = string.IsNullOrWhiteSpace(postContent);

            if (emptyCategory || emptyTitle || emptyContent)
            {
                throw new ArgumentException("All fields must be filled!");
            }

            Category category = this.EnsureCategory(postCategory);
            int postId = forumData.Posts.Any() ? forumData.Posts.Last().Id + 1 : 1;

            User author = this.userService.GetUserById(userId);

            Post post = new Post(postId, postTitle, postContent, category.Id, userId, new List<int>());

            forumData.Posts.Add(post);
            author.Posts.Add(post.Id);
            category.Posts.Add(post.Id);
            forumData.SaveChanges();

            return post.Id;
        }

        public void AddReplyToPost(int postId, string replyContents, int userId)
        {
            Post targetPost = GetPost(postId);
            User userSender = this.userService.GetUserById(userId);

            int replyId = this.forumData.Replies.Any() ? this.forumData.Replies.Last().Id + 1 : 1;
            Reply reply = new Reply(replyId, replyContents, userId, postId);

            targetPost.Replies.Add(reply.Id);
            this.forumData.Replies.Add(reply);

            this.forumData.SaveChanges();
        }

        public IEnumerable<ICategoryInfoViewModel> GetAllCategories()
        {
            IEnumerable<ICategoryInfoViewModel> categories = this.forumData.Categories.Select(c => new CategoryInfoViewModel(c.Id, c.Name, c.Posts.Count));

            return categories;
        }


        public string GetCategoryName(int categoryId)
        {
            string categoryName = this.forumData.Categories.Find(c => c.Id == categoryId)?.Name;

            if (categoryName == null)
            {
                throw new ArgumentException($"Category with id {categoryId} not found!");
            }

            return categoryName;
        }

        public IEnumerable<IPostInfoViewModel> GetCategoryPostsInfo(int categoryId)
        {
            Category targetCategory = this.forumData.Categories.FirstOrDefault(p => p.Id == categoryId);

            if (targetCategory == null)
            {
                throw new ArgumentException($"Category with id {categoryId} not found!");
            }

            int[] categoryPostIds = targetCategory.Posts.ToArray();
            Post[] categoryPosts = this.forumData.Posts.Where(i => categoryPostIds.Contains(i.Id)).ToArray();

            IEnumerable<IPostInfoViewModel> postInfoViewModels = categoryPosts.Select(i => new PostInfoViewModel(i.Id, i.Title, i.Replies.Count));

            return postInfoViewModels;
        }

        public IPostViewModel GetPostViewModel(int postId)
        {
            Post post = this.forumData.Posts.FirstOrDefault(p => p.Id == postId);
            IPostViewModel postView = new PostViewModel(post.Title, this.userService.GetUserName(post.AuthorId), post.Content, this.GetPostReplies(postId));

            return postView;
        }

        public Post GetPost(int postId)
        {
            Post post = this.forumData.Posts.FirstOrDefault(p => p.Id == postId);
            return post;
        }

        private IEnumerable<IReplyViewModel> GetPostReplies(int postId)
        {
            IEnumerable<IReplyViewModel> replies = this.forumData.Replies
                .Where(r => r.PostId == postId)
                .Select(r => new ReplyViewModel(this.userService.GetUserName(r.AuthorId), r.Content));

            return replies;
        }

        private Category EnsureCategory(string postCategory)
        {
            Category foundCategory = this.forumData.Categories.FirstOrDefault(c => c.Name == postCategory);

            if (foundCategory == null)
            {
                foundCategory = new Category(postCategory);
                foundCategory.Id = this.forumData.Categories.Any() ? this.forumData.Categories.Last().Id + 1 : 1;
                this.forumData.Categories.Add(foundCategory);
            }

            return foundCategory;
        }
    }
}

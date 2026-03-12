using System.Text.RegularExpressions;
using Travel_Blogging.DTOs.PostDto;
using Travel_Blogging.Models;
using Travel_Blogging.Repositories.Interfaces;
using Travel_Blogging.Services.Interfaces;

namespace Travel_Blogging.Services.Implementations
{
    public class PostService:IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PostService(IPostRepository postRepository, IWebHostEnvironment webHostEnvironment)
        {
            _postRepository = postRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        // this function is for creating the post
        public async Task<PostModel>CreatePost(CreatePostDto dto, int userId)
        {
            // create folder path if not exist inside wwwroot folder
            string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }
            // make the image extension
            var extension = Path.GetExtension(dto.Image.FileName).ToLower();
            // generate unique file name
            string uniqueFileName = Guid.NewGuid().ToString() + extension;
            string filePath = Path.Combine(uploadFolder, uniqueFileName);
            // save this filepath in local file
            using (var fileStream=new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(fileStream);
            }

            string imageUrl = "/images/" + uniqueFileName;

            // jodit editor return the description with p tag so this is for removing the p tag in the description
            string plainTextDescription = Regex.Replace(dto.Description, "<.*?>", string.Empty);

            var newPost = new PostModel
            {
                Title = dto.Title,
                Description = plainTextDescription,
                ImageUrl = imageUrl,
                Location = dto.Location,
                AuthorId = userId
            };

            await _postRepository.CreatePostAsync(newPost);
            return newPost;
        }

        // this function is for finding all the posts
        public async Task<List<PostModel>> FindPosts()
        {
            var posts = await _postRepository.FindPostsAsync();
            return posts;
        }

        // this function is for finding the post details of any specific post
        public async Task<PostModel> FindPostDetails(int postId)
        {
            var post = await _postRepository.FindPostDetailsAsync(postId);
            return post;
        }
    }
}

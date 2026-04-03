using System.Text.RegularExpressions;
using Travel_Blogging.DTOs.PostDto;
using Travel_Blogging.Models;
using Travel_Blogging.Models.Enums;
using Travel_Blogging.Repositories.Interfaces;
using Travel_Blogging.Services.Interfaces;

namespace Travel_Blogging.Services.Implementations
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IReviewRepository _reviewRepository;
        public PostService(IPostRepository postRepository, IWebHostEnvironment webHostEnvironment, IReviewRepository reviewRepository)
        {
            _postRepository = postRepository;
            _webHostEnvironment = webHostEnvironment;
            _reviewRepository = reviewRepository;
        }

        // this function is for creating the post
        public async Task<PostModel> CreatePost(CreatePostDto dto, int userId, string actionType)
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
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(fileStream);
            }

            string imageUrl = "/images/" + uniqueFileName;

            // jodit editor return the description with p tag so this is for removing the p tag in the description

            var newPost = new PostModel
            {
                Title = dto.Title,
                Description = dto.Description,
                ImageUrl = imageUrl,
                Location = dto.Location,
                AuthorId = userId,
                CreatedBy = userId,

                Status = actionType == "publish"
                    ? PostStatus.Published
                    : PostStatus.Draft
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

        // this function is for finding the latest post
        public async Task<List<PostModel>> FindLatestPosts()
        {
            return await _postRepository.FindLatestPostsAsync();
        }

        // this function is for finding the all posts of the loggedin user
        public async Task<List<PostModel>>FindUserPosts(int userId, bool isDraft = false)
        {
            var status = isDraft ? PostStatus.Draft : PostStatus.Published;
            return await _postRepository.FindUserPostsAsync(userId, status);
        }

        // this function is for delete any specific post of loggedin user
        public async Task DeleteUserPost(int postId, int userId)
        {
            var post = await _postRepository.FindPostByIdAsync(postId);
            // first deletes the post from the database
            await _postRepository.DeleteUserPostAsync(postId, userId);

            // now delete it's all review 
            await _reviewRepository.DeletePostReviewAsync(postId);

            // also delete the image from the local files
            if (!string.IsNullOrEmpty(post.ImageUrl))
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, post.ImageUrl.TrimStart('/'));
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }
        }


        // this function is for sending the saved data to the clint for edit the post
        public async Task<CreatePostDto> EditPost(int postId)
        {
            // get the post from service
            var post = await _postRepository.FindPostByIdAsync(postId);

            // convert into dto
            var dto = new CreatePostDto
            {
                Id = post.Id,
                Title = post.Title,
                Location = post.Location,
                Description = post.Description,
                ExistingImageUrl =post.ImageUrl 
            };

            return dto;
        }


        // this function is for edit the post details (i.e httpPost)
        public async Task<PostModel> UpdatePost(CreatePostDto dto, string actionType)
        {
            if (!dto.Id.HasValue) return null;

            var post = await _postRepository.FindPostByIdAsync(dto.Id.Value);
            if (post == null) return null;

            // update the fields
            post.Title = dto.Title;
            post.Description = dto.Description;
            post.Location = dto.Location;
            post.UpdatedAt = DateTime.Now;
            post.UpdatedBy = post.AuthorId;

            // Update status based on the button clicked
            post.Status = actionType == "publish" ? PostStatus.Published : PostStatus.Draft;

            // if image is changed
            if (dto.Image != null)
            {
                // delete the old image
                if (!string.IsNullOrEmpty(post.ImageUrl))
                {
                    string oldImagePath = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        post.ImageUrl.TrimStart('/')
                    );

                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }

                // save the new image
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                
                var extension = Path.GetExtension(dto.Image.FileName).ToLower();
                string uniqueFileName = Guid.NewGuid().ToString() + extension;
                string newFilePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(fileStream);
                }

                // update the database path
                post.ImageUrl = "/images/" + uniqueFileName;
            }

            // if image not change then do nothing

            // call the repository to save the changes in the database.
            await _postRepository.UpdatePostAsync();

            return post;
        }
    }
}

using Facebook.DataBaseContext;
using Facebook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace Facebook.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly PostContext _postContext;
        private readonly CommentContext _commentContext;
        private readonly UserManager<CustomUserFields> _userManager;


        public PostController(PostContext postContext, CommentContext commentContext, UserManager<CustomUserFields> userManager)
        {
            _postContext = postContext;
            _commentContext = commentContext;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserPost(PostModel userPost, IFormFile image)
        {
            var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;

            userPost.FirstName = currentUser.FirstName;
            userPost.LastName = currentUser.LastName;
            userPost.Email = currentUser.Email;
            userPost.PostDateTime = DateTime.Now;

            if (image != null)
            {
                byte[] imageBytes = GetImageBytes(image);

                // Create an HTTP client and set the content type to image/jpeg
                HttpClient client = new HttpClient();
                // Create a new HttpRequestMessage
                var request = new HttpRequestMessage(HttpMethod.Post, "http://127.0.0.1:5000/api/face_detection");

                // Create an HttpContent object representing the image data and set the Content-Type header
                var content = new ByteArrayContent(imageBytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                // Set the request's Content property to the HttpContent object
                request.Content = content;

                var response = await client.SendAsync(request);

                // Read the response from the API
                var responseString = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response
                var responseMsg = JsonConvert.DeserializeObject<ResponseMsg>(responseString);
                string resMsg = responseMsg.Result;
                int code = responseMsg.Code;

                if (response.IsSuccessStatusCode)
                {
                    if (code == 0)
                    {
                        // Upload image to any cload Storage
                        // Save Image Path to Database
                    }
                    else
                    {
                        // Use this message to warn this user that image contains faces
                        TempData["Warning"] = resMsg;
                    }
                }
                else
                {
                    TempData["Error"] = resMsg;
                }

            }

            _postContext.Add(userPost);
            //await _postContext.SaveChangesAsync();

            var url = Request.Headers["Referer"];

            return Redirect(url);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserComment(CommentModel userComment)
        {
            var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;

            userComment.FirstName = currentUser.FirstName;
            userComment.LastName = currentUser.LastName;
            userComment.Email = currentUser.Email;
            userComment.CommentDateTime = DateTime.Now;

            _commentContext.Add(userComment);
            await _commentContext.SaveChangesAsync();

            var url = Request.Headers["Referer"];

            return Redirect(url);
        }

        // GET: Post/Edit/5
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Profile", "Home");
            }

            var post = await _postContext.Posts.FindAsync(id);

            if (post == null)
            {
                return RedirectToAction("Profile", "Home");
            }

            return PartialView("~/Views/Partials/_EditPost.cshtml", post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(PostModel editedPost)
        {
            var post = await _postContext.Posts.FindAsync(editedPost.Id);
            post.Post = editedPost.Post;
            _postContext.Entry(post).State = EntityState.Modified;

            await _postContext.SaveChangesAsync();

            return RedirectToAction("Profile", "Home");
        }

        // GET: Post/Delete/5
        [HttpGet]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id != null)
            {
                var post = await _postContext.Posts.FindAsync(id);

                if (post != null)
                {
                    _postContext.Posts.Remove(post);

                    var comments = await _commentContext.Comments.ToListAsync();
                    foreach (var comment in comments)
                    {
                        if (comment.PostID == id)
                        {
                            _commentContext.Comments.Remove(comment);
                        }
                    }

                    await _postContext.SaveChangesAsync();
                    await _commentContext.SaveChangesAsync();
                }
            }

            return RedirectToAction("Profile", "Home");
        }

        // to convert image to byte array
        private static byte[] GetImageBytes(IFormFile image)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }

    // Class to deserialize the JSON response for face detection API
    class ResponseMsg
    {
        public string Result { get; set; }
        public int Code { get; set; }
    }
}

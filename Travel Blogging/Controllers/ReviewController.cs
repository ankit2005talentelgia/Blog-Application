using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Security.Claims;
using System.Text.Json;
using Travel_Blogging.DTOs.PostDto;
using Travel_Blogging.Services.Interfaces;

namespace Travel_Blogging.Controllers;

[Route("Review")]
public class ReviewController:Controller
{
    private readonly IToastNotification _toastNotification;
    private readonly IReviewService _reviewService;

    public ReviewController(IToastNotification toastNotification, IReviewService reviewService)
    {
        _toastNotification = toastNotification;
        _reviewService = reviewService;
    }

    // this function is for creating the review of the post
    [HttpPost("CreateReview")]
    public async Task<IActionResult> CreateReview(CreateReviewDto dto)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("PostDetails", "Post", new { id = dto.PostId });
        }

        if (!User.Identity.IsAuthenticated)
        {
            _toastNotification.AddWarningToastMessage("please login first");
            return RedirectToAction("Login", "User");
        }

        // if authenticated but not verify their email
        var isVerified = User.Claims
                    .FirstOrDefault(c => c.Type == "isEmailConfirmed")?.Value;

        if (isVerified == "False")
        {
            return View("CheckEmail", "User");
        }

        // if verify then gives the permission
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        await _reviewService.AddReview(dto, userId);

        _toastNotification.AddSuccessToastMessage("review created successfully");
        return RedirectToAction("PostDetails", "Post", new { id = dto.PostId });
    }

    // this function is for deleting the review 
    [HttpPost("DeleteReview")]
    public async Task<IActionResult> DeleteReview([FromBody] DeleteReviewDto dto)
    {
        // if it is not verify their email
        var isVerified = User.Claims
                    .FirstOrDefault(c => c.Type == "isEmailConfirmed")?.Value;

        if (isVerified == "False")
        {
            return View("CheckEmail", "User");
        }

        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        await _reviewService.DeleteReview(dto.PostId, dto.ReviewId, userId);
        _toastNotification.AddSuccessToastMessage("review deletes successfully");
        return Ok();
    }
}

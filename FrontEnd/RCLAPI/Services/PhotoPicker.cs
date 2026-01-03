namespace RCLAPI.Services
{
    public interface PhotoPicker
    {
        Task<string?> PickPhotoBase64Async();
    }
}
using CommunityToolkit.Maui.Behaviors;

namespace KarolK72.LegoAssignment.UI.Validation
{
    /// <summary>
    /// Validates whether a field contains a valid Url (hostname and IP address and not empty)
    /// </summary>
    internal class UrlValidationBehaviour : ValidationBehavior<string>
    {
        protected override ValueTask<bool> ValidateAsync(string value, CancellationToken token)
        {
            return new ValueTask<bool>(!string.IsNullOrWhiteSpace(value) && Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute));
        }
    }
}

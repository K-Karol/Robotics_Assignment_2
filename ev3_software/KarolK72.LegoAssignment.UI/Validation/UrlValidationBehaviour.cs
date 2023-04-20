using CommunityToolkit.Maui.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarolK72.LegoAssignment.UI.Validation
{
    internal class UrlValidationBehaviour : ValidationBehavior<string>
    {
        protected override ValueTask<bool> ValidateAsync(string value, CancellationToken token)
        {
            return new ValueTask<bool>(!string.IsNullOrWhiteSpace(value) && Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute));
        }
    }
}

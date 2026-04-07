using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiApp1.Converters;

public abstract class ValueConverterBase : ObservableObject
{
}

public partial class BoolToAuthTextConverter : ValueConverterBase
{
    public string LoginText { get; set; } = "Sign in to continue your journey";
    public string RegisterText { get; set; } = "Create your account to start exploring";
}

public partial class BoolToButtonTextConverter : ValueConverterBase
{
    public string LoginText { get; set; } = "Login";
    public string RegisterText { get; set; } = "Create Account";
}

public partial class BoolToToggleTextConverter : ValueConverterBase
{
    public string LoginText { get; set; } = "New here? Create an account";
    public string RegisterText { get; set; } = "Already have an account? Login";
}

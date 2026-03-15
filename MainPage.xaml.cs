namespace MauiApp1
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		private void OnCounterClicked(object? sender, EventArgs e)
		{
			if (sender is Button button)
			{
				int count = 0;
				if (button.Text is string s && s.StartsWith("Clicked "))
				{
					var parts = s.Split(' ');
					if (parts.Length == 2 && int.TryParse(parts[1], out int currentCount))
						count = currentCount;
				}
				count++;
				button.Text = $"Clicked {count}";
			}
		}
	}
}

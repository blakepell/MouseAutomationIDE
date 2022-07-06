using System.Text.Json.Serialization;
using System.Windows;

namespace MouseAutomation.Common
{
    /// <summary>
    /// AppSettings that are persisted to storage and reloaded on application launch.
    /// </summary>
    public class AppSettings : DependencyObject
    {
        public string? AutoSaveText { get; set; }

        [JsonIgnore]
        public static readonly DependencyProperty PollingIntervalProperty = DependencyProperty.Register(
            nameof(PollingInterval), typeof(int), typeof(AppSettings), new PropertyMetadata(10));

        public int PollingInterval
        {
            get => (int)GetValue(PollingIntervalProperty);
            set => SetValue(PollingIntervalProperty, value);
        }
    }
}

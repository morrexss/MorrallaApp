using MorrallaExpress.Controls;
using System;
using Xamarin.Forms;

namespace MorrallaExpress.Behaviors
{
    public class ExpirationDateBehavior : Behavior<CustomEntry>
    {
        public int MaxLength { get; set; }

        protected override void OnAttachedTo(CustomEntry bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.TextChanged += OnEntryTextChanged;
        }

        protected override void OnDetachingFrom(CustomEntry bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.TextChanged -= OnEntryTextChanged;
        }

        void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {

            if (sender is null || e.NewTextValue is null || e.OldTextValue is null)
                return;

            var entry = (Entry)sender;

            if (!Int32.TryParse(e.NewTextValue.Replace("/", ""), out int n) && !String.IsNullOrEmpty(e.NewTextValue))
            {
                entry.Text = e.OldTextValue ?? "";
                return;
            }

            if (entry.Text.Length == 1 && int.Parse(e.NewTextValue) > 1)
            {
                entry.Text = $"0{e.NewTextValue}";
                return;
            }

            if (entry.Text.Length > 3 && int.TryParse(e.NewTextValue.Substring(0, 2), out int s))
            {
                if (s > 12)
                    entry.Text = e.OldTextValue ?? "";
            }


            if (!string.IsNullOrEmpty(e.OldTextValue) && e.OldTextValue.Length < 2 && e.NewTextValue.Contains("/"))
            {
                entry.Text = e.OldTextValue ?? "";

                return;
            }

            if (e.NewTextValue.Length == 2 && !e.OldTextValue.Contains("/"))
            {

                if (int.Parse(e.NewTextValue) > 12)
                {
                    entry.Text = e.NewTextValue.Substring(0, 1);
                }
                else
                {
                    entry.Text = $"{e.NewTextValue}/";
                    return;
                }
            }

            if (entry.Text.Length > 1 && e.OldTextValue.Length == 3 && e.NewTextValue.Length == 2)
            {
                entry.Text = e.NewTextValue.Substring(0, 1);
                return;
            }

            if (entry.Text.Length > 2 && e.OldTextValue.Contains("/") && !e.NewTextValue.Contains("/"))
            {
                entry.Text = e.OldTextValue.Substring(0, 2);
                return;
            }

            if (entry.Text.Length > this.MaxLength)
            {
                // string entryText = entry.Text;
                entry.TextChanged -= OnEntryTextChanged;
                entry.Text = e.OldTextValue;
                entry.TextChanged += OnEntryTextChanged;
            }
        }
    }
}

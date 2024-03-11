using System;
using UnityEngine;

namespace Helpers
{
    public static class TextFormatHelper
    {
        private static string _daysTemplate;
        private static string _hoursTemplate;
        private static string _minutesTemplate;
        private static string _secondsTemplate;
        private static string _millisecondsTemplate;

        public static event System.Action TimeTemplatesChanged;
        
        static TextFormatHelper()
        {
            _UpdateTimeLocalizationTemplates();
            //LocalizationManager.OnLocalizeEvent += LocalizationManager_OnLocalizeEvent;
        }
        
        
        public static string GetTimeHhMmSs(TimeSpan time)
        {
            var totalHours = Mathf.FloorToInt((float)time.TotalHours);
            var hours = totalHours > 0 ? string.Format(_hoursTemplate, totalHours) : string.Empty;
            var minutes = time.Minutes > 0 ? string.Format(_minutesTemplate, time.Minutes ) : string.Empty;
            var seconds = time.Seconds > 0 || time.Minutes > 0 || totalHours > 0 ? string.Format(_secondsTemplate, time.Seconds.ToString(format:"00")) : string.Empty;
            return $"{hours} {minutes} {seconds}".Trim();
        }
        
        public static string GetTimeDdHhMmSs(TimeSpan time)
        {
            var totalDays = Mathf.FloorToInt((float)time.TotalDays);
            var days = totalDays > 0 ? string.Format(_daysTemplate, totalDays) : string.Empty;
            var hours = time.Hours > 0 ? string.Format(_hoursTemplate, time.Hours) : string.Empty;
            var minutes = time.Minutes > 0 ? string.Format(_minutesTemplate, time.Minutes) : string.Empty;
            var seconds = time.Seconds > 0 || time.Minutes > 0 || time.Hours > 0 || totalDays > 0 ? string.Format(_secondsTemplate, time.Seconds.ToString(format:"00")) : string.Empty;
            return $"{days} {hours} {minutes} {seconds}".Trim();
        }
        public static string GetTimeDdHhMm(TimeSpan time)
        {
            var totalDays = Mathf.FloorToInt((float)time.TotalDays);
            var days = totalDays > 0 ? string.Format(_daysTemplate, totalDays) : string.Empty;
            var hours = time.Hours > 0 ? string.Format(_hoursTemplate, time.Hours) : string.Empty;
            var minutes = time.Minutes > 0 ? string.Format(_minutesTemplate, time.Minutes) : string.Empty;
            return $"{days} {hours} {minutes} ".Trim();
        }

        public static string GetTimeDdHhMmOrSs(TimeSpan time)
        {
            var totalDays = Mathf.FloorToInt((float)time.TotalDays);
            var days = totalDays > 0 ? string.Format(_daysTemplate, totalDays) : string.Empty;
            var hours = time.Hours > 0 ? string.Format(_hoursTemplate, time.Hours) : string.Empty;
            var minutes = time.Minutes > 0 ? string.Format(_minutesTemplate, time.Minutes) : string.Empty;
            var seconds = time.Seconds > 0 && (time.Minutes == 0 && time.Hours == 0 && totalDays == 0) ? string.Format(_secondsTemplate, time.Seconds.ToString(format:"00")) : string.Empty;
            return $"{days} {hours} {minutes} {seconds}".Trim();
        }

        public static string GetTimeMmSsMs(TimeSpan time)
        {
            var totalMinutes = Mathf.FloorToInt((float)time.TotalMinutes);
            var minutes = totalMinutes > 0 ? string.Format(_minutesTemplate, totalMinutes) : string.Empty;
            var seconds = time.Seconds > 0 || totalMinutes > 0 ? string.Format(_secondsTemplate, time.Seconds.ToString(format:"00")) : string.Empty;
            var milliseconds = time.Milliseconds > 0 ? string.Format(_millisecondsTemplate, time.Milliseconds) : string.Empty;
            return $"{minutes} {seconds} {milliseconds}".Trim();
        }
        
        public static string GetTimeHhMmSsWithColons(TimeSpan time)
        {
            int hours = Mathf.FloorToInt((float) time.TotalHours);
            int minutes = time.Minutes;
            int seconds = time.Seconds;
            string formattedHours = hours > 0 ? hours.ToString("00:") : string.Empty;
            string formattedMinutes = hours > 0 || minutes > 0 ? minutes.ToString("00:") : string.Empty;
            string formattedSeconds = seconds.ToString("00");
            return $"{formattedHours}{formattedMinutes}{formattedSeconds}";
        }
        public static string GetTimeDdHhMmWithColons(TimeSpan time)
        {
            int days = Mathf.FloorToInt((float)time.TotalDays);
            int hours = Mathf.FloorToInt((float) time.TotalHours);
            int minutes = time.Minutes;
            string formattedDays = days > 0 ?  days.ToString("00:") : string.Empty; 
            string formattedHours = hours > 0 ? hours.ToString("00:") : string.Empty;
            string formattedMinutes = hours > 0 || minutes > 0 ? minutes.ToString("00") : string.Empty;
            return $"{formattedDays}{formattedHours}{formattedMinutes}";
        }
        #region delegates
        private static void LocalizationManager_OnLocalizeEvent()
        {
            _UpdateTimeLocalizationTemplates();
        }

        private static void _UpdateTimeLocalizationTemplates()
        {
            //TODO: Localizion when will be add
            // _daysTemplate = ScriptLocalization.Days;
            // _hoursTemplate = ScriptLocalization.Hours;
            // _minutesTemplate = ScriptLocalization.Minutes;
            // _secondsTemplate = ScriptLocalization.Seconds;
            // _millisecondsTemplate = ScriptLocalization.Milliseconds;
            
            _daysTemplate = "{0}D";
            _hoursTemplate = "{0}H";
            _minutesTemplate = "{0}M";
            _secondsTemplate = "{0}S";
            _millisecondsTemplate = "{0}Ms";
            
            TimeTemplatesChanged?.Invoke();
        }
        #endregion
    }
}